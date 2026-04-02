using BusNow.Core.Entities;
using BusNow.Infrastructure.Data;
using BusNow.Web.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace BusNow.Web.Services;

public class VehicleSimulationService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IHubContext<TransportHub> _hubContext;

    public VehicleSimulationService(
        IServiceScopeFactory scopeFactory,
        IHubContext<TransportHub> hubContext)
    {
        _scopeFactory = scopeFactory;
        _hubContext = hubContext;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var vehicles = await context.Vehicles
                .Include(v => v.TransportLine)
                .Include(v => v.Route)
                .Where(v => v.IsActive && v.RouteId != null)
                .ToListAsync(stoppingToken);

            foreach (var vehicle in vehicles)
            {
                var routeStops = await context.RouteStops
                    .Include(rs => rs.Stop)
                    .Where(rs => rs.RouteId == vehicle.RouteId)
                    .OrderBy(rs => rs.OrderIndex)
                    .ToListAsync(stoppingToken);

                if (routeStops.Count < 2)
                    continue;

                var currentRouteStop = routeStops
                    .FirstOrDefault(rs => rs.OrderIndex == vehicle.CurrentStopOrderIndex);

                if (currentRouteStop == null)
                {
                    var firstStop = routeStops.First();

                    vehicle.CurrentStopOrderIndex = firstStop.OrderIndex;
                    vehicle.IsForwardDirection = true;

                    if (firstStop.Stop != null)
                    {
                        vehicle.CurrentLatitude = firstStop.Stop.Latitude;
                        vehicle.CurrentLongitude = firstStop.Stop.Longitude;
                    }

                    vehicle.LastUpdateTime = DateTime.Now;
                    continue;
                }

                RouteStop? targetStop;

                if (vehicle.IsForwardDirection)
                {
                    targetStop = routeStops
                        .FirstOrDefault(rs => rs.OrderIndex == vehicle.CurrentStopOrderIndex + 1);

                    if (targetStop == null)
                    {
                        vehicle.IsForwardDirection = false;

                        targetStop = routeStops
                            .FirstOrDefault(rs => rs.OrderIndex == vehicle.CurrentStopOrderIndex - 1);
                    }
                }
                else
                {
                    targetStop = routeStops
                        .FirstOrDefault(rs => rs.OrderIndex == vehicle.CurrentStopOrderIndex - 1);

                    if (targetStop == null)
                    {
                        vehicle.IsForwardDirection = true;

                        targetStop = routeStops
                            .FirstOrDefault(rs => rs.OrderIndex == vehicle.CurrentStopOrderIndex + 1);
                    }
                }

                if (targetStop?.Stop == null)
                    continue;

                MoveVehicleTowards(
                    vehicle,
                    targetStop.Stop.Latitude,
                    targetStop.Stop.Longitude,
                    0.0008);

                var distanceToTarget = CalculateDistanceKm(
                    vehicle.CurrentLatitude,
                    vehicle.CurrentLongitude,
                    targetStop.Stop.Latitude,
                    targetStop.Stop.Longitude);

                if (distanceToTarget < 0.08)
                {
                    vehicle.CurrentStopOrderIndex = targetStop.OrderIndex;
                }

                vehicle.LastUpdateTime = DateTime.Now;
            }

            await context.SaveChangesAsync(stoppingToken);

            await UpdatePredictions(context, vehicles, stoppingToken);
            await BroadcastVehiclePositions(vehicles, stoppingToken);

            await Task.Delay(3000, stoppingToken);
        }
    }

    private async Task BroadcastVehiclePositions(
        List<BusNow.Core.Entities.Vehicle> vehicles,
        CancellationToken stoppingToken)
    {
        var vehicleData = vehicles.Select(v => new
        {
            id = v.Id,
            lineNumber = v.TransportLine?.LineNumber,
            registrationNumber = v.RegistrationNumber,
            latitude = v.CurrentLatitude,
            longitude = v.CurrentLongitude,
            lastUpdateTime = v.LastUpdateTime.ToString("HH:mm:ss"),
            isForwardDirection = v.IsForwardDirection
        }).ToList();

        await _hubContext.Clients.All.SendAsync(
            "ReceiveVehiclePositions",
            vehicleData,
            cancellationToken: stoppingToken);
    }

    private async Task UpdatePredictions(
        AppDbContext context,
        List<BusNow.Core.Entities.Vehicle> vehicles,
        CancellationToken stoppingToken)
    {
        var livePredictions = new List<object>();

        foreach (var vehicle in vehicles)
        {
            if (vehicle.RouteId == null)
                continue;

            var routeStops = await context.RouteStops
                .Include(rs => rs.Stop)
                .Where(rs => rs.RouteId == vehicle.RouteId)
                .OrderBy(rs => rs.OrderIndex)
                .ToListAsync(stoppingToken);

            if (!routeStops.Any())
                continue;

            List<RouteStop> upcomingStops;

            if (vehicle.IsForwardDirection)
            {
                upcomingStops = routeStops
                    .Where(rs => rs.OrderIndex >= vehicle.CurrentStopOrderIndex)
                    .ToList();
            }
            else
            {
                upcomingStops = routeStops
                    .Where(rs => rs.OrderIndex <= vehicle.CurrentStopOrderIndex)
                    .OrderByDescending(rs => rs.OrderIndex)
                    .ToList();
            }

            foreach (var rs in upcomingStops)
            {
                if (rs.Stop == null)
                    continue;

                var distance = CalculateDistanceKm(
                    vehicle.CurrentLatitude,
                    vehicle.CurrentLongitude,
                    rs.Stop.Latitude,
                    rs.Stop.Longitude);

                var etaMinutes = Math.Max(1, (int)Math.Ceiling((distance / 22.0) * 60.0));
                var estimatedArrival = DateTime.Now.AddMinutes(etaMinutes);

                var existingPrediction = await context.ArrivalPredictions
                    .FirstOrDefaultAsync(
                        p => p.VehicleId == vehicle.Id && p.StopId == rs.StopId,
                        stoppingToken);

                if (existingPrediction == null)
                {
                    existingPrediction = new BusNow.Core.Entities.ArrivalPrediction
                    {
                        VehicleId = vehicle.Id,
                        StopId = rs.StopId,
                        DelayMinutes = 0,
                        CalculatedAt = DateTime.Now,
                        EstimatedArrival = estimatedArrival
                    };

                    context.ArrivalPredictions.Add(existingPrediction);
                }
                else
                {
                    existingPrediction.EstimatedArrival = estimatedArrival;
                    existingPrediction.CalculatedAt = DateTime.Now;
                }

                livePredictions.Add(new
                {
                    stopId = rs.StopId,
                    vehicleId = vehicle.Id,
                    lineNumber = vehicle.TransportLine?.LineNumber,
                    registrationNumber = vehicle.RegistrationNumber,
                    estimatedArrival = estimatedArrival.ToString("HH:mm"),
                    etaMinutes = etaMinutes,
                    delayMinutes = existingPrediction.DelayMinutes
                });
            }
        }

        await context.SaveChangesAsync(stoppingToken);

        await _hubContext.Clients.All.SendAsync(
            "ReceiveArrivalPredictions",
            livePredictions,
            cancellationToken: stoppingToken);
    }

    private static void MoveVehicleTowards(
        BusNow.Core.Entities.Vehicle vehicle,
        double targetLat,
        double targetLng,
        double step)
    {
        var deltaLat = targetLat - vehicle.CurrentLatitude;
        var deltaLng = targetLng - vehicle.CurrentLongitude;

        var distance = Math.Sqrt(deltaLat * deltaLat + deltaLng * deltaLng);

        if (distance < step)
        {
            vehicle.CurrentLatitude = targetLat;
            vehicle.CurrentLongitude = targetLng;
            return;
        }

        vehicle.CurrentLatitude += (deltaLat / distance) * step;
        vehicle.CurrentLongitude += (deltaLng / distance) * step;
    }

    private static double CalculateDistanceKm(
        double lat1,
        double lon1,
        double lat2,
        double lon2)
    {
        const double R = 6371;

        var dLat = ToRadians(lat2 - lat1);
        var dLon = ToRadians(lon2 - lon1);

        var a =
            Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
            Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
            Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }

    private static double ToRadians(double angle)
    {
        return angle * Math.PI / 180.0;
    }
}