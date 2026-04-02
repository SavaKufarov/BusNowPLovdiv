using BusNow.Application.Services;
using BusNow.Core.Entities;
using BusNow.Infrastructure.Data;
using BusNow.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BusNow.Web.Controllers;

public class TripPlannerController : Controller
{
    private readonly AppDbContext _context;
    private readonly TripPlannerService _tripPlannerService;

    public TripPlannerController(AppDbContext context, TripPlannerService tripPlannerService)
    {
        _context = context;
        _tripPlannerService = tripPlannerService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var model = new TripPlannerViewModel
        {
            Stops = await GetStopsAsync()
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Index(TripPlannerViewModel model)
    {
        model.Stops = await GetStopsAsync();

        if (!model.FromStopId.HasValue || !model.ToStopId.HasValue)
        {
            model.HasResult = true;
            model.RouteFound = false;
            model.Message = "Моля, избери начална и крайна спирка.";
            return View(model);
        }

        if (model.FromStopId == model.ToStopId)
        {
            model.HasResult = true;
            model.RouteFound = false;
            model.Message = "Началната и крайната спирка не могат да бъдат еднакви.";
            return View(model);
        }

        var routeStops = await _context.RouteStops
            .Include(rs => rs.Route)
                .ThenInclude(r => r!.TransportLine)
            .Include(rs => rs.Stop)
            .Where(rs => rs.StopId == model.FromStopId || rs.StopId == model.ToStopId)
            .ToListAsync();

        var fromCandidates = routeStops.Where(rs => rs.StopId == model.FromStopId).ToList();
        var toCandidates = routeStops.Where(rs => rs.StopId == model.ToStopId).ToList();

        RouteStop? bestFrom = null;
        RouteStop? bestTo = null;

        foreach (var from in fromCandidates)
        {
            foreach (var to in toCandidates)
            {
                if (from.RouteId == to.RouteId && _tripPlannerService.IsValidDirection(from, to))
                {
                    bestFrom = from;
                    bestTo = to;
                    break;
                }
            }

            if (bestFrom != null && bestTo != null)
                break;
        }

        model.HasResult = true;

        if (bestFrom == null || bestTo == null)
        {
            model.RouteFound = false;
            model.Message = "Не е намерен директен маршрут между избраните спирки.";
            return View(model);
        }

        model.RouteFound = true;
        model.LineNumber = bestFrom.Route?.TransportLine?.LineNumber;
        model.RouteName = bestFrom.Route?.Name;
        model.Direction = bestFrom.Route?.Direction;
        model.StopsCount = bestTo.OrderIndex - bestFrom.OrderIndex;
        model.DistanceKm = Math.Abs(bestTo.DistanceFromStartKm - bestFrom.DistanceFromStartKm);
        model.EstimatedMinutes = _tripPlannerService.CalculateEstimatedMinutesByDistance(
            bestFrom.DistanceFromStartKm,
            bestTo.DistanceFromStartKm);

        return View(model);
    }

    private async Task<List<SelectListItem>> GetStopsAsync()
    {
        return await _context.Stops
            .OrderBy(s => s.Name)
            .Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = s.Name + " (" + s.Code + ")"
            })
            .ToListAsync();
    }
}