using FluentValidation;

namespace CustomerService.Application.Features.CustomerNote.Commands.Create;

public class CreateCustomerNoteCommandValidator: AbstractValidator<CreateCustomerNoteCommand>
{
    public CreateCustomerNoteCommandValidator()
    {
        RuleFor(c => c.Note).NotEmpty().MinimumLength(2); 
    }
} 