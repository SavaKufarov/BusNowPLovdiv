using BusNow.Core.Entities;
using BusNow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BusNow.Web.Data;

public static class SampleDataSeeder
{
    public static async Task SeedSampleDataAsync(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<AppDbContext>();

        await context.Database.MigrateAsync();

        if (!await context.TransportLines.AnyAsync())
        {
            var line1 = new TransportLine
            {
                LineNumber = "1",
                Type = "Автобус",
                DirectionA = "Кючук Париж",
                DirectionB = "Тракия",
                IsActive = true
            };

            var line2 = new TransportLine
            {
                LineNumber = "6",
                Type = "Автобус",
                DirectionA = "Смирненски",
                DirectionB = "Прослав",
                IsActive = true
            };

            context.TransportLines.AddRange(line1, line2);
            await context.SaveChangesAsync();
        }

        if (!await context.Stops.AnyAsync())
        {
            var stops = new List<Stop>
            {
                new Stop { Name = "Централна гара", Code = "PG001", Latitude = 42.1357, Longitude = 24.7306 },
                new Stop { Name = "Тримонциум", Code = "PG002", Latitude = 42.1451, Longitude = 24.7484 },
                new Stop { Name = "Панаир", Code = "PG003", Latitude = 42.1560, Longitude = 24.7509 },
                new Stop { Name = "Тракия", Code = "PG004", Latitude = 42.1522, Longitude = 24.7931 }
            };

            context.Stops.AddRange(stops);
            await context.SaveChangesAsync();
        }

        if (!await context.Routes.AnyAsync())
        {
            var line1 = await context.TransportLines.FirstAsync(x => x.LineNumber == "1");
            var line6 = await context.TransportLines.FirstAsync(x => x.LineNumber == "6");

            var routes = new List<Core.Entities.Route>
            {
                new Core.Entities.Route
                {
                    TransportLineId = line1.Id,
                    Name = "Линия 1 - Основен маршрут",
                    Direction = "Кючук Париж → Тракия",
                    IsTemporary = false
                },
                new Core.Entities.Route
                {
                    TransportLineId = line6.Id,
                    Name = "Линия 6 - Основен маршрут",
                    Direction = "Смирненски → Прослав",
                    IsTemporary = false
                }
            };

                context.Routes.AddRange(routes);
                await context.SaveChangesAsync();
        }

        if (!await context.Schedules.AnyAsync())
        {
            var line1 = await context.TransportLines.FirstAsync(x => x.LineNumber == "1");
            var firstRoute = await context.Routes.FirstAsync(x => x.TransportLineId == line1.Id);

            var schedules = new List<Schedule>
                    {
                        new Schedule
                        {
                            TransportLineId = line1.Id,
                            RouteId = firstRoute.Id,
                            DepartureTime = new TimeSpan(6, 0, 0),
                            IsWeekend = false,
                            IsHoliday = false
                        },
                        new Schedule
                        {
                            TransportLineId = line1.Id,
                            RouteId = firstRoute.Id,
                            DepartureTime = new TimeSpan(6, 30, 0),
                            IsWeekend = false,
                            IsHoliday = false
                        },
                        new Schedule
                        {
                            TransportLineId = line1.Id,
                            RouteId = firstRoute.Id,
                            DepartureTime = new TimeSpan(7, 0, 0),
                            IsWeekend = false,
                            IsHoliday = false
                        }
                    };

            context.Schedules.AddRange(schedules);
            await context.SaveChangesAsync();
        }

        if (!await context.Vehicles.AnyAsync())
        {
            var line1 = await context.TransportLines.FirstAsync(x => x.LineNumber == "1");

            context.Vehicles.Add(new Vehicle
            {
                TransportLineId = line1.Id,
                RegistrationNumber = "PB1234AB",
                VehicleType = "Автобус",
                CurrentLatitude = 42.1450,
                CurrentLongitude = 24.7480,
                LastUpdateTime = DateTime.Now,
                IsActive = true
            });

            await context.SaveChangesAsync();
        }

        if (!await context.ArrivalPredictions.AnyAsync())
        {
            var vehicle = await context.Vehicles.FirstAsync();
            var stop = await context.Stops.FirstAsync();

            context.ArrivalPredictions.Add(new ArrivalPrediction
            {
                VehicleId = vehicle.Id,
                StopId = stop.Id,
                EstimatedArrival = DateTime.Now.AddMinutes(4),
                DelayMinutes = 2,
                CalculatedAt = DateTime.Now
            });

            await context.SaveChangesAsync();
        }

        if (!await context.ServiceAlerts.AnyAsync())
        {
            var line1 = await context.TransportLines.FirstAsync(x => x.LineNumber == "1");

            context.ServiceAlerts.Add(new ServiceAlert
            {
                Title = "Временно закъснение по линия 1",
                Description = "Поради натоварен трафик са възможни закъснения до 10 минути.",
                AffectedLineId = line1.Id,
                StartDate = DateTime.Now.AddHours(-1),
                EndDate = DateTime.Now.AddHours(8),
                IsActive = true
            });

            await context.SaveChangesAsync();
        }
        if (!await context.RouteStops.AnyAsync())
        {
            var route = await context.Routes.FirstAsync();
            var stops = await context.Stops.OrderBy(x => x.Id).Take(3).ToListAsync();

            context.RouteStops.AddRange(
                new RouteStop
                {
                    RouteId = route.Id,
                    StopId = stops[0].Id,
                    OrderIndex = 1,
                    DistanceFromStartKm = 0
                },
                new RouteStop
                {
                    RouteId = route.Id,
                    StopId = stops[1].Id,
                    OrderIndex = 2,
                    DistanceFromStartKm = 1.2
                },
                new RouteStop
                {
                    RouteId = route.Id,
                    StopId = stops[2].Id,
                    OrderIndex = 3,
                    DistanceFromStartKm = 2.6
                }
            );

            await context.SaveChangesAsync();
        }
    }
}