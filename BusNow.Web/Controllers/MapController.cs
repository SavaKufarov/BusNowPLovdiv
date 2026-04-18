using BusNow.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace BusNow.Web.Controllers;

public class MapController : Controller
{
    private readonly AppDbContext _context;

    public MapController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(int? stopId, int? lineId)
    {
        var stopsQuery = _context.Stops.AsQueryable();
        var routesQuery = _context.Routes
            .Include(r => r.TransportLine)
            .Include(r => r.RouteStops)
                .ThenInclude(rs => rs.Stop)
            .AsQueryable();

        string? focusTitle = null;

        if (stopId.HasValue)
        {
            stopsQuery = stopsQuery.Where(s => s.Id == stopId.Value);
            routesQuery = routesQuery.Where(r => r.RouteStops.Any(rs => rs.StopId == stopId.Value));

            var stopName = await _context.Stops
                .Where(s => s.Id == stopId.Value)
                .Select(s => s.Name)
                .FirstOrDefaultAsync();

            if (!string.IsNullOrWhiteSpace(stopName))
            {
                focusTitle = $"Показва се любима спирка: {stopName}";
            }
        }

        if (lineId.HasValue)
        {
            stopsQuery = stopsQuery.Where(s =>
                _context.RouteStops.Any(rs => rs.StopId == s.Id && rs.Route!.TransportLineId == lineId.Value));

            routesQuery = routesQuery.Where(r => r.TransportLineId == lineId.Value);

            var lineNumber = await _context.TransportLines
                .Where(l => l.Id == lineId.Value)
                .Select(l => l.LineNumber)
                .FirstOrDefaultAsync();

            if (!string.IsNullOrWhiteSpace(lineNumber))
            {
                focusTitle = $"Показва се любима линия: {lineNumber}";
            }
        }

        var stops = await stopsQuery
            .Select(s => new
            {
                id = s.Id,
                name = s.Name,
                code = s.Code,
                latitude = s.Latitude,
                longitude = s.Longitude
            })
            .ToListAsync();

        var routes = await routesQuery
            .Select(r => new
            {
                id = r.Id,
                name = r.Name,
                direction = r.Direction,
                lineId = r.TransportLineId,
                lineNumber = r.TransportLine != null ? r.TransportLine.LineNumber : "",
                points = r.RouteStops
                    .OrderBy(rs => rs.OrderIndex)
                    .Where(rs => rs.Stop != null)
                    .Select(rs => new
                    {
                        latitude = rs.Stop!.Latitude,
                        longitude = rs.Stop!.Longitude,
                        stopName = rs.Stop!.Name,
                        stopId = rs.StopId,
                        orderIndex = rs.OrderIndex
                    })
                    .ToList()
            })
            .ToListAsync();

        object? focus = null;

        if (stopId.HasValue)
        {
            var stop = await _context.Stops
                .Where(s => s.Id == stopId.Value)
                .Select(s => new
                {
                    type = "stop",
                    id = s.Id,
                    name = s.Name,
                    latitude = s.Latitude,
                    longitude = s.Longitude
                })
                .FirstOrDefaultAsync();

            focus = stop;
        }
        else if (lineId.HasValue)
        {
            var firstPoint = routes
                .SelectMany(r => r.points)
                .FirstOrDefault();

            if (firstPoint != null)
            {
                focus = new
                {
                    type = "line",
                    id = lineId.Value,
                    latitude = firstPoint.latitude,
                    longitude = firstPoint.longitude
                };
            }
        }

        ViewBag.StopsJson = JsonSerializer.Serialize(stops);
        ViewBag.RoutesJson = JsonSerializer.Serialize(routes);
        ViewBag.FocusJson = JsonSerializer.Serialize(focus);
        ViewBag.FocusTitle = focusTitle;
        ViewBag.IsFiltered = stopId.HasValue || lineId.HasValue;

        return View();
    }
}