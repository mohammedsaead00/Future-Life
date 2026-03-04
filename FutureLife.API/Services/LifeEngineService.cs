using FutureLife.API.DTOs;
using System.Text.Json;

namespace FutureLife.API.Services;

public class LifeEngineService
{
    private static readonly Dictionary<string, double> CareerGrowthMultipliers = new()
    {
        { "Technology",  1.25 },
        { "Finance",     1.18 },
        { "Healthcare",  1.12 },
        { "Education",   1.05 },
        { "Creative",    1.10 },
        { "Engineering", 1.20 },
        { "Other",       1.08 }
    };

    // ── Main entry points ─────────────────────────────────────

    public SimulationResultFullDto RunSimulation(SimulationInputDto input, string name = "Unnamed Scenario")
    {
        var finance  = CalculateFinance(input);
        var knowledge = CalculateKnowledge(input);
        var health   = CalculateHealth(input);
        var career   = CalculateCareer(input);
        var social   = CalculateSocial(input);
        var energy   = CalculateEnergy(input);
        var risks    = CalculateRisks(input, finance, health, career);
        var strategy = CalculateLifeStrategy(finance, knowledge, health, career, social, energy, risks);

        var yearly  = BuildYearlySnapshots(input);
        var monthly = BuildMonthlySnapshots(input);

        return new SimulationResultFullDto(
            Id: 0, UserId: 0, Name: name,
            // Financial
            Savings1Y:      finance.Savings1Y,
            Savings5Y:      finance.Savings5Y,
            Savings10Y:     finance.Savings10Y,
            MonthlySavings: finance.MonthlySavings,
            NetWorth10Y:    finance.NetWorth10Y,
            Currency:       input.Currency,
            // Knowledge
            StudyHours1Y:   knowledge.Hours1Y,
            StudyHours5Y:   knowledge.Hours5Y,
            StudyHours10Y:  knowledge.Hours10Y,
            // Health
            HealthScore1Y:  health.Score1Y,
            HealthScore5Y:  health.Score5Y,
            HealthScore10Y: health.Score10Y,
            // Career
            CareerGrowthIndex:    career.GrowthIndex,
            SalaryMultiplier:     career.SalaryMultiplier,
            PromotionProbability: career.PromotionProb,
            // Social
            SocialBalanceScore: social.BalanceScore,
            IsolationRisk:      social.IsolationRisk,
            // Energy & Strategy
            LifeStrategyScore: strategy,
            EnergyScore1Y:     energy.Score1Y,
            EnergyScore5Y:     energy.Score5Y,
            EnergyScore10Y:    energy.Score10Y,
            BurnoutRisk:       energy.BurnoutRisk,
            // Risks
            FinancialCollapseRisk:  risks.FinancialCollapse,
            CareerStagnationRisk:   risks.CareerStagnation,
            EnergyDepletionRisk:    risks.EnergyDepletion,
            OverallRiskIndex:       risks.OverallRisk,
            // Charts
            YearlySnapshots:  yearly,
            MonthlySnapshots: monthly,
            CreatedAt: DateTime.UtcNow
        );
    }

    public ParallelFuturesDto GenerateParallelFutures(SimulationInputDto input)
    {
        var current = RunSimulation(input, "Current Path");

        // Optimized: +20% study, +15% networking, +5% saving capped at 50%, workout +1
        var optimizedInput = input with
        {
            DailyStudyHours   = Math.Min(input.DailyStudyHours + 2.0, 10.0),
            SavingPercentage  = Math.Min(input.SavingPercentage + 0.05, 0.50),
            WorkoutDaysPerWeek = Math.Min(input.WorkoutDaysPerWeek + 1, 7),
            NetworkingHours   = Math.Min(input.NetworkingHours + 1.0, 10.0),
            WeeklySkillHours  = Math.Min(input.WeeklySkillHours + 3.0, 20.0),
            SocialMediaHours  = Math.Max(input.SocialMediaHours - 1.0, 0.0)
        };
        var optimized = RunSimulation(optimizedInput, "Optimized Path");

        // Decline: less effort across the board
        var declineInput = input with
        {
            DailyStudyHours    = Math.Max(input.DailyStudyHours - 1.0, 0.0),
            SavingPercentage   = Math.Max(input.SavingPercentage - 0.05, 0.0),
            WorkoutDaysPerWeek = Math.Max(input.WorkoutDaysPerWeek - 1, 0),
            NetworkingHours    = Math.Max(input.NetworkingHours - 1.0, 0.0),
            WeeklySkillHours   = Math.Max(input.WeeklySkillHours - 2.0, 0.0),
            SocialMediaHours   = Math.Min(input.SocialMediaHours + 2.0, 10.0)
        };
        var decline = RunSimulation(declineInput, "Decline Path");

        return new ParallelFuturesDto(current, optimized, decline);
    }

