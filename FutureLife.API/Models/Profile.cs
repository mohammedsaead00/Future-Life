using System.ComponentModel.DataAnnotations;

namespace FutureLife.API.Models;

public class Profile
{
    public int Id { get; set; }

    [Required, MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    public int Age { get; set; }
    public string? Gender { get; set; }
    public string? Country { get; set; }

    // Finance
    public decimal CurrentSavings { get; set; }
    public decimal MonthlyIncome { get; set; }
    public decimal MonthlyExpenses { get; set; }
    public decimal InvestmentReturnRate { get; set; } = 0.07m;
    public decimal InflationRate { get; set; } = 0.03m;
    public string Currency { get; set; } = "USD";

    // Career
    public decimal CurrentSalary { get; set; }
    public decimal AnnualSalaryGrowthRate { get; set; } = 0.05m;
    public decimal PromotionProbability { get; set; } = 0.15m;
    public decimal PromotionSalaryBoost { get; set; } = 0.20m;
    public string? JobTitle { get; set; }

    // Health
    public int SleepHoursPerNight { get; set; } = 7;
    public int ExerciseDaysPerWeek { get; set; } = 3;
    public int StressLevel { get; set; } = 5; // 1-10
    public decimal BMI { get; set; }

    // Social
    public int SocialInteractionsPerWeek { get; set; } = 5;
    public int CloseFriendsCount { get; set; } = 3;
    public int CommunityEngagementScore { get; set; } = 5; // 1-10

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<SimulationResult> SimulationResults { get; set; } = new List<SimulationResult>();
}
