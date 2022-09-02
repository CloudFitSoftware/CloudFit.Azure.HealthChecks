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

    /*
    * InjectHealthChecks
    */
    // Static function for injecting health checks into the application service collection
    public static void InjectHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        services.InjectHealthChecks(configuration, null);
    }

    // Static function for injecting health checks into the application service collection
    public static void InjectHealthChecks(this IServiceCollection services, IConfiguration configuration, Type[]? dbContextTypes)
    {
        // create builder
        var healthCheckBuilder = services.AddHealthChecks();

        // parse configuration
        var healthCheckSettings = HealthCheckSettings.GetSettings(configuration);

        // If individual health checks defined
        if (healthCheckSettings != null && healthCheckSettings.HealthCheckConfigs != null)
        {
            // add each health check
            foreach (var config in healthCheckSettings.HealthCheckConfigs)
            {
                healthCheckBuilder.AddHealthCheck(config);
            }
        }

        // if dbContext types (primarily for Entity Framework)
        if (dbContextTypes != null)
        {
            // Define arguments for reflection of generic method
            var methodInputArgs = new[] { healthCheckBuilder, System.Type.Missing, System.Type.Missing, System.Type.Missing, System.Type.Missing };

            foreach (var dbContextType in dbContextTypes)
            {
                // Get generic method for adding ootb dbContext health check
                var methodInfo = typeof(EntityFrameworkCoreHealthChecksBuilderExtensions).GetMethod("AddDbContextCheck");
                if(methodInfo != null)
                {
                    // add health check for dbContext
                    methodInfo.MakeGenericMethod(dbContextType).Invoke(null, methodInputArgs);
                }
            }
        }
    }

    /*
    * ConfigureHealthChecks
    */
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
