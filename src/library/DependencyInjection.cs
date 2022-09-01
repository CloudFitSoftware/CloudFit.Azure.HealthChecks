using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using CloudFit.Azure.HealthChecks.ResponseWriter;
using CloudFit.Azure.HealthChecks.Configuration;

namespace CloudFit.Azure.HealthChecks;

public static class DI
{
    private static string _defaultPath = "/api/health";

    // Static function for injecting health checks into the application service collection
    public static void InjectHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        // parse configuration
        var healthCheckSettings = HealthCheckSettings.GetSettings(configuration);

        // create builder
        var healthCheckBuilder = services.AddHealthChecks();

        foreach (var config in healthCheckSettings.HealthCheckConfigs)
        {
            healthCheckBuilder.AddHealthCheck(config);
        }

    }

    public static void InjectHealthChecks(this IServiceCollection services, IConfiguration configuration, Type[] dbContextTypes)
    {
        // create builder
        var healthCheckBuilder = services.AddHealthChecks();

        // parse configuration
        var healthCheckSettings = HealthCheckSettings.GetSettings(configuration);

        if (healthCheckSettings != null && healthCheckSettings.HealthCheckConfigs != null)
        {
            foreach (var config in healthCheckSettings.HealthCheckConfigs)
            {
                healthCheckBuilder.AddHealthCheck(config);
            }
        }

        if (dbContextTypes != null)
        {
            foreach (var dbContextType in dbContextTypes)
            {
                //healthCheckBuilder.AddDbContextCheck();
                typeof(EntityFrameworkCoreHealthChecksBuilderExtensions).GetMethod("AddDbContextCheck").MakeGenericMethod(dbContextType).Invoke(healthCheckBuilder, null);
                //healthCheckBuilder.GetType().GetMethod("",1, null);
            }
        }
    }


    public static void ConfigureHealthChecks(this IApplicationBuilder app)
    {
        app.ConfigureHealthChecks(HealthCheckResponseWriter.ResponseWriter, _defaultPath);
    }

    public static void ConfigureHealthChecks(this IApplicationBuilder app, IConfiguration configuration)
    {
        // parse configuration
        var healthCheckSettings = HealthCheckSettings.GetSettings(configuration);

        var path = _defaultPath;
        if (!string.IsNullOrEmpty(healthCheckSettings.Path))
        {
            path = healthCheckSettings.Path;
        }

        app.ConfigureHealthChecks(HealthCheckResponseWriter.ResponseWriter, path);
    }

    public static void ConfigureHealthChecks(this IApplicationBuilder app, Func<HttpContext, HealthReport, Task> responseWriter, string relativeUriPath)
    {
        app.UseHealthChecks(relativeUriPath, new HealthCheckOptions
        {
            ResponseWriter = HealthCheckResponseWriter.ResponseWriter
        });
    }
}
