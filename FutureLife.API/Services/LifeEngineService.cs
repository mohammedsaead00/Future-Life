using FutureLife.API.Models;

namespace FutureLife.API.Services;

public class LifeEngineService
{
    public FinanceProjection CalculateFinance(Profile profile, int years, decimal exchangeRate = 1m)
    {
        var yearlyData = new List<YearlyFinanceData>();
        decimal savings = profile.CurrentSavings;
        decimal income = profile.MonthlyIncome * 12;
        decimal expenses = profile.MonthlyExpenses * 12;
        decimal rate = profile.InvestmentReturnRate;
        decimal inflation = profile.InflationRate;

        for (int y = 1; y <= years; y++)
        {
            decimal netContribution = income - expenses;
            savings = (savings + netContribution) * (1 + rate);
            decimal realSavings = savings / (decimal)Math.Pow((double)(1 + inflation), y);
            decimal netWorth = savings;

            yearlyData.Add(new YearlyFinanceData
            {
                Year = y,
                NominalSavings = Math.Round(savings * exchangeRate, 2),
                InflationAdjustedSavings = Math.Round(realSavings * exchangeRate, 2),
                NetWorth = Math.Round(netWorth * exchangeRate, 2)
            });
        }

        return new FinanceProjection
        {
            InitialSavings = profile.CurrentSavings * exchangeRate,
            FinalNetWorth = yearlyData.Last().NetWorth,
            InflationAdjustedFinalNetWorth = yearlyData.Last().InflationAdjustedSavings,
            YearlyBreakdown = yearlyData
        };
    }

    public CareerProjection CalculateCareer(Profile profile, int years, decimal exchangeRate = 1m)
    {
        var yearlyData = new List<YearlyCareerData>();
        decimal salary = profile.CurrentSalary;
        double totalLifetimeEarnings = 0;

        for (int y = 1; y <= years; y++)
        {
            salary *= (1 + profile.AnnualSalaryGrowthRate);
            bool promoted = new Random(y).NextDouble() < (double)profile.PromotionProbability;
            if (promoted) salary *= (1 + profile.PromotionSalaryBoost);

            totalLifetimeEarnings += (double)(salary * exchangeRate);

            yearlyData.Add(new YearlyCareerData
            {
                Year = y,
                Salary = Math.Round(salary * exchangeRate, 2),
                WasPromoted = promoted
            });
        }

        return new CareerProjection
        {
            InitialSalary = profile.CurrentSalary * exchangeRate,
            FinalSalary = yearlyData.Last().Salary,
            TotalLifetimeEarnings = Math.Round((decimal)totalLifetimeEarnings, 2),
            ExpectedPromotions = yearlyData.Count(d => d.WasPromoted),
            YearlyBreakdown = yearlyData
        };
    }

    public HealthProjection CalculateHealth(Profile profile, int years)
    {
        // Energy score formula: sleep (max 30pts) + exercise (max 40pts) - stress (max 30pts)
        double sleepScore = Math.Min(profile.SleepHoursPerNight / 8.0, 1.0) * 30;
        double exerciseScore = Math.Min(profile.ExerciseDaysPerWeek / 5.0, 1.0) * 40;
        double stressScore = (10 - profile.StressLevel) / 10.0 * 30;
        double energyScore = sleepScore + exerciseScore + stressScore;

        // Burnout risk: high stress + low sleep + low exercise
        double burnoutRisk = (profile.StressLevel / 10.0 * 0.5)
                           + ((8 - profile.SleepHoursPerNight) / 8.0 * 0.3)
                           + ((5 - profile.ExerciseDaysPerWeek) / 5.0 * 0.2);
        burnoutRisk = Math.Max(0, Math.Min(1, burnoutRisk));

        var yearlyData = new List<YearlyHealthData>();
        for (int y = 1; y <= years; y++)
        {
            // Energy declines slightly with age
            double ageFactor = 1.0 - (y * 0.005);
            yearlyData.Add(new YearlyHealthData
            {
                Year = y,
                EnergyScore = Math.Round(energyScore * ageFactor, 2),
                BurnoutRisk = Math.Round(burnoutRisk + (y * 0.002), 4)
            });
        }

        return new HealthProjection
        {
            CurrentEnergyScore = Math.Round(energyScore, 2),
            CurrentBurnoutRisk = Math.Round(burnoutRisk, 4),
            BurnoutRiskLevel = burnoutRisk < 0.3 ? "Low" : burnoutRisk < 0.6 ? "Medium" : "High",
            HealthRating = energyScore >= 70 ? "Excellent" : energyScore >= 50 ? "Good" : energyScore >= 30 ? "Fair" : "Poor",
            YearlyBreakdown = yearlyData
        };
    }

