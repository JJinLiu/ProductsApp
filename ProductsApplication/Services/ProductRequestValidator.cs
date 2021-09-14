using FluentValidation;
using ProductsApplication.Models;

namespace ProductsApplication.Services
{
    public class ProductRequestValidator : AbstractValidator<Product>
    {
        public ProductRequestValidator()
        {
            RuleFor(p => p.Name)
                .NotNull()
                .NotEmpty()
                .WithMessage(x => $"Product name is invalid, it cannot be empty.");
        }
    }
}
