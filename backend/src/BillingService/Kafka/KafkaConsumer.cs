using Confluent.Kafka;
using System.Text.Json;
using BillingService.Services;
using Common.Events;

namespace BillingService.Kafka;

public class KafkaConsumerService : BackgroundService
{
    private readonly ILogger<KafkaConsumerService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IConsumer<string, string> _consumer;

    public KafkaConsumerService(
        ILogger<KafkaConsumerService> logger,
        IServiceProvider serviceProvider,
        IConsumer<string, string> consumer)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _consumer = consumer;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _consumer.Subscribe("appointment-completed");
        
        _logger.LogInformation("Kafka Consumer Service started. Listening to 'appointment-completed' topic...");

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = _consumer.Consume(stoppingToken);
                    
                    if (consumeResult?.Message?.Value != null)
                    {
                        _logger.LogInformation($"Received message from Kafka: {consumeResult.Message.Value}");
                        
                        var appointmentEvent = JsonSerializer.Deserialize<AppointmentCompletedEvent>(
                            consumeResult.Message.Value);

                        if (appointmentEvent != null)
                        {
                            await ProcessAppointmentCompletedEvent(appointmentEvent);
                        }
                    }
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError(ex, "Error consuming message from Kafka");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing Kafka message");
                }
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Kafka Consumer Service is stopping.");
        }
        finally
        {
            _consumer.Close();
        }
    }

    private async Task ProcessAppointmentCompletedEvent(AppointmentCompletedEvent appointmentEvent)
    {
        using var scope = _serviceProvider.CreateScope();
        var billingService = scope.ServiceProvider.GetRequiredService<IBillingService>();

        try
        {
            await billingService.CreateBillFromAppointmentAsync(
                appointmentEvent.AppointmentId,
                appointmentEvent.PatientId,
                appointmentEvent.DoctorId,
                appointmentEvent.ConsultationFee,
                appointmentEvent.Notes);

            _logger.LogInformation($"Bill created for appointment: {appointmentEvent.AppointmentId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to create bill for appointment: {appointmentEvent.AppointmentId}");
        }
    }

    public override void Dispose()
    {
        _consumer?.Dispose();
        base.Dispose();
    }
}