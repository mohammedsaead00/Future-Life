using Microsoft.AspNetCore.Mvc;
using FutureLife.API.DTOs;
using FutureLife.API.Helpers;
using FutureLife.API.Services;

namespace FutureLife.API.Controllers;

[ApiController]
[Route("api/simulation")]
public class SimulationController : ControllerBase
{
    private readonly SimulationService _simulation;

    public SimulationController(SimulationService simulation)
    {
        _simulation = simulation;
    }

    private int? UserId => HttpContext.Items["UserId"] as int?;

    // ── POST /api/simulation/run ───────────────────────────────

    [HttpPost("run")]
    public IActionResult Run([FromBody] SimulationInputDto dto)
    {
        if (UserId == null) return Unauthorized(ApiResponse.Fail("Unauthorized."));
        if (!ModelState.IsValid) return BadRequest(ApiResponse.Fail("Invalid input."));

        var result = _simulation.Run(dto);
        return Ok(ApiResponse.Success(result, "Simulation completed."));
    }

    // ── POST /api/simulation ───────────────────────────────────

    [HttpPost]
    public async Task<IActionResult> Save([FromBody] SaveSimulationDto dto)
    {
        if (UserId == null) return Unauthorized(ApiResponse.Fail("Unauthorized."));
        if (!ModelState.IsValid) return BadRequest(ApiResponse.Fail("Invalid input."));

        var saved = await _simulation.SaveAsync(UserId.Value, dto.Name, dto.Result);
        return Ok(ApiResponse.Success(saved, "Simulation saved."));
    }

    // ── POST /api/simulation/parallel-futures ──────────────────

    [HttpPost("parallel-futures")]
    public IActionResult ParallelFutures([FromBody] SimulationInputDto dto)
    {
        if (UserId == null) return Unauthorized(ApiResponse.Fail("Unauthorized."));
        if (!ModelState.IsValid) return BadRequest(ApiResponse.Fail("Invalid input."));

        var result = _simulation.RunParallel(dto);
        return Ok(ApiResponse.Success(result, "Parallel futures generated."));
    }

    // ── GET /api/simulation/history ────────────────────────────

    [HttpGet("history")]
    public async Task<IActionResult> History()
    {
        if (UserId == null) return Unauthorized(ApiResponse.Fail("Unauthorized."));

        var results = await _simulation.GetHistoryAsync(UserId.Value);
        return Ok(ApiResponse.Success(results));
    }

    // ── GET /api/simulation/stats ──────────────────────────────

    [HttpGet("stats")]
    public async Task<IActionResult> Stats()
    {
        if (UserId == null) return Unauthorized(ApiResponse.Fail("Unauthorized."));

        var stats = await _simulation.GetStatsAsync(UserId.Value);
        return Ok(ApiResponse.Success(stats));
    }

    // ── GET /api/simulation/{id} ───────────────────────────────

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        if (UserId == null) return Unauthorized(ApiResponse.Fail("Unauthorized."));

        var result = await _simulation.GetByIdAsync(id, UserId.Value);
        if (result == null) return NotFound(ApiResponse.Fail("Simulation not found."));

        return Ok(ApiResponse.Success(result));
    }

    // ── DELETE /api/simulation/{id} ────────────────────────────

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (UserId == null) return Unauthorized(ApiResponse.Fail("Unauthorized."));

        var deleted = await _simulation.DeleteAsync(id, UserId.Value);
        if (!deleted) return NotFound(ApiResponse.Fail("Simulation not found."));

        return Ok(ApiResponse.Success<object>(null, "Simulation deleted."));
    }
}
