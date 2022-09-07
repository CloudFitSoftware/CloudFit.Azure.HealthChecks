using CloudFit.Azure.HealthChecks.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace CloudFit.Azure.HealthChecks;

public static class HealthChecksFactory
{
    public static IHealthChecksBuilder AddHealthCheck(this IHealthChecksBuilder builder, HealthCheckConfig config)
    {
        if (config != null)
        {
            var healthCheck = (Activator.CreateInstance(null, $"CloudFit.Azure.HealthChecks.{config.Type}").Unwrap());

            if (((healthCheck as IConfigureHealthCheck)) != null)
            {
                (healthCheck as IConfigureHealthCheck).SetHealthCheckProperties(config.Props);
            }

            return builder.AddCheck(config.Name, (healthCheck as IHealthCheck));
        }

        return builder;
    }
}