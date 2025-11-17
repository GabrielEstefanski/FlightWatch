using FlightWatch.Api.Tests.Helpers;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace FlightWatch.Api.Tests.Controllers;

public class FlightMonitoringControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public FlightMonitoringControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task HealthCheck_Should_Return_Ok_With_Service_Status()
    {
        // Act
        var response = await _client.GetAsync("/api/FlightMonitoring/health");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("FlightMonitoring");
        content.Should().Contain("healthy");
        content.Should().Contain("timestamp");
    }

    [Fact]
    public async Task HealthCheck_Should_Return_Json_Content_Type()
    {
        // Act
        var response = await _client.GetAsync("/api/FlightMonitoring/health");

        // Assert
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");
    }

    [Fact]
    public async Task HealthCheck_Should_Have_Valid_Json_Structure()
    {
        // Act
        var response = await _client.GetAsync("/api/FlightMonitoring/health");

        // Assert
        response.EnsureSuccessStatusCode();
        
        var jsonResponse = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();
        jsonResponse.Should().NotBeNull();
        jsonResponse!.Should().ContainKey("service");
        jsonResponse.Should().ContainKey("status");
        jsonResponse.Should().ContainKey("timestamp");
        
        jsonResponse["service"]?.ToString().Should().NotBeNull().And.Be("FlightMonitoring");
        jsonResponse["status"]?.ToString().Should().NotBeNull().And.Be("healthy");
    }
}
