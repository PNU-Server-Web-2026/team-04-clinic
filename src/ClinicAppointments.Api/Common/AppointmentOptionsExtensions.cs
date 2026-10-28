namespace ClinicAppointments.Api.Common;

public static class AppointmentOptionsExtensions
{
    public static IServiceCollection AddAppointmentOptions(this IServiceCollection services)
    {
        services
            .AddOptions<AppointmentOptions>()
            .BindConfiguration(AppointmentOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }
}
