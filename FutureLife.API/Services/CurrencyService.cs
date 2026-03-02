using Microsoft.EntityFrameworkCore;
using FutureLife.API.Data;
using FutureLife.API.DTOs;

namespace FutureLife.API.Services;

public class CurrencyService
{
    private readonly AppDbContext _db;

    public CurrencyService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<ExchangeRateDto>> GetAllRatesAsync() =>
        await _db.ExchangeRates
            .Select(r => new ExchangeRateDto(r.Id, r.CurrencyCode, r.RateToUsd, r.UpdatedAt))
            .ToListAsync();

    public async Task<decimal> GetRateAsync(string fromCurrency, string toCurrency)
    {
        if (fromCurrency == toCurrency) return 1m;

        var rates = await _db.ExchangeRates.ToListAsync();
        var fromRate = rates.FirstOrDefault(r => r.CurrencyCode == fromCurrency)?.RateToUsd ?? 1m;
        var toRate = rates.FirstOrDefault(r => r.CurrencyCode == toCurrency)?.RateToUsd ?? 1m;

        // Convert: from -> USD -> to
        return toRate / fromRate;
    }
}
