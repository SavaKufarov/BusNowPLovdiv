using BusNow.Infrastructure.Data;
using BusNow.Web.Areas.Admin.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BusNow.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class DashboardController : Controller
{
    private readonly AppDbContext _context;

    public DashboardController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var now = DateTime.Now;

        var model = new DashboardViewModel
        {
            LinesCount = await _context.TransportLines.CountAsync(),
            StopsCount = await _context.Stops.CountAsync(),
            RoutesCount = await _context.Routes.CountAsync(),
            ActiveVehiclesCount = await _context.Vehicles.CountAsync(v => v.IsActive),
            SchedulesCount = await _context.Schedules.CountAsync(),
            ActiveAlertsCount = await _context.ServiceAlerts.CountAsync(a =>
                a.IsActive && a.StartDate <= now && a.EndDate >= now)
        };

        return View(model);
    }
}