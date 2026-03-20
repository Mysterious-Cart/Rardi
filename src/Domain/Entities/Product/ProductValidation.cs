namespace CHKS.Validator;

using FluentValidation;
using CHKS.Domain.Entities;

public class ProductValidator : AbstractValidator<CreateProductRequest>
{
    public ProductValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required");

        RuleFor(x => x.Name)
            .Length(3, 50)
            .WithMessage("Name must be between 3 and 50 characters");

        RuleFor(x => x.Name)
            .Matches(@"^[a-zA-Z0-9\s\-\/\.\,\(\)]+$")
            .WithMessage("Name contains invalid characters");

        RuleFor(x => x.Name)
            .Must(x => x != "Product")
            .WithMessage("Name cannot be 'Product'");

        RuleFor(x => x.Stock)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Stock cannot be negative");

        RuleFor(x => x.Stock)
            .LessThan(10000)
            .WithMessage("Stock must be less than 10000");

        RuleFor(x => x.Import)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Import price cannot be negative");

        RuleFor(x => x.Export)
            .NotEmpty()
            .WithMessage("Export price is required");

        RuleFor(x => x.Export)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Export price cannot be negative");

        RuleFor(x => x.Export)
            .GreaterThanOrEqualTo(x => x.Import)
            .WithMessage("Export price cannot be less than import price");

        RuleFor(x => x.Status).NotEmpty().WithMessage("Status is required");
        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("Status not recognized");

        RuleFor(x => x.Setting)
            .NotNull()
            .WithMessage("Setting cannot be null");
    }
}