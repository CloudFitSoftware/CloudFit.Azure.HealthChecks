using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace CloudFit.Azure.HealthChecks.Base;

public abstract class ManagementHealthCheckBase : RestApiHealthCheckBase
{
    protected string ApiVersion { get; set; }
    protected string Provider { get; set; }
    protected string ProviderGroup { get; set; }
    protected string ItemNameKey { get; set; }

    public ManagementHealthCheckBase() : base()
    {
        this.Props.Add(_subIdKey, string.Empty);
        this.Props.Add(_rgNameKey, string.Empty);

        this.ApiVersion = string.Empty;
        this.ItemNameKey = string.Empty;
        this.RestBaseUri = "https://management.azure.com/";
        this.Provider = string.Empty;
        this.ProviderGroup = string.Empty;
    }

    public override async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            // Check for the required ItemNameKey that will come through configuration
            if (string.IsNullOrEmpty(this.Props[this.ItemNameKey]))
            {
                // Reporting missing, required configuration properties
                return HealthCheckResult.Degraded($"Configuration for '{context.Registration.Name}' requires ({this.ItemNameKey}) in Props.");
            }

            // Check for the required _subIdKey that will come through configuration
            if (string.IsNullOrEmpty(this.Props[_subIdKey]))
            {
                // Reporting missing, required configuration properties
                return HealthCheckResult.Degraded($"Configuration for '{context.Registration.Name}' requires ({_subIdKey}) in Props.");
            }

            // Check for the required _rgNameKey that will come through configuration
            if (string.IsNullOrEmpty(this.Props[_rgNameKey]))
            {
                // Reporting missing, required configuration properties
                return HealthCheckResult.Degraded($"Configuration for '{context.Registration.Name}' requires ({_rgNameKey}) in Props.");
            }

            // Get a token to use w/ rest call
            var token = await this.GetTokenAsync($"{this.RestBaseUri}{this.Props[_tokenScopeKey]}");

            if (!string.IsNullOrEmpty(token))
            {
                // create a rest request
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, token);

                    // Call the management url to get the identified item from the provider and group under the provider
                    var response = await client.GetAsync($"{this.RestBaseUri}subscriptions/{this.Props[_subIdKey]}/resourceGroups/{this.Props[_rgNameKey]}/providers/{this.Provider}/{this.ProviderGroup}/{this.Props[this.ItemNameKey]}?{this.ApiVersion}");
                    var content = await response.Content.ReadAsStringAsync();
                    if (!response.IsSuccessStatusCode)
                    {
                        return HealthCheckResult.Unhealthy($"Failed to connect to {this.Provider} ({this.Props[this.ItemNameKey]}) service.  ({content})");
                    }
                    else
                    {
                        var jNode = JsonNode.Parse(content);
                        string? provState = jNode?["properties"]?["provisioningState"]?.GetValue<string>();
                        if(string.IsNullOrEmpty(provState) || (provState != "Succeeded"))
                        {
                            return HealthCheckResult.Unhealthy($"Failed to vaildate {this.Provider} ({this.Props[this.ItemNameKey]}) is accessible.  ({content})");
                        }
                    }
                }
            }
            else
            {
                return HealthCheckResult.Unhealthy($"Failed to get token for ({this.RestBaseUri}). {this.Provider} ({this.Props[this.ItemNameKey]})");
            }
        }
        catch (Exception e)
        {
            return HealthCheckResult.Unhealthy($"Failed during health check.  {this.Provider} ({this.Props[this.ItemNameKey]}).  {e.Message}", e);
        }

        return HealthCheckResult.Healthy($"Successfully validated {this.Provider} ({this.Props[this.ItemNameKey]}).");
    }
}