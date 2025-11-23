using System.ComponentModel.DataAnnotations;

namespace AppointmentService.Models;

public class Appointment
{
    [Key]
    public Guid Id { get; set; }
    
    [Required]
    public Guid PatientId { get; set; }
    
    [Required]
    public Guid DoctorId { get; set; }
    
    public DateTime AppointmentDateTime { get; set; }
    
    [Required]
    [MaxLength(500)]
    public string Reason { get; set; } = string.Empty;
    
    [MaxLength(20)]
    public string Status { get; set; } = "Scheduled"; // Scheduled, Completed, Cancelled
    
    [MaxLength(1000)]
    public string Notes { get; set; } = string.Empty;
    
    public int DurationMinutes { get; set; } = 30;
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;
}

public class AppointmentDto
{
    public Guid? Id { get; set; }
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }
    public DateTime AppointmentDateTime { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
}