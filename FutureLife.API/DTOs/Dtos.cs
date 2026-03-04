using System.ComponentModel.DataAnnotations;

namespace FutureLife.API.DTOs;

// ── Auth ─────────────────────────────────────────────────────

public record RegisterDto(
    [Required, MaxLength(100)] string FullName,
    [Required, EmailAddress] string Email,
    [Required, MinLength(6)] string Password,
    string? PreferredCurrency,
    string? Avatar
);

public record LoginDto(
    [Required, EmailAddress] string Email,
    [Required] string Password
);

public record RefreshTokenDto(
    [Required] string RefreshToken
);

public record GoogleSignInDto(
    [Required] string IdToken
);

public record AuthResponseDto(
    string AccessToken,
    string RefreshToken,
    UserDto User
);

// ── User ─────────────────────────────────────────────────────

public record UserDto(
    int Id,
    string FullName,
    string Email,
    string? PreferredCurrency,
    string? Avatar,
    DateTime CreatedAt,
    DateTime? LastLoginAt
);

public record UpdateUserDto(
    [MaxLength(100)] string? FullName,
    string? PreferredCurrency,
    string? Avatar
);

// ── Simulation Input ─────────────────────────────────────────

public record SimulationInputDto(
    [Required, Range(100, 1_000_000)] double MonthlyIncome,
    [Required, Range(0.0, 0.50)] double SavingPercentage,
    [Range(0.0, 10.0)] double DailyStudyHours,
    [Range(0, 7)] int WorkoutDaysPerWeek,
    string Currency,
    string? CareerField,
    [Range(0.0, 20.0)] double WeeklySkillHours,
    [Range(0, 20)] int CertsPerYear,
    [Range(0.0, 10.0)] double SocialMediaHours,
    [Range(0.0, 10.0)] double FamilyHours,
    [Range(0.0, 10.0)] double NetworkingHours
);

// ── Simulation Result ────────────────────────────────────────

public record YearlySnapshotDto(
    int Year,
    double Savings,
    double StudyHours,
    double HealthScore,
    double EnergyScore
);

public record MonthlySnapshotDto(
    int Month,
    double Savings,
    double StudyHours,
    double HealthScore
);

public record SimulationResultFullDto(
    int Id,
    int UserId,
    string Name,
    // Financial
    double Savings1Y, double Savings5Y, double Savings10Y,
    double MonthlySavings, double NetWorth10Y, string Currency,
    // Knowledge
    double StudyHours1Y, double StudyHours5Y, double StudyHours10Y,
    // Health
    double HealthScore1Y, double HealthScore5Y, double HealthScore10Y,
    // Career
    double CareerGrowthIndex, double SalaryMultiplier, double PromotionProbability,
    // Social
    double SocialBalanceScore, double IsolationRisk,
    // Energy & Strategy
    double LifeStrategyScore,
    double EnergyScore1Y, double EnergyScore5Y, double EnergyScore10Y,
    double BurnoutRisk,
    // Risks
    double FinancialCollapseRisk, double CareerStagnationRisk,
    double EnergyDepletionRisk, double OverallRiskIndex,
    // Charts
    List<YearlySnapshotDto> YearlySnapshots,
    List<MonthlySnapshotDto> MonthlySnapshots,
    DateTime CreatedAt
);

public record SaveSimulationDto(
    [MaxLength(200)] string Name,
    [Required] SimulationResultFullDto Result
);

public record SimulationStatsDto(
    int TotalScenarios,
    double? BestLifeStrategyScore,
    double? BestSavings10Y,
    string? BestCareerField,
    DateTime? LastSimulationAt
);

public record ParallelFuturesDto(
    SimulationResultFullDto Current,
    SimulationResultFullDto Optimized,
    SimulationResultFullDto Decline
);

// ── Exchange Rate ─────────────────────────────────────────────

public record ExchangeRateDto(
    int Id,
    string CurrencyCode,
    decimal RateToUsd,
    DateTime UpdatedAt
);

// ── Profile (kept for backward-compat with ProfilesController) ───

public record CreateProfileDto(
    [Required, MaxLength(150)] string Name,
    int Age,
    string? Gender,
    string? Country,
    decimal CurrentSavings,
    decimal MonthlyIncome,
    decimal MonthlyExpenses,
    decimal InvestmentReturnRate,
    decimal InflationRate,
    string Currency,
    decimal CurrentSalary,
    decimal AnnualSalaryGrowthRate,
    decimal PromotionProbability,
    decimal PromotionSalaryBoost,
    string? JobTitle,
    int SleepHoursPerNight,
    int ExerciseDaysPerWeek,
    int StressLevel,
    decimal BMI,
    int SocialInteractionsPerWeek,
    int CloseFriendsCount,
    int CommunityEngagementScore
);

public record ProfileDto(
    int Id,
    string Name,
    int Age,
    string? Gender,
    string? Country,
    decimal CurrentSavings,
    decimal MonthlyIncome,
    decimal MonthlyExpenses,
    decimal InvestmentReturnRate,
    decimal InflationRate,
    string Currency,
    decimal CurrentSalary,
    decimal AnnualSalaryGrowthRate,
    decimal PromotionProbability,
    decimal PromotionSalaryBoost,
    string? JobTitle,
    int SleepHoursPerNight,
    int ExerciseDaysPerWeek,
    int StressLevel,
    decimal BMI,
    int SocialInteractionsPerWeek,
    int CloseFriendsCount,
    int CommunityEngagementScore,
    int UserId,
    DateTime CreatedAt
);
