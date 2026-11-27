using System.Net;
using System.Net.Http.Json;
using ClinicAppointments.Api.Features.Cabinets;
using Microsoft.AspNetCore.Mvc;

namespace ClinicAppointments.Api.Tests.Features.Cabinets;

public class CabinetsApiTests : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client;

    public CabinetsApiTests(ApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_ReturnsCabinets()
    {
        var createRequest = new CreateCabinetRequest
        {
            Number = "101",
            Floor = 1,
            Description = "Test cabinet"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/cabinets", createRequest);
        var createdCabinet = await createResponse.Content.ReadFromJsonAsync<CabinetResponse>();
        Assert.NotNull(createdCabinet);

        var response = await _client.GetAsync("/api/cabinets");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var cabinets = await response.Content.ReadFromJsonAsync<List<CabinetResponse>>();
        Assert.NotNull(cabinets);
        Assert.Contains(cabinets, c => c.Id == createdCabinet.Id);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenCabinetDoesNotExist()
    {
        var response = await _client.GetAsync("/api/cabinets/999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenIdIsNotInteger()
    {
        var response = await _client.GetAsync("/api/cabinets/abc");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_ReturnsCreatedWithLocationHeader()
    {
        var request = new CreateCabinetRequest
        {
            Number = "201",
            Floor = 2,
            Description = "Main office"
        };

        var response = await _client.PostAsJsonAsync("/api/cabinets", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);

        var cabinet = await response.Content.ReadFromJsonAsync<CabinetResponse>();
        Assert.NotNull(cabinet);
        Assert.Equal("201", cabinet.Number);
        Assert.Equal(2, cabinet.Floor);
        Assert.Equal("Main office", cabinet.Description);
    }

    [Fact]
    public async Task GetById_ReturnsCabinet_WhenCabinetExists()
    {
        var createRequest = new CreateCabinetRequest
        {
            Number = "301",
            Floor = 3,
            Description = "Consultation room"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/cabinets", createRequest);
        var createdCabinet = await createResponse.Content.ReadFromJsonAsync<CabinetResponse>();
        Assert.NotNull(createdCabinet);

        var response = await _client.GetAsync($"/api/cabinets/{createdCabinet.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var cabinet = await response.Content.ReadFromJsonAsync<CabinetResponse>();
        Assert.NotNull(cabinet);
        Assert.Equal(createdCabinet.Id, cabinet.Id);
        Assert.Equal("301", cabinet.Number);
        Assert.Equal(3, cabinet.Floor);
    }

    [Fact]
    public async Task Update_ReturnsNoContent_WhenCabinetExists()
    {
        var createRequest = new CreateCabinetRequest
        {
            Number = "401",
            Floor = 4,
            Description = "Old description"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/cabinets", createRequest);
        var createdCabinet = await createResponse.Content.ReadFromJsonAsync<CabinetResponse>();
        Assert.NotNull(createdCabinet);

        var updateRequest = new UpdateCabinetRequest
        {
            Number = "401",
            Floor = 4,
            Description = "New description"
        };

        var response = await _client.PutAsJsonAsync($"/api/cabinets/{createdCabinet.Id}", updateRequest);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var getResponse = await _client.GetAsync($"/api/cabinets/{createdCabinet.Id}");
        var updatedCabinet = await getResponse.Content.ReadFromJsonAsync<CabinetResponse>();
        Assert.NotNull(updatedCabinet);
        Assert.Equal("401", updatedCabinet.Number);
        Assert.Equal(4, updatedCabinet.Floor);
        Assert.Equal("New description", updatedCabinet.Description);
    }

    [Fact]
    public async Task Update_ReturnsNotFound_WhenCabinetDoesNotExist()
    {
        var updateRequest = new UpdateCabinetRequest
        {
            Number = "501",
            Floor = 5,
            Description = "Test"
        };

        var response = await _client.PutAsJsonAsync("/api/cabinets/999", updateRequest);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenCabinetExists()
    {
        var createRequest = new CreateCabinetRequest
        {
            Number = "601",
            Floor = 6,
            Description = "To be deleted"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/cabinets", createRequest);
        var createdCabinet = await createResponse.Content.ReadFromJsonAsync<CabinetResponse>();
        Assert.NotNull(createdCabinet);

        var response = await _client.DeleteAsync($"/api/cabinets/{createdCabinet.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var getResponse = await _client.GetAsync($"/api/cabinets/{createdCabinet.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenCabinetDoesNotExist()
    {
        var response = await _client.DeleteAsync("/api/cabinets/999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Update_ConcurrentUpdates_ReturnsNoContent()
    {
        var createRequest = new CreateCabinetRequest
        {
            Number = "701",
            Floor = 7,
            Description = "Initial description"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/cabinets", createRequest);
        var createdCabinet = await createResponse.Content.ReadFromJsonAsync<CabinetResponse>();
        Assert.NotNull(createdCabinet);

        var tasks = Enumerable.Range(0, 10).Select(async i =>
        {
            var updateRequest = new UpdateCabinetRequest
            {
                Number = "701",
                Floor = 7,
                Description = $"Updated Description {i}"
            };
            return await _client.PutAsJsonAsync($"/api/cabinets/{createdCabinet.Id}", updateRequest);
        }).ToArray();

        var responses = await Task.WhenAll(tasks);

        Assert.All(responses, response => Assert.Equal(HttpStatusCode.NoContent, response.StatusCode));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("12345678901")]
    public async Task Create_ReturnsBadRequest_WhenNumberIsInvalid(string? number)
    {
        var request = new CreateCabinetRequest
        {
            Number = number!,
            Floor = 1,
            Description = "Test cabinet"
        };

        var response = await _client.PostAsJsonAsync("/api/cabinets", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        Assert.NotNull(problem);
        Assert.Equal(400, problem.Status);
        Assert.True(problem.Errors.ContainsKey("Number"));
        Assert.NotEmpty(problem.Errors["Number"]);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("12345678901")]
    public async Task Update_ReturnsBadRequest_WhenNumberIsInvalid(string? number)
    {
        var createRequest = new CreateCabinetRequest
        {
            Number = "801",
            Floor = 8,
            Description = "Original cabinet"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/cabinets", createRequest);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var createdCabinet = await createResponse.Content.ReadFromJsonAsync<CabinetResponse>();

        Assert.NotNull(createdCabinet);

        var updateRequest = new UpdateCabinetRequest
        {
            Number = number!,
            Floor = 9,
            Description = "Updated cabinet"
        };

        var response = await _client.PutAsJsonAsync(
            $"/api/cabinets/{createdCabinet.Id}", updateRequest);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        Assert.NotNull(problem);
        Assert.Equal(400, problem.Status);
        Assert.True(problem.Errors.ContainsKey("Number"));
        Assert.NotEmpty(problem.Errors["Number"]);

        var getResponse = await _client.GetAsync($"/api/cabinets/{createdCabinet.Id}");

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var unchangedCabinet = await getResponse.Content.ReadFromJsonAsync<CabinetResponse>();

        Assert.NotNull(unchangedCabinet);
        Assert.Equal("801", unchangedCabinet.Number);
        Assert.Equal(8, unchangedCabinet.Floor);
        Assert.Equal("Original cabinet", unchangedCabinet.Description);
    }

    [Theory]
    [InlineData("1", "1234567890")]
    [InlineData("1234567890", "1")]
    public async Task CreateAndUpdate_AcceptValidBoundaryNumbers(
        string createNumber,
        string updateNumber)
    {
        var createRequest = new CreateCabinetRequest
        {
            Number = createNumber,
            Floor = 1,
            Description = "Boundary cabinet"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/cabinets", createRequest);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var createdCabinet = await createResponse.Content.ReadFromJsonAsync<CabinetResponse>();

        Assert.NotNull(createdCabinet);
        Assert.Equal(createNumber, createdCabinet.Number);

        var updateRequest = new UpdateCabinetRequest
        {
            Number = updateNumber,
            Floor = 1,
            Description = "Updated boundary cabinet"
        };

        var updateResponse = await _client.PutAsJsonAsync(
            $"/api/cabinets/{createdCabinet.Id}", updateRequest);

        Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);

        var getResponse = await _client.GetAsync($"/api/cabinets/{createdCabinet.Id}");

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var updatedCabinet = await getResponse.Content.ReadFromJsonAsync<CabinetResponse>();

        Assert.NotNull(updatedCabinet);
        Assert.Equal(updateNumber, updatedCabinet.Number);
    }
}
