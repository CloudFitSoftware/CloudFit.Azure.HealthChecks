[<-- configuration](/docs/configuration.md)
<br />
<br />
# DataFactoryHealthCheck
Health check that reports the status of being able to connect to Azure Data Factory and do a GET against the pipelines endpoint.


## **appsettings.json**
```json
{
  "Name": "ADF Service Check",
  "Type": "DataFactoryHealthCheck",
  "Props": {
    "FactoryName": "NameOfDataFactory",
    "ResourceGroupName": "NameOfResourceGroup",
    "SubscriptionId": "SubscriptionGuid"
  }
}
```

## Properties
All three properties are needed to perform a successful health status check. 

## Health Statuses
_**Healthy**_  
This status is achieved when:
- a token is successfully obtained for "https://management.azure.com/.default"
- a _GET_ is successfully executed for pulling the list of pipelines base on health check properties.

_**Unhealth**_  
This status is reported in the following scenarios
- _TokenAcquisition is null_.  This check is expecting a ITokenAcquisition to have been created during dependency injection.
- _Failed during health check_.  Any exception thrown during the process of executing the health check.
- _Failed to vaildate DataFactory service_.  Any non-successful responses returned when calling the rest endpoint.