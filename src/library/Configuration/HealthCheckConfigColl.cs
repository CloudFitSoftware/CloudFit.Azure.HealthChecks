using Microsoft.Extensions.Configuration;

namespace CloudFit.Azure.HealthChecks.Configuration;

public class HealthCheckConfigColl
{
    private HealthCheckConfigColl () {}

    public static HealthCheckConfigColl GetConfigCollection(IConfiguration configuration)
    {
        var cfgColl = new HealthCheckConfigColl();
        var cfgSec = configuration.GetSection("HealthChecks");

        if(cfgSec != null) {
            cfgSec.Bind(cfgColl);
        }

        return cfgColl;
    }

    public IEnumerable<HealthCheckBaseConfig> HealthCheckConfigs { get; set; }
}