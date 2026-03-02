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

    public AuthController(AuthService auth)
    {
        _auth = auth;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ApiResponse.Fail("Invalid input."));

        var (success, error, response) = await _auth.RegisterAsync(dto);
        if (!success) return Conflict(ApiResponse.Fail(error!));

        return Ok(ApiResponse.Success(response, "Registration successful."));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ApiResponse.Fail("Invalid input."));

        var (success, error, response) = await _auth.LoginAsync(dto);
        if (!success) return Unauthorized(ApiResponse.Fail(error!));

        return Ok(ApiResponse.Success(response, "Login successful."));
    }

    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var userId = HttpContext.Items["UserId"] as int?;
        if (userId == null) return Unauthorized(ApiResponse.Fail("Unauthorized."));

        var user = await _auth.GetByIdAsync(userId.Value);
        if (user == null) return NotFound(ApiResponse.Fail("User not found."));

        return Ok(ApiResponse.Success(AuthService.MapUser(user)));
    }
}
