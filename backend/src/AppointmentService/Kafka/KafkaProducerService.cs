using Confluent.Kafka;
using System.Text.Json;

namespace AppointmentService.Kafka;

public interface IKafkaProducerService
{
    Task PublishAsync<T>(string topic, T message);
    Task PublishRawAsync(string topic, string payload);
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
        var serializedMessage = JsonSerializer.Serialize(message);
        await PublishRawAsync(topic, serializedMessage);
    }

    public async Task PublishRawAsync(string topic, string payload)
    {
        var kafkaMessage = new Message<string, string>
        {
            Key = Guid.NewGuid().ToString(),
            Value = payload
        };

        await _producer.ProduceAsync(topic, kafkaMessage);
        _logger.LogInformation($"Message published to {topic}");
    }
}