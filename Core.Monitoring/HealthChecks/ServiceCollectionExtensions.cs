using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Prometheus;

namespace Core.Monitoring.HealthChecks;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMonitoring(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<IHealthChecksBuilder>? builder = null)
    {
        var healthCheckBuilder = services
            .AddHealthChecks()
            .ForwardToPrometheus();

        builder?.Invoke(healthCheckBuilder);

        return services;
    }

    public static IApplicationBuilder UseMonitoring(this IApplicationBuilder app)
    {
        app.UseHttpMetrics();
        app.UseGrpcMetrics();
        app.UseMetricServer();

        app.UseHealthChecks("/health", new HealthCheckOptions
        {
            Predicate = _ => true,
            AllowCachingResponses = false
        });

        app.UseHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = _ => true,
            AllowCachingResponses = false
        });

        return app;
    }
}