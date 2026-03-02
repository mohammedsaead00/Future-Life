using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using FutureLife.API.Data;
using FutureLife.API.DTOs;
using FutureLife.API.Models;

namespace FutureLife.API.Services;

public class SimulationService
{
    private readonly AppDbContext _db;
    private readonly LifeEngineService _engine;
    private readonly CurrencyService _currency;

    public SimulationService(AppDbContext db, LifeEngineService engine, CurrencyService currency)
    {
        _db = db;
        _engine = engine;
        _currency = currency;
    }

    public async Task<(bool Success, string? Error, object? Result)> SimulateAsync(int profileId, int userId, SimulateDto dto)
    {
        var profile = await _db.Profiles
            .FirstOrDefaultAsync(p => p.Id == profileId && p.UserId == userId);

        if (profile == null) return (false, "Profile not found.", null);

        var targetCurrency = dto.Currency ?? profile.Currency;
        var rate = await _currency.GetRateAsync(profile.Currency, targetCurrency);

        var projection = _engine.GenerateProjection(profile, dto.ProjectionYears, rate);

        var resultJson = JsonSerializer.Serialize(projection);
        var simResult = new SimulationResult
        {
            ProfileId = profileId,
            ProjectionYears = dto.ProjectionYears,
            Currency = targetCurrency,
            ResultJson = resultJson
        };

        _db.SimulationResults.Add(simResult);
        await _db.SaveChangesAsync();

        return (true, null, projection);
    }

    public async Task<List<object>> GetResultsAsync(int profileId, int userId)
    {
        var profileExists = await _db.Profiles.AnyAsync(p => p.Id == profileId && p.UserId == userId);
        if (!profileExists) return new List<object>();

        var results = await _db.SimulationResults
            .Where(s => s.ProfileId == profileId)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();

        return results.Select(r => (object)new
        {
            r.Id,
            r.ProfileId,
            r.ProjectionYears,
            r.Currency,
            resultData = JsonSerializer.Deserialize<object>(r.ResultJson),
            r.CreatedAt
        }).ToList();
    }
}
