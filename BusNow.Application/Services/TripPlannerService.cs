using BusNow.Core.Entities;

namespace BusNow.Application.Services;

public class TripPlannerService
{
    public int CalculateEstimatedMinutesByDistance(double fromKm, double toKm, double avgSpeedKmH = 22)
    {
        var distance = Math.Abs(toKm - fromKm);

        if (avgSpeedKmH <= 0)
            return 0;

        var hours = distance / avgSpeedKmH;
        var minutes = (int)Math.Ceiling(hours * 60);

        return Math.Max(1, minutes);
    }

    public int CalculateEstimatedMinutesByStops(int stopsCount, int minutesPerStop = 2)
    {
        if (stopsCount <= 0)
            return 0;

        return stopsCount * minutesPerStop;
    }

    public bool IsValidDirection(RouteStop fromStop, RouteStop toStop)
    {
        return fromStop.OrderIndex < toStop.OrderIndex;
    }
}