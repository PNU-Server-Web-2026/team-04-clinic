using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace ClinicAppointments.Api.Tests.Features.Ping;

public class PingTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Get_ReturnsOkWithPong()
    {
        var response = await _client.GetAsync("/api/ping");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<PingResponse>();
        Assert.NotNull(body);
        Assert.Equal("pong", body.Message);
    }

    [Fact]
    public async Task Development_ProvidesScalarAndOpenApiMetadata()
    {
        await using var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Development");
        });

        using var client = factory.CreateClient();

        var scalarResponse = await client.GetAsync("/scalar");
        Assert.Equal(HttpStatusCode.OK, scalarResponse.StatusCode);
        var scalarHtml = await scalarResponse.Content.ReadAsStringAsync();
        Assert.Contains("Scalar", scalarHtml, StringComparison.OrdinalIgnoreCase);

        var openApiResponse = await client.GetAsync("/openapi/v1.json");
        Assert.Equal(HttpStatusCode.OK, openApiResponse.StatusCode);

        using var document = JsonDocument.Parse(await openApiResponse.Content.ReadAsStringAsync());
        var info = document.RootElement.GetProperty("info");

        Assert.Equal("Clinic Appointment API", info.GetProperty("title").GetString());
        Assert.Equal("v1", info.GetProperty("version").GetString());
        Assert.Contains("clinic appointments", info.GetProperty("description").GetString(), StringComparison.OrdinalIgnoreCase);
    }

    private sealed record PingResponse(string Message, DateTime Utc);
}
