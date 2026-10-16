using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.DTOs;
using RaceDay.API.Models;
namespace RaceDay.API.Controllers;

[ApiController, Route("api/events")]
public class EventsController : ControllerBase
{
    private readonly RaceDayDbContext _db;
    public EventsController(RaceDayDbContext db) => _db = db;

    [AllowAnonymous, HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _db.Events.AsNoTracking()
        .Select(e => new { e.EventID, e.EventName, e.Description, e.Location, e.EventDate, e.RegistrationDeadline, e.Status }).ToListAsync());

    [AllowAnonymous, HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var item = await _db.Events.AsNoTracking().Where(e => e.EventID == id)
            .Select(e => new { e.EventID, e.EventName, e.Description, e.Location, e.EventDate, e.RegistrationDeadline, e.Status }).SingleOrDefaultAsync();
        return item is null ? NotFound() : Ok(item);
    }

    [Authorize(Roles = "Organiser"), HttpPost]
    public async Task<IActionResult> Create(EventCreateRequest request)
    {
        if (request.EventDate.Date < DateTime.Today || request.RegistrationDeadline.Date > request.EventDate.Date)
            return BadRequest(new { message = "Event date must not be in the past, and registration must close by the event date." });
        if (!new[] { "Upcoming", "Open", "Closed", "Completed", "Cancelled" }.Contains(request.Status, StringComparer.OrdinalIgnoreCase))
            return BadRequest(new { message = "Invalid event status." });
        var organiserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var e = new Event { OrganiserID = organiserId, EventName = request.EventName.Trim(), Description = request.Description, Location = request.Location.Trim(), EventDate = request.EventDate.Date, RegistrationDeadline = request.RegistrationDeadline.Date, Status = request.Status, CreatedAt = DateTime.Now };
        _db.Events.Add(e); await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = e.EventID }, new { e.EventID, e.EventName });
    }

    [Authorize(Roles = "Organiser"), HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, EventCreateRequest request)
    {
        var e = await _db.Events.FindAsync(id); if (e is null) return NotFound();
        if (request.EventDate.Date < DateTime.Today || request.RegistrationDeadline.Date > request.EventDate.Date) return BadRequest();
        if (!new[] { "Upcoming", "Open", "Closed", "Completed", "Cancelled" }.Contains(request.Status, StringComparer.OrdinalIgnoreCase)) return BadRequest(new { message = "Invalid event status." });
        e.EventName = request.EventName.Trim(); e.Description = request.Description; e.Location = request.Location.Trim(); e.EventDate = request.EventDate.Date; e.RegistrationDeadline = request.RegistrationDeadline.Date; e.Status = request.Status;
        await _db.SaveChangesAsync(); return Ok(new { e.EventID, e.EventName, e.Status });
    }

    [Authorize(Roles = "Organiser"), HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var e = await _db.Events.FindAsync(id); if (e is null) return NotFound();
        if (await _db.Enrolments.AnyAsync(x => x.Category!.EventID == id) || await _db.Weather.AnyAsync(w => w.EventID == id)) return Conflict(new { message = "This event has enrolments or weather records and cannot be deleted." });
        _db.Events.Remove(e); await _db.SaveChangesAsync(); return NoContent();
    }

    [AllowAnonymous, HttpGet("{eventId:int}/categories")]
    public async Task<IActionResult> Categories(int eventId)
    {
        if (!await _db.Events.AnyAsync(e => e.EventID == eventId)) return NotFound();
        return Ok(await _db.Categories.AsNoTracking().Where(c => c.EventID == eventId).Select(c => new { c.CategoryID, c.EventID, c.CategoryName, c.DistanceKm, c.EntryFee, c.MaximumParticipants }).ToListAsync());
    }

    [Authorize(Roles = "Organiser"), HttpPost("{eventId:int}/categories")]
    public async Task<IActionResult> AddCategory(int eventId, CategoryCreateRequest request)
    {
        if (!await _db.Events.AnyAsync(e => e.EventID == eventId)) return NotFound(new { message = "Event not found." });
        if (request.DistanceKm <= 0 || request.EntryFee < 0 || request.MaximumParticipants < 1) return BadRequest(new { message = "Distance and maximum participants must be positive; fee cannot be negative." });
        var c = new Category { EventID = eventId, CategoryName = request.CategoryName.Trim(), DistanceKm = request.DistanceKm, EntryFee = request.EntryFee, MaximumParticipants = request.MaximumParticipants };
        _db.Categories.Add(c); await _db.SaveChangesAsync(); return Created($"/api/events/{eventId}/categories", new { c.CategoryID, c.EventID, c.CategoryName });
    }
}
