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

        var user = new User
        {
            FullName = dto.FullName,
            Email = dto.Email.ToLower(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            PreferredCurrency = dto.PreferredCurrency ?? "USD"
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var token = _jwt.GenerateToken(user);
        return (true, null, new AuthResponseDto(token, MapUser(user)));
    }

    public async Task<(bool Success, string? Error, AuthResponseDto? Response)> LoginAsync(LoginDto dto)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email.ToLower());
        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return (false, "Invalid credentials.", null);

        var token = _jwt.GenerateToken(user);
        return (true, null, new AuthResponseDto(token, MapUser(user)));
    }

    public async Task<User?> GetByIdAsync(int id) =>
        await _db.Users.FindAsync(id);

    public static UserDto MapUser(User user) => new(
        user.Id, user.FullName, user.Email, user.PreferredCurrency, user.CreatedAt
    );
}
