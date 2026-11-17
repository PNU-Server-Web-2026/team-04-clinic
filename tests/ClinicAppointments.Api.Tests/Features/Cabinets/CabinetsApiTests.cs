using System.Net;
using System.Net.Http.Json;
using ClinicAppointments.Api.Features.Cabinets;

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
}
