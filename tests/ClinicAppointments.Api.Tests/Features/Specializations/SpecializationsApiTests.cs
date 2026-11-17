using System.Net;
using System.Net.Http.Json;
using ClinicAppointments.Api.Features.Specializations;

namespace ClinicAppointments.Api.Tests.Features.Specializations;

public class SpecializationsApiTests : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client;

    public SpecializationsApiTests(ApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_ReturnsSpecializations()
    {
        var createRequest = new CreateSpecializationRequest
        {
            Name = "Cardiology",
            Description = "Heart specialist"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/specializations", createRequest);
        var createdSpecialization = await createResponse.Content.ReadFromJsonAsync<SpecializationResponse>();
        Assert.NotNull(createdSpecialization);

        var response = await _client.GetAsync("/api/specializations");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var specializations = await response.Content.ReadFromJsonAsync<List<SpecializationResponse>>();
        Assert.NotNull(specializations);
        Assert.Contains(specializations, s => s.Id == createdSpecialization.Id);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenSpecializationDoesNotExist()
    {
        var response = await _client.GetAsync("/api/specializations/999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenIdIsNotInteger()
    {
        var response = await _client.GetAsync("/api/specializations/abc");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_ReturnsCreatedWithLocationHeader()
    {
        var request = new CreateSpecializationRequest
        {
            Name = "Neurology",
            Description = "Brain specialist"
        };

        var response = await _client.PostAsJsonAsync("/api/specializations", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);

        var specialization = await response.Content.ReadFromJsonAsync<SpecializationResponse>();
        Assert.NotNull(specialization);
        Assert.Equal("Neurology", specialization.Name);
        Assert.Equal("Brain specialist", specialization.Description);
    }

    [Fact]
    public async Task GetById_ReturnsSpecialization_WhenSpecializationExists()
    {
        var createRequest = new CreateSpecializationRequest
        {
            Name = "Orthopedics",
            Description = "Bone specialist"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/specializations", createRequest);
        var createdSpecialization = await createResponse.Content.ReadFromJsonAsync<SpecializationResponse>();
        Assert.NotNull(createdSpecialization);

        var response = await _client.GetAsync($"/api/specializations/{createdSpecialization.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var specialization = await response.Content.ReadFromJsonAsync<SpecializationResponse>();
        Assert.NotNull(specialization);
        Assert.Equal(createdSpecialization.Id, specialization.Id);
        Assert.Equal("Orthopedics", specialization.Name);
        Assert.Equal("Bone specialist", specialization.Description);
    }

    [Fact]
    public async Task Update_ReturnsNoContent_WhenSpecializationExists()
    {
        var createRequest = new CreateSpecializationRequest
        {
            Name = "Pediatrics",
            Description = "Children specialist"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/specializations", createRequest);
        var createdSpecialization = await createResponse.Content.ReadFromJsonAsync<SpecializationResponse>();
        Assert.NotNull(createdSpecialization);

        var updateRequest = new UpdateSpecializationRequest
        {
            Name = "Pediatrics & Neonatology",
            Description = "Children and newborn specialist"
        };

        var response = await _client.PutAsJsonAsync($"/api/specializations/{createdSpecialization.Id}", updateRequest);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var getResponse = await _client.GetAsync($"/api/specializations/{createdSpecialization.Id}");
        var updatedSpecialization = await getResponse.Content.ReadFromJsonAsync<SpecializationResponse>();
        Assert.NotNull(updatedSpecialization);
        Assert.Equal("Pediatrics & Neonatology", updatedSpecialization.Name);
        Assert.Equal("Children and newborn specialist", updatedSpecialization.Description);
    }

    [Fact]
    public async Task Update_ReturnsNotFound_WhenSpecializationDoesNotExist()
    {
        var updateRequest = new UpdateSpecializationRequest
        {
            Name = "Dermatology",
            Description = "Skin specialist"
        };

        var response = await _client.PutAsJsonAsync("/api/specializations/999", updateRequest);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenSpecializationExists()
    {
        var createRequest = new CreateSpecializationRequest
        {
            Name = "Psychiatry",
            Description = "Mental health specialist"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/specializations", createRequest);
        var createdSpecialization = await createResponse.Content.ReadFromJsonAsync<SpecializationResponse>();
        Assert.NotNull(createdSpecialization);

        var response = await _client.DeleteAsync($"/api/specializations/{createdSpecialization.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var getResponse = await _client.GetAsync($"/api/specializations/{createdSpecialization.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenSpecializationDoesNotExist()
    {
        var response = await _client.DeleteAsync("/api/specializations/999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
