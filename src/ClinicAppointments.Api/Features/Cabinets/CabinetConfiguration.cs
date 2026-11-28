using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicAppointments.Api.Features.Cabinets;

/// <summary>
/// Обмеження кабінету в PostgreSQL: довжини й обов'язковість.
/// </summary>
public class CabinetConfiguration : IEntityTypeConfiguration<Cabinet>
{
    public const int NumberMaxLength = 10;

    public const int DescriptionMaxLength = 500;

    public void Configure(EntityTypeBuilder<Cabinet> builder)
    {
        builder.HasKey(cabinet => cabinet.Id);

        builder.Property(cabinet => cabinet.Number)
            .IsRequired()
            .HasMaxLength(NumberMaxLength);

        builder.Property(cabinet => cabinet.Floor)
            .IsRequired();

        builder.Property(cabinet => cabinet.Description)
            .HasMaxLength(DescriptionMaxLength);
    }
}
