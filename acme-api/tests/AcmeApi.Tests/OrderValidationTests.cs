using FluentAssertions;
using FluentValidation;
using Xunit;
using AcmeApi.Models;
using AcmeApi.Validators;

namespace AcmeApi.Tests;

public class OrderValidationTests
{
    private readonly CreateOrderValidator _validator;

    public OrderValidationTests()
    {
        _validator = new CreateOrderValidator();
    }

    [Fact]
    public async Task ValidOrder_ShouldPass()
    {
        // Arrange
        var request = new CreateOrderRequest(
            "customer@example.com",
            new List<OrderItemDto>
            {
                new(Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890"), 1)
            }
        );

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task InvalidEmail_ShouldFail()
    {
        // Arrange
        var request = new CreateOrderRequest(
            "not-an-email",
            new List<OrderItemDto>
            {
                new(Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890"), 1)
            }
        );

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "CustomerEmail");
    }

    [Fact]
    public async Task EmptyEmail_ShouldFail()
    {
        // Arrange
        var request = new CreateOrderRequest(
            string.Empty,
            new List<OrderItemDto>
            {
                new(Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890"), 1)
            }
        );

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "CustomerEmail");
    }

    [Fact]
    public async Task ZeroQuantity_ShouldFail()
    {
        // Arrange
        var request = new CreateOrderRequest(
            "customer@example.com",
            new List<OrderItemDto>
            {
                new(Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890"), 0)
            }
        );

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Items[0].Quantity");
    }

    [Fact]
    public async Task NegativeQuantity_ShouldFail()
    {
        // Arrange
        var request = new CreateOrderRequest(
            "customer@example.com",
            new List<OrderItemDto>
            {
                new(Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890"), -5)
            }
        );

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Items[0].Quantity");
    }

    [Fact]
    public async Task EmptyProductId_ShouldFail()
    {
        // Arrange
        var request = new CreateOrderRequest(
            "customer@example.com",
            new List<OrderItemDto>
            {
                new(Guid.Empty, 1)
            }
        );

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Items[0].ProductId");
    }

    [Fact]
    public async Task MultipleItems_ShouldPass()
    {
        // Arrange
        var request = new CreateOrderRequest(
            "customer@example.com",
            new List<OrderItemDto>
            {
                new(Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890"), 2),
                new(Guid.Parse("b2c3d4e5-f6a7-8901-bcde-f12345678901"), 3)
            }
        );

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task EmptyItemsList_ShouldPass()
    {
        // Arrange - NOTE: This is a known bug! Empty items list should fail but doesn't.
        // This test demonstrates the bug that will be fixed in the demo.
        var request = new CreateOrderRequest(
            "customer@example.com",
            new List<OrderItemDto>()
        );

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert - Currently passes (the bug), but will fail after fix
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task NullItemsList_ShouldFail()
    {
        // Arrange
        var request = new CreateOrderRequest(
            "customer@example.com",
            null!
        );

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Items");
    }
}
