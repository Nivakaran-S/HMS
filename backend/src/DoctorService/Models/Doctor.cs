using System.ComponentModel.DataAnnotations;

namespace DoctorService.Models;

public class Doctor
{
    [Key]
    public Guid Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    [Phone]
    public string Phone { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string Specialization { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(50)]
    public string LicenseNumber { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string Qualifications { get; set; } = string.Empty;
    
    public int YearsOfExperience { get; set; }
    
    [MaxLength(500)]
    public string Biography { get; set; } = string.Empty;
    
    public decimal ConsultationFee { get; set; }
    
    [MaxLength(200)]
    public string ClinicAddress { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string Department { get; set; } = string.Empty;
    
    public bool IsAvailable { get; set; } = true;
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;
}

public class DoctorDto
{
    public Guid? Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
    public string LicenseNumber { get; set; } = string.Empty;
    public string Qualifications { get; set; } = string.Empty;
    public int YearsOfExperience { get; set; }
    public string Biography { get; set; } = string.Empty;
    public decimal ConsultationFee { get; set; }
    public string ClinicAddress { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public bool IsAvailable { get; set; }
}