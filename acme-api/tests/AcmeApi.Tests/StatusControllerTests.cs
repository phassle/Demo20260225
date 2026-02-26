using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace AcmeApi.Tests;

public class StatusControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public StatusControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetStatus_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/status");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetStatus_ReturnsVersionField()
    {
        var response = await _client.GetAsync("/api/status");

        var body = await response.Content.ReadFromJsonAsync<StatusResponse>();
        body!.Version.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetStatus_ReturnsUptimeField()
    {
        var response = await _client.GetAsync("/api/status");

        var body = await response.Content.ReadFromJsonAsync<StatusResponse>();
        body!.Uptime.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetStatus_ReturnsEnvironmentField()
    {
        var response = await _client.GetAsync("/api/status");

        // WebApplicationFactory defaults to Development environment
        var body = await response.Content.ReadFromJsonAsync<StatusResponse>();
        body!.Environment.Should().Be("Development");
    }

    [Fact]
    public async Task GetStatus_ReturnsProductCount()
    {
        var response = await _client.GetAsync("/api/status");

        var body = await response.Content.ReadFromJsonAsync<StatusResponse>();
        body!.ProductCount.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task GetStatus_ReturnsOrderCount()
    {
        var response = await _client.GetAsync("/api/status");

        var body = await response.Content.ReadFromJsonAsync<StatusResponse>();
        body!.OrderCount.Should().BeGreaterThanOrEqualTo(0);
    }

    private record StatusResponse(string Version, string Uptime, string Environment, int ProductCount, int OrderCount);
}
