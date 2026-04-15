using BusNow.Core.Entities;
using BusNow.Infrastructure.Data;
using BusNow.Tests.Helpers;
using BusNow.Web.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace BusNow.Tests.Controllers;

public class LinesControllerTests
{
    [Fact]
    public async Task Index_ShouldReturnAllActiveLines()
    {
        using var context = TestDbContextFactory.Create();

        context.TransportLines.AddRange(
            new TransportLine
            {
                LineNumber = "1",
                Type = "Автобус",
                DirectionA = "Тракия",
                DirectionB = "Център",
                IsActive = true
            },
            new TransportLine
            {
                LineNumber = "2",
                Type = "Автобус",
                DirectionA = "Смирненски",
                DirectionB = "Център",
                IsActive = false
            });

        await context.SaveChangesAsync();

        var controller = new LinesController(context);

        var result = await controller.Index(null);

        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        var model = viewResult.Model.Should().BeAssignableTo<IEnumerable<TransportLine>>().Subject;

        model.Should().HaveCount(1);
        model.First().LineNumber.Should().Be("1");
    }

    [Fact]
    public async Task Index_ShouldFilterLines_BySearch()
    {
        using var context = TestDbContextFactory.Create();

        context.TransportLines.AddRange(
            new TransportLine
            {
                LineNumber = "1",
                Type = "Автобус",
                DirectionA = "Тракия",
                DirectionB = "Център",
                IsActive = true
            },
            new TransportLine
            {
                LineNumber = "6",
                Type = "Автобус",
                DirectionA = "Прослав",
                DirectionB = "Център",
                IsActive = true
            });

        await context.SaveChangesAsync();

        var controller = new LinesController(context);

        var result = await controller.Index("6");

        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        var model = viewResult.Model.Should().BeAssignableTo<IEnumerable<TransportLine>>().Subject;

        model.Should().HaveCount(1);
        model.First().LineNumber.Should().Be("6");
    }

    [Fact]
    public async Task Details_ShouldReturnNotFound_WhenLineMissing()
    {
        using var context = TestDbContextFactory.Create();
        var controller = new LinesController(context);

        var result = await controller.Details(999);

        result.Should().BeOfType<NotFoundResult>();
    }
}