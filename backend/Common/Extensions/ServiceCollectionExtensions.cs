using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddKafkaProducer(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        var config = new ProducerConfig
        {
            BootstrapServers = configuration["Kafka:BootstrapServers"] ?? "localhost:9092",
            ClientId = configuration["Spring:Application:Name"] ?? "default-service"
        };

        services.AddSingleton<IProducer<string, string>>(sp => 
            new ProducerBuilder<string, string>(config).Build());

        return services;
    }

    public static IServiceCollection AddKafkaConsumer(
        this IServiceCollection services,
        IConfiguration configuration,
        string groupId)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = configuration["Kafka:BootstrapServers"] ?? "localhost:9092",
            GroupId = groupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = true
        };

        services.AddSingleton<IConsumer<string, string>>(sp =>
            new ConsumerBuilder<string, string>(config).Build());

        return services;
    }
}