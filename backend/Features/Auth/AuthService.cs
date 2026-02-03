using Backend.Common;
using Backend.Common.Exceptions;
using Backend.Entities;
using Backend.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace Backend.Features.Auth;

public class AuthService
{
    private readonly AppDbContext _db;
    private readonly JwtTokenGenerator _jwt;

    public AuthService(AppDbContext db, JwtTokenGenerator jwt)
    {
        _db = db;
        _jwt = jwt;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest req)
    {
        var user = await _db.Users
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.Username == req.Username);

        if (user == null)
            throw new BadRequestException("Invalid credentials");

        //  ถูก lock
        if (user.LockoutEnd > DateTime.UtcNow)
        {
            throw new BadRequestException(
        $"Account locked until {user.LockoutEnd:HH:mm}"
    );
        }


        //  รหัสผิด
        if (!PasswordHasher.Verify(req.Password, user.PasswordHash))
        {
            user.FailedLoginCount++;

            if (user.FailedLoginCount >= 3)
            {
                user.LockoutEnd = DateTime.UtcNow.AddMinutes(5);
                user.FailedLoginCount = 0;
            }

            await _db.SaveChangesAsync();
            throw new BadRequestException("Invalid credentials");
        }

        //  login สำเร็จ
        user.FailedLoginCount = 0;
        user.LockoutEnd = null;

        var accessToken = _jwt.GenerateAccessToken(user);
        var refreshToken = _jwt.GenerateRefreshToken(user.Id);

        _db.RefreshTokens.Add(refreshToken);
        await _db.SaveChangesAsync();

        return new AuthResponse(accessToken, refreshToken.Token, user.Username);
    }

    public async Task<AuthResponse> RefreshAsync(RefreshRequest req)
    {
        var token = await _db.RefreshTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Token == req.RefreshToken);

        if (token == null)
            throw new BadRequestException("Invalid refresh token");

        if (token.RevokedAt != null)
        {
            var activeTokens = _db.RefreshTokens
                .Where(x => x.UserId == token.UserId && x.RevokedAt == null);

            foreach (var t in activeTokens)
                t.RevokedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            throw new BadRequestException("Refresh token reuse detected");
        }

        if (token.IsExpired)
            throw new BadRequestException("Refresh token expired");

        // ROTATION
        token.RevokedAt = DateTime.UtcNow;

        var newRefresh = _jwt.GenerateRefreshToken(token.UserId);
        token.ReplacedByToken = newRefresh.Token;

        _db.RefreshTokens.Add(newRefresh);

        var newAccess = _jwt.GenerateAccessToken(token.User);

        await _db.SaveChangesAsync();

        return new AuthResponse(newAccess, newRefresh.Token, token.User.Username);
    }

    public RefreshToken GenerateRefreshToken(Guid userId)
    {
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        return new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Token = token,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(30)
        };
    }
    public async Task<AuthResponse> RegisterAsync(RegisterRequest req)
    {
        var username = req.Username.ToLower();

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(req.Password))
            throw new BadRequestException("Username and password are required");

        //  เช็ค username ซ้ำ
        var exists = await _db.Users.AnyAsync(u => u.Username == username);
        if (exists)
            throw new BadRequestException("Username already exists");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = username,
            PasswordHash = PasswordHasher.Hash(req.Password)
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        //  สมัครเสร็จ login ทันที
        var accessToken = _jwt.GenerateAccessToken(user);
        var refreshToken = _jwt.GenerateRefreshToken(user.Id);

        _db.RefreshTokens.Add(refreshToken);
        await _db.SaveChangesAsync();

        return new AuthResponse(accessToken, refreshToken.Token, user.Username);
    }
}
