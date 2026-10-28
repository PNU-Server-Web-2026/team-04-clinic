using System.Text.Json;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ClinicAppointments.Api.Features.Health;

internal static class HealthCheckJsonResponseWriter
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    public static Task WriteResponse(HttpContext context, HealthReport report)
    {
        context.Response.ContentType = "application/json; charset=utf-8";

        var payload = new HealthReportResponse(
            report.Status.ToString(),
            report.Entries.ToDictionary(
                entry => entry.Key,
                entry => new HealthCheckEntryResponse(
                    entry.Value.Status.ToString(),
                    entry.Value.Description,
                    entry.Value.Duration.TotalMilliseconds)));

        return context.Response.WriteAsJsonAsync(payload, SerializerOptions);
    }

    private sealed record HealthReportResponse(
        string Status,
        IReadOnlyDictionary<string, HealthCheckEntryResponse> Checks);

    private sealed record HealthCheckEntryResponse(
        string Status,
        string? Description,
        double DurationMs);
}
