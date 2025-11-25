using FluentValidation;
using DoctorService.Models;

namespace DoctorService.Validators;

public class DoctorDtoValidator : AbstractValidator<DoctorDto>
{
    public DoctorDtoValidator()
    {
        RuleFor(d => d.FirstName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(d => d.LastName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(d => d.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(d => d.Phone)
            .NotEmpty()
            .MaximumLength(25);

        RuleFor(d => d.Specialization)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(d => d.LicenseNumber)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(d => d.YearsOfExperience)
            .GreaterThanOrEqualTo(0);

        RuleFor(d => d.ConsultationFee)
            .GreaterThan(0);
    }
}


