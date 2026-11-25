using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicAppointments.Api.Features.MedicalServices;

public sealed class MedicalServiceConfiguration : IEntityTypeConfiguration<MedicalService>
{
    public void Configure(EntityTypeBuilder<MedicalService> builder)
    {
        builder.ToTable("MedicalServices", table =>
        {
            table.HasCheckConstraint(
                "CK_MedicalServices_DurationMinutes",
                "\"DurationMinutes\" >= 10 AND \"DurationMinutes\" <= 240 AND \"DurationMinutes\" % 5 = 0");
            table.HasCheckConstraint(
                "CK_MedicalServices_Price",
                "\"Price\" >= 0");
        });

        builder.HasKey(service => service.Id);

        builder.Property(service => service.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(service => service.DurationMinutes)
            .IsRequired();

        builder.Property(service => service.Price)
            .IsRequired()
            .HasPrecision(10, 2);

        builder.HasIndex(service => service.Name)
            .IsUnique();
    }
}
