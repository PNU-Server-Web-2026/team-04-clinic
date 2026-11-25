using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicAppointments.Api.Features.MedicalServices;

/// <summary>
/// Налаштування збереження медичних послуг у базі даних.
/// </summary>
public class MedicalServiceConfiguration : IEntityTypeConfiguration<MedicalService>
{
    public const int NameMaxLength = 200;

    public const int DurationMinutesMin = 10;

    public const int DurationMinutesMax = 240;

    public const int DurationMinutesStep = 5;

    public const int PriceMin = 0;

    public const int PricePrecision = 10;

    public const int PriceScale = 2;

    public void Configure(EntityTypeBuilder<MedicalService> builder)
    {
        builder.HasKey(service => service.Id);

        builder.Property(service => service.Name)
            .IsRequired()
            .HasMaxLength(NameMaxLength);

        builder.Property(service => service.DurationMinutes)
            .IsRequired();

        builder.Property(service => service.Price)
            .IsRequired()
            .HasPrecision(PricePrecision, PriceScale);

        builder.ToTable(table =>
        {
            table.HasCheckConstraint(
                "CK_MedicalServices_DurationMinutes",
                $"\"DurationMinutes\" >= {DurationMinutesMin} AND " +
                $"\"DurationMinutes\" <= {DurationMinutesMax} AND " +
                $"\"DurationMinutes\" % {DurationMinutesStep} = 0");
            table.HasCheckConstraint(
                "CK_MedicalServices_Price",
                $"\"Price\" >= {PriceMin}");
        });
    }
}
