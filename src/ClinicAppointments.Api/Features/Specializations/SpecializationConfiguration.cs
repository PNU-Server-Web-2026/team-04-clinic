using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicAppointments.Api.Features.Specializations;

/// <summary>
/// Обмеження спеціалізації в PostgreSQL: довжина, обов'язковість і унікальність назви.
/// </summary>
public class SpecializationConfiguration : IEntityTypeConfiguration<Specialization>
{
    public const int NameMaxLength = 100;

    public const int DescriptionMaxLength = 1000;

    public void Configure(EntityTypeBuilder<Specialization> builder)
    {
        builder.HasKey(specialization => specialization.Id);

        builder.Property(specialization => specialization.Name)
            .IsRequired()
            .HasMaxLength(NameMaxLength);

        builder.Property(specialization => specialization.Description)
            .HasMaxLength(DescriptionMaxLength);

        builder.HasIndex(specialization => specialization.Name)
            .IsUnique();
    }
}
