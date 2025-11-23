using Microsoft.EntityFrameworkCore;
using PatientService.Data;
using PatientService.Models;
using PatientService.Kafka;
using Common.Events;

namespace PatientService.Services;

public interface IPatientService
{
    Task<IEnumerable<Patient>> GetAllPatientsAsync();
    Task<Patient?> GetPatientByIdAsync(Guid id);
    Task<Patient> CreatePatientAsync(PatientDto patientDto);
    Task<Patient?> UpdatePatientAsync(Guid id, PatientDto patientDto);
    Task<bool> DeletePatientAsync(Guid id);
    Task<IEnumerable<Patient>> SearchPatientsAsync(string term);
}

public class PatientServiceImpl : IPatientService
{
    private readonly PatientDbContext _context;
    private readonly IKafkaProducerService _kafkaProducer;
    private readonly ILogger<PatientServiceImpl> _logger;

    public PatientServiceImpl(
        PatientDbContext context,
        IKafkaProducerService kafkaProducer,
        ILogger<PatientServiceImpl> logger)
    {
        _context = context;
        _kafkaProducer = kafkaProducer;
        _logger = logger;
    }

    public async Task<IEnumerable<Patient>> GetAllPatientsAsync()
    {
        return await _context.Patients
            .Where(p => p.IsActive)
            .OrderBy(p => p.LastName)
            .ToListAsync();
    }

    public async Task<Patient?> GetPatientByIdAsync(Guid id)
    {
        return await _context.Patients
            .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);
    }

    public async Task<Patient> CreatePatientAsync(PatientDto patientDto)
    {
        var patient = new Patient
        {
            Id = Guid.NewGuid(),
            FirstName = patientDto.FirstName,
            LastName = patientDto.LastName,
            Email = patientDto.Email,
            Phone = patientDto.Phone,
            DateOfBirth = patientDto.DateOfBirth,
            Gender = patientDto.Gender,
            Address = patientDto.Address,
            MedicalHistory = patientDto.MedicalHistory,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsActive = true
        };

        _context.Patients.Add(patient);
        await _context.SaveChangesAsync();

        // Publish event
        var patientCreatedEvent = new PatientCreatedEvent
        {
            PatientId = patient.Id,
            FirstName = patient.FirstName,
            LastName = patient.LastName,
            Email = patient.Email,
            Phone = patient.Phone,
            DateOfBirth = patient.DateOfBirth,
            CreatedAt = patient.CreatedAt
        };

        await _kafkaProducer.PublishAsync("patient-created", patientCreatedEvent);
        _logger.LogInformation($"Patient created with ID: {patient.Id}");

        return patient;
    }

    public async Task<Patient?> UpdatePatientAsync(Guid id, PatientDto patientDto)
    {
        var patient = await _context.Patients.FindAsync(id);
        if (patient == null || !patient.IsActive) return null;

        patient.FirstName = patientDto.FirstName;
        patient.LastName = patientDto.LastName;
        patient.Email = patientDto.Email;
        patient.Phone = patientDto.Phone;
        patient.Address = patientDto.Address;
        patient.MedicalHistory = patientDto.MedicalHistory;
        patient.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        _logger.LogInformation($"Patient updated with ID: {id}");
        
        return patient;
    }

    public async Task<bool> DeletePatientAsync(Guid id)
    {
        var patient = await _context.Patients.FindAsync(id);
        if (patient == null) return false;

        patient.IsActive = false; // Soft delete
        patient.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Patient>> SearchPatientsAsync(string term)
    {
        term = term.ToLower();
        return await _context.Patients
            .Where(p => p.IsActive && 
                       (p.LastName.ToLower().Contains(term) || 
                        p.FirstName.ToLower().Contains(term) || 
                        p.Email.ToLower().Contains(term)))
            .ToListAsync();
    }
}