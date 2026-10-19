using System.Net;
using System.Net.Http.Json;
using ClinicAppointments.Api.Features.Doctors;
using Microsoft.Extensions.DependencyInjection;

namespace ClinicAppointments.Api.Tests.Features.Doctors;

public class DoctorsApiTests : IClassFixture<ApiFactory>, IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly IDoctorService _doctorService;

    public DoctorsApiTests(ApiFactory factory)
    {
        _client = factory.CreateClient();
        var scope = factory.Services.CreateScope();
        _doctorService = scope.ServiceProvider.GetRequiredService<IDoctorService>();
    }

    public Task InitializeAsync()
    {
        _doctorService.Clear();
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    [Fact]
    public async Task GetAll_ReturnsEmptyArray_WhenNoDoctors()
    {
        var response = await _client.GetAsync("/api/doctors");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var doctors = await response.Content.ReadFromJsonAsync<List<DoctorResponse>>();
        Assert.NotNull(doctors);
        Assert.Empty(doctors);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenDoctorDoesNotExist()
    {
        var response = await _client.GetAsync("/api/doctors/999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_ReturnsCreatedWithLocationHeader()
    {
        var request = new CreateDoctorRequest
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            Phone = "+380501234567",
            LicenseNumber = "LIC12345",
            ExperienceYears = 5
        };

        var response = await _client.PostAsJsonAsync("/api/doctors", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);

        var doctor = await response.Content.ReadFromJsonAsync<DoctorResponse>();
        Assert.NotNull(doctor);
        Assert.Equal("John", doctor.FirstName);
        Assert.Equal("Doe", doctor.LastName);
        Assert.Equal("john.doe@example.com", doctor.Email);
        Assert.Equal("+380501234567", doctor.Phone);
        Assert.Equal("LIC12345", doctor.LicenseNumber);
        Assert.Equal(5, doctor.ExperienceYears);
    }

    [Fact]
    public async Task GetById_ReturnsDoctor_WhenDoctorExists()
    {
        // Create a doctor first
        var createRequest = new CreateDoctorRequest
        {
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane.smith@example.com",
            Phone = "+380507654321",
            LicenseNumber = "LIC67890",
            ExperienceYears = 10
        };

        var createResponse = await _client.PostAsJsonAsync("/api/doctors", createRequest);
        var createdDoctor = await createResponse.Content.ReadFromJsonAsync<DoctorResponse>();
        Assert.NotNull(createdDoctor);

        // Get the doctor by ID
        var response = await _client.GetAsync($"/api/doctors/{createdDoctor.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var doctor = await response.Content.ReadFromJsonAsync<DoctorResponse>();
        Assert.NotNull(doctor);
        Assert.Equal(createdDoctor.Id, doctor.Id);
        Assert.Equal("Jane", doctor.FirstName);
        Assert.Equal("Smith", doctor.LastName);
    }

    [Fact]
    public async Task Update_ReturnsNoContent_WhenDoctorExists()
    {
        // Create a doctor first
        var createRequest = new CreateDoctorRequest
        {
            FirstName = "Bob",
            LastName = "Johnson",
            Email = "bob@example.com",
            Phone = "+380501112223",
            LicenseNumber = "LIC11111",
            ExperienceYears = 3
        };

        var createResponse = await _client.PostAsJsonAsync("/api/doctors", createRequest);
        var createdDoctor = await createResponse.Content.ReadFromJsonAsync<DoctorResponse>();
        Assert.NotNull(createdDoctor);

        // Update the doctor
        var updateRequest = new UpdateDoctorRequest
        {
            FirstName = "Robert",
            ExperienceYears = 8
        };

        var response = await _client.PutAsJsonAsync($"/api/doctors/{createdDoctor.Id}", updateRequest);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Verify the update
        var getResponse = await _client.GetAsync($"/api/doctors/{createdDoctor.Id}");
        var updatedDoctor = await getResponse.Content.ReadFromJsonAsync<DoctorResponse>();
        Assert.NotNull(updatedDoctor);
        Assert.Equal("Robert", updatedDoctor.FirstName);
        Assert.Equal(8, updatedDoctor.ExperienceYears);
    }

    [Fact]
    public async Task Update_ReturnsNotFound_WhenDoctorDoesNotExist()
    {
        var updateRequest = new UpdateDoctorRequest
        {
            FirstName = "Robert"
        };

        var response = await _client.PutAsJsonAsync("/api/doctors/999", updateRequest);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenDoctorExists()
    {
        // Create a doctor first
        var createRequest = new CreateDoctorRequest
        {
            FirstName = "Alice",
            LastName = "Williams",
            Email = "alice@example.com",
            Phone = "+380503334445",
            LicenseNumber = "LIC22222",
            ExperienceYears = 7
        };

        var createResponse = await _client.PostAsJsonAsync("/api/doctors", createRequest);
        var createdDoctor = await createResponse.Content.ReadFromJsonAsync<DoctorResponse>();
        Assert.NotNull(createdDoctor);

        // Delete the doctor
        var response = await _client.DeleteAsync($"/api/doctors/{createdDoctor.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Verify deletion
        var getResponse = await _client.GetAsync($"/api/doctors/{createdDoctor.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenDoctorDoesNotExist()
    {
        var response = await _client.DeleteAsync("/api/doctors/999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
