using ClinicAppointments.Api.Data;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ClinicAppointments.Api.Features.Health;

public static class HealthFeatureExtensions
{
    public const string ReadyTag = "ready";

    public static IServiceCollection AddHealthFeature(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddDbContextCheck<AppDbContext>(tags: [ReadyTag]);

        return services;
    }

    public static WebApplication MapHealthFeature(this WebApplication app)
    {
        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            Predicate = _ => false,
            ResponseWriter = HealthCheckJsonResponseWriter.WriteResponse,
        });

        app.MapHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = registration => registration.Tags.Contains(ReadyTag),
            ResponseWriter = HealthCheckJsonResponseWriter.WriteResponse,
        });

        return app;
    }
}
