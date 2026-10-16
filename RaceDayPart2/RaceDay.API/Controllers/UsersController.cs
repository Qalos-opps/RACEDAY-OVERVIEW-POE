using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.DTOs;
namespace RaceDay.API.Controllers;

[ApiController, Route("api/users")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly RaceDayDbContext _db;
    public UsersController(RaceDayDbContext db) => _db = db;
    [HttpGet("me")]
    public async Task<ActionResult<UserProfileDto>> Me()
    {
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id)) return Unauthorized();
        var user = await _db.Users.FindAsync(id);
        if (user is null) return NotFound();
        return new UserProfileDto(user.UserID, user.FirstName, user.LastName, user.Email, user.Role, user.PhoneNumber);
    }
    [HttpPut("me")]
    public async Task<IActionResult> UpdateMe(UpdateProfileRequest request)
    {
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id)) return Unauthorized();
        var user = await _db.Users.FindAsync(id);
        if (user is null) return NotFound();
        user.FirstName = request.FirstName.Trim(); user.LastName = request.LastName.Trim(); user.PhoneNumber = request.PhoneNumber?.Trim();
        await _db.SaveChangesAsync();
        return Ok(new UserProfileDto(user.UserID, user.FirstName, user.LastName, user.Email, user.Role, user.PhoneNumber));
    }
}
public record UpdateProfileRequest(string FirstName, string LastName, string? PhoneNumber);
