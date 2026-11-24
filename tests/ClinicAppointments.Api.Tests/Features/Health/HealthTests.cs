using System.Net;
using System.Net.Http.Json;

namespace ClinicAppointments.Api.Tests.Features.Health;

public class HealthTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task GetHealth_ReturnsOkWithHealthyStatusAndNoChecks()
    {
        var response = await _client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<HealthReportResponse>();
        Assert.NotNull(body);
        Assert.Equal("Healthy", body.Status);
        Assert.NotNull(body.Checks);
        Assert.Empty(body.Checks);
    }

    private sealed record HealthReportResponse(
        string Status,
        IReadOnlyDictionary<string, HealthCheckEntryResponse> Checks);

    private sealed record HealthCheckEntryResponse(
        string Status,
        string? Description,
        double DurationMs);
}
