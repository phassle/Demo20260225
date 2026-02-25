using FluentAssertions;
using FluentValidation;
using Xunit;
using AcmeApi.Models;
using AcmeApi.Validators;

namespace AcmeApi.Tests;

public class ProductValidationTests
{
    private readonly CreateProductValidator _validator;

    public ProductValidationTests()
    {
        _validator = new CreateProductValidator();
    }

    [Fact]
    public async Task ValidProduct_ShouldPass()
    {
        // Arrange
        var request = new CreateProductRequest(
            "Wireless Keyboard",
            79.99m,
            "electronics",
            true
        );

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task EmptyName_ShouldFail()
    {
        // Arrange
        var request = new CreateProductRequest(
            string.Empty,
            79.99m,
            "electronics",
            true
        );

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact]
    public async Task NegativePrice_ShouldFail()
    {
        // Arrange
        var request = new CreateProductRequest(
            "Wireless Keyboard",
            -10m,
            "electronics",
            true
        );

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Price");
    }

    [Fact]
    public async Task ZeroPrice_ShouldFail()
    {
        // Arrange
        var request = new CreateProductRequest(
            "Wireless Keyboard",
            0m,
            "electronics",
            true
        );

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Price");
    }

    [Fact]
    public async Task InvalidCategory_ShouldFail()
    {
        // Arrange
        var request = new CreateProductRequest(
            "Wireless Keyboard",
            79.99m,
            "invalid-category",
            true
        );

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Category");
    }

    [Theory]
    [InlineData("electronics")]
    [InlineData("clothing")]
    [InlineData("books")]
    [InlineData("home")]
    public async Task ValidCategory_ShouldPass(string category)
    {
        // Arrange
        var request = new CreateProductRequest(
            "Test Product",
            49.99m,
            category,
            true
        );

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task NameExceedingMaxLength_ShouldFail()
    {
        // Arrange
        var longName = new string('a', 201);
        var request = new CreateProductRequest(
            longName,
            79.99m,
            "electronics",
            true
        );

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }
}
