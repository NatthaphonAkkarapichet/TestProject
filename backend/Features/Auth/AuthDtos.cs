namespace Backend.Features.Auth;

public record LoginRequest(string Username, string Password);
public record RefreshRequest(string RefreshToken);

public record AuthResponse(string AccessToken, string RefreshToken, string Username);

public record RegisterRequest(string Username, string Password, string PasswordConfirm);

