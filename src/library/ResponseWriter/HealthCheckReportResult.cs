namespace CloudFit.Azure.HealthChecks.ResponseWriter;

public class HealthCheckReportResult
{
    public string? Status { get; set; }
    public string? Component { get; set; }
    public string? Description { get; set; }
    public TimeSpan Duration { get; set; }
}