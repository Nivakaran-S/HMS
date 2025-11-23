using System.ComponentModel.DataAnnotations;

namespace BillingService.Models;

public class Bill
{
    [Key]
    public Guid Id { get; set; }
    
    [Required]
    public Guid AppointmentId { get; set; }
    
    [Required]
    public Guid PatientId { get; set; }
    
    [Required]
    public Guid DoctorId { get; set; }
    
    [Required]
    public decimal ConsultationFee { get; set; }
    
    public decimal LabCharges { get; set; }
    
    public decimal MedicineCharges { get; set; }
    
    public decimal OtherCharges { get; set; }
    
    public decimal TotalAmount { get; set; }
    
    public decimal Discount { get; set; }
    
    public decimal TaxAmount { get; set; }
    
    public decimal NetAmount { get; set; }
    
    [Required]
    [MaxLength(20)]
    public string PaymentStatus { get; set; } = "Pending"; // Pending, Paid, Cancelled
    
    [MaxLength(50)]
    public string PaymentMethod { get; set; } = string.Empty;
    
    public DateTime? PaymentDate { get; set; }
    
    [MaxLength(100)]
    public string TransactionId { get; set; } = string.Empty;
    
    [MaxLength(1000)]
    public string Notes { get; set; } = string.Empty;
    
    public DateTime BillDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;
}

public class BillDto
{
    public Guid? Id { get; set; }
    public Guid AppointmentId { get; set; }
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }
    public decimal ConsultationFee { get; set; }
    public decimal LabCharges { get; set; }
    public decimal MedicineCharges { get; set; }
    public decimal OtherCharges { get; set; }
    public decimal Discount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}

public class PaymentDto
{
    public string PaymentMethod { get; set; } = string.Empty;
    public string TransactionId { get; set; } = string.Empty;
    public DateTime PaymentDate { get; set; }
}