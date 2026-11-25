using ClinicAppointments.Api.Data;
using ClinicAppointments.Api.Features.MedicalServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ClinicAppointments.Api.Tests.Features.MedicalServices;

public class MedicalServiceConfigurationTests
{
    [Fact]
    public void MedicalService_IsConfiguredForPersistence()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql("Host=localhost;Database=clinic_model_test;Username=test;Password=test")
            .Options;

        using var context = new AppDbContext(options);
        var model = context.GetService<IDesignTimeModel>().Model;
        var entity = model.FindEntityType(typeof(MedicalService));

        Assert.NotNull(entity);
        Assert.Equal("MedicalServices", entity.GetTableName());

        var name = entity.FindProperty(nameof(MedicalService.Name));
        Assert.NotNull(name);
        Assert.False(name.IsNullable);
        Assert.Equal(200, name.GetMaxLength());

        var duration = entity.FindProperty(nameof(MedicalService.DurationMinutes));
        Assert.NotNull(duration);
        Assert.False(duration.IsNullable);

        var price = entity.FindProperty(nameof(MedicalService.Price));
        Assert.NotNull(price);
        Assert.False(price.IsNullable);
        Assert.Equal((10, 2), (price.GetPrecision()!.Value, price.GetScale()!.Value));

        Assert.Contains(entity.GetIndexes(), index =>
            index.IsUnique &&
            index.Properties.Count == 1 &&
            index.Properties[0].Name == nameof(MedicalService.Name));

        var table = entity.GetCheckConstraints().ToDictionary(
            constraint => constraint.Name!,
            constraint => constraint.Sql);

        Assert.Contains("CK_MedicalServices_DurationMinutes", table.Keys);
        Assert.Contains("\"DurationMinutes\" >= 10", table["CK_MedicalServices_DurationMinutes"]);
        Assert.Contains("\"DurationMinutes\" <= 240", table["CK_MedicalServices_DurationMinutes"]);
        Assert.Contains("\"DurationMinutes\" % 5 = 0", table["CK_MedicalServices_DurationMinutes"]);
        Assert.Contains("CK_MedicalServices_Price", table.Keys);
        Assert.Contains("\"Price\" >= 0", table["CK_MedicalServices_Price"]);
    }
}
