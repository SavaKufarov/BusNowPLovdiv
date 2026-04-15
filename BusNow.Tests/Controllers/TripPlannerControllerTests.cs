using BusNow.Application.Services;
using BusNow.Core.Entities;
using BusNow.Tests.Helpers;
using BusNow.Web.Controllers;
using BusNow.Web.ViewModels;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace BusNow.Tests.Controllers;

public class TripPlannerControllerTests
{
    [Fact]
    public async Task Index_Get_ShouldReturnView()
    {
        using var context = TestDbContextFactory.Create();
        var controller = new TripPlannerController(context, new TripPlannerService());

        var result = await controller.Index();

        result.Should().BeOfType<ViewResult>();
    }

    [Fact]
    public async Task Index_Post_ShouldReturnMessage_WhenStopsAreSame()
    {
        using var context = TestDbContextFactory.Create();
        var controller = new TripPlannerController(context, new TripPlannerService());

        var model = new TripPlannerViewModel
        {
            FromStopId = 1,
            ToStopId = 1
        };

        var result = await controller.Index(model);

        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        var returnedModel = viewResult.Model.Should().BeAssignableTo<TripPlannerViewModel>().Subject;

        returnedModel.RouteFound.Should().BeFalse();
        returnedModel.Message.Should().NotBeNull();
    }

    [Fact]
    public async Task Index_Post_ShouldFindDirectRoute()
    {
        using var context = TestDbContextFactory.Create();

        var line = new TransportLine
        {
            Id = 1,
            LineNumber = "1",
            Type = "Автобус",
            DirectionA = "А",
            DirectionB = "Б",
            IsActive = true
        };

        var route = new Route
        {
            Id = 1,
            TransportLineId = 1,
            Name = "Маршрут 1",
            Direction = "А-Б"
        };

        var stop1 = new Stop { Id = 1, Name = "Спирка 1", Code = "S1", Latitude = 1, Longitude = 1 };
        var stop2 = new Stop { Id = 2, Name = "Спирка 2", Code = "S2", Latitude = 2, Longitude = 2 };

        context.TransportLines.Add(line);
        context.Routes.Add(route);
        context.Stops.AddRange(stop1, stop2);
        context.RouteStops.AddRange(
            new RouteStop { RouteId = 1, StopId = 1, OrderIndex = 1, DistanceFromStartKm = 0 },
            new RouteStop { RouteId = 1, StopId = 2, OrderIndex = 2, DistanceFromStartKm = 2.2 });

        await context.SaveChangesAsync();

        var controller = new TripPlannerController(context, new TripPlannerService());

        var model = new TripPlannerViewModel
        {
            FromStopId = 1,
            ToStopId = 2
        };

        var result = await controller.Index(model);

        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        var returnedModel = viewResult.Model.Should().BeAssignableTo<TripPlannerViewModel>().Subject;

        returnedModel.RouteFound.Should().BeTrue();
        returnedModel.LineNumber.Should().Be("1");
    }
}