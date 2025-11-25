using AppointmentService.Data;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Retry;

namespace AppointmentService.Kafka;

public class OutboxPublisherService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IKafkaProducerService _kafkaProducer;
    private readonly ILogger<OutboxPublisherService> _logger;
    private readonly AsyncRetryPolicy _retryPolicy;

    public OutboxPublisherService(
        IServiceProvider serviceProvider,
        IKafkaProducerService kafkaProducer,
        ILogger<OutboxPublisherService> logger)
    {
        _serviceProvider = serviceProvider;
        _kafkaProducer = kafkaProducer;
        _logger = logger;

        _retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                3,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (exception, timespan, retryAttempt, _) =>
                {
                    _logger.LogWarning(exception,
                        "Retrying outbox publish attempt {Attempt} after {Delay}", retryAttempt, timespan);
                });
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await PublishPendingMessagesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while publishing outbox messages");
            }

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }

    private async Task PublishPendingMessagesAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppointmentDbContext>();

        var pendingMessages = await dbContext.OutboxMessages
            .Where(message => message.SentAt == null)
            .OrderBy(message => message.CreatedAt)
            .Take(50)
            .ToListAsync(cancellationToken);

        if (pendingMessages.Count == 0)
        {
            return;
        }

        foreach (var message in pendingMessages)
        {
            try
            {
                await _retryPolicy.ExecuteAsync(async () =>
                {
                    await _kafkaProducer.PublishRawAsync(message.Topic, message.Payload);
                });

                message.SentAt = DateTime.UtcNow;
                message.LastError = null;
            }
            catch (Exception ex)
            {
                message.RetryCount += 1;
                message.LastError = ex.Message;
                _logger.LogError(ex, "Failed to publish outbox message {MessageId}", message.Id);
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}


