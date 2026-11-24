using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace ClinicAppointments.Api.Tests.Features.OpenApi;

public class OpenApiTests
{
    [Fact]
    public async Task Development_ProvidesScalarAndOpenApiMetadata()
    {
        await using var factory = new DevelopmentApiFactory();
        using var client = factory.CreateClient();

        var scalarResponse = await client.GetAsync("/scalar");

        Assert.Equal(HttpStatusCode.OK, scalarResponse.StatusCode);

        var scalarHtml = await scalarResponse.Content.ReadAsStringAsync();

        Assert.Contains(
            "Scalar",
            scalarHtml,
            StringComparison.OrdinalIgnoreCase);

        var openApiResponse = await client.GetAsync("/openapi/v1.json");

        Assert.Equal(HttpStatusCode.OK, openApiResponse.StatusCode);

        using var document = JsonDocument.Parse(
            await openApiResponse.Content.ReadAsStringAsync());

        var info = document.RootElement.GetProperty("info");

        Assert.Equal(
            "Clinic Appointment API",
            info.GetProperty("title").GetString());

        Assert.Equal(
            "v1",
            info.GetProperty("version").GetString());

        Assert.Contains(
            "clinic appointments",
            info.GetProperty("description").GetString(),
            StringComparison.OrdinalIgnoreCase);

        var pingOperation = document.RootElement
            .GetProperty("paths")
            .GetProperty("/api/ping")
            .GetProperty("get");

        Assert.Equal(
            "GET /api/ping — повертає \"pong\" і поточний час UTC.",
            pingOperation.GetProperty("summary").GetString());
    }

    [Fact]
    public async Task Testing_DoesNotExposeScalarOrOpenApi()
    {
        await using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var scalarResponse = await client.GetAsync("/scalar");
        var openApiResponse = await client.GetAsync("/openapi/v1.json");

        Assert.Equal(
            HttpStatusCode.NotFound,
            scalarResponse.StatusCode);

        Assert.Equal(
            HttpStatusCode.NotFound,
            openApiResponse.StatusCode);
    }

    [Fact]
    public async Task Production_DoesNotExposeScalarOrOpenApi()
    {
        await using var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Production");
            });

        using var client = factory.CreateClient();

        var scalarResponse = await client.GetAsync("/scalar");
        var openApiResponse = await client.GetAsync("/openapi/v1.json");

        Assert.Equal(
            HttpStatusCode.NotFound,
            scalarResponse.StatusCode);

        Assert.Equal(
            HttpStatusCode.NotFound,
            openApiResponse.StatusCode);
    }
}