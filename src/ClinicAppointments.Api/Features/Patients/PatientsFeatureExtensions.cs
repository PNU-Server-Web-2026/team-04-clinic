namespace ClinicAppointments.Api.Features.Patients;

public static class PatientsFeatureExtensions
{
    public static IServiceCollection AddPatientsFeature(this IServiceCollection services)
    {
        services.AddSingleton<IPatientService, InMemoryPatientService>();
        return services;
    }
}
