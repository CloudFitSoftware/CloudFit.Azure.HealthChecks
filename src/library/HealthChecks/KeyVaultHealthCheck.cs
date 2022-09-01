using Microsoft.Extensions.Diagnostics.HealthChecks;
using Azure.Security.KeyVault.Secrets;
using Azure.Identity;

namespace CloudFit.Azure.HealthChecks;

public class KeyVaultHealthCheck : IHealthCheck, IConfigureHealthCheck
{
    private readonly IEnumerable<string> PropNames = (new[] { "KeyVaultName" });

    private string? KeyVaultName { get; set; }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var kvUri = $"https://{this.KeyVaultName}.vault.azure.net";
            var keyVaultClient = new SecretClient(new Uri(kvUri), new DefaultAzureCredential());

            if (keyVaultClient == null)
            {
                return HealthCheckResult.Unhealthy($"Key Vault client not created.  (KeyVaultName: {this.KeyVaultName})");
            }

            keyVaultClient.GetPropertiesOfSecrets();
        }
        catch (Exception e)
        {
            return HealthCheckResult.Unhealthy($"Error in creataing Key Vault client.  (KeyVaultName: {this.KeyVaultName})", e);
        }

        return HealthCheckResult.Healthy();
    }

    public void SetHealthCheckProperties(IDictionary<string, object> props)
    {
        foreach (var name in this.PropNames)
        {
            switch(name){
                case "KeyVaultName": {
                    this.KeyVaultName = (props[name] as string);
                    break;
                }
            }
        }
    }
}