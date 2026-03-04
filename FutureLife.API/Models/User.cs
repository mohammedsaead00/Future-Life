using System.ComponentModel.DataAnnotations;

namespace FutureLife.API.Models;

public class User
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required, MaxLength(200), EmailAddress]
    public string Email { get; set; } = string.Empty;

    // Nullable to support Google OAuth users (no password)
    public string? PasswordHash { get; set; }

    [MaxLength(100)]
    public string? GoogleId { get; set; }

    public string? PreferredCurrency { get; set; } = "USD";

    public string? Avatar { get; set; }

    public DateTime? LastLoginAt { get; set; }

    public string? RefreshToken { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Profile> Profiles { get; set; } = new List<Profile>();
    public ICollection<SimulationResult> SimulationResults { get; set; } = new List<SimulationResult>();
}
