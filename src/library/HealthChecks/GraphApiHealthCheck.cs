using CloudFit.Azure.HealthChecks.Base;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace CloudFit.Azure.HealthChecks;

public class GraphApiHealthCheck : RestApiHealthCheckBase
{
    private static string _graphScopeKey = "GraphScope";


    // set some expected default values
    public GraphApiHealthCheck()
    {
        this.RestBaseUri = "https://graph.microsoft.com/";

        this.Props.Add(_clientIdKey, string.Empty);
        this.Props.Add(_clientSecretKey, string.Empty);
        this.Props.Add(_tenantIdKey, string.Empty);
        this.Props.Add(_graphScopeKey, _defaultTokenScope);
        this.Props.Add(_useClientTokenKey, "true");
    }

    public override async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var token = await this.GetTokenAsync($"{this.RestBaseUri}{this.Props[_graphScopeKey]}");

            if(string.IsNullOrEmpty(token))
            {
                return HealthCheckResult.Unhealthy($"Failed to get token for graph api.\n tenant: {this.Props[_tenantIdKey]}\n client: {this.Props[_clientIdKey]}\n scope: {this.Props[_graphScopeKey]}");
            }
        }
        catch (Exception e)
        {
            return HealthCheckResult.Unhealthy($"Error in getting graph api token.\n tenant: {this.Props[_tenantIdKey]}\n client: {this.Props[_clientIdKey]}\n scope: {this.Props[_graphScopeKey]}\n (error: {e.Message})", e);
        }

        return HealthCheckResult.Healthy("Successfully obtained a token for Graph API.");
    }
}