using System.Net;
using Microsoft.AspNetCore.Hosting;

namespace ClinicAppointments.Api.Tests.Cors;

public class CorsTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    [Theory]
    [InlineData("http://localhost:5173")]
    [InlineData("http://localhost:3000")]
    public async Task Preflight_DevelopmentOrigin_ReturnsAllowHeaders(string origin)
    {
        using var client = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Development");
        }).CreateClient();

        var response = await SendPreflightAsync(client, origin);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Equal(origin, Header(response, "Access-Control-Allow-Origin"));
        Assert.Contains("POST", Header(response, "Access-Control-Allow-Methods"), StringComparison.Ordinal);
    }

    [Fact]
    public async Task Preflight_DisallowedOrigin_HasNoAllowHeaders()
    {
        using var client = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Development");
        }).CreateClient();

        var response = await SendPreflightAsync(client, "http://evil.example");

        Assert.DoesNotContain(
            response.Headers,
            header => header.Key.StartsWith("Access-Control-Allow-", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task Preflight_UsesOriginsFromConfiguration()
    {
        const string configuredOrigin = "http://localhost:4173";

        using var client = factory.WithWebHostBuilder(builder =>
        {
            builder.UseSetting("Cors:Origins:0", configuredOrigin);
        }).CreateClient();

        var allowed = await SendPreflightAsync(client, configuredOrigin);
        var denied = await SendPreflightAsync(client, "http://localhost:5173");

        Assert.Equal(HttpStatusCode.NoContent, allowed.StatusCode);
        Assert.Equal(configuredOrigin, Header(allowed, "Access-Control-Allow-Origin"));
        Assert.DoesNotContain(
            denied.Headers,
            header => header.Key.StartsWith("Access-Control-Allow-", StringComparison.OrdinalIgnoreCase));
    }

    private static async Task<HttpResponseMessage> SendPreflightAsync(HttpClient client, string origin)
    {
        var request = new HttpRequestMessage(HttpMethod.Options, "/api/ping");
        request.Headers.TryAddWithoutValidation("Origin", origin);
        request.Headers.TryAddWithoutValidation("Access-Control-Request-Method", "POST");
        return await client.SendAsync(request);
    }

    private static string Header(HttpResponseMessage response, string name) =>
        response.Headers.TryGetValues(name, out var values)
            ? string.Join(", ", values)
            : "";
}
