using CloudFit.Azure.HealthChecks.Base;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace CloudFit.Azure.HealthChecks;

public class DataFactoryHealthCheck : RestApiHealthCheckBase, IHealthCheck, IConfigureHealthCheck
{
    // Property keys
    private static string _subIdKey = "SubscriptionId";
    private static string _rgNameKey = "ResourceGroupName";
    private static string _factNameKey = "FactoryName";

    // Local variables
    private string _apiVersion = "api-version=2018-06-01";

    public DataFactoryHealthCheck() : base()
    {
        this.Props.Add(_subIdKey, string.Empty);
        this.Props.Add(_rgNameKey, string.Empty);
        this.Props.Add(_factNameKey, string.Empty);


        this.RestBaseUri = "https://management.azure.com/";
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        if (TokenAcquisition != null)
        {
            try
            {
                var token = await this.GetTokenAsync($"{this.RestBaseUri}{this.Props[_tokenScopeKey]}");

                if (!string.IsNullOrEmpty(token))
                {
                    using (var client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, token);

                        var response = await client.GetAsync($"{this.RestBaseUri}subscriptions/{this.Props[_subIdKey]}/resourceGroups/{this.Props[_rgNameKey]}/providers/Microsoft.DataFactory/factories/{this.Props[_factNameKey]}/pipelines?{_apiVersion}");
                        var content = response.Content.ReadAsStringAsync();
                        if (response.IsSuccessStatusCode)
                        {
                            return HealthCheckResult.Healthy($"Successfully validated DataFactory service.");
                        }
                        else
                        {
                            return HealthCheckResult.Unhealthy($"Failed to vaildate DataFactory service.  ({content})");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                return HealthCheckResult.Unhealthy($"Failed during health check.  {e.Message}", e);
            }
        }

        return HealthCheckResult.Unhealthy("TokenAcquisition is null.");
    }
}