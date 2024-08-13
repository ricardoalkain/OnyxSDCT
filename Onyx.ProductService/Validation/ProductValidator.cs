using FluentValidation;
using Onyx.ProductService.Entities;

namespace Onyx.ProductService.Validation;

internal class ProductValidator : AbstractValidator<Product>
{
    public ProductValidator()
    {
        RuleFor(p => p.Name).NotEmpty().MinimumLength(2).MaximumLength(100);
        RuleFor(p => p.Color).NotEmpty().MaximumLength(50);
        RuleFor(p => p.Price).GreaterThan(0);
    }
}
