using Microsoft.Extensions.DependencyInjection;

namespace ClinicAppointments.Api.Features.Specializations;

public static class SpecializationsFeatureExtensions
{
    public static IServiceCollection AddSpecializationsFeature(
        this IServiceCollection services)
    {
        services.AddSingleton<
            ISpecializationService,
            InMemorySpecializationService>();

        return services;
    }
}