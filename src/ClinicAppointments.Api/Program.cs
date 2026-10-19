using ClinicAppointments.Api.Data;
using ClinicAppointments.Api.Features.Health;
using ClinicAppointments.Api.Features.Specializations;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddSpecializationsFeature();
builder.Services.AddOpenApi();
builder.Services.AddHealthFeature();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();
app.MapHealthFeature();

app.Run();

public partial class Program;
