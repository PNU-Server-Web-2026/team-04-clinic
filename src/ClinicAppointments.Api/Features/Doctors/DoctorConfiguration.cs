using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicAppointments.Api.Features.Doctors;

/// <summary>
/// Обмеження лікаря в PostgreSQL: довжини, обов'язковість, унікальність і перевірки.
/// </summary>
public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
{
    public const int NameMaxLength = 100;

    public const int EmailMaxLength = 256;

    /// <summary>E.164: «+» і щонайбільше 15 цифр.</summary>
    public const int PhoneMaxLength = 16;

    public const int LicenseNumberMaxLength = 20;

    public const int ExperienceYearsMin = 0;

    public const int ExperienceYearsMax = 60;

    public void Configure(EntityTypeBuilder<Doctor> builder)
    {
        builder.HasKey(doctor => doctor.Id);

        builder.Property(doctor => doctor.FirstName)
            .IsRequired()
            .HasMaxLength(NameMaxLength);

        builder.Property(doctor => doctor.LastName)
            .IsRequired()
            .HasMaxLength(NameMaxLength);

        builder.Property(doctor => doctor.Email)
            .IsRequired()
            .HasMaxLength(EmailMaxLength);

        builder.Property(doctor => doctor.Phone)
            .IsRequired()
            .HasMaxLength(PhoneMaxLength);

        builder.Property(doctor => doctor.LicenseNumber)
            .IsRequired()
            .HasMaxLength(LicenseNumberMaxLength);

        builder.Property(doctor => doctor.ExperienceYears)
            .IsRequired();

        builder.HasIndex(doctor => doctor.Email)
            .IsUnique();

        builder.HasIndex(doctor => doctor.LicenseNumber)
            .IsUnique();

        builder.ToTable(table =>
        {
            table.HasCheckConstraint(
                "CK_Doctors_ExperienceYears",
                $"\"ExperienceYears\" >= {ExperienceYearsMin} AND \"ExperienceYears\" <= {ExperienceYearsMax}");

            table.HasCheckConstraint(
                "CK_Doctors_Phone_E164",
                "\"Phone\" ~ '^\\+[1-9][0-9]{1,14}$'");
        });
    }
}
