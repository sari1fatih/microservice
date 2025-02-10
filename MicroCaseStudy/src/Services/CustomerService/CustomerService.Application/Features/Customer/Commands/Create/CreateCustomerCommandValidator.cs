using FluentValidation;

namespace CustomerService.Application.Features.Customer.Commands.Create;

public class CreateCustomerCommandValidator: AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerCommandValidator()
    {
        RuleFor(c => c.Name).NotEmpty().MinimumLength(2); 
        RuleFor(c => c.Email).NotEmpty().EmailAddress();
        RuleFor(c => c.Phone).NotEmpty().MinimumLength(4);
        RuleFor(c => c.Company).NotEmpty().MinimumLength(4);
    }
} 