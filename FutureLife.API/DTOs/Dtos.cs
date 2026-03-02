using System.ComponentModel.DataAnnotations;

namespace FutureLife.API.DTOs;

// Auth DTOs
public record RegisterDto(
    [Required, MaxLength(100)] string FullName,
    [Required, EmailAddress] string Email,
    [Required, MinLength(6)] string Password,
    string? PreferredCurrency
);

public record LoginDto(
    [Required, EmailAddress] string Email,
    [Required] string Password
);

public record AuthResponseDto(
    string Token,
    UserDto User
);

// User DTOs
public record UserDto(
    int Id,
    string FullName,
    string Email,
    string? PreferredCurrency,
    DateTime CreatedAt
);

public record UpdateUserDto(
    [MaxLength(100)] string? FullName,
    string? PreferredCurrency
);

// Profile DTOs
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

// Simulation DTOs
public record SimulateDto(
    [Range(1, 50)] int ProjectionYears,
    string? Currency
);

public record SimulationResultDto(
    int Id,
    int ProfileId,
    int ProjectionYears,
    string Currency,
    object ResultData,
    DateTime CreatedAt
);

// Exchange Rate DTOs
public record ExchangeRateDto(
    int Id,
    string CurrencyCode,
    decimal RateToUsd,
    DateTime UpdatedAt
);
