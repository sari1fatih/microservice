using FluentValidation;

namespace CustomerService.Application.Features.CustomerNote.Commands.Update;

public class UpdateCustomerNoteCommandValidator: AbstractValidator<UpdateCustomerNoteCommand>
{
    public UpdateCustomerNoteCommandValidator()
    {
        RuleFor(c => c.Note).NotEmpty().MinimumLength(2);
    }
} 