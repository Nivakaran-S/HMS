using Microsoft.EntityFrameworkCore;
using AppointmentService.Data;
using AppointmentService.Models;
using AppointmentService.Kafka;
using Common.Events;

namespace AppointmentService.Services;

public interface IAppointmentService
{
    Task<IEnumerable<Appointment>> GetAllAppointmentsAsync();
    Task<Appointment?> GetAppointmentByIdAsync(Guid id);
    Task<Appointment> CreateAppointmentAsync(AppointmentDto appointmentDto);
    Task<Appointment?> UpdateAppointmentAsync(Guid id, AppointmentDto appointmentDto);
    Task<bool> DeleteAppointmentAsync(Guid id);
    Task<IEnumerable<Appointment>> GetAppointmentsByPatientAsync(Guid patientId);
    Task<IEnumerable<Appointment>> GetAppointmentsByDoctorAsync(Guid doctorId);
    Task<Appointment?> CompleteAppointmentAsync(Guid id, string notes, decimal consultationFee);
    Task<Appointment?> CancelAppointmentAsync(Guid id, string reason);
}

public class AppointmentServiceImpl : IAppointmentService
{
    private readonly AppointmentDbContext _context;
    private readonly IKafkaProducerService _kafkaProducer;
    private readonly ILogger<AppointmentServiceImpl> _logger;

    public AppointmentServiceImpl(
        AppointmentDbContext context,
        IKafkaProducerService kafkaProducer,
        ILogger<AppointmentServiceImpl> logger)
    {
        _context = context;
        _kafkaProducer = kafkaProducer;
        _logger = logger;
    }

    public async Task<IEnumerable<Appointment>> GetAllAppointmentsAsync()
    {
        return await _context.Appointments
            .Where(a => a.IsActive)
            .OrderByDescending(a => a.AppointmentDateTime)
            .ToListAsync();
    }

    public async Task<Appointment?> GetAppointmentByIdAsync(Guid id)
    {
        return await _context.Appointments
            .FirstOrDefaultAsync(a => a.Id == id && a.IsActive);
    }

    public async Task<Appointment> CreateAppointmentAsync(AppointmentDto appointmentDto)
    {
        var appointment = new Appointment
        {
            Id = Guid.NewGuid(),
            PatientId = appointmentDto.PatientId,
            DoctorId = appointmentDto.DoctorId,
            AppointmentDateTime = appointmentDto.AppointmentDateTime,
            Reason = appointmentDto.Reason,
            Status = "Scheduled",
            Notes = appointmentDto.Notes,
            DurationMinutes = appointmentDto.DurationMinutes,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsActive = true
        };

        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync();

        // Publish event to Kafka
        var appointmentEvent = new AppointmentCreatedEvent
        {
            AppointmentId = appointment.Id,
            PatientId = appointment.PatientId,
            DoctorId = appointment.DoctorId,
            AppointmentDateTime = appointment.AppointmentDateTime,
            Reason = appointment.Reason,
            Status = appointment.Status,
            CreatedAt = appointment.CreatedAt
        };

        await _kafkaProducer.PublishAsync("appointment-created", appointmentEvent);
        _logger.LogInformation($"Appointment created with ID: {appointment.Id}");

        return appointment;
    }

    public async Task<Appointment?> UpdateAppointmentAsync(Guid id, AppointmentDto appointmentDto)
    {
        var appointment = await _context.Appointments.FindAsync(id);
        if (appointment == null || !appointment.IsActive)
            return null;

        appointment.PatientId = appointmentDto.PatientId;
        appointment.DoctorId = appointmentDto.DoctorId;
        appointment.AppointmentDateTime = appointmentDto.AppointmentDateTime;
        appointment.Reason = appointmentDto.Reason;
        appointment.Notes = appointmentDto.Notes;
        appointment.DurationMinutes = appointmentDto.DurationMinutes;
        appointment.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        _logger.LogInformation($"Appointment updated with ID: {appointment.Id}");

        return appointment;
    }

    public async Task<bool> DeleteAppointmentAsync(Guid id)
    {
        var appointment = await _context.Appointments.FindAsync(id);
        if (appointment == null)
            return false;

        appointment.IsActive = false;
        appointment.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Appointment deleted with ID: {appointment.Id}");
        return true;
    }

    public async Task<IEnumerable<Appointment>> GetAppointmentsByPatientAsync(Guid patientId)
    {
        return await _context.Appointments
            .Where(a => a.IsActive && a.PatientId == patientId)
            .OrderByDescending(a => a.AppointmentDateTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<Appointment>> GetAppointmentsByDoctorAsync(Guid doctorId)
    {
        return await _context.Appointments
            .Where(a => a.IsActive && a.DoctorId == doctorId)
            .OrderByDescending(a => a.AppointmentDateTime)
            .ToListAsync();
    }

    public async Task<Appointment?> CompleteAppointmentAsync(Guid id, string notes, decimal consultationFee)
    {
        var appointment = await _context.Appointments.FindAsync(id);
        if (appointment == null || !appointment.IsActive)
            return null;

        appointment.Status = "Completed";
        appointment.Notes = notes;
        appointment.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        // Publish event to Kafka for Billing Service
        var completedEvent = new AppointmentCompletedEvent
        {
            AppointmentId = appointment.Id,
            PatientId = appointment.PatientId,
            DoctorId = appointment.DoctorId,
            CompletedAt = DateTime.UtcNow,
            Notes = notes,
            ConsultationFee = consultationFee
        };

        await _kafkaProducer.PublishAsync("appointment-completed", completedEvent);
        _logger.LogInformation($"Appointment completed with ID: {appointment.Id}");

        return appointment;
    }

    public async Task<Appointment?> CancelAppointmentAsync(Guid id, string reason)
    {
        var appointment = await _context.Appointments.FindAsync(id);
        if (appointment == null || !appointment.IsActive)
            return null;

        appointment.Status = "Cancelled";
        appointment.Notes = $"Cancelled: {reason}";
        appointment.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        var cancelledEvent = new AppointmentCancelledEvent
        {
            AppointmentId = appointment.Id,
            PatientId = appointment.PatientId,
            Reason = reason,
            CancelledAt = DateTime.UtcNow
        };

        await _kafkaProducer.PublishAsync("appointment-cancelled", cancelledEvent);
        _logger.LogInformation($"Appointment cancelled with ID: {appointment.Id}");

        return appointment;
    }
}