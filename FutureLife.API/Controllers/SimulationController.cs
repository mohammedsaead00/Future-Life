using Microsoft.AspNetCore.Mvc;
using FutureLife.API.DTOs;
using FutureLife.API.Helpers;
using FutureLife.API.Services;

namespace FutureLife.API.Controllers;

[ApiController]
[Route("profiles/{profileId:int}")]
public class SimulationController : ControllerBase
{
    private readonly SimulationService _simulation;

    public SimulationController(SimulationService simulation)
    {
        _simulation = simulation;
    }

    private int? UserId => HttpContext.Items["UserId"] as int?;

    [HttpPost("simulate")]
    public async Task<IActionResult> Simulate(int profileId, [FromBody] SimulateDto dto)
    {
        if (UserId == null) return Unauthorized(ApiResponse.Fail("Unauthorized."));
        if (!ModelState.IsValid) return BadRequest(ApiResponse.Fail("Invalid input."));

        var (success, error, result) = await _simulation.SimulateAsync(profileId, UserId.Value, dto);
        if (!success) return NotFound(ApiResponse.Fail(error!));

        return Ok(ApiResponse.Success(result, "Simulation completed."));
    }

    [HttpGet("results")]
    public async Task<IActionResult> GetResults(int profileId)
    {
        if (UserId == null) return Unauthorized(ApiResponse.Fail("Unauthorized."));
        var results = await _simulation.GetResultsAsync(profileId, UserId.Value);
        return Ok(ApiResponse.Success(results));
    }
}
