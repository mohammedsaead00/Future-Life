using Microsoft.EntityFrameworkCore;
using FutureLife.API.Models;

namespace FutureLife.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Profile> Profiles => Set<Profile>();
    public DbSet<SimulationResult> SimulationResults => Set<SimulationResult>();
    public DbSet<ExchangeRate> ExchangeRates => Set<ExchangeRate>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User
        modelBuilder.Entity<User>(e =>
        {
            e.HasIndex(u => u.Email).IsUnique();
        });

        // Profile
        modelBuilder.Entity<Profile>(e =>
        {
            e.Property(p => p.CurrentSavings).HasColumnType("decimal(18,2)");
            e.Property(p => p.MonthlyIncome).HasColumnType("decimal(18,2)");
            e.Property(p => p.MonthlyExpenses).HasColumnType("decimal(18,2)");
            e.Property(p => p.InvestmentReturnRate).HasColumnType("decimal(5,4)");
            e.Property(p => p.InflationRate).HasColumnType("decimal(5,4)");
            e.Property(p => p.CurrentSalary).HasColumnType("decimal(18,2)");
            e.Property(p => p.AnnualSalaryGrowthRate).HasColumnType("decimal(5,4)");
            e.Property(p => p.PromotionProbability).HasColumnType("decimal(5,4)");
            e.Property(p => p.PromotionSalaryBoost).HasColumnType("decimal(5,4)");
            e.Property(p => p.BMI).HasColumnType("decimal(5,2)");

            e.HasOne(p => p.User)
             .WithMany(u => u.Profiles)
             .HasForeignKey(p => p.UserId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // SimulationResult
        modelBuilder.Entity<SimulationResult>(e =>
        {
            e.HasOne(s => s.Profile)
             .WithMany(p => p.SimulationResults)
             .HasForeignKey(s => s.ProfileId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // ExchangeRate
        modelBuilder.Entity<ExchangeRate>(e =>
        {
            e.HasIndex(er => er.CurrencyCode).IsUnique();
            e.Property(er => er.RateToUsd).HasColumnType("decimal(18,6)");
        });

        // Seed exchange rates
        modelBuilder.Entity<ExchangeRate>().HasData(
            new ExchangeRate { Id = 1, CurrencyCode = "USD", RateToUsd = 1.0m, UpdatedAt = new DateTime(2026, 3, 2, 0, 0, 0, DateTimeKind.Utc) },
            new ExchangeRate { Id = 2, CurrencyCode = "EGP", RateToUsd = 0.0204m, UpdatedAt = new DateTime(2026, 3, 2, 0, 0, 0, DateTimeKind.Utc) },
            new ExchangeRate { Id = 3, CurrencyCode = "SAR", RateToUsd = 0.2667m, UpdatedAt = new DateTime(2026, 3, 2, 0, 0, 0, DateTimeKind.Utc) },
            new ExchangeRate { Id = 4, CurrencyCode = "AED", RateToUsd = 0.2723m, UpdatedAt = new DateTime(2026, 3, 2, 0, 0, 0, DateTimeKind.Utc) },
            new ExchangeRate { Id = 5, CurrencyCode = "KWD", RateToUsd = 3.2614m, UpdatedAt = new DateTime(2026, 3, 2, 0, 0, 0, DateTimeKind.Utc) },
            new ExchangeRate { Id = 6, CurrencyCode = "QAR", RateToUsd = 0.2747m, UpdatedAt = new DateTime(2026, 3, 2, 0, 0, 0, DateTimeKind.Utc) }
        );
    }
}
