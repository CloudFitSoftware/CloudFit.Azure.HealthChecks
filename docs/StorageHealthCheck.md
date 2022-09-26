[<-- configuration](/docs/configuration.md)
<br />
<br />
# StorageHealthCheck
Health check that reports the status of being able to connect to a specific Azure Storage and do a GET for its properties.


## **appsettings.json**
```json
{
  "Name": "Azure Storage Check",
  "Type": "StorageHealthCheck",
  "Props": {
    "AccountName": "NameOfStorageAccount",
    "StorageType": "SpecificStorageType"
  }
}
```

## Properties
All properties are needed to perform a successful health status check. 
_StorageType_ is a specific Azure Storage.  Current valid values are:
- Blob
- Queue
- Table

## Health Statuses
_**Healthy**_  
This status is achieved when:
- a token is successfully obtained for "https://management.azure.com/.default".
- a _GET_ is successfully executed for "https://{AccountName}.{StorageType}.core.windows.net/?restype=service&comp=properties".

_**Unhealth**_  
This status is reported in the following scenarios
- _Failed to vaildate {StorageType} storage account_.  When a non-successful result occurs for the GET of the storage account properties.
- _Failed to get token for_.  When an authentication token could not be obtained.
- _Storage account type is invalid_.  This check is verifying that the _StorageType_ property is a valid value.
- _Failed during health check_.  Any exception thrown during the process of executing the health check.