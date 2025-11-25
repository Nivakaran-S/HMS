using FluentValidation;
using AppointmentService.Models;

namespace AppointmentService.Validators;

public class AppointmentDtoValidator : AbstractValidator<AppointmentDto>
{
    public AppointmentDtoValidator()
    {
        RuleFor(a => a.PatientId)
            .NotEmpty();

        RuleFor(a => a.DoctorId)
            .NotEmpty();

        RuleFor(a => a.AppointmentDateTime)
            .GreaterThan(DateTime.UtcNow.AddMinutes(-1))
            .WithMessage("Appointment date must be in the future.");

        RuleFor(a => a.Reason)
            .NotEmpty()
            .MaximumLength(500);

        RuleFor(a => a.DurationMinutes)
            .GreaterThan(0)
            .LessThanOrEqualTo(480);
    }
}


