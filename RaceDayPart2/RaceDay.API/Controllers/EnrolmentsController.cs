using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.DTOs;
using RaceDay.API.Models;
namespace RaceDay.API.Controllers;

[ApiController, Route("api/enrolments"), Authorize(Roles = "Participant")]
public class EnrolmentsController : ControllerBase
{
    private readonly RaceDayDbContext _db;
    public EnrolmentsController(RaceDayDbContext db) => _db = db;
    [HttpPost]
    public async Task<IActionResult> Create(EnrolmentCreateRequest request)
    {
        var participantId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var category = await _db.Categories.Include(c => c.Event).SingleOrDefaultAsync(c => c.CategoryID == request.CategoryID);
        if (category is null) return NotFound(new { message = "Category not found." });
        if (category.Event!.RegistrationDeadline.Date < DateTime.Today || category.Event.Status != "Open") return BadRequest(new { message = "Registration is closed for this event." });
        if (await _db.Enrolments.AnyAsync(e => e.ParticipantID == participantId && e.CategoryID == request.CategoryID)) return Conflict(new { message = "You are already enrolled in this category." });
        if (await _db.Enrolments.CountAsync(e => e.CategoryID == request.CategoryID && e.Status == "Active") >= category.MaximumParticipants) return Conflict(new { message = "This category is full." });
        var enrolment = new Enrolment { ParticipantID = participantId, CategoryID = request.CategoryID, EmergencyContact = request.EmergencyContact, EnrolmentDate = DateTime.UtcNow, Status = "Active" };
        _db.Enrolments.Add(enrolment); await _db.SaveChangesAsync(); return Created($"/api/enrolments/{enrolment.EnrolmentID}", new { enrolment.EnrolmentID, enrolment.CategoryID, enrolment.Status });
    }
    [HttpGet("me")]
    public async Task<IActionResult> Mine()
    {
        var id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return Ok(await _db.Enrolments.AsNoTracking().Where(e => e.ParticipantID == id).Select(e => new { e.EnrolmentID, e.EnrolmentDate, e.Status, e.CategoryID, Category = e.Category!.CategoryName, Event = e.Category.Event!.EventName, Result = e.Result == null ? null : new { e.Result.ResultID, e.Result.FinishTime, e.Result.Position, e.Result.ResultStatus } }).ToListAsync());
    }
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Cancel(int id)
    {
        var participantId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var e = await _db.Enrolments.SingleOrDefaultAsync(x => x.EnrolmentID == id && x.ParticipantID == participantId);
        if (e is null) return NotFound();
        e.Status = "Cancelled"; await _db.SaveChangesAsync(); return NoContent();
    }
}
