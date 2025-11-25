using System.ComponentModel.DataAnnotations;

namespace AppointmentService.Models;

public class OutboxMessage
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Topic { get; set; } = string.Empty;

    [Required]
    public string Payload { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? SentAt { get; set; }
    public int RetryCount { get; set; }

    [MaxLength(500)]
    public string? LastError { get; set; }
}


