namespace ClinicAppointments.Api.Features.Doctors;

public static class DoctorsFeatureExtensions
{
    public static IServiceCollection AddDoctorsFeature(this IServiceCollection services)
    {
        services.AddSingleton<IDoctorService, InMemoryDoctorService>();
        return services;
    }

    public static WebApplication MapDoctorsFeature(this WebApplication app)
    {
        // Controllers are mapped automatically by MapControllers() in Program.cs
        return app;
    }
}
