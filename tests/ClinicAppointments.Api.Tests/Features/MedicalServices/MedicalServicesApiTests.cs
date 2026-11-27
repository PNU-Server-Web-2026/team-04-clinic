using System.Net;
using System.Net.Http.Json;
using ClinicAppointments.Api.Features.MedicalServices;
using Microsoft.AspNetCore.Mvc;

namespace ClinicAppointments.Api.Tests.Features.MedicalServices;

public class MedicalServicesApiTests : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client;

    public MedicalServicesApiTests(ApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_ReturnsServices()
    {
        var createRequest = new CreateMedicalServiceRequest
        {
            Name = "Test Service",
            DurationMinutes = 30,
            Price = 50.00m
        };

        var createResponse = await _client.PostAsJsonAsync("/api/medical-services", createRequest);
        var createdService = await createResponse.Content.ReadFromJsonAsync<MedicalServiceResponse>();
        Assert.NotNull(createdService);

        var response = await _client.GetAsync("/api/medical-services");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var services = await response.Content.ReadFromJsonAsync<List<MedicalServiceResponse>>();
        Assert.NotNull(services);
        Assert.Contains(services, s => s.Id == createdService.Id);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenServiceDoesNotExist()
    {
        var response = await _client.GetAsync("/api/medical-services/999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenIdIsNotInteger()
    {
        var response = await _client.GetAsync("/api/medical-services/abc");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_ReturnsCreatedWithLocationHeader()
    {
        var request = new CreateMedicalServiceRequest
        {
            Name = "Consultation",
            DurationMinutes = 30,
            Price = 50.00m
        };

        var response = await _client.PostAsJsonAsync("/api/medical-services", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);

        var service = await response.Content.ReadFromJsonAsync<MedicalServiceResponse>();
        Assert.NotNull(service);
        Assert.Equal($"/api/medical-services/{service.Id}", response.Headers.Location!.AbsolutePath);
        Assert.Equal("Consultation", service.Name);
        Assert.Equal(30, service.DurationMinutes);
        Assert.Equal(50.00m, service.Price);
    }

    [Fact]
    public async Task GetById_ReturnsService_WhenServiceExists()
    {
        var createRequest = new CreateMedicalServiceRequest
        {
            Name = "Blood Test",
            DurationMinutes = 15,
            Price = 25.00m
        };

        var createResponse = await _client.PostAsJsonAsync("/api/medical-services", createRequest);
        var createdService = await createResponse.Content.ReadFromJsonAsync<MedicalServiceResponse>();
        Assert.NotNull(createdService);

        var response = await _client.GetAsync($"/api/medical-services/{createdService.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var service = await response.Content.ReadFromJsonAsync<MedicalServiceResponse>();
        Assert.NotNull(service);
        Assert.Equal(createdService.Id, service.Id);
        Assert.Equal("Blood Test", service.Name);
    }

    [Fact]
    public async Task Update_ReturnsNoContent_WhenServiceExists()
    {
        var createRequest = new CreateMedicalServiceRequest
        {
            Name = "X-Ray",
            DurationMinutes = 20,
            Price = 100.00m
        };

        var createResponse = await _client.PostAsJsonAsync("/api/medical-services", createRequest);
        var createdService = await createResponse.Content.ReadFromJsonAsync<MedicalServiceResponse>();
        Assert.NotNull(createdService);

        var updateRequest = new UpdateMedicalServiceRequest
        {
            Name = "MRI",
            DurationMinutes = 30,
            Price = 200.00m
        };

        var response = await _client.PutAsJsonAsync($"/api/medical-services/{createdService.Id}", updateRequest);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var getResponse = await _client.GetAsync($"/api/medical-services/{createdService.Id}");
        var updatedService = await getResponse.Content.ReadFromJsonAsync<MedicalServiceResponse>();
        Assert.NotNull(updatedService);
        Assert.Equal("MRI", updatedService.Name);
        Assert.Equal(30, updatedService.DurationMinutes);
        Assert.Equal(200.00m, updatedService.Price);
    }

    [Fact]
    public async Task Update_ReturnsNotFound_WhenServiceDoesNotExist()
    {
        var updateRequest = new UpdateMedicalServiceRequest
        {
            Name = "MRI",
            DurationMinutes = 30,
            Price = 200.00m
        };

        var response = await _client.PutAsJsonAsync("/api/medical-services/999", updateRequest);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenServiceExists()
    {
        var createRequest = new CreateMedicalServiceRequest
        {
            Name = "Vaccination",
            DurationMinutes = 10,
            Price = 15.00m
        };

        var createResponse = await _client.PostAsJsonAsync("/api/medical-services", createRequest);
        var createdService = await createResponse.Content.ReadFromJsonAsync<MedicalServiceResponse>();
        Assert.NotNull(createdService);

        var response = await _client.DeleteAsync($"/api/medical-services/{createdService.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var getResponse = await _client.GetAsync($"/api/medical-services/{createdService.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenServiceDoesNotExist()
    {
        var response = await _client.DeleteAsync("/api/medical-services/999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Update_ConcurrentUpdates_ReturnsNoContent()
    {
        var createRequest = new CreateMedicalServiceRequest
        {
            Name = "Concurrent Test",
            DurationMinutes = 30,
            Price = 50.00m
        };

        var createResponse = await _client.PostAsJsonAsync("/api/medical-services", createRequest);
        var createdService = await createResponse.Content.ReadFromJsonAsync<MedicalServiceResponse>();
        Assert.NotNull(createdService);

        var tasks = Enumerable.Range(0, 10).Select(async i =>
        {
            var updateRequest = new UpdateMedicalServiceRequest
            {
                Name = $"Updated Service {i}",
                DurationMinutes = 30 + i * 5,
                Price = 50.00m + i
            };

            return await _client.PutAsJsonAsync($"/api/medical-services/{createdService.Id}", updateRequest);
        }).ToArray();

        var responses = await Task.WhenAll(tasks);

        Assert.All(responses, response => Assert.Equal(HttpStatusCode.NoContent, response.StatusCode));
    }

    [Theory]
    [InlineData(null, 30, 100, "Name")]
    [InlineData("", 30, 100, "Name")]
    [InlineData("   ", 30, 100, "Name")]
    [InlineData("Consultation", 5, 100, "DurationMinutes")]
    [InlineData("Consultation", 245, 100, "DurationMinutes")]
    [InlineData("Consultation", 33, 100, "DurationMinutes")]
    [InlineData("Consultation", 30, -1, "Price")]
    public async Task Create_ReturnsBadRequest_WhenRequestIsInvalid(
        string? name,
        int durationMinutes,
        int price,
        string expectedField)
    {
        var request = new CreateMedicalServiceRequest
        {
            Name = name!,
            DurationMinutes = durationMinutes,
            Price = price
        };

        var response = await _client.PostAsJsonAsync("/api/medical-services", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        Assert.NotNull(problem);
        Assert.Equal(400, problem.Status);
        Assert.True(problem.Errors.ContainsKey(expectedField));
        Assert.NotEmpty(problem.Errors[expectedField]);
    }

    [Theory]
    [InlineData(null, 30, 100, "Name")]
    [InlineData("", 30, 100, "Name")]
    [InlineData("   ", 30, 100, "Name")]
    [InlineData("Consultation", 5, 100, "DurationMinutes")]
    [InlineData("Consultation", 245, 100, "DurationMinutes")]
    [InlineData("Consultation", 33, 100, "DurationMinutes")]
    [InlineData("Consultation", 30, -1, "Price")]
    public async Task Update_ReturnsBadRequest_WhenRequestIsInvalid(
        string? name,
        int durationMinutes,
        int price,
        string expectedField)
    {
        var createRequest = new CreateMedicalServiceRequest
        {
            Name = "Original Service",
            DurationMinutes = 30,
            Price = 100m
        };

        var createResponse = await _client.PostAsJsonAsync("/api/medical-services", createRequest);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var createdService = await createResponse.Content.ReadFromJsonAsync<MedicalServiceResponse>();

        Assert.NotNull(createdService);

        var updateRequest = new UpdateMedicalServiceRequest
        {
            Name = name!,
            DurationMinutes = durationMinutes,
            Price = price
        };

        var response = await _client.PutAsJsonAsync(
            $"/api/medical-services/{createdService.Id}", updateRequest);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        Assert.NotNull(problem);
        Assert.Equal(400, problem.Status);
        Assert.True(problem.Errors.ContainsKey(expectedField));
        Assert.NotEmpty(problem.Errors[expectedField]);

        var getResponse = await _client.GetAsync($"/api/medical-services/{createdService.Id}");

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var unchangedService = await getResponse.Content.ReadFromJsonAsync<MedicalServiceResponse>();

        Assert.NotNull(unchangedService);
        Assert.Equal("Original Service", unchangedService.Name);
        Assert.Equal(30, unchangedService.DurationMinutes);
        Assert.Equal(100m, unchangedService.Price);
    }

    [Theory]
    [InlineData(10)]
    [InlineData(240)]
    public async Task CreateAndUpdate_AcceptValidBoundaryValues(int durationMinutes)
    {
        var createRequest = new CreateMedicalServiceRequest
        {
            Name = "Boundary Service",
            DurationMinutes = durationMinutes,
            Price = 0m
        };

        var createResponse = await _client.PostAsJsonAsync("/api/medical-services", createRequest);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var createdService = await createResponse.Content.ReadFromJsonAsync<MedicalServiceResponse>();

        Assert.NotNull(createdService);
        Assert.Equal(durationMinutes, createdService.DurationMinutes);
        Assert.Equal(0m, createdService.Price);

        var updateRequest = new UpdateMedicalServiceRequest
        {
            Name = "Updated Boundary Service",
            DurationMinutes = durationMinutes,
            Price = 0m
        };

        var updateResponse = await _client.PutAsJsonAsync(
            $"/api/medical-services/{createdService.Id}", updateRequest);

        Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);
    }
}
