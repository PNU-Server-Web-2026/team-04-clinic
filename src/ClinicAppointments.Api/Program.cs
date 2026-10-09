using ClinicAppointments.Api.Data;
using ClinicAppointments.Api.Features.Health;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;

const string FrontendCorsPolicy = "Frontend";

var builder = WebApplication.CreateBuilder(args);

// Сервіси. Реєстрацію сервісів конкретних фіч додавайте через extension-методи
// у папці фічі (наприклад, builder.Services.AddHotelsFeature();), див. CONTRIBUTING.md.
builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();
builder.Services.AddHealthFeature();

var origins = builder.Configuration.GetSection("Cors:Origins").Get<string[]>() ?? [];

builder.Services.AddCors(o => o.AddPolicy(FrontendCorsPolicy, policy =>
{
    policy.WithOrigins(origins)
        .WithMethods(HttpMethods.Get, HttpMethods.Post, HttpMethods.Put, HttpMethods.Patch, HttpMethods.Delete)
        .WithHeaders(HeaderNames.Accept, HeaderNames.Authorization, HeaderNames.ContentType);
}));

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

var app = builder.Build();

// HTTP-конвеєр. CORS — до авторизації та маршрутів контролерів,
// інакше браузерний preflight не отримає Access-Control-Allow-*.
app.UseCors(FrontendCorsPolicy);

if (app.Environment.IsDevelopment())
{
    // OpenAPI-документ: /openapi/v1.json
    app.MapOpenApi();
}

app.MapControllers();
app.MapHealthFeature();

app.Run();

// Потрібно для інтеграційних тестів (WebApplicationFactory<Program>).
public partial class Program;
