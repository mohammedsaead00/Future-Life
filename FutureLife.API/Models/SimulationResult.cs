using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace FutureLife.API.Models;

public class SimulationResult
{
    public int Id { get; set; }

    public int ProfileId { get; set; }
    public Profile Profile { get; set; } = null!;

    public int ProjectionYears { get; set; }
    public string Currency { get; set; } = "USD";

    // Stored as JSON
    public string ResultJson { get; set; } = "{}";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
