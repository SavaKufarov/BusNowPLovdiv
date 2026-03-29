using BusNow.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BusNow.Web.Controllers;

public class LinesController : Controller
{
    private readonly AppDbContext _context;

    public LinesController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string? search)
    {
        var query = _context.TransportLines
            .Where(x => x.IsActive)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim();

            query = query.Where(x =>
                x.LineNumber.Contains(search) ||
                x.DirectionA.Contains(search) ||
                x.DirectionB.Contains(search) ||
                x.Type.Contains(search));
        }

        var lines = await query
            .OrderBy(x => x.LineNumber)
            .ToListAsync();

        ViewBag.Search = search;

        return View(lines);
    }

    public async Task<IActionResult> Details(int id)
    {
        var line = await _context.TransportLines
            .Include(x => x.Routes)
            .Include(x => x.Schedules)
            .Include(x => x.Vehicles)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (line == null)
        {
            return NotFound();
        }

        return View(line);
    }
}