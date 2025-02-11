using FluentValidation;

namespace CustomerService.Application.Features.Customer.Commands.SaleCreated;

public class SaleCreatedCommandValidator: AbstractValidator<SaleCreatedCommand>
{
    public SaleCreatedCommandValidator()
    {
        RuleFor(c => c.SaleName).NotEmpty().WithMessage("Satış adı boş olamaz.").MinimumLength(2).WithMessage("Satış adı en az 2 karakter olmalıdır.");; 
        RuleFor(c => c.Note).NotEmpty().WithMessage("Not boş olamaz.").MinimumLength(2).WithMessage("Not en az 2 karakter olmalıdır.");; 
       
    }
} 