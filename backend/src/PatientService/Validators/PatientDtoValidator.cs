using FluentValidation;
using PatientService.Models;

namespace PatientService.Validators;

public class PatientDtoValidator : AbstractValidator<PatientDto>
{
    public PatientDtoValidator()
    {
        RuleFor(p => p.FirstName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(p => p.LastName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(p => p.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(p => p.Phone)
            .NotEmpty()
            .MaximumLength(25);

        RuleFor(p => p.DateOfBirth)
            .LessThan(DateTime.UtcNow)
            .WithMessage("Date of birth must be in the past.");

        RuleFor(p => p.Gender)
            .NotEmpty()
            .MaximumLength(10);
    }
}


