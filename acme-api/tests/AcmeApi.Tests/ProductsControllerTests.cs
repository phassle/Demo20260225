using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using AcmeApi.Models;

namespace AcmeApi.Tests;

public class ProductsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ProductsControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Delete_ExistingProduct_Returns204()
    {
        // Arrange — create a product so we don't affect seed data
        var request = new CreateProductRequest("Temp Product", 10.00m, "electronics", true);
        var createResponse = await _client.PostAsJsonAsync("/api/products", request);
        var created = await createResponse.Content.ReadFromJsonAsync<ProductDto>();

        // Act
        var response = await _client.DeleteAsync($"/api/products/{created!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_ExistingProduct_RemovesFromStore()
    {
        // Arrange
        var request = new CreateProductRequest("Temp Product 2", 15.00m, "books", true);
        var createResponse = await _client.PostAsJsonAsync("/api/products", request);
        var created = await createResponse.Content.ReadFromJsonAsync<ProductDto>();

        // Act
        await _client.DeleteAsync($"/api/products/{created!.Id}");

        // Assert — GET should return 404
        var getResponse = await _client.GetAsync($"/api/products/{created.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_NonExistentProduct_Returns404()
    {
        // Arrange
        var randomId = Guid.NewGuid();

        // Act
        var response = await _client.DeleteAsync($"/api/products/{randomId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var body = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        body!.Error.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Delete_ProductReferencedInOrder_Returns409()
    {
        // Arrange — seed product a1b2c3d4-... is referenced in seed order
        var seedProductId = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890");

        // Act
        var response = await _client.DeleteAsync($"/api/products/{seedProductId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        var body = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        body!.Error.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Delete_InvalidGuid_ReturnsBadRequestOrNotFound()
    {
        // Act
        var response = await _client.DeleteAsync("/api/products/not-a-guid");

        // Assert — framework returns 400, 404, or 405 for invalid GUID route
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.BadRequest,
            HttpStatusCode.NotFound,
            HttpStatusCode.MethodNotAllowed);
    }

    private record ErrorResponse(string Error);
}
