namespace ClinicAppointments.Api.Features.MedicalServices;

public static class MedicalServicesFeatureExtensions
{
    public static IServiceCollection AddMedicalServicesFeature(this IServiceCollection services)
    {
        services.AddSingleton<IMedicalServiceService, InMemoryMedicalServiceService>();
        return services;
    }

    public static WebApplication MapMedicalServicesFeature(this WebApplication app)
    {
        return app;
    }
}
