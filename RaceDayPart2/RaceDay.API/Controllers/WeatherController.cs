using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
namespace RaceDay.API.Controllers;

[ApiController, Route("api/events/{eventId:int}/weather")]
public class WeatherController : ControllerBase
{
    private readonly RaceDayDbContext _db;
    public WeatherController(RaceDayDbContext db) => _db = db;
    [AllowAnonymous, HttpGet]
    public async Task<IActionResult> ForEvent(int eventId)
    {
        if (!await _db.Events.AnyAsync(e => e.EventID == eventId)) return NotFound(new { message = "Event not found." });
        return Ok(await _db.Weather.AsNoTracking().Where(w => w.EventID == eventId).OrderByDescending(w => w.RecordedAt).Select(w => new { w.WeatherID, w.EventID, w.Temperature, w.WeatherCondition, w.WindSpeed, w.RainProbability, w.RecordedAt }).ToListAsync());
    }
}