    // ── Finance ───────────────────────────────────────────────

    private record FinanceResult(double Savings1Y, double Savings5Y, double Savings10Y,
        double MonthlySavings, double NetWorth10Y);

    private static FinanceResult CalculateFinance(SimulationInputDto i)
    {
        double monthlySavings = i.MonthlyIncome * i.SavingPercentage;
        double investmentRate = 0.07; // assumed annual return
        double s = 0;
        double s1 = 0, s5 = 0, s10 = 0;
        for (int y = 1; y <= 10; y++)
        {
            s = (s + monthlySavings * 12) * (1 + investmentRate);
            if (y == 1)  s1  = s;
            if (y == 5)  s5  = s;
            if (y == 10) s10 = s;
        }
        return new(Round(s1), Round(s5), Round(s10), Round(monthlySavings), Round(s10));
    }

    // ── Knowledge ─────────────────────────────────────────────

    private record KnowledgeResult(double Hours1Y, double Hours5Y, double Hours10Y);

    private static KnowledgeResult CalculateKnowledge(SimulationInputDto i)
    {
        double dailyHours = i.DailyStudyHours;
        return new(Round(dailyHours * 365), Round(dailyHours * 365 * 5), Round(dailyHours * 365 * 10));
    }

    // ── Health ────────────────────────────────────────────────

    private record HealthResult(double Score1Y, double Score5Y, double Score10Y);

    private static HealthResult CalculateHealth(SimulationInputDto i)
    {
        double base_ = (i.WorkoutDaysPerWeek / 7.0) * 60.0
                     + (i.FamilyHours / 10.0) * 20.0
                     + Math.Max(0, (8 - i.SocialMediaHours) / 8.0) * 20.0;
        base_ = Math.Min(100, base_);
        return new(Round(base_), Round(base_ * 0.98), Round(base_ * 0.95));
    }

    // ── Career ────────────────────────────────────────────────

    private record CareerResult(double GrowthIndex, double SalaryMultiplier, double PromotionProb);

    private static CareerResult CalculateCareer(SimulationInputDto i)
    {
        double fieldMultiplier = CareerGrowthMultipliers.GetValueOrDefault(i.CareerField ?? "Other", 1.08);
        double skillScore = Math.Min(i.WeeklySkillHours / 20.0, 1.0) * 0.5
                          + Math.Min(i.CertsPerYear / 5.0, 1.0) * 0.3
                          + Math.Min(i.NetworkingHours / 10.0, 1.0) * 0.2;
        double growthIndex     = Round(skillScore * fieldMultiplier, 4);
        double salaryMultiplier = Round(1 + (growthIndex * 0.6 * 10), 2); // 10-year multiplier
        double promotionProb   = Round(Math.Min(0.95, skillScore * fieldMultiplier * 0.5), 4);
        return new(growthIndex, salaryMultiplier, promotionProb);
    }

    // ── Social ────────────────────────────────────────────────

    private record SocialResult(double BalanceScore, double IsolationRisk);

    private static SocialResult CalculateSocial(SimulationInputDto i)
    {
        double balance = (i.FamilyHours / 10.0) * 40.0
                       + (i.NetworkingHours / 10.0) * 35.0
                       + Math.Max(0, (5 - i.SocialMediaHours) / 5.0) * 25.0;
        balance = Math.Min(100, balance);
        double isolation = Round(Math.Max(0, 1.0 - balance / 100.0), 4);
        return new(Round(balance, 1), isolation);
    }

