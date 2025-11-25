using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AppointmentService.Models;
using AppointmentService.Services;

namespace AppointmentService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "admin,reception")]
public class AppointmentsController : ControllerBase
{
    private readonly IAppointmentService _appointmentService;
    private readonly ILogger<AppointmentsController> _logger;

    public AppointmentsController(
        IAppointmentService appointmentService,
        ILogger<AppointmentsController> logger)
    {
        _appointmentService = appointmentService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Appointment>>> GetAllAppointments()
    {
        try
        {
            var appointments = await _appointmentService.GetAllAppointmentsAsync();
            return Ok(appointments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving appointments");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Appointment>> GetAppointment(Guid id)
    {
        try
        {
            var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
            if (appointment == null)
                return NotFound($"Appointment with ID {id} not found");

            return Ok(appointment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error retrieving appointment {id}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost]
    public async Task<ActionResult<Appointment>> CreateAppointment([FromBody] AppointmentDto appointmentDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var appointment = await _appointmentService.CreateAppointmentAsync(appointmentDto);
            return CreatedAtAction(nameof(GetAppointment), new { id = appointment.Id }, appointment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating appointment");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Appointment>> UpdateAppointment(Guid id, [FromBody] AppointmentDto appointmentDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var appointment = await _appointmentService.UpdateAppointmentAsync(id, appointmentDto);
            if (appointment == null)
                return NotFound($"Appointment with ID {id} not found");

            return Ok(appointment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating appointment {id}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAppointment(Guid id)
    {
        try
        {
            var result = await _appointmentService.DeleteAppointmentAsync(id);
            if (!result)
                return NotFound($"Appointment with ID {id} not found");

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting appointment {id}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("patient/{patientId}")]
    public async Task<ActionResult<IEnumerable<Appointment>>> GetAppointmentsByPatient(Guid patientId)
    {
        try
        {
            var appointments = await _appointmentService.GetAppointmentsByPatientAsync(patientId);
            return Ok(appointments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving patient appointments");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("doctor/{doctorId}")]
    public async Task<ActionResult<IEnumerable<Appointment>>> GetAppointmentsByDoctor(Guid doctorId)
    {
        try
        {
            var appointments = await _appointmentService.GetAppointmentsByDoctorAsync(doctorId);
            return Ok(appointments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving doctor appointments");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("{id}/complete")]
    public async Task<ActionResult<Appointment>> CompleteAppointment(
        Guid id,
        [FromBody] CompleteAppointmentRequest request)
    {
        try
        {
            var appointment = await _appointmentService.CompleteAppointmentAsync(
                id, request.Notes, request.ConsultationFee);
            
            if (appointment == null)
                return NotFound($"Appointment with ID {id} not found");

            return Ok(appointment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error completing appointment {id}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("{id}/cancel")]
    public async Task<ActionResult<Appointment>> CancelAppointment(
        Guid id,
        [FromBody] CancelAppointmentRequest request)
    {
        try
        {
            var appointment = await _appointmentService.CancelAppointmentAsync(id, request.Reason);
            if (appointment == null)
                return NotFound($"Appointment with ID {id} not found");

            return Ok(appointment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error cancelling appointment {id}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("health")]
    [AllowAnonymous]
    public IActionResult Health()
    {
        return Ok(new { status = "healthy", service = "appointment-service" });
    }
}

public record CompleteAppointmentRequest(string Notes, decimal ConsultationFee);
public record CancelAppointmentRequest(string Reason);