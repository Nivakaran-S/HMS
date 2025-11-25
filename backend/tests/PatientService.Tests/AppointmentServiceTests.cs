using System;
using System.Text.Json;
using System.Threading.Tasks;
using AppointmentService.Data;
using AppointmentService.Kafka;
using AppointmentService.Models;
using AppointmentService.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace PatientService.Tests;

public class AppointmentServiceTests
{
    private static AppointmentDbContext BuildContext()
    {
        var options = new DbContextOptionsBuilder<AppointmentDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppointmentDbContext(options);
    }

    [Fact]
    public async Task CompleteAppointment_ShouldQueueOutboxMessage()
    {
        using var context = BuildContext();
        var appointment = new Appointment
        {
            Id = Guid.NewGuid(),
            PatientId = Guid.NewGuid(),
            DoctorId = Guid.NewGuid(),
            AppointmentDateTime = DateTime.UtcNow.AddDays(-1),
            Reason = "Follow up",
            Status = "Scheduled",
            CreatedAt = DateTime.UtcNow.AddDays(-2),
            UpdatedAt = DateTime.UtcNow.AddDays(-2),
            IsActive = true
        };

        context.Appointments.Add(appointment);
        await context.SaveChangesAsync();

        var kafkaProducer = new TestKafkaProducer();
        var logger = NullLogger<AppointmentServiceImpl>.Instance;

        var service = new AppointmentServiceImpl(context, kafkaProducer, logger);

        var result = await service.CompleteAppointmentAsync(appointment.Id, "All good", 150m);

        Assert.NotNull(result);
        Assert.Equal("Completed", result.Status);
        Assert.Equal("Invoiced", result.BillingStatus);
        Assert.Single(context.OutboxMessages);
    }

    private sealed class TestKafkaProducer : IKafkaProducerService
    {
        public Task PublishAsync<T>(string topic, T message)
        {
            // No-op for unit test
            return Task.CompletedTask;
        }

        public Task PublishRawAsync(string topic, string payload)
        {
            // No-op for unit test
            return Task.CompletedTask;
        }
    }
}


