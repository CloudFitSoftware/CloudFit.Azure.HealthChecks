[<-- configuration](/docs/configuration.md)
<br />
<br />
# RedisHealthCheck
Health check that reports the status of being able to connect to an Redis endpoints.

Uses [AspNetCore.HealthChecks.Redis](https://www.nuget.org/packages/AspNetCore.HealthChecks.Redis).  **required** to add to your project.

## **appsettings.json**
```json
{
  "Name": "Redis Health Check",
  "Type": "RedisHealthCheck",
  "Props": {
    "ConnectionString": "RedisConnectionString",
    "ResourceGroupName": "NameOfResourceGroup",
    "SubscriptionId": "SubscriptionGuid"
  }
}
```

## Properties
_ConnectionString_ is required property value for a successful check.
_ResourceGroupName_ and _SubscriptionId_ are optional.  One or both can exist.  Some additiona notes:
- if either property does not exist at the root HealthCheckSettings, then they are required here.
- if either property does exist at the root HealthCheckSettings, setting the value here takes precedence for the individual health check.

## Health Statuses
_**Healthy**_  
This status is achieved when:  
- successfully connecting to redis.
- successfully connecting to each end point.
- successfully reaching any databases.
- each cluster reports "ok"

_**Degraded**_
This status is achieved when:
- configuration is invalid
- missing dependency package

_**Unhealth**_  
This status is reported in the following scenarios:  
- any healthy status is _not_ achieved.  
- any unhandled exception during the health check.