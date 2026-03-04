using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FutureLife.API.Models;

public class SimulationResult
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    [MaxLength(200)]
    public string Name { get; set; } = "Unnamed Scenario";

    // ── Financial ────────────────────────────────────────────
    public double Savings1Y  { get; set; }
    public double Savings5Y  { get; set; }
    public double Savings10Y { get; set; }
    public double MonthlySavings { get; set; }
    public double NetWorth10Y { get; set; }
    public string Currency { get; set; } = "USD";

    // ── Knowledge ────────────────────────────────────────────
    public double StudyHours1Y  { get; set; }
    public double StudyHours5Y  { get; set; }
    public double StudyHours10Y { get; set; }

    // ── Health ───────────────────────────────────────────────
    public double HealthScore1Y  { get; set; }
    public double HealthScore5Y  { get; set; }
    public double HealthScore10Y { get; set; }

    // ── Career ───────────────────────────────────────────────
    public double CareerGrowthIndex     { get; set; }
    public double SalaryMultiplier      { get; set; }
    public double PromotionProbability  { get; set; }

    // ── Social ───────────────────────────────────────────────
    public double SocialBalanceScore { get; set; }
    public double IsolationRisk      { get; set; }

    // ── Energy & Strategy ─────────────────────────────────────
    public double LifeStrategyScore  { get; set; }
    public double EnergyScore1Y  { get; set; }
    public double EnergyScore5Y  { get; set; }
    public double EnergyScore10Y { get; set; }
    public double BurnoutRisk    { get; set; }

    // ── Risks ─────────────────────────────────────────────────
    public double FinancialCollapseRisk  { get; set; }
    public double CareerStagnationRisk   { get; set; }
    public double EnergyDepletionRisk    { get; set; }
    public double OverallRiskIndex       { get; set; }

    // ── Chart Data (stored as JSON strings) ──────────────────
    [Column(TypeName = "nvarchar(max)")]
    public string YearlySnapshotsJson  { get; set; } = "[]";

    [Column(TypeName = "nvarchar(max)")]
    public string MonthlySnapshotsJson { get; set; } = "[]";

    // ── Input snapshot (store input as JSON for reference) ───
    [Column(TypeName = "nvarchar(max)")]
    public string InputJson { get; set; } = "{}";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
