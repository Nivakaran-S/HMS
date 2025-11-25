using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Steeltoe.Discovery.Client;

namespace Common.Extensions;

public static class EurekaExtensions
{
    /// <summary>
    /// Registers Steeltoe's Eureka discovery client and wires basic health checks so that
    /// every service exposes consistent metadata to the registry.
    /// </summary>
    public static IServiceCollection AddEurekaRegistration(
        this IServiceCollection services,
        IConfiguration configuration,
        string serviceName)
    {
        services.AddDiscoveryClient(configuration);
        services.AddHealthChecks()
            .AddCheck(serviceName, () => HealthCheckResult.Healthy());

        return services;
    }

    /// <summary>
    /// Maps a consistent set of health/metrics endpoints that can be scraped by
    /// the gateway, Kubernetes and external monitors.
    /// </summary>
    public static void MapDefaultHealthEndpoints(
        this WebApplication app,
        string serviceName,
        string apiVersion,
        DateTimeOffset? startedAt = null)
    {
        var bootTime = startedAt ?? DateTimeOffset.UtcNow;
        var group = app.MapGroup($"/api/{serviceName}/health")
            .WithTags("Health")
            .AllowAnonymous();

        group.MapGet("/live", () => Results.Ok(new
            {
                status = "live",
                service = serviceName,
                version = apiVersion
            }))
            .WithName($"{serviceName}-live");

        group.MapGet("/ready", async (HealthCheckService healthChecks, CancellationToken ct) =>
            {
                var report = await healthChecks.CheckHealthAsync(ct);
                return report.Status == HealthStatus.Healthy
                    ? Results.Ok(new
                    {
                        status = "ready",
                        service = serviceName,
                        version = apiVersion
                    })
                    : Results.Problem(
                        title: "Service not ready",
                        statusCode: StatusCodes.Status503ServiceUnavailable,
                        extensions: new Dictionary<string, object?>
                        {
                            ["service"] = serviceName,
                            ["version"] = apiVersion,
                            ["details"] = report.Entries
                        });
            })
            .WithName($"{serviceName}-ready");

        group.MapGet("/metrics", () => Results.Ok(new
            {
                service = serviceName,
                version = apiVersion,
                uptimeSeconds = (DateTimeOffset.UtcNow - bootTime).TotalSeconds
            }))
            .WithName($"{serviceName}-metrics");
    }
}

