using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text;
using System.Text.Json;

namespace Core.Monitoring.HealthChecks;

internal static class HealthCheckHelper
{
    public static Task WriteResponseAsync(HttpContext context, HealthReport result)
    {
        context.Response.ContentType = "application/json; charset=utf-8";

        var options = new JsonWriterOptions { Indented = true };

        using var stream = new MemoryStream();

        using (var writer = new Utf8JsonWriter(stream, options))
        {
            writer.WriteStartObject();

            writer.WriteString("status", result.Status.ToString());

            writer.WriteStartObject("results");

            foreach (var entry in result.Entries)
            {
                writer.WriteStartObject(entry.Key);

                writer.WriteString("status", entry.Value.Status.ToString());

                writer.WriteEndObject();
            }

            writer.WriteEndObject();

            writer.WriteEndObject();
        }

        var json = Encoding.UTF8.GetString(stream.ToArray());

        return context.Response.WriteAsync(json, default);
    }
}