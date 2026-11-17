using ClinicAppointments.Api.Common;
using ClinicAppointments.Api.Data;
using ClinicAppointments.Api.Features.Cors;
using ClinicAppointments.Api.Features.Health;
using ClinicAppointments.Api.Features.MedicalServices;
using ClinicAppointments.Api.Features.Patients;
using ClinicAppointments.Api.Features.Cabinets;
using ClinicAppointments.Api.Features.Specializations;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Сервіси. Реєстрацію сервісів конкретних фіч додавайте через extension-методи
// у папці фічі (наприклад, builder.Services.AddHotelsFeature();), див. CONTRIBUTING.md.
builder.Services.AddControllers();
builder.Services.AddProblemDetails();

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, _, _) =>
    {
        document.Info = new()
        {
            Title = "Clinic Appointment API",
            Version = "v1",
            Description = "REST API for managing clinic appointments and scheduling."
        };

        return Task.CompletedTask;
    });
});

builder.Services.AddHealthFeature();
builder.Services.AddAppointmentOptions();
builder.Services.AddCorsFeature(builder.Configuration);
builder.Services.AddMedicalServicesFeature();
builder.Services.AddPatientsFeature();
builder.Services.AddCabinetsFeature();
builder.Services.AddSpecializationsFeature();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("Default")));

var app = builder.Build();

// HTTP-конвеєр. CORS — до авторизації та маршрутів контролерів,
// інакше браузерний preflight не отримає Access-Control-Allow-*.
app.UseCorsFeature();

if (app.Environment.IsDevelopment())
{
    // OpenAPI-документ: /openapi/v1.json
    app.MapOpenApi();

    app.MapScalarApiReference(options =>
    {
        options
            .WithTitle("Clinic Appointment API")
            .WithDefaultHttpClient(
                ScalarTarget.CSharp,
                ScalarClient.HttpClient);
    });
}

app.MapControllers();
app.MapHealthFeature();

app.Run();

// Потрібно для інтеграційних тестів (WebApplicationFactory<Program>).
public partial class Program;
