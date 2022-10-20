using Microsoft.Extensions.DependencyInjection;
using CloudFit.Azure.HealthChecks.Configuration;
using CloudFit.Azure.HealthChecks.Attributes;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using CloudFit.Azure.HealthChecks.Base;

namespace CloudFit.Azure.HealthChecks;

[NugetPackageAttribute("AspNetCore.HealthChecks.AzureKeyVault")]
public class KeyVaultHealthCheck : ManagementHealthCheckBase
{
    // properties that are a part of configuration
    private const string keyVaultNameKey = "KeyVaultName";
    private const string keyVaultUriKey = "KeyVaultUri";

    private const string secretNamesKey = "SecretNames";
    private const string keyNamesKey = "KeyNames";
    private const string certNamesKey = "CertificateNames";

    // variables specific to Key Vault
    private const string _apiVersion = "api-version=2021-10-01";
    private const string _provider = "Microsoft.KeyVault";
    private const string _providerGroup = "vaults";

    public KeyVaultHealthCheck()
    {
        this.Props.Add(keyVaultNameKey, string.Empty);
        this.Props.Add(keyVaultUriKey, string.Empty);

        this.ItemNameKey = keyVaultNameKey;
        this.ApiVersion = _apiVersion;
        this.Provider = _provider;
        this.ProviderGroup = _providerGroup;
    }

    public override async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(this.Props[keyVaultNameKey]) && string.IsNullOrEmpty(this.Props[keyVaultUriKey]))
        {
            // Reporting missing, required configuration properties
            return HealthCheckResult.Degraded($"Configuration for '{context.Registration.Name}' requires either a ({keyVaultNameKey}) or ({keyVaultUriKey}) in Props.");
        }

        // use the base class for checking accessibility of the key vault.
        return await base.CheckHealthAsync(context, cancellationToken);
    }

    public override IHealthChecksBuilder AddHealthCheck(IHealthChecksBuilder builder, HealthCheckConfig config)
    {
        // use the associated nuget package when desired to check secrets, keys, and certificates
        if (config.Props != null)
        {
            if (config.Props.ContainsKey(secretNamesKey) || config.Props.ContainsKey(keyNamesKey) || config.Props.ContainsKey(certNamesKey))
            {
                if (config.Props.ContainsKey(keyVaultNameKey))
                {
                    return KeyVaultExtensionHealthCheck.AddHealthCheck(builder, (new Uri($"https://{config.Props[keyVaultNameKey]}.vault.azure.net")), config.Props, config.Name ?? "KeyVault Health Check Name Missing");
                }
                else if (config.Props.ContainsKey(keyVaultUriKey))
                {
                    return KeyVaultExtensionHealthCheck.AddHealthCheck(builder, (new Uri($"{config.Props[keyVaultUriKey]}")), config.Props, config.Name ?? "KeyVault Health Check Name Missing");
                }
            }
        }

        // use this class for validating reachability of the key vault.
        return base.AddHealthCheck(builder, config);
    }
}