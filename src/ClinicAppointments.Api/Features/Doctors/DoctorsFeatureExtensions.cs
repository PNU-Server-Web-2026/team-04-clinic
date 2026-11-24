namespace ClinicAppointments.Api.Features.Doctors;

public static class DoctorsFeatureExtensions
{
    public static IServiceCollection AddDoctorsFeature(this IServiceCollection services)
    {
        services.AddSingleton<IDoctorService, InMemoryDoctorService>();
        return services;
    }
}
