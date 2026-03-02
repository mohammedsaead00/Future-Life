using Microsoft.AspNetCore.Mvc;
using FutureLife.API.DTOs;
using FutureLife.API.Helpers;
using FutureLife.API.Services;

namespace FutureLife.API.Controllers;

[ApiController]
[Route("profiles")]
public class ProfilesController : ControllerBase
{
    private readonly ProfileService _profiles;

    public ProfilesController(ProfileService profiles)
    {
        _profiles = profiles;
    }

    private int? UserId => HttpContext.Items["UserId"] as int?;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        if (UserId == null) return Unauthorized(ApiResponse.Fail("Unauthorized."));
        var result = await _profiles.GetAllAsync(UserId.Value);
        return Ok(ApiResponse.Success(result));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        if (UserId == null) return Unauthorized(ApiResponse.Fail("Unauthorized."));
        var profile = await _profiles.GetAsync(id, UserId.Value);
        if (profile == null) return NotFound(ApiResponse.Fail("Profile not found."));
        return Ok(ApiResponse.Success(profile));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProfileDto dto)
    {
        if (UserId == null) return Unauthorized(ApiResponse.Fail("Unauthorized."));
        if (!ModelState.IsValid) return BadRequest(ApiResponse.Fail("Invalid input."));

        var profile = await _profiles.CreateAsync(dto, UserId.Value);
        return CreatedAtAction(nameof(GetById), new { id = profile.Id }, ApiResponse.Success(profile, "Profile created."));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateProfileDto dto)
    {
        if (UserId == null) return Unauthorized(ApiResponse.Fail("Unauthorized."));
        if (!ModelState.IsValid) return BadRequest(ApiResponse.Fail("Invalid input."));

        var (success, profile) = await _profiles.UpdateAsync(id, UserId.Value, dto);
        if (!success) return NotFound(ApiResponse.Fail("Profile not found."));
        return Ok(ApiResponse.Success(profile, "Profile updated."));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (UserId == null) return Unauthorized(ApiResponse.Fail("Unauthorized."));
        var deleted = await _profiles.DeleteAsync(id, UserId.Value);
        if (!deleted) return NotFound(ApiResponse.Fail("Profile not found."));
        return Ok(ApiResponse.Success(null, "Profile deleted."));
    }
}
