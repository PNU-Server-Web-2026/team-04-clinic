using ClinicAppointments.Api.Features.Specializations;
using Microsoft.EntityFrameworkCore;

namespace ClinicAppointments.Api.Data;

/// <summary>
/// Контекст бази даних застосунку.
/// </summary>
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Specialization> Specializations => Set<Specialization>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Підхоплює всі класи IEntityTypeConfiguration<T> з цієї збірки.
        // Конфігурацію сутності тримайте поруч із фічею: Features/<Name>/<Entity>Configuration.cs
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
