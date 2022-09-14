[<-- configuration](/docs/configuration.md)
<br />
<br />
# GraphApiHealthCheck
Health check that reports the status of being able to connect to the GraphAPI and obtain a token for the specified scope(s).


## **appsettings.json**
```json
{
  "Name": "Graph API Health Check",
  "Type": "GraphApiHealthCheck",
  "Props": {
    "GraphScope": "Email.Send",
    "TenantId": "guid",
    "ClientId": "guid",
    "ClientSecret": "guid"
  }
}
```

## Properties
_GraphScope_: (optional).  Use to override the use of ".default" as the scope to validate.

## Health Statuses
_**Healthy**_  
This status is achieved when successfully obtaining a token for use with Graph API under the specified scope(s).

_**Unhealth**_  
This status is reported in the following scenarios
- _Error in getting graph api token_.  Any exception thrown during the process of executing the health check.
- _Failed to get token for graph api_.  This check is expecting to obtain a token for Graph API with the specified scope(s).