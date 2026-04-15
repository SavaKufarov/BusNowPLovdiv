using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace BusNow.Tests.Integration;

public class AppSmokeTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public AppSmokeTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task HomePage_ShouldLoadSuccessfully()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task LinesPage_ShouldLoadSuccessfully()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/Lines");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task StopsPage_ShouldLoadSuccessfully()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/Stops");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}