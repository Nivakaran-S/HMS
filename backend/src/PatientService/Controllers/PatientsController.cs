using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PatientService.Models;
using PatientService.Services;

namespace PatientService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "admin,reception")]
public class PatientsController : ControllerBase
{
    private readonly IPatientService _patientService;
    private readonly ILogger<PatientsController> _logger;

    public PatientsController(
        IPatientService patientService,
        ILogger<PatientsController> logger)
    {
        _patientService = patientService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Patient>>> GetAllPatients()
    {
        try
        {
            var patients = await _patientService.GetAllPatientsAsync();
            return Ok(patients);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving patients");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Patient>> GetPatient(Guid id)
    {
        try
        {
            var patient = await _patientService.GetPatientByIdAsync(id);
            if (patient == null)
                return NotFound($"Patient with ID {id} not found");

            return Ok(patient);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error retrieving patient {id}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost]
    public async Task<ActionResult<Patient>> CreatePatient([FromBody] PatientDto patientDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var patient = await _patientService.CreatePatientAsync(patientDto);
            return CreatedAtAction(nameof(GetPatient), new { id = patient.Id }, patient);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating patient");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Patient>> UpdatePatient(Guid id, [FromBody] PatientDto patientDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var patient = await _patientService.UpdatePatientAsync(id, patientDto);
            if (patient == null)
                return NotFound($"Patient with ID {id} not found");

            return Ok(patient);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating patient {id}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeletePatient(Guid id)
    {
        try
        {
            var result = await _patientService.DeletePatientAsync(id);
            if (!result)
                return NotFound($"Patient with ID {id} not found");

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting patient {id}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<Patient>>> SearchPatients([FromQuery] string term)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(term))
                return BadRequest("Search term is required");

            var patients = await _patientService.SearchPatientsAsync(term);
            return Ok(patients);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching patients");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("health")]
    [AllowAnonymous]
    public IActionResult Health()
    {
        return Ok(new { status = "healthy", service = "patient-service" });
    }
}