using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace CloudFit.Azure.HealthChecks;

public class KeyVaultHealthCheck : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var result = "";
        HealthCheckResult hcResult = HealthCheckResult.Healthy();

        if (result == "degraded")
        {
            return HealthCheckResult.Degraded();
        }

        if (result == "unhealth")
        {
            return HealthCheckResult.Unhealthy();

        }

        return HealthCheckResult.Healthy();
    }
}