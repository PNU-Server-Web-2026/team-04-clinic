using System.Net;
using System.Net.Http.Json;
using ClinicAppointments.Api.Features.Patients;

namespace ClinicAppointments.Api.Tests.Features.Patients;

public class PatientsApiTests : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client;

    public PatientsApiTests(ApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_ReturnsPatients()
    {
        var createRequest = new CreatePatientRequest
        {
            FirstName = "Test",
            LastName = "Patient",
            BirthDate = new DateOnly(1990, 1, 1),
            Phone = "+380501234567",
            Email = "test@example.com"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/patients", createRequest);
        var createdPatient = await createResponse.Content.ReadFromJsonAsync<PatientResponse>();
        Assert.NotNull(createdPatient);

        var response = await _client.GetAsync("/api/patients");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var patients = await response.Content.ReadFromJsonAsync<List<PatientResponse>>();
        Assert.NotNull(patients);
        Assert.Contains(patients, p => p.Id == createdPatient.Id);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenPatientDoesNotExist()
    {
        var response = await _client.GetAsync("/api/patients/999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenIdIsNotInteger()
    {
        var response = await _client.GetAsync("/api/patients/abc");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_ReturnsCreatedWithLocationHeader()
    {
        var request = new CreatePatientRequest
        {
            FirstName = "John",
            LastName = "Doe",
            BirthDate = new DateOnly(1985, 5, 15),
            Phone = "+380509876543",
            Email = "john.doe@example.com"
        };

        var response = await _client.PostAsJsonAsync("/api/patients", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);

        var patient = await response.Content.ReadFromJsonAsync<PatientResponse>();
        Assert.NotNull(patient);
        Assert.Equal("John", patient.FirstName);
        Assert.Equal("Doe", patient.LastName);
        Assert.Equal(new DateOnly(1985, 5, 15), patient.BirthDate);
        Assert.Equal("+380509876543", patient.Phone);
        Assert.Equal("john.doe@example.com", patient.Email);
    }

    [Fact]
    public async Task GetById_ReturnsPatient_WhenPatientExists()
    {
        var createRequest = new CreatePatientRequest
        {
            FirstName = "Jane",
            LastName = "Smith",
            BirthDate = new DateOnly(1992, 8, 20),
            Phone = "+380501112223",
            Email = "jane.smith@example.com"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/patients", createRequest);
        var createdPatient = await createResponse.Content.ReadFromJsonAsync<PatientResponse>();
        Assert.NotNull(createdPatient);

        var response = await _client.GetAsync($"/api/patients/{createdPatient.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var patient = await response.Content.ReadFromJsonAsync<PatientResponse>();
        Assert.NotNull(patient);
        Assert.Equal(createdPatient.Id, patient.Id);
        Assert.Equal("Jane", patient.FirstName);
        Assert.Equal("Smith", patient.LastName);
    }

    [Fact]
    public async Task Update_ReturnsNoContent_WhenPatientExists()
    {
        var createRequest = new CreatePatientRequest
        {
            FirstName = "Bob",
            LastName = "Johnson",
            BirthDate = new DateOnly(1988, 3, 10),
            Phone = "+380503334445",
            Email = "bob@example.com"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/patients", createRequest);
        var createdPatient = await createResponse.Content.ReadFromJsonAsync<PatientResponse>();
        Assert.NotNull(createdPatient);

        var updateRequest = new UpdatePatientRequest
        {
            FirstName = "Robert",
            LastName = "Williams",
            BirthDate = new DateOnly(1988, 3, 10),
            Phone = "+380503334446",
            Email = "robert@example.com"
        };

        var response = await _client.PutAsJsonAsync($"/api/patients/{createdPatient.Id}", updateRequest);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var getResponse = await _client.GetAsync($"/api/patients/{createdPatient.Id}");
        var updatedPatient = await getResponse.Content.ReadFromJsonAsync<PatientResponse>();
        Assert.NotNull(updatedPatient);
        Assert.Equal("Robert", updatedPatient.FirstName);
        Assert.Equal("Williams", updatedPatient.LastName);
        Assert.Equal("+380503334446", updatedPatient.Phone);
        Assert.Equal("robert@example.com", updatedPatient.Email);
    }

    [Fact]
    public async Task Update_ReturnsNotFound_WhenPatientDoesNotExist()
    {
        var updateRequest = new UpdatePatientRequest
        {
            FirstName = "Robert",
            LastName = "Williams",
            BirthDate = new DateOnly(1988, 3, 10),
            Phone = "+380503334446",
            Email = "robert@example.com"
        };

        var response = await _client.PutAsJsonAsync("/api/patients/999", updateRequest);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenPatientExists()
    {
        var createRequest = new CreatePatientRequest
        {
            FirstName = "Alice",
            LastName = "Brown",
            BirthDate = new DateOnly(1995, 12, 5),
            Phone = "+380505556667",
            Email = "alice@example.com"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/patients", createRequest);
        var createdPatient = await createResponse.Content.ReadFromJsonAsync<PatientResponse>();
        Assert.NotNull(createdPatient);

        var response = await _client.DeleteAsync($"/api/patients/{createdPatient.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var getResponse = await _client.GetAsync($"/api/patients/{createdPatient.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenPatientDoesNotExist()
    {
        var response = await _client.DeleteAsync("/api/patients/999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
