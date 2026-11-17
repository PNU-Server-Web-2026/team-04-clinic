namespace ClinicAppointments.Api.Features.Cabinets;

public static class CabinetsFeatureExtensions
{
    public static IServiceCollection AddCabinetsFeature(this IServiceCollection services)
    {
        services.AddSingleton<ICabinetService, InMemoryCabinetService>();
        return services;
    }
}
