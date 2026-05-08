using Microsoft.EntityFrameworkCore;
using FutureLife.API.Data;

namespace FutureLife.API.Services;

/// <summary>
/// Runs every day and checks if it is the 1st of the month → sends monthly summary emails.
/// </summary>
public class MonthlyEmailJob : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<MonthlyEmailJob> _logger;

    public MonthlyEmailJob(IServiceScopeFactory scopeFactory, ILogger<MonthlyEmailJob> logger)
    {
        _scopeFactory = scopeFactory;
        _logger       = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        _logger.LogInformation("MonthlyEmailJob started.");

        while (!ct.IsCancellationRequested)
        {
            var now = DateTime.UtcNow;

            // Fire on the 1st of every month at 08:00 UTC
            if (now.Day == 1 && now.Hour == 8)
            {
                await SendMonthlyEmailsAsync(ct);

                // Sleep 23 hours so we don't fire twice on the same day
                await Task.Delay(TimeSpan.FromHours(23), ct);
            }
            else
            {
                // Check every hour
                await Task.Delay(TimeSpan.FromHours(1), ct);
            }
        }
    }

    private async Task SendMonthlyEmailsAsync(CancellationToken ct)
    {
        _logger.LogInformation("Sending monthly summary emails...");

        using var scope      = _scopeFactory.CreateScope();
        var db               = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var emailService     = scope.ServiceProvider.GetRequiredService<EmailService>();

        // Get all users who have at least one simulation result
        var users = await db.Users
            .Include(u => u.SimulationResults)
            .Where(u => u.SimulationResults.Any())
            .ToListAsync(ct);

        int sent = 0;
        foreach (var user in users)
        {
            if (ct.IsCancellationRequested) break;

            var latest = user.SimulationResults
                .OrderByDescending(r => r.CreatedAt)
                .FirstOrDefault();

            var success = await emailService.SendMonthlySummaryAsync(user, latest);
            if (success) sent++;

            // Small delay between emails to respect SendGrid rate limits
            await Task.Delay(300, ct);
        }

        _logger.LogInformation("Monthly emails done. Sent: {Sent}/{Total}", sent, users.Count);
    }
}