    public SocialProjection CalculateSocial(Profile profile, int years)
    {
        // Isolation risk: low friends + low interactions + low community
        double isolationRisk = 1.0
            - (Math.Min(profile.CloseFriendsCount / 5.0, 1.0) * 0.4)
            - (Math.Min(profile.SocialInteractionsPerWeek / 7.0, 1.0) * 0.35)
            - (profile.CommunityEngagementScore / 10.0 * 0.25);
        isolationRisk = Math.Max(0, Math.Min(1, isolationRisk));

        // Influence growth index: community × interactions growth
        double influenceGrowth = (profile.CommunityEngagementScore / 10.0 * 0.5)
                               + (Math.Min(profile.SocialInteractionsPerWeek / 14.0, 1.0) * 0.3)
                               + (Math.Min(profile.CloseFriendsCount / 10.0, 1.0) * 0.2);

        var yearlyData = new List<YearlySocialData>();
        for (int y = 1; y <= years; y++)
        {
            yearlyData.Add(new YearlySocialData
            {
                Year = y,
                IsolationRisk = Math.Round(Math.Max(0, isolationRisk - (y * 0.005)), 4),
                InfluenceGrowthIndex = Math.Round(influenceGrowth * (1 + y * 0.03), 4)
            });
        }

        return new SocialProjection
        {
            CurrentIsolationRisk = Math.Round(isolationRisk, 4),
            IsolationRiskLevel = isolationRisk < 0.3 ? "Low" : isolationRisk < 0.6 ? "Medium" : "High",
            CurrentInfluenceGrowthIndex = Math.Round(influenceGrowth, 4),
            YearlyBreakdown = yearlyData
        };
    }

    public LifeProjection GenerateProjection(Profile profile, int years, decimal exchangeRate = 1m)
    {
        return new LifeProjection
        {
            ProfileId = profile.Id,
            ProfileName = profile.Name,
            ProjectionYears = years,
            GeneratedAt = DateTime.UtcNow,
            Finance = CalculateFinance(profile, years, exchangeRate),
            Career = CalculateCareer(profile, years, exchangeRate),
            Health = CalculateHealth(profile, years),
            Social = CalculateSocial(profile, years)
        };
    }
}

// ---- Result Models ----

public class LifeProjection
{
    public int ProfileId { get; set; }
    public string ProfileName { get; set; } = string.Empty;
    public int ProjectionYears { get; set; }
    public DateTime GeneratedAt { get; set; }
    public FinanceProjection Finance { get; set; } = null!;
    public CareerProjection Career { get; set; } = null!;
    public HealthProjection Health { get; set; } = null!;
    public SocialProjection Social { get; set; } = null!;
}

public class FinanceProjection
{
    public decimal InitialSavings { get; set; }
    public decimal FinalNetWorth { get; set; }
    public decimal InflationAdjustedFinalNetWorth { get; set; }
    public List<YearlyFinanceData> YearlyBreakdown { get; set; } = new();
}

public class YearlyFinanceData
{
    public int Year { get; set; }
    public decimal NominalSavings { get; set; }
    public decimal InflationAdjustedSavings { get; set; }
    public decimal NetWorth { get; set; }
}

public class CareerProjection
{
    public decimal InitialSalary { get; set; }
    public decimal FinalSalary { get; set; }
    public decimal TotalLifetimeEarnings { get; set; }
    public int ExpectedPromotions { get; set; }
    public List<YearlyCareerData> YearlyBreakdown { get; set; } = new();
}

public class YearlyCareerData
{
    public int Year { get; set; }
    public decimal Salary { get; set; }
    public bool WasPromoted { get; set; }
}

public class HealthProjection
{
    public double CurrentEnergyScore { get; set; }
    public double CurrentBurnoutRisk { get; set; }
    public string BurnoutRiskLevel { get; set; } = string.Empty;
    public string HealthRating { get; set; } = string.Empty;
    public List<YearlyHealthData> YearlyBreakdown { get; set; } = new();
}

public class YearlyHealthData
{
    public int Year { get; set; }
    public double EnergyScore { get; set; }
    public double BurnoutRisk { get; set; }
}

public class SocialProjection
{
    public double CurrentIsolationRisk { get; set; }
    public string IsolationRiskLevel { get; set; } = string.Empty;
    public double CurrentInfluenceGrowthIndex { get; set; }
    public List<YearlySocialData> YearlyBreakdown { get; set; } = new();
}

public class YearlySocialData
{
    public int Year { get; set; }
    public double IsolationRisk { get; set; }
    public double InfluenceGrowthIndex { get; set; }
}
