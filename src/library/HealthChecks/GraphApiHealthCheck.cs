using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Azure.Core;
using Azure.Identity;
using CloudFit.Azure.HealthChecks.Base;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace CloudFit.Azure.HealthChecks;

public class GraphApiHealthCheck : RestApiHealthCheckBase, IHealthCheck, IConfigureHealthCheck
{
    private static string _clientIdKey = "";
    private static string _clientSecretKey = "";
    private static string _tenantIdKey = "";
    private static string _graphScopeKey = "";
    
    private readonly IEnumerable<string> PropNames = (new[] { "RestBaseUri", "ClientId", "ClientSecret", "TenantId", "GraphScope" });

    // values supplied from configuration
    private string RestBaseUri { get; set; }
    private string ClientId { get; set; }
    private string ClientSecret { get; set; }
    private string TenantId { get; set; }
    private string GraphScope { get; set; }

    // holding variables for successful token
    private string Token { get; set; }
    private DateTime TokenExpiration { get; set; }

    // default graph scope
    private string _defaultGraphScope = ".default";

    // set some expected default values
    public GraphApiHealthCheck()
    {
        this.RestBaseUri = "https://graph.microsoft.com";
        this.GraphScope = this._defaultGraphScope;
        this.Token = string.Empty;
        this.TokenExpiration = DateTime.Now.AddSeconds(-10);
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            if ((this.TokenExpiration < DateTime.Now) || string.IsNullOrEmpty(this.Token))
            {
                var dataDict = new Dictionary<string, string>()
                {
                    { "client_id", this.ClientId },
                    { "client_secret", this.ClientSecret },
                    { "scope", $"{this.RestBaseUri}/{this.GraphScope}" },
                    { "grant_type", "client_credentials" }
                };

                using (var client = new HttpClient())
                {
                    client.Timeout = new TimeSpan(0, 0, 5);

                    var content = new FormUrlEncodedContent(dataDict);

                    var response_message = await client.PostAsync($"https://login.microsoftonline.com/{this.TenantId}/oauth2/v2.0/token", content);
                    var response_content = await response_message.Content.ReadAsStringAsync();
                    if (response_message.IsSuccessStatusCode)
                    {
                        var response = JsonSerializer.Deserialize<System.Text.Json.Nodes.JsonNode>(response_content);
                        if (response != null)
                        {
                            //token = response.access_token;
                            this.Token = response["access_token"].GetValue<string>();
                            this.TokenExpiration = DateTime.Now.AddSeconds((response["expires_in"].GetValue<int>()));
                        }
                    }
                    else
                    {
                        return HealthCheckResult.Unhealthy($"Failed to get token for graph api ({response_content}).\n tenant: {this.TenantId}\n client: {this.ClientId}\n scope: {this.GraphScope}");
                    }
                }
            }
        }
        catch (Exception e)
        {
            return HealthCheckResult.Unhealthy($"Error in getting graph api token.\n tenant: {this.TenantId}\n client: {this.ClientId}\n scope: {this.GraphScope}\n (error: {e.Message})", e);
        }

        return HealthCheckResult.Healthy($"Successfully obtained a token for: \n tenant: {this.TenantId}\n client: {this.ClientId}\n scope: {this.GraphScope}");
    }

    public void SetHealthCheckProperties(IDictionary<string, object>? props)
    {
        if (props != null)
        {
            foreach (var name in this.PropNames)
            {
                if (props.ContainsKey(name) && (props[name] != null))
                {
                    switch (name)
                    {
                        case "ClientId":
                            {
                                this.ClientId = (string)(props[name]);
                                break;
                            }
                        case "ClientSecret":
                            {
                                this.ClientSecret = (string)(props[name]);
                                break;
                            }
                        case "TenantId":
                            {
                                this.TenantId = (string)(props[name]);
                                break;
                            }
                    }
                }
            }
        }
    }
}