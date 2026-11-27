using System.Net;
using System.Net.Http.Json;
using ClinicAppointments.Api.Features.Specializations;
using Microsoft.AspNetCore.Mvc;

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

    [Fact]
    public async Task Update_ConcurrentUpdates_ReturnsNoContent()
    {
        var createRequest = new CreateSpecializationRequest
        {
            Name = "Concurrent Test",
            Description = "Initial description"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/specializations", createRequest);
        var createdSpecialization = await createResponse.Content.ReadFromJsonAsync<SpecializationResponse>();
        Assert.NotNull(createdSpecialization);

        var tasks = Enumerable.Range(0, 10).Select(async i =>
        {
            var updateRequest = new UpdateSpecializationRequest
            {
                Name = $"Updated Name {i}",
                Description = $"Updated Description {i}"
            };

            return await _client.PutAsJsonAsync($"/api/specializations/{createdSpecialization.Id}", updateRequest);
        }).ToArray();

        var responses = await Task.WhenAll(tasks);

        Assert.All(responses, response => Assert.Equal(HttpStatusCode.NoContent, response.StatusCode));
    }

    public static TheoryData<string?> InvalidNames => new()
    {
        null,
        "",
        "   ",
        new string('A', 101)
    };

    [Theory]
    [MemberData(nameof(InvalidNames))]
    public async Task Create_ReturnsBadRequest_WhenNameIsInvalid(string? name)
    {
        var request = new CreateSpecializationRequest
        {
            Name = name!,
            Description = "Test specialization"
        };

        var response = await _client.PostAsJsonAsync("/api/specializations", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        Assert.NotNull(problem);
        Assert.Equal(400, problem.Status);
        Assert.True(problem.Errors.ContainsKey("Name"));
        Assert.NotEmpty(problem.Errors["Name"]);
    }

    [Theory]
    [MemberData(nameof(InvalidNames))]
    public async Task Update_ReturnsBadRequest_WhenNameIsInvalid(string? name)
    {
        var createRequest = new CreateSpecializationRequest
        {
            Name = "Cardiology",
            Description = "Original specialization"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/specializations", createRequest);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var createdSpecialization = await createResponse.Content.ReadFromJsonAsync<SpecializationResponse>();

        Assert.NotNull(createdSpecialization);

        var updateRequest = new UpdateSpecializationRequest
        {
            Name = name!,
            Description = "Updated specialization"
        };

        var response = await _client.PutAsJsonAsync(
            $"/api/specializations/{createdSpecialization.Id}", updateRequest);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        Assert.NotNull(problem);
        Assert.Equal(400, problem.Status);
        Assert.True(problem.Errors.ContainsKey("Name"));
        Assert.NotEmpty(problem.Errors["Name"]);

        var getResponse = await _client.GetAsync(
            $"/api/specializations/{createdSpecialization.Id}");

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var unchangedSpecialization = await getResponse.Content.ReadFromJsonAsync<SpecializationResponse>();

        Assert.NotNull(unchangedSpecialization);
        Assert.Equal("Cardiology", unchangedSpecialization.Name);
        Assert.Equal("Original specialization", unchangedSpecialization.Description);
    }

    [Theory]
    [InlineData(1, 100)]
    [InlineData(100, 1)]
    public async Task CreateAndUpdate_AcceptValidBoundaryNames(
        int createNameLength,
        int updateNameLength)
    {
        var createName = new string('A', createNameLength);
        var updateName = new string('B', updateNameLength);

        var createRequest = new CreateSpecializationRequest
        {
            Name = createName,
            Description = "Boundary specialization"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/specializations", createRequest);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var createdSpecialization = await createResponse.Content.ReadFromJsonAsync<SpecializationResponse>();

        Assert.NotNull(createdSpecialization);
        Assert.Equal(createName, createdSpecialization.Name);

        var updateRequest = new UpdateSpecializationRequest
        {
            Name = updateName,
            Description = "Updated boundary specialization"
        };

        var updateResponse = await _client.PutAsJsonAsync(
            $"/api/specializations/{createdSpecialization.Id}", updateRequest);

        Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);

        var getResponse = await _client.GetAsync(
            $"/api/specializations/{createdSpecialization.Id}");

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var updatedSpecialization = await getResponse.Content.ReadFromJsonAsync<SpecializationResponse>();

        Assert.NotNull(updatedSpecialization);
        Assert.Equal(updateName, updatedSpecialization.Name);
    }
}
