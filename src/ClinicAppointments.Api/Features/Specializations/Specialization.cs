namespace ClinicAppointments.Api.Features.Specializations;

/// <summary>
/// Спеціалізація лікаря. Таблицю і обмеження задає <see cref="SpecializationConfiguration"/>.
/// </summary>
public class Specialization
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }
}
