using BusNow.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BusNow.Web.Controllers;

public class MapController : Controller
{
    private readonly AppDbContext _context;

    public MapController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var stops = await _context.Stops
            .Select(s => new
            {
                id = s.Id,
                name = s.Name,
                code = s.Code,
                latitude = s.Latitude,
                longitude = s.Longitude
            })
            .ToListAsync();

        var routes = await _context.Routes
            .Include(r => r.TransportLine)
            .Include(r => r.RouteStops)
                .ThenInclude(rs => rs.Stop)
            .Select(r => new
            {
                id = r.Id,
                name = r.Name,
                direction = r.Direction,
                lineNumber = r.TransportLine != null ? r.TransportLine.LineNumber : "",
                points = r.RouteStops
                    .OrderBy(rs => rs.OrderIndex)
                    .Where(rs => rs.Stop != null)
                    .Select(rs => new
                    {
                        latitude = rs.Stop!.Latitude,
                        longitude = rs.Stop!.Longitude,
                        stopName = rs.Stop!.Name,
                        orderIndex = rs.OrderIndex
                    })
                    .ToList()
            })
            .ToListAsync();

        ViewBag.StopsJson = System.Text.Json.JsonSerializer.Serialize(stops);
        ViewBag.RoutesJson = System.Text.Json.JsonSerializer.Serialize(routes);

        return View();
    }
}