using Microsoft.Extensions.Configuration;

namespace CloudFit.Azure.HealthChecks.Configuration;

public class HealthCheckSettings
{
    public static HealthCheckSettings GetSettings(IConfiguration configuration)
    {
        var hcSet = new HealthCheckSettings();

        var cfgSec = configuration.GetSection("HealthCheckSettings") as IConfiguration;

        if (cfgSec != null)
        {
            cfgSec.Bind(hcSet, options =>
            {
                options.ErrorOnUnknownConfiguration = true;
            });
        }

        // Resolve any placeholders to values for top level settings
        ResolveSettingValues(hcSet, configuration);

        foreach (var hcCfg in hcSet.HealthCheckConfigs)
        {
            if (hcCfg.Props != null)
            {
                if (hcCfg.Props.Count > 0)
                {
                    foreach (var key in hcCfg.Props.Keys)
                    {
                        if (hcCfg.Props[key] != null)
                        {
                            // Resolve any placeholders to values for properties
                            hcCfg.Props[key] = ResolveConfigValue(hcCfg.Props[key], configuration);
                        }
                    }
                }

                // Copy top level settings down to individual health check property collection, if not existing
                CascadeDownGlobalProperties(hcCfg.Props, hcSet);
            }
        }

        return hcSet;
    }

    private static void CascadeDownGlobalProperties(Dictionary<string, object?> localProps, HealthCheckSettings globalSettings)
    {
        if (!localProps.ContainsKey("ResourceGroupName")) { localProps.Add("ResourceGroupName", globalSettings.ResourceGroupName); }
        if (!localProps.ContainsKey("SubscriptionId")) { localProps.Add("SubscriptionId", globalSettings.SubscriptionId); }
    }

    private static void ResolveSettingValues(HealthCheckSettings settings, IConfiguration configuration)
    {
        if (!string.IsNullOrEmpty(settings.ResourceGroupName))
        {
            var newValue = ResolveConfigValue((object?)settings.ResourceGroupName, configuration);
            settings.ResourceGroupName = (string?)newValue;
        }

        if (!string.IsNullOrEmpty(settings.SubscriptionId))
        {
            var newValue = ResolveConfigValue((object?)settings.SubscriptionId, configuration);
            settings.SubscriptionId = (string?)newValue;
        }

        if (settings.IncludeDbContext != null && settings.IncludeDbContext.Count() > 0)
        {
            var newValue = ResolveConfigValue((object?)(settings.IncludeDbContext.ToArray()), configuration);
            settings.IncludeDbContext = ((string[]?)newValue)?.ToList();
        }
    }

    private static object? ResolveConfigValue(object? value, IConfiguration configuration)
    {
        if ((value != null))
        {
            if (value is string && ((string)value).StartsWith("::"))
            {
                var altCfgKey = ((string)value).TrimStart(':');
                var altCfgValue = configuration[altCfgKey];
                value = altCfgValue;
            }
            else if (value is string[])
            {
                var aryValue = (string[])value;
                for (var v = 0; v < aryValue.Length; v++)
                {
                    if (aryValue[v].StartsWith("::"))
                    {
                        var altCfgKey = ((string)value).TrimStart(':');
                        var altCfgValue = configuration[altCfgKey];
                        aryValue[v] = altCfgValue;
                    }
                }
                value = aryValue;
            }
        }

        return value;
    }

    public string? Path { get; set; }

    public IEnumerable<string>? IncludeDbContext { get; set; }

    public string? ResourceGroupName { get; set; }

    public string? SubscriptionId { get; set; }

    public string? AppInsightsInstrumentationKey { get; set; }

    public IEnumerable<HealthCheckConfig> HealthCheckConfigs { get; set; }
}