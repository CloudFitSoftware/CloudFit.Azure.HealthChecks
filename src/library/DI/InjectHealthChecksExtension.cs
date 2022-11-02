using CloudFit.Azure.HealthChecks.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CloudFit.Azure.HealthChecks.DI;

public static class InjectHealthChecksExtensions
{
    /*
    * InjectHealthChecks
    */
    // Static function for injecting health checks into the application service collection
    public static IHealthChecksBuilder InjectHealthChecks(this IServiceCollection services)
    {
        // create builder
        var healthCheckBuilder = services.AddHealthChecks();

        // pull individual services out of the collection
        var sp = services.BuildServiceProvider();

        sp.ProcessIConfiguration(services, healthCheckBuilder);

        return healthCheckBuilder;
    }

    private static void AddHealthChecksFromConfig(this IHealthChecksBuilder builder, HealthCheckSettings settings)
    {
        // if HealthCheckConfigs have been defined
        if (settings.HealthCheckConfigs != null)
        {
            // add each health check
            foreach (var config in settings.HealthCheckConfigs)
            {
                builder.AddHealthCheck(config);
            }
        }
    }

    private static void AddDbContextHealthChecks(this IHealthChecksBuilder builder, IServiceCollection services, HealthCheckSettings settings)
    {
        // if IncludeDbContext (primarily for Entity Framework DbContext added to the IServiceCollection)
        if (settings.IncludeDbContext != null)
        {
            var dbContexts = services.Where(srv => (srv.ServiceType.BaseType != null) &&
                                                (srv.ServiceType.BaseType.Name == "DbContext") &&
                                                ((settings.IncludeDbContext.Count() == 0) || (settings.IncludeDbContext.Contains(srv.ServiceType.Name))))
                                     .Select(srv => srv.ServiceType).ToList();

            if (dbContexts != null)
            {
                // Define arguments for reflection of generic method
                var methodInputArgs = new[] { builder, System.Type.Missing, System.Type.Missing, System.Type.Missing, System.Type.Missing };

                foreach (var dbContext in dbContexts)
                {
                    // Get generic method for adding ootb dbContext health check
                    var methodInfo = typeof(EntityFrameworkCoreHealthChecksBuilderExtensions).GetMethod("AddDbContextCheck");
                    if (methodInfo != null)
                    {
                        // add health check for dbContext
                        methodInfo.MakeGenericMethod(dbContext).Invoke(null, methodInputArgs);
                    }
                }
            }
        }
    }

    private static void ProcessIConfiguration(this ServiceProvider serviceProvider, IServiceCollection services, IHealthChecksBuilder builder)
    {
        var configuration = serviceProvider.GetService<IConfiguration>();

        if (configuration != null)
        {
            // parse configuration
            var healthCheckSettings = HealthCheckSettings.GetSettings(configuration);

            // If individual health checks defined
            if (healthCheckSettings != null)
            {

                builder.AddHealthChecksFromConfig(healthCheckSettings);

                builder.AddDbContextHealthChecks(services, healthCheckSettings);

                try
                {
                    if (healthCheckSettings.AppInsightsInstrumentKey != null)
                    {
                        HealthChecksPublisherAppInsights.AddAppInsightsPublisher(builder, healthCheckSettings);
                    }
                }
                catch (System.Exception ex)
                {
                    var message = $"Error in adding health checks publisher.\n\nError message: {ex.Message}";

                    // Return a configuration health check to alert to an error in establishing the health check
                    builder.AddCheck("Add Health Check Publishers", new ConfigurationHealthCheck(message, ex));
                }
            }
        }
    }
}