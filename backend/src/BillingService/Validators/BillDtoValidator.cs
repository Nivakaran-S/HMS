using BillingService.Models;
using FluentValidation;

namespace BillingService.Validators;

public class BillDtoValidator : AbstractValidator<BillDto>
{
    public BillDtoValidator()
    {
        RuleFor(b => b.AppointmentId).NotEmpty();
        RuleFor(b => b.PatientId).NotEmpty();
        RuleFor(b => b.DoctorId).NotEmpty();
        RuleFor(b => b.ConsultationFee).GreaterThanOrEqualTo(0);
        RuleFor(b => b.LabCharges).GreaterThanOrEqualTo(0);
        RuleFor(b => b.MedicineCharges).GreaterThanOrEqualTo(0);
        RuleFor(b => b.OtherCharges).GreaterThanOrEqualTo(0);
        RuleFor(b => b.Discount).GreaterThanOrEqualTo(0);
    }
}

public class PaymentDtoValidator : AbstractValidator<PaymentDto>
{
    public PaymentDtoValidator()
    {
        RuleFor(p => p.PaymentMethod).NotEmpty();
        RuleFor(p => p.TransactionId).NotEmpty();
        RuleFor(p => p.PaymentDate).LessThanOrEqualTo(DateTime.UtcNow.AddMinutes(5));
    }
}


