using Backend.Entities;
using Backend.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Backend.Features.Users;

public class UserService
{
    private readonly AppDbContext _db;

    public UserService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<UserDto>> GetUsersAsync()
    {
        var users = await _db.Users
            .AsNoTracking()
            .Select(u => new UserDto
            {
                Id = u.Id,
                Username = u.Username
            })
            .ToListAsync();

        return users;
    }
}
