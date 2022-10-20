
using Azure.Identity;
using CloudFit.Azure.HealthChecks.Attributes;
using HealthChecks.AzureKeyVault;
using Microsoft.Extensions.DependencyInjection;

namespace CloudFit.Azure.HealthChecks;

[NugetPackageAttribute("AspNetCore.HealthChecks.AzureKeyVault")]
public class KeyVaultExtensionHealthCheck
{
    private const string secretNamesKey = "SecretNames";
    private const string keyNamesKey = "KeyNames";
    private const string certNamesKey = "CertificateNames";

    public static IHealthChecksBuilder AddHealthCheck(IHealthChecksBuilder builder, Uri keyVaultUri, IDictionary<string, object> props, string checkName)
    {
        // use external extension class to add health checks for incorporating checking individual secrets, keys, and certificates.
        var kvOptions = new Action<AzureKeyVaultOptions>(options =>
                {
                    AddSecretNames(props, options);
                    AddKeyNames(props, options);
                    AddCertNames(props, options);
                });

        // Use extension method for adding a key vault health check
        return builder.AddAzureKeyVault(keyVaultUri, new DefaultAzureCredential(), kvOptions, checkName);
    }

    private static void AddSecretNames(IDictionary<string, object> props, AzureKeyVaultOptions options)
    {
        if (props.ContainsKey(secretNamesKey))
        {
            var secretNames = (props[secretNamesKey] as string)?.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (secretNames != null)
            {
                foreach (var sn in secretNames)
                {
                    options.AddSecret(sn);
                }
            }
        }
    }

    private static void AddKeyNames(IDictionary<string, object> props, AzureKeyVaultOptions options)
    {
        if (props.ContainsKey(keyNamesKey))
        {
            var keyNames = (props[keyNamesKey] as string)?.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (keyNames != null)
            {
                foreach (var kn in keyNames)
                {
                    options.AddKey(kn);
                }
            }
        }
    }

    private static void AddCertNames(IDictionary<string, object> props, AzureKeyVaultOptions options)
    {
        if (props.ContainsKey(certNamesKey))
        {
            var certNames = (props[certNamesKey] as string)?.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (certNames != null)
            {
                foreach (var cn in certNames)
                {
                    options.AddCertificate(cn, true);
                }
            }
        }
    }

}