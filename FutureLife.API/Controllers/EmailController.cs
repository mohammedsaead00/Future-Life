using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FutureLife.API.Data;
using FutureLife.API.Helpers;
using FutureLife.API.Services;

namespace FutureLife.API.Controllers;

[ApiController]
[Route("api/email")]
public class EmailController : ControllerBase
{
    private readonly EmailService _email;
    private readonly AppDbContext _db;

    public EmailController(EmailService email, AppDbContext db)
    {
        _email = email;
        _db    = db;
    }

    private int? UserId => HttpContext.Items["UserId"] as int?;

    /// <summary>GET /api/email/ping — no auth needed, just confirms the controller is alive.</summary>
    [HttpGet("ping")]
    public IActionResult Ping() => Ok(ApiResponse.Success("pong", "EmailController is alive!"));

    /// <summary>
    /// POST /api/email/send-test
    /// Sends a monthly summary email immediately to the authenticated user (for testing).
    /// </summary>
    [HttpPost("send-test")]
    public async Task<IActionResult> SendTest()
    {
        if (UserId == null) return Unauthorized(ApiResponse.Fail("Unauthorized."));

        var user = await _db.Users
            .Include(u => u.SimulationResults)
            .FirstOrDefaultAsync(u => u.Id == UserId.Value);

        if (user == null) return NotFound(ApiResponse.Fail("User not found."));

        var latest = user.SimulationResults.OrderByDescending(r => r.CreatedAt).FirstOrDefault();
        var sent   = await _email.SendMonthlySummaryAsync(user, latest);

        if (!sent)
            return StatusCode(500, ApiResponse.Fail("Failed to send email. Check SendGrid config."));

        return Ok(ApiResponse.Success<object>(null, $"Test email sent to {user.Email}"));
    }
}
