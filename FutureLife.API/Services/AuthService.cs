using Microsoft.EntityFrameworkCore;
using FutureLife.API.Data;
using FutureLife.API.DTOs;
using FutureLife.API.Helpers;
using FutureLife.API.Models;

namespace FutureLife.API.Services;

public class AuthService
{
    private readonly AppDbContext _db;
    private readonly JwtHelper _jwt;

    public AuthService(AppDbContext db, JwtHelper jwt)
    {
        _db = db;
        _jwt = jwt;
    }

    public async Task<(bool Success, string? Error, AuthResponseDto? Response)> RegisterAsync(RegisterDto dto)
    {
        if (await _db.Users.AnyAsync(u => u.Email == dto.Email.ToLower()))
            return (false, "Email already registered.", null);

        var refreshToken = GenerateRefreshToken();
        var user = new User
        {
            FullName     = dto.FullName,
            Email        = dto.Email.ToLower(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            PreferredCurrency = dto.PreferredCurrency ?? "USD",
            Avatar       = dto.Avatar,
            RefreshToken = refreshToken,
            LastLoginAt  = DateTime.UtcNow
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var accessToken = _jwt.GenerateToken(user);
        return (true, null, new AuthResponseDto(accessToken, refreshToken, MapUser(user)));
    }

    public async Task<(bool Success, string? Error, AuthResponseDto? Response)> LoginAsync(LoginDto dto)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email.ToLower());
        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return (false, "Invalid email or password.", null);

        var refreshToken = GenerateRefreshToken();
        user.RefreshToken = refreshToken;
        user.LastLoginAt  = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        var accessToken = _jwt.GenerateToken(user);
        return (true, null, new AuthResponseDto(accessToken, refreshToken, MapUser(user)));
    }

    public async Task<(bool Success, string? Error, string? AccessToken)> RefreshAsync(string refreshToken)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
        if (user == null)
            return (false, "Invalid or expired refresh token.", null);

        // Issue new pair
        var newRefreshToken = GenerateRefreshToken();
        user.RefreshToken = newRefreshToken;
        await _db.SaveChangesAsync();

        var accessToken = _jwt.GenerateToken(user);
        return (true, null, accessToken);
    }

    public async Task<bool> LogoutAsync(int userId)
    {
        var user = await _db.Users.FindAsync(userId);
        if (user == null) return false;

        user.RefreshToken = null;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<(bool Success, string? Error, UserDto? User)> UpdateProfileAsync(int userId, UpdateUserDto dto)
    {
        var user = await _db.Users.FindAsync(userId);
        if (user == null) return (false, "User not found.", null);

        if (dto.FullName != null)          user.FullName          = dto.FullName;
        if (dto.PreferredCurrency != null) user.PreferredCurrency = dto.PreferredCurrency;
        if (dto.Avatar != null)            user.Avatar            = dto.Avatar;
        user.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return (true, null, MapUser(user));
    }

    public async Task<User?> GetByIdAsync(int id) =>
        await _db.Users.FindAsync(id);

    public static UserDto MapUser(User user) => new(
        user.Id, user.FullName, user.Email,
        user.PreferredCurrency, user.Avatar,
        user.CreatedAt, user.LastLoginAt
    );

    private static string GenerateRefreshToken() =>
        Convert.ToBase64String(Guid.NewGuid().ToByteArray()) +
        Convert.ToBase64String(Guid.NewGuid().ToByteArray());
}
