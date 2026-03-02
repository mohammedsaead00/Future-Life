using System.ComponentModel.DataAnnotations;

namespace FutureLife.API.Models;

public class ExchangeRate
{
    public int Id { get; set; }

    [Required, MaxLength(10)]
    public string CurrencyCode { get; set; } = string.Empty;

    public decimal RateToUsd { get; set; }

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
