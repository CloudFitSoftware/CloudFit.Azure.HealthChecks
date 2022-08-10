using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace CloudFit.Azure.HealthChecks;

public static class DI
{
    // Static function for injecting health checks into the application service collection
    public static void InjectHealthChecks(this IServiceCollection services)
    {
        //TODO:  Determine configuration introduction

        // TODO:  Add checks according to configuration
        services.AddHealthChecks()
            .AddCheck<KeyVaultHealthCheck>("KeyValut Health Check");
        
    }
}
