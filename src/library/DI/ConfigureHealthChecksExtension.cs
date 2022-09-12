using CloudFit.Azure.HealthChecks.Configuration;
using CloudFit.Azure.HealthChecks.ResponseWriter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace CloudFit.Azure.HealthChecks.DI;

public static class ConfigureHealthChecksExtension
{
    private static string _defaultPath = "/api/health";

    
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