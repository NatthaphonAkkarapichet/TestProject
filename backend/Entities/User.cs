namespace Backend.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public int FailedLoginCount { get; set; }
    public DateTime? LockoutEnd { get; set; }

    public List<RefreshToken> RefreshTokens { get; set; } = new();
}
