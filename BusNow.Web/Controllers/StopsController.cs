using BusNow.Application.Services;
using BusNow.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BusNow.Web.Controllers;

public class StopsController : Controller
{
    private readonly AppDbContext _context;
    private readonly ArrivalPredictionService _arrivalPredictionService;

    public StopsController(AppDbContext context, ArrivalPredictionService arrivalPredictionService)
    {
        _context = context;
        _arrivalPredictionService = arrivalPredictionService;
    }

    public async Task<IActionResult> Index(string? search)
    {
        var query = _context.Stops.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim();
            query = query.Where(x =>
                x.Name.Contains(search) ||
                x.Code.Contains(search));
        }

        var stops = await query
            .OrderBy(x => x.Name)
            .ToListAsync();

        ViewBag.Search = search;

        return View(stops);
    }

    public async Task<IActionResult> Details(int id)
    {
        var stop = await _context.Stops
            .Include(x => x.ArrivalPredictions)
                .ThenInclude(x => x.Vehicle)
                    .ThenInclude(v => v!.TransportLine)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (stop == null)
            return NotFound();

        var routeStops = await _context.RouteStops
            .Include(x => x.Route)
                .ThenInclude(r => r!.TransportLine)
            .Where(x => x.StopId == id)
            .OrderBy(x => x.OrderIndex)
            .ToListAsync();

        ViewBag.RouteStops = routeStops;

        return View(stop);
    }
}