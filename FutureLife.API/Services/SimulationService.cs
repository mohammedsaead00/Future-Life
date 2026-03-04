using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using FutureLife.API.Data;
using FutureLife.API.DTOs;
using FutureLife.API.Models;

namespace FutureLife.API.Services;

public class SimulationService
{
    private readonly AppDbContext _db;
    private readonly LifeEngineService _engine;
    private static readonly JsonSerializerOptions _json = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public SimulationService(AppDbContext db, LifeEngineService engine)
    {
        _db = db;
        _engine = engine;
    }

    // ── Run only (no save) ────────────────────────────────────

    public SimulationResultFullDto Run(SimulationInputDto input) =>
        _engine.RunSimulation(input);

    public ParallelFuturesDto RunParallel(SimulationInputDto input) =>
        _engine.GenerateParallelFutures(input);

    // ── Save ──────────────────────────────────────────────────

    public async Task<SimulationResultFullDto> SaveAsync(int userId, string name, SimulationResultFullDto result)
    {
        var entity = MapToEntity(userId, name, result);
        _db.SimulationResults.Add(entity);
        await _db.SaveChangesAsync();
        return MapToDto(entity, result.YearlySnapshots, result.MonthlySnapshots);
    }

    // ── History ───────────────────────────────────────────────

    public async Task<List<SimulationResultFullDto>> GetHistoryAsync(int userId)
    {
        var list = await _db.SimulationResults
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        return list.Select(ToDtoFromEntity).ToList();
    }

    // ── Get by id ─────────────────────────────────────────────

    public async Task<SimulationResultFullDto?> GetByIdAsync(int id, int userId)
    {
        var entity = await _db.SimulationResults.FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);
        return entity == null ? null : ToDtoFromEntity(entity);
    }

    // ── Delete ────────────────────────────────────────────────

    public async Task<bool> DeleteAsync(int id, int userId)
    {
        var entity = await _db.SimulationResults.FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);
        if (entity == null) return false;
        _db.SimulationResults.Remove(entity);
        await _db.SaveChangesAsync();
        return true;
    }

    // ── Stats ─────────────────────────────────────────────────

    public async Task<SimulationStatsDto> GetStatsAsync(int userId)
    {
        var results = await _db.SimulationResults.Where(r => r.UserId == userId).ToListAsync();
        return new SimulationStatsDto(
            TotalScenarios:      results.Count,
            BestLifeStrategyScore: results.Any() ? results.Max(r => r.LifeStrategyScore) : null,
            BestSavings10Y:      results.Any() ? results.Max(r => r.Savings10Y) : null,
            BestCareerField:     null,
            LastSimulationAt:    results.Any() ? results.Max(r => r.CreatedAt) : null
        );
    }

    // ── Mapping helpers ───────────────────────────────────────

    private static SimulationResult MapToEntity(int userId, string name, SimulationResultFullDto r) => new()
    {
        UserId = userId, Name = name,
        Savings1Y = r.Savings1Y, Savings5Y = r.Savings5Y, Savings10Y = r.Savings10Y,
        MonthlySavings = r.MonthlySavings, NetWorth10Y = r.NetWorth10Y, Currency = r.Currency,
        StudyHours1Y = r.StudyHours1Y, StudyHours5Y = r.StudyHours5Y, StudyHours10Y = r.StudyHours10Y,
        HealthScore1Y = r.HealthScore1Y, HealthScore5Y = r.HealthScore5Y, HealthScore10Y = r.HealthScore10Y,
        CareerGrowthIndex = r.CareerGrowthIndex, SalaryMultiplier = r.SalaryMultiplier,
        PromotionProbability = r.PromotionProbability,
        SocialBalanceScore = r.SocialBalanceScore, IsolationRisk = r.IsolationRisk,
        LifeStrategyScore = r.LifeStrategyScore,
        EnergyScore1Y = r.EnergyScore1Y, EnergyScore5Y = r.EnergyScore5Y, EnergyScore10Y = r.EnergyScore10Y,
        BurnoutRisk = r.BurnoutRisk,
        FinancialCollapseRisk = r.FinancialCollapseRisk, CareerStagnationRisk = r.CareerStagnationRisk,
        EnergyDepletionRisk = r.EnergyDepletionRisk, OverallRiskIndex = r.OverallRiskIndex,
        YearlySnapshotsJson  = JsonSerializer.Serialize(r.YearlySnapshots, _json),
        MonthlySnapshotsJson = JsonSerializer.Serialize(r.MonthlySnapshots, _json)
    };

    private static SimulationResultFullDto MapToDto(SimulationResult e,
        List<YearlySnapshotDto>? yearly = null, List<MonthlySnapshotDto>? monthly = null) => new(
        e.Id, e.UserId, e.Name,
        e.Savings1Y, e.Savings5Y, e.Savings10Y, e.MonthlySavings, e.NetWorth10Y, e.Currency,
        e.StudyHours1Y, e.StudyHours5Y, e.StudyHours10Y,
        e.HealthScore1Y, e.HealthScore5Y, e.HealthScore10Y,
        e.CareerGrowthIndex, e.SalaryMultiplier, e.PromotionProbability,
        e.SocialBalanceScore, e.IsolationRisk,
        e.LifeStrategyScore,
        e.EnergyScore1Y, e.EnergyScore5Y, e.EnergyScore10Y,
        e.BurnoutRisk,
        e.FinancialCollapseRisk, e.CareerStagnationRisk, e.EnergyDepletionRisk, e.OverallRiskIndex,
        yearly  ?? JsonSerializer.Deserialize<List<YearlySnapshotDto>>(e.YearlySnapshotsJson,  _json)  ?? new(),
        monthly ?? JsonSerializer.Deserialize<List<MonthlySnapshotDto>>(e.MonthlySnapshotsJson, _json) ?? new(),
        e.CreatedAt
    );

    private static SimulationResultFullDto ToDtoFromEntity(SimulationResult e) => MapToDto(e);
}
