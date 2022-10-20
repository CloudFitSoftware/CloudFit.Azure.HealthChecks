[<-- configuration](/docs/configuration.md)
<br />
<br />
# EventGridHealthCheck
Health check that reports the status of connecting to the Event Grid Topic Endpoint.


## **appsettings.json**
```json
{
  "Name": "Event Grid Endpoint Check",
  "Type": "EventGridHealthCheck",
  "Props": {
    "TopicName": "NameOfTopic",
    "SubscriptionId": "IdOfSubscription",
    "ResourceGroupName": "NameOfResourceGroup"
  }
}
```

## Properties
_TopicName_ is required.
_ResourceGroupName_ and _SubscriptionId_ are optional.  One or both can exist.  Some additiona notes:
- if either property does not exist at the root HealthCheckSettings, then they are required here.
- if either property does exist at the root HealthCheckSettings, setting the value here takes precedence for the individual health check.

## Health Statuses
_**Healthy**_  
This status is achieved when:
- a token is successfully obtained for "https://management.azure.com/.default"
- a _GET_ is successfully executed for retrieving the properties of the Event Grid Topic Endpoint.

_**Degraded**_
This status is achieved when:
- configuration is invalid

_**Unhealth**_  
This status is reported in the following scenarios
- _Failed to get token for_.  When an authentication token could not be obtained.
- _Failed during health check_.  Any exception thrown during the process of executing the health check.
- _Failed to vaildate DataFactory service_.  Any non-successful responses returned when calling the rest endpoint.