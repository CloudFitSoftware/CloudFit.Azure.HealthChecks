[<-- configuration](/docs/configuration.md)
<br />
<br />
# KeyVaultHealthCheck
Health check that reports the status of being able to connect to an Azure Key Vault.


## **appsettings.json**
```json
{
  "Name": "KeyVault Health Check",
  "Type": "KeyVaultHealthCheck",
  "Props": {
      "KeyVaultName": "name-of-key-vault"
  }
}
```

## Properties
_KeyVaultName_ is a required property value for a successful check. 

## Health Statuses
_**Healthy**_  
This status is achieved when successfully accessing the properties of the key vault.  (_Not the secrets inside_)

_**Unhealth**_  
This status is reported in the following scenarios
- _Error in creataing Key Vault client_.  Any exception thrown during the process of executing the health check.
- _Key Vault client not created_.  This check is expecting a successful creation of a KeyVault client.