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
        // Arrange — create a fresh product to delete
        var createRequest = new CreateProductRequest("Test Product", 9.99m, "books", true);
        var createResponse = await _client.PostAsJsonAsync("/api/products", createRequest);
        createResponse.EnsureSuccessStatusCode();
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
        var createRequest = new CreateProductRequest("Delete Me", 5.00m, "books", true);
        var createResponse = await _client.PostAsJsonAsync("/api/products", createRequest);
        createResponse.EnsureSuccessStatusCode();
        var created = await createResponse.Content.ReadFromJsonAsync<ProductDto>();

        // Act
        await _client.DeleteAsync($"/api/products/{created!.Id}");
        var getResponse = await _client.GetAsync($"/api/products/{created.Id}");

        // Assert
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
        body!.Error.Should().Be($"Product with id {randomId} not found");
    }

    [Fact]
    public async Task Delete_ProductReferencedInOrder_Returns409()
    {
        // Arrange — seed product a1b2c3d4-... is referenced by the seed order
        var referencedProductId = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890");

        // Act
        var response = await _client.DeleteAsync($"/api/products/{referencedProductId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        var body = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        body!.Error.Should().Be("Cannot delete product that is referenced in existing orders");
    }

    [Fact]
    public async Task Delete_InvalidGuid_ReturnsNonSuccess()
    {
        // Act
        var response = await _client.DeleteAsync("/api/products/not-a-guid");

        // Assert
        ((int)response.StatusCode).Should().BeGreaterThanOrEqualTo(400);
    }

    private record ErrorResponse(string Error);
}
