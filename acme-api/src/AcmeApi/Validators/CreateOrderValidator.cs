using FluentValidation;
using AcmeApi.Models;

namespace AcmeApi.Validators;

public class CreateOrderValidator : AbstractValidator<CreateOrderRequest>
{
    public CreateOrderValidator()
    {
        RuleFor(x => x.CustomerEmail)
            .NotEmpty()
            .WithMessage("Customer email is required")
            .EmailAddress()
            .WithMessage("Customer email must be a valid email address");

        // IMPORTANT: This is a known bug for demo purposes.
        // The validator only checks NotNull() on Items, which allows an empty list.
        // The bug fix demo will add .MinimumLength(1) here.
        RuleFor(x => x.Items)
            .NotNull()
            .WithMessage("Items list cannot be null");

        RuleForEach(x => x.Items)
            .ChildRules(item =>
            {
                item.RuleFor(i => i.ProductId)
                    .NotEqual(Guid.Empty)
                    .WithMessage("Product ID cannot be empty");

                item.RuleFor(i => i.Quantity)
                    .GreaterThan(0)
                    .WithMessage("Quantity must be greater than 0");
            });
    }
}