    // ── Energy ────────────────────────────────────────────────

    private record EnergyResult(double Score1Y, double Score5Y, double Score10Y, double BurnoutRisk);

    private static EnergyResult CalculateEnergy(SimulationInputDto i)
    {
        double overwork = i.DailyStudyHours + i.WeeklySkillHours / 7.0;
        double base_ = 100 - (overwork / 16.0 * 40)
                           - (i.SocialMediaHours / 10.0 * 20)
                           + (i.WorkoutDaysPerWeek / 7.0 * 30);
        base_ = Math.Clamp(base_, 0, 100);
        double burnout = Round(Math.Clamp((overwork / 16.0 * 0.6) + (i.SocialMediaHours / 10.0 * 0.2) - (i.WorkoutDaysPerWeek / 7.0 * 0.2), 0, 1), 4);
        return new(Round(base_), Round(base_ * 0.97), Round(base_ * 0.93), burnout);
    }

    // ── Risks ─────────────────────────────────────────────────

    private record RiskResult(double FinancialCollapse, double CareerStagnation, double EnergyDepletion, double OverallRisk);

    private static RiskResult CalculateRisks(SimulationInputDto i, FinanceResult f, HealthResult h, CareerResult c)
    {
        double financialRisk = Round(Math.Clamp(1 - (i.SavingPercentage / 0.5), 0, 1), 4);
        double careerRisk    = Round(Math.Clamp(1 - c.GrowthIndex, 0, 1), 4);
        double energyRisk    = Round(Math.Clamp(1 - h.Score10Y / 100.0, 0, 1), 4);
        double overall       = Round((financialRisk * 0.4 + careerRisk * 0.35 + energyRisk * 0.25), 4);
        return new(financialRisk, careerRisk, energyRisk, overall);
    }

    private static double CalculateLifeStrategy(FinanceResult f, KnowledgeResult k, HealthResult h,
        CareerResult c, SocialResult s, EnergyResult e, RiskResult r)
    {
        return Round(
            (f.Savings10Y > 0 ? Math.Min(f.Savings10Y / 500_000, 1) * 25 : 0) +
            (Math.Min(k.Hours10Y / 10000, 1) * 20) +
            (h.Score10Y / 100 * 20) +
            (c.GrowthIndex * 20) +
            (s.BalanceScore / 100 * 15)
        , 2);
    }

    // ── Chart Data ────────────────────────────────────────────

    private static List<YearlySnapshotDto> BuildYearlySnapshots(SimulationInputDto i)
    {
        var snaps = new List<YearlySnapshotDto>();
        double s = 0, monthlySavings = i.MonthlyIncome * i.SavingPercentage;
        double healthBase = (i.WorkoutDaysPerWeek / 7.0 * 100);
        double energyBase = Math.Clamp(100 - (i.DailyStudyHours / 16 * 40) + (i.WorkoutDaysPerWeek / 7.0 * 30), 0, 100);
        for (int y = 1; y <= 10; y++)
        {
            s = (s + monthlySavings * 12) * 1.07;
            snaps.Add(new YearlySnapshotDto(
                Year:        y,
                Savings:     Round(s),
                StudyHours:  Round(i.DailyStudyHours * 365 * y),
                HealthScore: Round(healthBase * Math.Pow(0.99, y - 1)),
                EnergyScore: Round(energyBase * Math.Pow(0.98, y - 1))
            ));
        }
        return snaps;
    }

    private static List<MonthlySnapshotDto> BuildMonthlySnapshots(SimulationInputDto i)
    {
        var snaps = new List<MonthlySnapshotDto>();
        double s = 0, monthlySavings = i.MonthlyIncome * i.SavingPercentage;
        double healthBase = (i.WorkoutDaysPerWeek / 7.0 * 100);
        for (int m = 1; m <= 12; m++)
        {
            s += monthlySavings;
            snaps.Add(new MonthlySnapshotDto(
                Month:       m,
                Savings:     Round(s),
                StudyHours:  Round(i.DailyStudyHours * 30 * m),
                HealthScore: Round(healthBase)
            ));
        }
        return snaps;
    }

    private static double Round(double v, int dec = 2) => Math.Round(v, dec);
}
