using FluentValidation;
using AcmeApi.Models;

namespace AcmeApi.Validators;

public class CreateProductValidator : AbstractValidator<CreateProductRequest>
{
    private static readonly string[] ValidCategories = { "electronics", "clothing", "books", "home" };

    public CreateProductValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Product name is required")
            .MaximumLength(200)
            .WithMessage("Product name cannot exceed 200 characters");

        RuleFor(x => x.Price)
            .GreaterThan(0)
            .WithMessage("Price must be greater than 0");

        RuleFor(x => x.Category)
            .NotEmpty()
            .WithMessage("Category is required")
            .Must(c => ValidCategories.Contains(c))
            .WithMessage($"Category must be one of: {string.Join(", ", ValidCategories)}");
    }
}
