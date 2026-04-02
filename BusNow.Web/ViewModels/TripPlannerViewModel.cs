using Microsoft.AspNetCore.Mvc.Rendering;

namespace BusNow.Web.ViewModels;

public class TripPlannerViewModel
{
    public int? FromStopId { get; set; }
    public int? ToStopId { get; set; }

    public List<SelectListItem> Stops { get; set; } = new();

    public bool HasResult { get; set; }
    public bool RouteFound { get; set; }

    public string? LineNumber { get; set; }
    public string? RouteName { get; set; }
    public string? Direction { get; set; }

    public int StopsCount { get; set; }
    public double DistanceKm { get; set; }
    public int EstimatedMinutes { get; set; }

    public string? Message { get; set; }
}