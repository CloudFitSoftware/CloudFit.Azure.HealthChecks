using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using CloudFit.Azure.HealthChecks.Configuration;
using CloudFit.Azure.HealthChecks.Attributes;

namespace CloudFit.Azure.HealthChecks;

[NugetPackageAttribute("AspNetCore.HealthChecks.Redis")]
public class RedisHealthCheck : IConfigureHealthCheck, IHealthCheck
{
    private readonly string connStrKey = "ConnectionString";

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        await Task.Delay(10);

        // Reporting missing, required configuration properties
        return HealthCheckResult.Degraded($"Configuration for {context.Registration.Name} requires a ({connStrKey}) in Props.");
    }

    public IHealthChecksBuilder AddHealthCheck(IHealthChecksBuilder builder, HealthCheckConfig config)
    {
        // use this class as the health check if missing required property    
        if (config.Props == null || !config.Props.ContainsKey(connStrKey) || string.IsNullOrEmpty((config.Props[connStrKey] as string)))
        {
            return builder.AddCheck((config.Name ?? string.Empty), this);
        }

        // use extension method for adding a Redis health check
        return builder.AddRedis(((config.Props[connStrKey] as string) ?? string.Empty), config.Name);
    }
}