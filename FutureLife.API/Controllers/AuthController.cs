using Microsoft.AspNetCore.Mvc;
using FutureLife.API.DTOs;
using FutureLife.API.Helpers;
using FutureLife.API.Services;

namespace FutureLife.API.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _auth;
    private readonly GoogleAuthService _googleAuth;

    public AuthController(AuthService auth, GoogleAuthService googleAuth)
    {
        _auth       = auth;
        _googleAuth = googleAuth;
    }

    private int? UserId => HttpContext.Items["UserId"] as int?;

    // ── POST /auth/register ────────────────────────────────────

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ApiResponse.Fail("Invalid input."));

        var (success, error, response) = await _auth.RegisterAsync(dto);
        if (!success) return Conflict(ApiResponse.Fail(error!));

        return Ok(ApiResponse.Success(response, "Registration successful."));
    }

    // ── POST /auth/login ───────────────────────────────────────

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ApiResponse.Fail("Invalid input."));

        var (success, error, response) = await _auth.LoginAsync(dto);
        if (!success) return Unauthorized(ApiResponse.Fail(error!));

        return Ok(ApiResponse.Success(response, "Login successful."));
    }

    // ── POST /auth/refresh ─────────────────────────────────────

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ApiResponse.Fail("Invalid input."));

        var (success, error, accessToken) = await _auth.RefreshAsync(dto.RefreshToken);
        if (!success) return Unauthorized(ApiResponse.Fail(error!));

        return Ok(ApiResponse.Success(new { accessToken }, "Token refreshed."));
    }

    // ── POST /auth/logout ──────────────────────────────────────

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        if (UserId == null) return Unauthorized(ApiResponse.Fail("Unauthorized."));

        await _auth.LogoutAsync(UserId.Value);
        return Ok(ApiResponse.Success<object>(null, "Logged out successfully."));
    }

    // ── GET /auth/me ───────────────────────────────────────────

    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        if (UserId == null) return Unauthorized(ApiResponse.Fail("Unauthorized."));

        var user = await _auth.GetByIdAsync(UserId.Value);
        if (user == null) return NotFound(ApiResponse.Fail("User not found."));

        return Ok(ApiResponse.Success(AuthService.MapUser(user)));
    }

    // ── PATCH /auth/profile ────────────────────────────────────

    [HttpPatch("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserDto dto)
    {
        if (UserId == null) return Unauthorized(ApiResponse.Fail("Unauthorized."));

        var (success, error, user) = await _auth.UpdateProfileAsync(UserId.Value, dto);
        if (!success) return NotFound(ApiResponse.Fail(error!));

        return Ok(ApiResponse.Success(user, "Profile updated."));
    }

    // ── POST /auth/google ──────────────────────────────────────

    [HttpPost("google")]
    public async Task<IActionResult> GoogleSignIn([FromBody] GoogleSignInDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ApiResponse.Fail("Invalid input."));

        var (success, error, response) = await _googleAuth.SignInWithGoogleAsync(dto.IdToken);
        if (!success) return Unauthorized(ApiResponse.Fail(error!));

        return Ok(ApiResponse.Success(response, "Signed in with Google."));
    }
}
