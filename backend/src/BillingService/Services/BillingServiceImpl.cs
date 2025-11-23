using Microsoft.EntityFrameworkCore;
using BillingService.Data;
using BillingService.Models;

namespace BillingService.Services;

public interface IBillingService
{
    Task<IEnumerable<Bill>> GetAllBillsAsync();
    Task<Bill?> GetBillByIdAsync(Guid id);
    Task<Bill> CreateBillAsync(BillDto billDto);
    Task<Bill?> UpdateBillAsync(Guid id, BillDto billDto);
    Task<bool> DeleteBillAsync(Guid id);
    Task<IEnumerable<Bill>> GetBillsByPatientAsync(Guid patientId);
    Task<Bill?> ProcessPaymentAsync(Guid billId, PaymentDto paymentDto);
    Task<Bill> CreateBillFromAppointmentAsync(Guid appointmentId, Guid patientId, Guid doctorId, decimal consultationFee, string notes);
}

public class BillingServiceImpl : IBillingService
{
    private readonly BillingDbContext _context;
    private readonly ILogger<BillingServiceImpl> _logger;
    private const decimal TAX_RATE = 0.10m; // 10% tax

    public BillingServiceImpl(
        BillingDbContext context,
        ILogger<BillingServiceImpl> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<Bill>> GetAllBillsAsync()
    {
        return await _context.Bills
            .Where(b => b.IsActive)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
    }

    public async Task<Bill?> GetBillByIdAsync(Guid id)
    {
        return await _context.Bills
            .FirstOrDefaultAsync(b => b.Id == id && b.IsActive);
    }

    public async Task<Bill> CreateBillAsync(BillDto billDto)
    {
        var bill = CreateBillFromDto(billDto);
        CalculateBillAmounts(bill);

        _context.Bills.Add(bill);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Bill created with ID: {bill.Id}");
        return bill;
    }

    public async Task<Bill> CreateBillFromAppointmentAsync(
        Guid appointmentId, 
        Guid patientId, 
        Guid doctorId, 
        decimal consultationFee, 
        string notes)
    {
        var bill = new Bill
        {
            Id = Guid.NewGuid(),
            AppointmentId = appointmentId,
            PatientId = patientId,
            DoctorId = doctorId,
            ConsultationFee = consultationFee,
            LabCharges = 0,
            MedicineCharges = 0,
            OtherCharges = 0,
            Discount = 0,
            PaymentStatus = "Pending",
            Notes = notes,
            BillDate = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(30),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsActive = true
        };

        CalculateBillAmounts(bill);

        _context.Bills.Add(bill);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Bill auto-created from appointment {appointmentId}");
        return bill;
    }

    public async Task<Bill?> UpdateBillAsync(Guid id, BillDto billDto)
    {
        var bill = await _context.Bills.FindAsync(id);
        if (bill == null || !bill.IsActive)
            return null;

        UpdateBillFromDto(bill, billDto);
        CalculateBillAmounts(bill);
        bill.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        _logger.LogInformation($"Bill updated with ID: {bill.Id}");

        return bill;
    }

    public async Task<bool> DeleteBillAsync(Guid id)
    {
        var bill = await _context.Bills.FindAsync(id);
        if (bill == null)
            return false;

        bill.IsActive = false;
        bill.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Bill deleted with ID: {bill.Id}");
        return true;
    }

    public async Task<IEnumerable<Bill>> GetBillsByPatientAsync(Guid patientId)
    {
        return await _context.Bills
            .Where(b => b.IsActive && b.PatientId == patientId)
            .OrderByDescending(b => b.BillDate)
            .ToListAsync();
    }

    public async Task<Bill?> ProcessPaymentAsync(Guid billId, PaymentDto paymentDto)
    {
        var bill = await _context.Bills.FindAsync(billId);
        if (bill == null || !bill.IsActive)
            return null;

        bill.PaymentStatus = "Paid";
        bill.PaymentMethod = paymentDto.PaymentMethod;
        bill.TransactionId = paymentDto.TransactionId;
        bill.PaymentDate = paymentDto.PaymentDate;
        bill.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        _logger.LogInformation($"Payment processed for bill {billId}");

        return bill;
    }

    private Bill CreateBillFromDto(BillDto billDto)
    {
        return new Bill
        {
            Id = Guid.NewGuid(),
            AppointmentId = billDto.AppointmentId,
            PatientId = billDto.PatientId,
            DoctorId = billDto.DoctorId,
            ConsultationFee = billDto.ConsultationFee,
            LabCharges = billDto.LabCharges,
            MedicineCharges = billDto.MedicineCharges,
            OtherCharges = billDto.OtherCharges,
            Discount = billDto.Discount,
            PaymentStatus = "Pending",
            PaymentMethod = billDto.PaymentMethod,
            Notes = billDto.Notes,
            BillDate = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(30),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsActive = true
        };
    }

    private void UpdateBillFromDto(Bill bill, BillDto billDto)
    {
        bill.ConsultationFee = billDto.ConsultationFee;
        bill.LabCharges = billDto.LabCharges;
        bill.MedicineCharges = billDto.MedicineCharges;
        bill.OtherCharges = billDto.OtherCharges;
        bill.Discount = billDto.Discount;
        bill.PaymentMethod = billDto.PaymentMethod;
        bill.Notes = billDto.Notes;
    }

    private void CalculateBillAmounts(Bill bill)
    {
        bill.TotalAmount = bill.ConsultationFee + bill.LabCharges + 
                          bill.MedicineCharges + bill.OtherCharges;
        
        bill.TaxAmount = (bill.TotalAmount - bill.Discount) * TAX_RATE;
        bill.NetAmount = bill.TotalAmount - bill.Discount + bill.TaxAmount;
    }
}