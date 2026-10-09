using System.ComponentModel.DataAnnotations;

namespace ClinicAppointments.Api.Common;

public sealed class AppointmentOptions
{
    public const string SectionName = "Appointment";

    [Required]
    [Range(1, int.MaxValue)]
    public int MinCancelHours { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int MaxBookingDaysAhead { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int NoShowAfterMinutes { get; set; }
}
