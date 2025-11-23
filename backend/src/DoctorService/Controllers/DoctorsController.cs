using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DoctorService.Models;
using DoctorService.Services;

namespace DoctorService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DoctorsController : ControllerBase
{
    private readonly IDoctorService _doctorService;
    private readonly ILogger<DoctorsController> _logger;

    public DoctorsController(
        IDoctorService doctorService,
        ILogger<DoctorsController> logger)
    {
        _doctorService = doctorService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Doctor>>> GetAllDoctors()
    {
        try
        {
            var doctors = await _doctorService.GetAllDoctorsAsync();
            return Ok(doctors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving doctors");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Doctor>> GetDoctor(Guid id)
    {
        try
        {
            var doctor = await _doctorService.GetDoctorByIdAsync(id);
            if (doctor == null)
                return NotFound($"Doctor with ID {id} not found");

            return Ok(doctor);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error retrieving doctor {id}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost]
    public async Task<ActionResult<Doctor>> CreateDoctor([FromBody] DoctorDto doctorDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var doctor = await _doctorService.CreateDoctorAsync(doctorDto);
            return CreatedAtAction(nameof(GetDoctor), new { id = doctor.Id }, doctor);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating doctor");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Doctor>> UpdateDoctor(Guid id, [FromBody] DoctorDto doctorDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var doctor = await _doctorService.UpdateDoctorAsync(id, doctorDto);
            if (doctor == null)
                return NotFound($"Doctor with ID {id} not found");

            return Ok(doctor);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating doctor {id}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteDoctor(Guid id)
    {
        try
        {
            var result = await _doctorService.DeleteDoctorAsync(id);
            if (!result)
                return NotFound($"Doctor with ID {id} not found");

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting doctor {id}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("specialization/{specialization}")]
    public async Task<ActionResult<IEnumerable<Doctor>>> GetDoctorsBySpecialization(string specialization)
    {
        try
        {
            var doctors = await _doctorService.GetDoctorsBySpecializationAsync(specialization);
            return Ok(doctors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving doctors by specialization");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("available")]
    public async Task<ActionResult<IEnumerable<Doctor>>> GetAvailableDoctors()
    {
        try
        {
            var doctors = await _doctorService.GetAvailableDoctorsAsync();
            return Ok(doctors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving available doctors");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("health")]
    [AllowAnonymous]
    public IActionResult Health()
    {
        return Ok(new { status = "healthy", service = "doctor-service" });
    }
}