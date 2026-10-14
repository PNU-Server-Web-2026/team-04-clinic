
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace ClinicAppointments.Api.Tests.Features.Specializations;

public class SpecializationsApiTests : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client;

    public SpecializationsApiTests(ApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    public record SpecializationRequest(
        string Name,
        string? Description);

    public record SpecializationResponse(
        int Id,
        string Name,
        string? Description);

    [Fact]
    public async Task GetAll_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/specializations");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetById_WhenNotFound_ReturnsNotFound()
    {
        var response = await _client.GetAsync(
            "/api/specializations/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_ReturnsCreatedWithLocation()
    {
        var request = new SpecializationRequest(
            "Cardiology",
            "Heart and cardiovascular system");

        var response = await _client.PostAsJsonAsync(
            "/api/specializations",
            request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);

        var created = await response.Content
            .ReadFromJsonAsync<SpecializationResponse>();

        Assert.NotNull(created);
        Assert.Equal("Cardiology", created.Name);
        Assert.True(created.Id > 0);
    }

    [Fact]
    public async Task GetById_WhenExists_ReturnsOk()
    {
        var created = await CreateSpecializationAsync();

        var response = await _client.GetAsync(
            $"/api/specializations/{created.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Update_WhenExists_ReturnsNoContent()
    {
        var created = await CreateSpecializationAsync();

        var request = new SpecializationRequest(
            "Updated Cardiology",
            "Updated description");

        var response = await _client.PutAsJsonAsync(
            $"/api/specializations/{created.Id}",
            request);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Update_WhenNotFound_ReturnsNotFound()
    {
        var request = new SpecializationRequest(
            "Cardiology",
            "Description");

        var response = await _client.PutAsJsonAsync(
            "/api/specializations/999999",
            request);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_WhenExists_ReturnsNoContent()
    {
        var created = await CreateSpecializationAsync();

        var response = await _client.DeleteAsync(
            $"/api/specializations/{created.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Delete_WhenNotFound_ReturnsNotFound()
    {
        var response = await _client.DeleteAsync(
            "/api/specializations/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private async Task<SpecializationResponse> CreateSpecializationAsync()
    {
        var request = new SpecializationRequest(
            "Neurology",
            "Nervous system");

        var response = await _client.PostAsJsonAsync(
            "/api/specializations",
            request);

        response.EnsureSuccessStatusCode();

        var created = await response.Content
            .ReadFromJsonAsync<SpecializationResponse>();

        Assert.NotNull(created);

        return created;
    }
}
