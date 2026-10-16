using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.DTOs;
using RaceDay.API.Models;
namespace RaceDay.API.Controllers;

[ApiController, Route("api/categories")]
public class CategoriesController : ControllerBase
{
    private readonly RaceDayDbContext _db;
    public CategoriesController(RaceDayDbContext db) => _db = db;
    [Authorize(Roles = "Organiser"), HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, CategoryCreateRequest request)
    {
        var c = await _db.Categories.FindAsync(id); if (c is null) return NotFound();
        if (request.DistanceKm <= 0 || request.EntryFee < 0 || (request.MaximumParticipants < 1)) return BadRequest();
        c.CategoryName = request.CategoryName.Trim(); c.DistanceKm = request.DistanceKm; c.EntryFee = request.EntryFee; c.MaximumParticipants = request.MaximumParticipants;
        await _db.SaveChangesAsync(); return NoContent();
    }
    [Authorize(Roles = "Organiser"), HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var c = await _db.Categories.FindAsync(id); if (c is null) return NotFound();
        if (await _db.Enrolments.AnyAsync(e => e.CategoryID == id)) return Conflict(new { message = "Cannot delete a category with enrolments." });
        _db.Categories.Remove(c); await _db.SaveChangesAsync(); return NoContent();
    }
}
