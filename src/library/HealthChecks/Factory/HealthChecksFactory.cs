using CloudFit.Azure.HealthChecks.Attributes;
using CloudFit.Azure.HealthChecks.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CloudFit.Azure.HealthChecks;

public static class HealthChecksFactory
{
    public static IHealthChecksBuilder AddHealthCheck(this IHealthChecksBuilder builder, HealthCheckConfig config)
    {
        if (config != null)
        {
            // Check for required base required parameters of a health check
            if (string.IsNullOrEmpty(config.Name) || string.IsNullOrEmpty(config.Type))
            {
                var message = "Invalid health check config.  Type and Name are required.";
                message += $"\n  Type: {config.Type}";
                message += $"\n  Name: {config.Name}";

                return builder.AddCheck(System.Guid.NewGuid().ToString(), new ConfigurationHealthCheck($"Invalid: {config.Type} ({config.Name})"));
            }

            // Get the health check via reflection
            var hcType = System.Type.GetType($"CloudFit.Azure.HealthChecks.{config.Type}");
            if (hcType != null)
            {
                try
                {
                    // instantiate a new instance of the health check
                    var healthCheck = (Activator.CreateInstance(hcType));

                    if (healthCheck != null)
                    {
                        // Go to health check for parameter parsing and adding to the builder
                        return (healthCheck as IConfigureHealthCheck)?.AddHealthCheck(builder, config) ?? builder;
                    }
                }
                catch (System.Exception ex)
                {
                    var message = $"Error in instantiating health check: {config.Type} ({config.Name})\n\nError message: {ex.Message}";

                    // if custom attribute exists, then modify message based on attribute contents
                    var nugetPkgAttr = hcType?.GetCustomAttributes(typeof(NugetPackageAttribute), true);

                    if (nugetPkgAttr != null)
                    {
                        message += "\n\n Additional package dependency:";
                        foreach (var attr in nugetPkgAttr)
                        {
                            message += $"\n\t  {(attr as NugetPackageAttribute)?.NugetPackage}";
                        }
                    }

                    // Return a configuration health check to alert to an error in establishing the health check
                    return builder.AddCheck(config.Name, new ConfigurationHealthCheck(message, ex));
                }
            }
            else
            {
                // Return a configuration health check to report on a bad configuration
                return builder.AddCheck(config.Name, new ConfigurationHealthCheck($"Health Check does not exist: {config.Type} ({config.Name})"));
            }
        }

        return builder;
    }
}