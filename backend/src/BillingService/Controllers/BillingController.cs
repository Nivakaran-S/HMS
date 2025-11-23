using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BillingService.Models;
using BillingService.Services;

namespace BillingService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BillingController : ControllerBase
{
    private readonly IBillingService _billingService;
    private readonly ILogger<BillingController> _logger;

    public BillingController(
        IBillingService billingService,
        ILogger<BillingController> logger)
    {
        _billingService = billingService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Bill>>> GetAllBills()
    {
        try
        {
            var bills = await _billingService.GetAllBillsAsync();
            return Ok(bills);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving bills");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Bill>> GetBill(Guid id)
    {
        try
        {
            var bill = await _billingService.GetBillByIdAsync(id);
            if (bill == null)
                return NotFound($"Bill with ID {id} not found");

            return Ok(bill);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error retrieving bill {id}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost]
    public async Task<ActionResult<Bill>> CreateBill([FromBody] BillDto billDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var bill = await _billingService.CreateBillAsync(billDto);
            return CreatedAtAction(nameof(GetBill), new { id = bill.Id }, bill);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating bill");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Bill>> UpdateBill(Guid id, [FromBody] BillDto billDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var bill = await _billingService.UpdateBillAsync(id, billDto);
            if (bill == null)
                return NotFound($"Bill with ID {id} not found");

            return Ok(bill);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating bill {id}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteBill(Guid id)
    {
        try
        {
            var result = await _billingService.DeleteBillAsync(id);
            if (!result)
                return NotFound($"Bill with ID {id} not found");

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting bill {id}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("patient/{patientId}")]
    public async Task<ActionResult<IEnumerable<Bill>>> GetBillsByPatient(Guid patientId)
    {
        try
        {
            var bills = await _billingService.GetBillsByPatientAsync(patientId);
            return Ok(bills);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving patient bills");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("{id}/payment")]
    public async Task<ActionResult<Bill>> ProcessPayment(Guid id, [FromBody] PaymentDto paymentDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var bill = await _billingService.ProcessPaymentAsync(id, paymentDto);
            if (bill == null)
                return NotFound($"Bill with ID {id} not found");

            return Ok(bill);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error processing payment for bill {id}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("health")]
    [AllowAnonymous]
    public IActionResult Health()
    {
        return Ok(new { status = "healthy", service = "billing-service" });
    }
}