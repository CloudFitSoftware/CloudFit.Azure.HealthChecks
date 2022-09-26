using CloudFit.Azure.HealthChecks.Base;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace CloudFit.Azure.HealthChecks;

public class EventGridHealthCheck : RestApiHealthCheckBase, IHealthCheck, IConfigureHealthCheck
{
    // Property keys
    private static string _topicNameKey = "TopicName";
    private static string _subIdKey = "SubscriptionId";
    private static string _rgNameKey = "ResourceGroupName";

    // Local variables
    private static string _apiVersion = "api-version=2022-06-15";

    public EventGridHealthCheck() : base()
    {
        this.Props.Add(_topicNameKey, string.Empty);
        this.Props.Add(_subIdKey, string.Empty);
        this.Props.Add(_rgNameKey, string.Empty);

        this.RestBaseUri = "https://management.azure.com/";
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var token = await this.GetTokenAsync($"{this.RestBaseUri}{this.Props[_tokenScopeKey]}");

            if (!string.IsNullOrEmpty(token))
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, token);

                    var url = $"{this.RestBaseUri}subscriptions/{this.Props[_subIdKey]}/resourceGroups/{this.Props[_rgNameKey]}/providers/Microsoft.EventGrid/topics/{this.Props[_topicNameKey]}?{_apiVersion}";
                    var response = await client.GetAsync(url);
                    var content = response.Content.ReadAsStringAsync();
                    
                    if (!response.IsSuccessStatusCode)
                    {
                        return HealthCheckResult.Unhealthy($"Failed to vaildate Event Grid.  ({content.Result})");
                    }
                }
            }
            else
            {
                return HealthCheckResult.Unhealthy($"Failed to get token for ({this.RestBaseUri})");
            }
        }
        catch (Exception e)
        {
            return HealthCheckResult.Unhealthy($"Failed during health check.  {e.Message}", e);
        }

        return HealthCheckResult.Healthy($"Successfully validated Event Grid.");
    }
}