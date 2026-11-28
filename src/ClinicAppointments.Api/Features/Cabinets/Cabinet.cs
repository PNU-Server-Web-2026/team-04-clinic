namespace ClinicAppointments.Api.Features.Cabinets;

/// <summary>
/// Кабінет клініки. Таблицю і обмеження задає <see cref="CabinetConfiguration"/>.
/// </summary>
public class Cabinet
{
    public int Id { get; set; }

    public string Number { get; set; } = string.Empty;

    public int Floor { get; set; }

    public string? Description { get; set; }
}
