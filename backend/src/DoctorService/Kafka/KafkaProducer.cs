using Confluent.Kafka;
using System.Text.Json;

namespace DoctorService.Kafka;

public interface IKafkaProducerService
{
    Task PublishAsync<T>(string topic, T message);
}

public class KafkaProducerService : IKafkaProducerService
{
    private readonly IProducer<string, string> _producer;
    private readonly ILogger<KafkaProducerService> _logger;

    public KafkaProducerService(
        IProducer<string, string> producer,
        ILogger<KafkaProducerService> logger)
    {
        _producer = producer;
        _logger = logger;
    }

    public async Task PublishAsync<T>(string topic, T message)
    {
        try
        {
            var serializedMessage = JsonSerializer.Serialize(message);
            var kafkaMessage = new Message<string, string>
            {
                Key = Guid.NewGuid().ToString(),
                Value = serializedMessage
            };

            var result = await _producer.ProduceAsync(topic, kafkaMessage);
            _logger.LogInformation(
                $"Message published to {topic} at offset {result.Offset}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error publishing message to {topic}");
            throw;
        }
    }
}