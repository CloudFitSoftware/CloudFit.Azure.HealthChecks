[<-- configuration](/docs/configuration.md)
<br />
<br />
# KeyVaultHealthCheck
Health check that reports the status of being able to connect to an Azure Key Vault.

Uses [AspNetCore.HealthChecks.AzureKeyVault](https://www.nuget.org/packages/AspNetCore.HealthChecks.AzureKeyVault) for check secrets, keys, and certificates.  See [How to add dependencies](/docs/AddDependenciesToProject.md).

## **appsettings.json**
```json
{
  "Name": "KeyVault Health Check",
  "Type": "KeyVaultHealthCheck",
  "Props": {
      "KeyVaultName": "name-of-key-vault",
      "SecretNames": "SecretToCheck1;SecretToCheck2",
      "KeyNames": "KeyToCheck1;KeyToCheck2",
      "CertificateNames": "CertToCheck1;CertToCheck2",
      "ResourceGroupName": "NameOfResourceGroup",
      "SubscriptionId": "SubscriptionGuid"
  }
}
```
_alternative_
```json
{
  "Name": "KeyVault Health Check",
  "Type": "KeyVaultHealthCheck",
  "Props": {
      "KeyVaultUri": "fully-qualified-url-of-key-vault",
      "SecretNames": "SecretToCheck1;SecretToCheck2",
      "KeyNames": "KeyToCheck1;KeyToCheck2",
      "CertificateNames": "CertToCheck1;CertToCheck2"
  }
}
```

## Properties
_KeyVaultName_ is required property value for a successful check.  _KeyVaultName_ is prepended to ".vault.azure.net", in order to create a fully qualified uri.
_KeyVaultUri_ can be used in place of _KeyVaultName_ when checking secrets, keys, and certificates.  If both exist, _KeyVaultName_ takes precedence.
_SecretNames_ (optional): semi-colon delimited string of secret names to test retrieval.
_KeyNames_ (optional): semi-colon delimited string of key names to test retrieval.
_CertificateNames_ (optional): semi-colon delimited string of certificate names to test retrieval and expiration.

_ResourceGroupName_ and _SubscriptionId_ are optional.  One or both can exist.  Some additiona notes:
- if either property does not exist at the root HealthCheckSettings, then they are required here.
- if either property does exist at the root HealthCheckSettings, setting the value here takes precedence for the individual health check.
- this properties are ignored when the additional checking of secrets, keys, and certificates are included in the configuration.

## Health Statuses
_**Healthy**_  
This status is achieved when:
- successfully accessing the properties of the key vault.  (_Not the secrets inside_)
- successfully accessing all secrets, keys, and certificates defined in the configuration.

_**Degraded**_
This status is achieved when:
- configuration is invalid
- missing dependency package

_**Unhealth**_  
This status is reported in the following scenarios:
- failed to connect to the key vault.
- failed to get the properties of the key vault.
- failed to validate any one of the secrets, keys, or certicates defined in the configuration.
- failed to get an access token
- any unhandled exceptions during health check