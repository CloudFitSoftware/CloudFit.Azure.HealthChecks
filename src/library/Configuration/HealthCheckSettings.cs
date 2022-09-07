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
            if ((hcCfg.KeyRefs != null) && (hcCfg.KeyRefs.Count > 0))
            {
                if (hcCfg.Props == null) { hcCfg.Props = new Dictionary<string, object>(); }

                foreach (var key in hcCfg.KeyRefs)
                {
                    var cfgVal = configuration[key];
                    hcCfg.Props.Add((key.Split(':').Last()), cfgVal);
                }
            }
        }

        return hcSet;
    }

    public string Path { get; set; }

    public IEnumerable<HealthCheckConfig> HealthCheckConfigs { get; set; }
}