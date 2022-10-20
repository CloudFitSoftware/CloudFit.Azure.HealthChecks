using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace CloudFit.Azure.HealthChecks.ResponseWriter;

public static class HealthCheckResponseWriter
{
    public static Func<HttpContext, HealthReport, Task> ResponseWriter { get; } = WriteResponse;

    private async static Task WriteResponse(HttpContext httpContext, HealthReport healthReport)
    {
        LogStates(httpContext, healthReport);
        httpContext.Response.ContentType = "application/json";
        var response = new HealthCheckOverview
        {
            Status = healthReport.Status.ToString(),
            AllHealthChecks = healthReport.Entries.Select(x => new HealthCheckReportResult
            {
                Component = x.Key,
                Status = x.Value.Status.ToString(),
                Description = x.Value.Description + "\n\t  " + (x.Value.Exception?.Message ?? string.Empty),
                Duration = x.Value.Duration
            }),
            TotalDuration = healthReport.TotalDuration
        };
        await httpContext.Response.WriteAsync(JsonSerializer.Serialize(response));
    }

    private static void LogStates(HttpContext httpContext, HealthReport healthReport)
    {
        var logger = httpContext.RequestServices.GetRequiredService<ILogger<HealthCheckOverview>>();
        var reports = healthReport.Entries.Where(e => e.Value.Status == HealthStatus.Unhealthy
            || e.Value.Status == HealthStatus.Degraded).ToList();
        if (reports != null && reports.Count > 0)
        {

            foreach (var report in reports)
            {
                var messageBuilder = new StringBuilder();
                var parameters = new List<String>();

                messageBuilder.Append("{LogType}: ");
                parameters.Add("HealthReportLog");

                messageBuilder.Append("Component->{Component}");
                parameters.Add(report.Key);

                messageBuilder.Append("Status->{Status};");
                parameters.Add(report.Value.Status.ToString());

                if (!string.IsNullOrEmpty(report.Value.Description))
                {
                    messageBuilder.Append("Description->{Description}");
                    parameters.Add(report.Value.Description);
                }

                if (report.Value.Status == HealthStatus.Unhealthy)
                {
                    if (report.Value.Exception != null)
                    {
                        logger.LogCritical(report.Value.Exception, messageBuilder.ToString(), parameters.ToArray());
                    }
                    else
                    {
                        logger.LogCritical(messageBuilder.ToString(), parameters.ToArray());
                    }
                }
                else
                {
                    if (report.Value.Exception != null)
                    {
                        logger.LogWarning(report.Value.Exception, messageBuilder.ToString(), parameters.ToArray());
                    }
                    else
                    {
                        logger.LogWarning(messageBuilder.ToString(), parameters.ToArray());
                    }
                }
            }
        }
        else
        {
            logger.LogInformation("{LogType}: {Status} - {Duration}",
                "HealthReportLog", "Normal Operation", healthReport.TotalDuration.ToString());
        }
    }
}