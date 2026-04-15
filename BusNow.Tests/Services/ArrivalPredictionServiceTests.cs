using BusNow.Application.Services;
using FluentAssertions;
using Xunit;

namespace BusNow.Tests.Services;

public class ArrivalPredictionServiceTests
{
    private readonly ArrivalPredictionService _service = new();

    [Fact]
    public void CalculateEtaMinutes_ShouldReturnCorrectMinutes()
    {
        var result = _service.CalculateEtaMinutes(11, 22);

        result.Should().Be(30);
    }

    [Fact]
    public void CalculateEtaMinutes_ShouldReturnZero_WhenSpeedIsInvalid()
    {
        var result = _service.CalculateEtaMinutes(10, 0);

        result.Should().Be(0);
    }

    [Fact]
    public void CalculateDistanceKm_ShouldReturnZero_ForSameCoordinates()
    {
        var result = _service.CalculateDistanceKm(42.1354, 24.7453, 42.1354, 24.7453);

        result.Should().BeApproximately(0, 0.001);
    }

    [Fact]
    public void CalculateDistanceKm_ShouldReturnPositiveDistance_ForDifferentCoordinates()
    {
        var result = _service.CalculateDistanceKm(42.1354, 24.7453, 42.1454, 24.7553);

        result.Should().BeGreaterThan(0);
    }
}