using Microsoft.EntityFrameworkCore;
using FutureLife.API.Data;
using FutureLife.API.DTOs;
using FutureLife.API.Models;

namespace FutureLife.API.Services;

public class ProfileService
{
    private readonly AppDbContext _db;

    public ProfileService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<ProfileDto>> GetAllAsync(int userId) =>
        await _db.Profiles
            .Where(p => p.UserId == userId)
            .Select(p => MapProfile(p))
            .ToListAsync();

    public async Task<Profile?> GetEntityAsync(int id, int userId) =>
        await _db.Profiles.FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);

    public async Task<ProfileDto?> GetAsync(int id, int userId)
    {
        var profile = await GetEntityAsync(id, userId);
        return profile == null ? null : MapProfile(profile);
    }

    public async Task<ProfileDto> CreateAsync(CreateProfileDto dto, int userId)
    {
        var profile = new Profile
        {
            Name = dto.Name,
            Age = dto.Age,
            Gender = dto.Gender,
            Country = dto.Country,
            CurrentSavings = dto.CurrentSavings,
            MonthlyIncome = dto.MonthlyIncome,
            MonthlyExpenses = dto.MonthlyExpenses,
            InvestmentReturnRate = dto.InvestmentReturnRate,
            InflationRate = dto.InflationRate,
            Currency = dto.Currency,
            CurrentSalary = dto.CurrentSalary,
            AnnualSalaryGrowthRate = dto.AnnualSalaryGrowthRate,
            PromotionProbability = dto.PromotionProbability,
            PromotionSalaryBoost = dto.PromotionSalaryBoost,
            JobTitle = dto.JobTitle,
            SleepHoursPerNight = dto.SleepHoursPerNight,
            ExerciseDaysPerWeek = dto.ExerciseDaysPerWeek,
            StressLevel = dto.StressLevel,
            BMI = dto.BMI,
            SocialInteractionsPerWeek = dto.SocialInteractionsPerWeek,
            CloseFriendsCount = dto.CloseFriendsCount,
            CommunityEngagementScore = dto.CommunityEngagementScore,
            UserId = userId
        };

        _db.Profiles.Add(profile);
        await _db.SaveChangesAsync();

        return MapProfile(profile);
    }

    public async Task<(bool Success, ProfileDto? Profile)> UpdateAsync(int id, int userId, CreateProfileDto dto)
    {
        var profile = await GetEntityAsync(id, userId);
        if (profile == null) return (false, null);

        profile.Name = dto.Name;
        profile.Age = dto.Age;
        profile.Gender = dto.Gender;
        profile.Country = dto.Country;
        profile.CurrentSavings = dto.CurrentSavings;
        profile.MonthlyIncome = dto.MonthlyIncome;
        profile.MonthlyExpenses = dto.MonthlyExpenses;
        profile.InvestmentReturnRate = dto.InvestmentReturnRate;
        profile.InflationRate = dto.InflationRate;
        profile.Currency = dto.Currency;
        profile.CurrentSalary = dto.CurrentSalary;
        profile.AnnualSalaryGrowthRate = dto.AnnualSalaryGrowthRate;
        profile.PromotionProbability = dto.PromotionProbability;
        profile.PromotionSalaryBoost = dto.PromotionSalaryBoost;
        profile.JobTitle = dto.JobTitle;
        profile.SleepHoursPerNight = dto.SleepHoursPerNight;
        profile.ExerciseDaysPerWeek = dto.ExerciseDaysPerWeek;
        profile.StressLevel = dto.StressLevel;
        profile.BMI = dto.BMI;
        profile.SocialInteractionsPerWeek = dto.SocialInteractionsPerWeek;
        profile.CloseFriendsCount = dto.CloseFriendsCount;
        profile.CommunityEngagementScore = dto.CommunityEngagementScore;
        profile.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return (true, MapProfile(profile));
    }

    public async Task<bool> DeleteAsync(int id, int userId)
    {
        var profile = await GetEntityAsync(id, userId);
        if (profile == null) return false;

        _db.Profiles.Remove(profile);
        await _db.SaveChangesAsync();
        return true;
    }

    public static ProfileDto MapProfile(Profile p) => new(
        p.Id, p.Name, p.Age, p.Gender, p.Country,
        p.CurrentSavings, p.MonthlyIncome, p.MonthlyExpenses,
        p.InvestmentReturnRate, p.InflationRate, p.Currency,
        p.CurrentSalary, p.AnnualSalaryGrowthRate, p.PromotionProbability,
        p.PromotionSalaryBoost, p.JobTitle,
        p.SleepHoursPerNight, p.ExerciseDaysPerWeek, p.StressLevel, p.BMI,
        p.SocialInteractionsPerWeek, p.CloseFriendsCount, p.CommunityEngagementScore,
        p.UserId, p.CreatedAt
    );
}
