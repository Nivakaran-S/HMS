using Microsoft.EntityFrameworkCore;
using DoctorService.Data;
using DoctorService.Models;

namespace DoctorService.Services;

public interface IDoctorService
{
    Task<IEnumerable<Doctor>> GetAllDoctorsAsync();
    Task<Doctor?> GetDoctorByIdAsync(Guid id);
    Task<Doctor> CreateDoctorAsync(DoctorDto doctorDto);
    Task<Doctor?> UpdateDoctorAsync(Guid id, DoctorDto doctorDto);
    Task<bool> DeleteDoctorAsync(Guid id);
    Task<IEnumerable<Doctor>> GetDoctorsBySpecializationAsync(string specialization);
    Task<IEnumerable<Doctor>> GetAvailableDoctorsAsync();
}

public class DoctorServiceImpl : IDoctorService
{
    private readonly DoctorDbContext _context;
    private readonly ILogger<DoctorServiceImpl> _logger;

    public DoctorServiceImpl(
        DoctorDbContext context,
        ILogger<DoctorServiceImpl> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<Doctor>> GetAllDoctorsAsync()
    {
        return await _context.Doctors
            .Where(d => d.IsActive)
            .OrderBy(d => d.LastName)
            .ToListAsync();
    }

    public async Task<Doctor?> GetDoctorByIdAsync(Guid id)
    {
        return await _context.Doctors
            .FirstOrDefaultAsync(d => d.Id == id && d.IsActive);
    }

    public async Task<Doctor> CreateDoctorAsync(DoctorDto doctorDto)
    {
        var doctor = new Doctor
        {
            Id = Guid.NewGuid(),
            FirstName = doctorDto.FirstName,
            LastName = doctorDto.LastName,
            Email = doctorDto.Email,
            Phone = doctorDto.Phone,
            Specialization = doctorDto.Specialization,
            LicenseNumber = doctorDto.LicenseNumber,
            Qualifications = doctorDto.Qualifications,
            YearsOfExperience = doctorDto.YearsOfExperience,
            Biography = doctorDto.Biography,
            ConsultationFee = doctorDto.ConsultationFee,
            ClinicAddress = doctorDto.ClinicAddress,
            Department = doctorDto.Department,
            IsAvailable = doctorDto.IsAvailable,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsActive = true
        };

        _context.Doctors.Add(doctor);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Doctor created with ID: {doctor.Id}");
        return doctor;
    }

    public async Task<Doctor?> UpdateDoctorAsync(Guid id, DoctorDto doctorDto)
    {
        var doctor = await _context.Doctors.FindAsync(id);
        if (doctor == null || !doctor.IsActive)
            return null;

        doctor.FirstName = doctorDto.FirstName;
        doctor.LastName = doctorDto.LastName;
        doctor.Email = doctorDto.Email;
        doctor.Phone = doctorDto.Phone;
        doctor.Specialization = doctorDto.Specialization;
        doctor.LicenseNumber = doctorDto.LicenseNumber;
        doctor.Qualifications = doctorDto.Qualifications;
        doctor.YearsOfExperience = doctorDto.YearsOfExperience;
        doctor.Biography = doctorDto.Biography;
        doctor.ConsultationFee = doctorDto.ConsultationFee;
        doctor.ClinicAddress = doctorDto.ClinicAddress;
        doctor.Department = doctorDto.Department;
        doctor.IsAvailable = doctorDto.IsAvailable;
        doctor.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        _logger.LogInformation($"Doctor updated with ID: {doctor.Id}");

        return doctor;
    }

    public async Task<bool> DeleteDoctorAsync(Guid id)
    {
        var doctor = await _context.Doctors.FindAsync(id);
        if (doctor == null)
            return false;

        doctor.IsActive = false;
        doctor.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Doctor deleted with ID: {doctor.Id}");
        return true;
    }

    public async Task<IEnumerable<Doctor>> GetDoctorsBySpecializationAsync(string specialization)
    {
        return await _context.Doctors
            .Where(d => d.IsActive && 
                   d.Specialization.ToLower().Contains(specialization.ToLower()))
            .ToListAsync();
    }

    public async Task<IEnumerable<Doctor>> GetAvailableDoctorsAsync()
    {
        return await _context.Doctors
            .Where(d => d.IsActive && d.IsAvailable)
            .OrderBy(d => d.LastName)
            .ToListAsync();
    }
}