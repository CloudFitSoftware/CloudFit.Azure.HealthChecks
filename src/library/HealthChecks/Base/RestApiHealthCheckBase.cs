using Azure.Core;
using Azure.Identity;
using CloudFit.Azure.HealthChecks.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Identity.Client;

namespace CloudFit.Azure.HealthChecks.Base;

public abstract class RestApiHealthCheckBase : IHealthCheck, IConfigureHealthCheck
{
    internal static TokenCredential TokenProvider = new DefaultAzureCredential();

    internal protected IConfidentialClientApplication? ConfidentialClientApplication { get; set; }

    internal protected IDictionary<string, string> Props { get; set; }

    internal protected string RestBaseUri { get; set; }

    internal protected string AccessTokenString { get; set; }
    internal protected DateTimeOffset AccessTokenExpiration { get; set; }

    // internal strings representing keys in the Props dictionary.
    internal protected static string _rgNameKey = "ResourceGroupName";
    internal protected static string _subIdKey = "SubscriptionId";

    internal protected static string _clientIdKey = "ClientId";
    internal protected static string _clientSecretKey = "ClientSecret";
    internal protected static string _tenantIdKey = "TenantId";
    internal protected static string _tokenScopeKey = "TokenScope";
    internal protected static string _useClientTokenKey = "UseClientToken";
    internal protected static string _defaultTokenScope = ".default";

    internal RestApiHealthCheckBase()
    {
        this.Props = new Dictionary<string, string>()
        {
            { _tokenScopeKey, _defaultTokenScope }
        };

        this.AccessTokenString = string.Empty;
    }

    // Using reflection to add the value of a property to the ConfidentialClientApplicationOptions object
    internal void AddPropToOptions(ConfidentialClientApplicationOptions options, string propName)
    {
        if (!string.IsNullOrEmpty(propName) && this.Props.ContainsKey(propName))
        {
            var prop = typeof(ConfidentialClientApplicationOptions).GetProperty(propName);
            if (prop != null) { prop.SetValue(options, this.Props[propName]); }
        }
    }

    // Create an instance of a IConfidentialClientApplication
    internal void CreateClientAuthProviderFromProps()
    {
        var ccAppOptions = new ConfidentialClientApplicationOptions();

        AddPropToOptions(ccAppOptions, _clientIdKey);
        AddPropToOptions(ccAppOptions, _clientSecretKey);
        AddPropToOptions(ccAppOptions, _tenantIdKey);

        this.ConfidentialClientApplication = ConfidentialClientApplicationBuilder.CreateWithApplicationOptions(ccAppOptions).Build();
    }

    // Get a token for use in authentication of rest api calls
    internal async Task<string> GetTokenAsync(string scope, string authScheme = JwtBearerDefaults.AuthenticationScheme)
    {
        var token = string.Empty;

        if (UseClientToken && (this.ConfidentialClientApplication != null))
        {
            token = (await this.ConfidentialClientApplication.AcquireTokenForClient(new[] { scope }).ExecuteAsync()).AccessToken;
        }
        else if (TokenProvider != null)
        {
            if ((string.IsNullOrEmpty(AccessTokenString)) || (AccessTokenExpiration < DateTime.Now))
            {
                var accessToken = await TokenProvider.GetTokenAsync(new TokenRequestContext(scopes: new[] { scope }) { }, new CancellationToken());
                AccessTokenExpiration = accessToken.ExpiresOn;
                AccessTokenString = accessToken.Token;
            }

            token = AccessTokenString;
        }

        return token;
    }

    // Determine whether to use a client application token
    internal bool UseClientToken
    {
        get
        {
            var useClientToken = false;

            if ((this.Props.ContainsKey(_useClientTokenKey)) &&
                (!string.IsNullOrEmpty(this.Props[_useClientTokenKey])) &&
                (bool.Parse(this.Props[_useClientTokenKey])))
            {
                useClientToken = true;
            }

            return useClientToken;
        }
    }

    public virtual async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        await Task.Delay(10);

        return HealthCheckResult.Degraded($"Only base class implementation found.");
    }

    public virtual IHealthChecksBuilder AddHealthCheck(IHealthChecksBuilder builder, HealthCheckConfig config)
    {
        // Update dictionary of properties based on input argument
        if (config.Props != null)
        {
            foreach (var key in config.Props.Keys)
            {
                if (this.Props.ContainsKey(key))
                {
                    this.Props[key] = ((string)config.Props[key]);
                }
            }
        }

        // Try and build a client auth provider from the Props dictionary
        if (this.UseClientToken)
        {
            this.CreateClientAuthProviderFromProps();
        }

        return builder.AddCheck(config.Name, this);
    }
}