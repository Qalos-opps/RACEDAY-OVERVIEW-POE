using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.DTOs;
using RaceDay.API.Models;
namespace RaceDay.API.Controllers;

[ApiController, Route("api/results")]
public class ResultsController : ControllerBase
{
    private readonly RaceDayDbContext _db;
    public ResultsController(RaceDayDbContext db) => _db = db;
    [Authorize(Roles = "Organiser"), HttpPost]
    public async Task<IActionResult> Create(ResultCreateRequest request)
    {
        if (!await _db.Enrolments.AnyAsync(e => e.EnrolmentID == request.EnrolmentID)) return NotFound(new { message = "Enrolment not found." });
        if (await _db.Results.AnyAsync(r => r.EnrolmentID == request.EnrolmentID)) return Conflict(new { message = "A result already exists for this enrolment." });
        var result = new Result { EnrolmentID = request.EnrolmentID, FinishTime = request.FinishTime, Position = request.Position, ResultStatus = request.ResultStatus, RecordedAt = DateTime.UtcNow };
        _db.Results.Add(result); await _db.SaveChangesAsync(); return Ok(result);
    }
    [Authorize(Roles = "Organiser"), HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ResultCreateRequest request)
    {
        var result = await _db.Results.FindAsync(id); if (result is null) return NotFound();
        result.FinishTime = request.FinishTime; result.Position = request.Position; result.ResultStatus = request.ResultStatus;
        await _db.SaveChangesAsync(); return NoContent();
    }
    [Authorize(Roles = "Participant"), HttpGet("me")]
    public async Task<IActionResult> Mine()
    {
        var id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return Ok(await _db.Results.AsNoTracking().Where(r => r.Enrolment!.ParticipantID == id).Select(r => new { r.ResultID, r.FinishTime, r.Position, r.ResultStatus, r.RecordedAt, r.Enrolment!.Category!.Event!.EventName }).ToListAsync());
    }
    [Authorize(Roles = "Organiser"), HttpGet("/api/events/{eventId:int}/results")]
    public async Task<IActionResult> ForEvent(int eventId)
    {
        return Ok(await _db.Results.AsNoTracking().Where(r => r.Enrolment!.Category!.EventID == eventId).Select(r => new { r.ResultID, r.FinishTime, r.Position, r.ResultStatus, Participant = r.Enrolment!.Participant!.FirstName + " " + r.Enrolment.Participant.LastName }).ToListAsync());
    }
}
