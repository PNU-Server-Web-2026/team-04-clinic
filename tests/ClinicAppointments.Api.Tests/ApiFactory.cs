using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace ClinicAppointments.Api.Tests;

/// <summary>
/// Фабрика тестового сервера для інтеграційних тестів.
/// Запускає API в середовищі "Testing".
/// </summary>
public class ApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
    }
}

/// <summary>
/// Фабрика тестового сервера для перевірки Development-конфігурації API.
/// </summary>
public class DevelopmentApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
    }
}