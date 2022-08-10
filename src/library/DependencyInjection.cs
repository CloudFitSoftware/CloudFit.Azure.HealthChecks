using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;

namespace CloudFit.Azure.HealthChecks;

public static class DI
{
    // Static function for injecting health checks into the application service collection
    public static void InjectHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        //TODO:  Determine configuration introduction
        var healthCheckSettings = configuration["HealthCheckSettings"];

        // TODO:  Add checks according to configuration
        services.AddHealthChecks()
            .AddCheck<KeyVaultHealthCheck>("KeyVault Health Check");

    }

    public static void ConfigureHealthChecks(this IApplicationBuilder app)
    {
        app.ConfigureHealthChecks(HealthCheckResponseWriter.ResponseWriter);
    }

    public static void ConfigureHealthChecks(this IApplicationBuilder app, Func<HttpContext, HealthReport, Task> responseWriter, string path = "/api/health")
    {
        app.UseHealthChecks(path, new HealthCheckOptions
        {
            ResponseWriter = HealthCheckResponseWriter.ResponseWriter
        });
    }
}
