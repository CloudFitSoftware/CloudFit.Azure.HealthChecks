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
            cfgSec.Bind(hcSet);
        }

        foreach (var hcCfg in hcSet.HealthCheckConfigs)
        {
            if(hcCfg.Props != null && hcCfg.Props.Count > 0)
            {
                foreach(var key in hcCfg.Props.Keys)
                {
                    var value = hcCfg.Props[key];
                    if((value != null) && ((string)value).StartsWith("::"))
                    {
                        var altCfgKey = ((string)value).TrimStart(':');
                        var altCfgValue = configuration[altCfgKey];
                        hcCfg.Props[key] = altCfgValue;
                    }
                }
            }
        }

        return hcSet;
    }

    public string? Path { get; set; }

    public IEnumerable<string>? IncludeDbContext { get; set; }

    public IEnumerable<HealthCheckConfig> HealthCheckConfigs { get; set; }
}