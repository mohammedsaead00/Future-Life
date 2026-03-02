using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FutureLife.API.Data;
using FutureLife.API.DTOs;
using FutureLife.API.Helpers;
using FutureLife.API.Services;

namespace FutureLife.API.Controllers;

[ApiController]
[Route("users")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly AuthService _auth;

    public UsersController(AppDbContext db, AuthService auth)
    {
        _db = db;
        _auth = auth;
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        var userId = HttpContext.Items["UserId"] as int?;
        if (userId == null) return Unauthorized(ApiResponse.Fail("Unauthorized."));

        var user = await _auth.GetByIdAsync(userId.Value);
        if (user == null) return NotFound(ApiResponse.Fail("User not found."));

        return Ok(ApiResponse.Success(AuthService.MapUser(user)));
    }

    [HttpPut("me")]
    public async Task<IActionResult> UpdateMe([FromBody] UpdateUserDto dto)
    {
        var userId = HttpContext.Items["UserId"] as int?;
        if (userId == null) return Unauthorized(ApiResponse.Fail("Unauthorized."));

        var user = await _auth.GetByIdAsync(userId.Value);
        if (user == null) return NotFound(ApiResponse.Fail("User not found."));

        if (!string.IsNullOrWhiteSpace(dto.FullName)) user.FullName = dto.FullName;
        if (!string.IsNullOrWhiteSpace(dto.PreferredCurrency)) user.PreferredCurrency = dto.PreferredCurrency;
        user.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return Ok(ApiResponse.Success(AuthService.MapUser(user), "User updated."));
    }
}
