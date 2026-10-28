using Microsoft.Net.Http.Headers;

namespace ClinicAppointments.Api.Features.Cors;

public static class CorsFeatureExtensions
{
    public const string PolicyName = "Frontend";

    public static IServiceCollection AddCorsFeature(this IServiceCollection services, IConfiguration configuration)
    {
        var origins = configuration.GetSection("Cors:Origins").Get<string[]>() ?? [];

        services.AddCors(options => options.AddPolicy(PolicyName, policy =>
        {
            policy.WithOrigins(origins)
                .WithMethods(HttpMethods.Get, HttpMethods.Post, HttpMethods.Put, HttpMethods.Patch, HttpMethods.Delete)
                .WithHeaders(HeaderNames.Accept, HeaderNames.Authorization, HeaderNames.ContentType);
        }));

        return services;
    }

    public static WebApplication UseCorsFeature(this WebApplication app)
    {
        app.UseCors(PolicyName);
        return app;
    }
}
