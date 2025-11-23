namespace Common.Events;

public class AppointmentCreatedEvent
{
    public Guid AppointmentId { get; set; }
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }
    public DateTime AppointmentDateTime { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string Status { get; set; } = "Scheduled";
    public DateTime CreatedAt { get; set; }
}

public class AppointmentCompletedEvent
{
    public Guid AppointmentId { get; set; }
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }
    public DateTime CompletedAt { get; set; }
    public string Notes { get; set; } = string.Empty;
    public decimal ConsultationFee { get; set; }
}

public class AppointmentCancelledEvent
{
    public Guid AppointmentId { get; set; }
    public Guid PatientId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public DateTime CancelledAt { get; set; }
}