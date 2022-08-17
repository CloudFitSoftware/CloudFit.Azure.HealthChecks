using Microsoft.Extensions.Configuration;

namespace CloudFit.Azure.HealthChecks.Configuration;

public class HealthCheckSettings
{
    public static HealthCheckSettings GetSettings(IConfiguration configuration)
    {
        var hcSet = new HealthCheckSettings();
        var cfgSec = configuration.GetSection("HealthCheckSettings") as IConfiguration;

        if(cfgSec != null) {
            cfgSec.Bind(hcSet);
        }

        return hcSet;
    }

    public string Path {get; set; }

    public IEnumerable<HealthCheckConfig> HealthCheckConfigs { get; set; }
}