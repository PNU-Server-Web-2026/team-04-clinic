using System.Net;
using System.Net.Http.Json;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace ClinicAppointments.Api.Tests.Features.Info;

public class InfoTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Get_ReturnsApiVersionEnvironmentAndUtcTime()
    {
        var response = await _client.GetAsync("/api/info");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<InfoResponse>();
        Assert.NotNull(body);
        Assert.Equal("Clinic Appointment API", body.Name);
        Assert.Equal(Assembly.GetEntryAssembly()?.GetName().Version?.ToString(), body.Version);
        Assert.Equal("Testing", body.Environment);
        Assert.Equal(TimeSpan.Zero, body.Utc.Offset);
    }

    [Fact]
    public void InvalidOptions_PreventApplicationStartupWithValidationMessage()
    {
        using var factory = new InvalidOptionsApiFactory();

        var exception = Assert.ThrowsAny<Exception>(() => factory.CreateClient());

        Assert.Contains("MinCancelHours", exception.ToString());
    }

    private sealed record InfoResponse(
        string Name,
        string Version,
        string Environment,
        DateTimeOffset Utc);

    private sealed class InvalidOptionsApiFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");
            builder.ConfigureAppConfiguration((_, configuration) =>
                configuration.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Appointment:MinCancelHours"] = "0"
                }));
        }
    }
}
