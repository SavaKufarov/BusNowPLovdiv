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
    }
}