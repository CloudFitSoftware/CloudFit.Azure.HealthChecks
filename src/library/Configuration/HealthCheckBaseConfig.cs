

namespace CloudFit.Azure.HealthChecks.Configuration;

public class HealthCheckConfig
{
    public string? Name { get; set; }

    public string? Type { get; set; }

    public Dictionary<string, string>? Props { get; set; }
}