

using CloudFit.Azure.HealthChecks.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CloudFit.Azure.HealthChecks.DI;

public static class HealthChecksPublisherAppInsights
{
    public static void AddAppInsightsPublisher(this IHealthChecksBuilder builder, HealthCheckSettings settings)
    {
        builder.AddApplicationInsightsPublisher(settings.AppInsightsInstrumentationKey, true, false);
    }
}