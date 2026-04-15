using BusNow.Application.Services;
using BusNow.Core.Entities;
using BusNow.Tests.Helpers;
using BusNow.Web.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace BusNow.Tests.Controllers;

public class StopsControllerTests
{
    [Fact]
    public async Task Index_ShouldReturnMatchingStops()
    {
        using var context = TestDbContextFactory.Create();

        context.Stops.AddRange(
            new Stop { Name = "Тракия", Code = "PG001", Latitude = 42.1, Longitude = 24.7 },
            new Stop { Name = "Централна гара", Code = "PG002", Latitude = 42.2, Longitude = 24.8 });

        await context.SaveChangesAsync();

        var controller = new StopsController(context, new ArrivalPredictionService());

        var result = await controller.Index("Тракия");

        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        var model = viewResult.Model.Should().BeAssignableTo<IEnumerable<Stop>>().Subject;

        model.Should().ContainSingle();
        model.First().Name.Should().Be("Тракия");
    }

    [Fact]
    public async Task Details_ShouldReturnNotFound_WhenStopMissing()
    {
        using var context = TestDbContextFactory.Create();
        var controller = new StopsController(context, new ArrivalPredictionService());

        var result = await controller.Details(123);

        result.Should().BeOfType<NotFoundResult>();
    }
}