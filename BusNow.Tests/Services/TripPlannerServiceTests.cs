using BusNow.Application.Services;
using BusNow.Core.Entities;
using FluentAssertions;
using Xunit;

namespace BusNow.Tests.Services;

public class TripPlannerServiceTests
{
    private readonly TripPlannerService _service = new();

    [Fact]
    public void CalculateEstimatedMinutesByDistance_ShouldReturnCorrectValue()
    {
        var result = _service.CalculateEstimatedMinutesByDistance(1.0, 3.2, 22);

        result.Should().BeGreaterThan(0);
    }

    [Fact]
    public void CalculateEstimatedMinutesByStops_ShouldReturnExpectedMinutes()
    {
        var result = _service.CalculateEstimatedMinutesByStops(5, 2);

        result.Should().Be(10);
    }

    [Fact]
    public void IsValidDirection_ShouldReturnTrue_WhenFromBeforeTo()
    {
        var from = new RouteStop { OrderIndex = 2 };
        var to = new RouteStop { OrderIndex = 5 };

        var result = _service.IsValidDirection(from, to);

        result.Should().BeTrue();
    }

    [Fact]
    public void IsValidDirection_ShouldReturnFalse_WhenFromAfterTo()
    {
        var from = new RouteStop { OrderIndex = 6 };
        var to = new RouteStop { OrderIndex = 3 };

        var result = _service.IsValidDirection(from, to);

        result.Should().BeFalse();
    }
}