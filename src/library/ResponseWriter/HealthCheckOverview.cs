
namespace CloudFit.Azure.HealthChecks.ResponseWriter;

public class HealthCheckOverview
{
    public string? Status { get; set; }
    public IEnumerable<HealthCheckReportResult> AllHealthChecks { get; set; } = new List<HealthCheckReportResult>();
    public TimeSpan TotalDuration { get; set; }
}