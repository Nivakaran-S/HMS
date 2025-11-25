namespace Common.Events;

public class BillPaymentProcessedEvent
{
    public Guid BillId { get; set; }
    public Guid AppointmentId { get; set; }
    public Guid PatientId { get; set; }
    public decimal NetAmount { get; set; }
    public string PaymentStatus { get; set; } = string.Empty;
    public DateTime PaymentDate { get; set; }
}


