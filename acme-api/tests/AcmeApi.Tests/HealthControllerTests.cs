using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace AcmeApi.Tests;

public class HealthControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public HealthControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetHealth_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/health");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetHealth_ReturnsHealthyStatus()
    {
        var response = await _client.GetAsync("/api/health");

        var body = await response.Content.ReadFromJsonAsync<HealthResponse>();
        body!.Status.Should().Be("healthy");
    }

    [Fact]
    public async Task GetHealth_ReturnsJsonContentType()
    {
        var response = await _client.GetAsync("/api/health");

        response.Content.Headers.ContentType!.MediaType.Should().Be("application/json");
    }

    private record HealthResponse(string Status);
}
