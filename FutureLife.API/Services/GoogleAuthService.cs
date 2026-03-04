using Google.Apis.Auth;
using Microsoft.EntityFrameworkCore;
using FutureLife.API.Data;
using FutureLife.API.DTOs;
using FutureLife.API.Helpers;
using FutureLife.API.Models;

namespace FutureLife.API.Services;

public class GoogleAuthService
{
    private readonly AppDbContext _db;
    private readonly JwtHelper _jwt;
    private readonly IConfiguration _config;

    public GoogleAuthService(AppDbContext db, JwtHelper jwt, IConfiguration config)
    {
        _db     = db;
        _jwt    = jwt;
        _config = config;
    }

    public async Task<(bool Success, string? Error, AuthResponseDto? Response)> SignInWithGoogleAsync(string idToken)
    {
        // ── 1. Verify the token with Google ───────────────────
        GoogleJsonWebSignature.Payload payload;
        try
        {
            var clientId = _config["Google:ClientId"]
                ?? throw new InvalidOperationException("Google:ClientId not configured.");

            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { clientId }
            };

            payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
        }
        catch (InvalidJwtException ex)
        {
            return (false, $"Invalid Google token: {ex.Message}", null);
        }
        catch (Exception ex)
        {
            return (false, $"Token verification failed: {ex.Message}", null);
        }

        // ── 2. Extract user data from Google payload ──────────
        var googleId = payload.Subject;       // Unique Google user ID
        var email    = payload.Email;
        var fullName = payload.Name ?? email;
        var avatar   = payload.Picture;

        // ── 3. Find or create user ────────────────────────────
        var user = await _db.Users.FirstOrDefaultAsync(u => u.GoogleId == googleId)
                ?? await _db.Users.FirstOrDefaultAsync(u => u.Email == email.ToLower());

        if (user == null)
        {
            // New user — create account (no password)
            user = new User
            {
                FullName  = fullName,
                Email     = email.ToLower(),
                GoogleId  = googleId,
                Avatar    = avatar,
                LastLoginAt = DateTime.UtcNow
            };
            _db.Users.Add(user);
        }
        else
        {
            // Existing user — update Google info
            user.GoogleId   = googleId;
            user.LastLoginAt = DateTime.UtcNow;
            if (user.Avatar == null && avatar != null) user.Avatar = avatar;
        }

        // ── 4. Generate refresh token and save ────────────────
        var refreshToken = AuthService.CreateRefreshToken();
        user.RefreshToken = refreshToken;
        await _db.SaveChangesAsync();

        // ── 5. Issue internal JWT ─────────────────────────────
        var accessToken = _jwt.GenerateToken(user);

        return (true, null, new AuthResponseDto(
            accessToken, refreshToken, AuthService.MapUser(user)
        ));
    }
}
