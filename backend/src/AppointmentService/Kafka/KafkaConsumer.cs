using System.Text.Json;
using AppointmentService.Data;
using Confluent.Kafka;
using Common.Events;
using Microsoft.EntityFrameworkCore;

namespace AppointmentService.Kafka;

public class AppointmentPaymentsConsumer : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConsumer<string, string> _consumer;
    private readonly ILogger<AppointmentPaymentsConsumer> _logger;

    public AppointmentPaymentsConsumer(
        IServiceProvider serviceProvider,
        IConsumer<string, string> consumer,
        ILogger<AppointmentPaymentsConsumer> logger)
    {
        _serviceProvider = serviceProvider;
        _consumer = consumer;
        _logger = logger;

        _consumer.Subscribe("billing-payment-processed");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Appointment payment consumer started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var consumeResult = _consumer.Consume(stoppingToken);
                if (consumeResult?.Message?.Value == null)
                {
                    continue;
                }

                var paymentEvent = JsonSerializer.Deserialize<BillPaymentProcessedEvent>(consumeResult.Message.Value);
                if (paymentEvent == null)
                {
                    _logger.LogWarning("Unable to deserialize payment event: {Payload}", consumeResult.Message.Value);
                    continue;
                }

                await ProcessPaymentEventAsync(paymentEvent, stoppingToken);
            }
            catch (ConsumeException ex)
            {
                _logger.LogError(ex, "Kafka consume error");
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Appointment payment consumer cancellation requested.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while processing Kafka message");
            }
        }
    }

    private async Task ProcessPaymentEventAsync(BillPaymentProcessedEvent paymentEvent, CancellationToken ct)
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppointmentDbContext>();

        var appointment = await dbContext.Appointments
            .FirstOrDefaultAsync(a => a.Id == paymentEvent.AppointmentId, ct);

        if (appointment == null)
        {
            _logger.LogWarning("Appointment {AppointmentId} not found for payment event.", paymentEvent.AppointmentId);
            return;
        }

        appointment.BillingStatus = paymentEvent.PaymentStatus;
        appointment.UpdatedAt = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(ct);
        _logger.LogInformation("Appointment {AppointmentId} billing status set to {Status}",
            appointment.Id, appointment.BillingStatus);
    }

    public override void Dispose()
    {
        _consumer.Close();
        _consumer.Dispose();
        base.Dispose();
    }
}

