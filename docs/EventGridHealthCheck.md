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
All three properties are needed to perform a successful health status check. 

## Health Statuses
_**Healthy**_  
This status is achieved when:
- a token is successfully obtained for "https://management.azure.com/.default"
- a _GET_ is successfully executed for retrieving the properties of the Event Grid Topic Endpoint.

_**Unhealth**_  
This status is reported in the following scenarios
- _Failed to get token for_.  When an authentication token could not be obtained.
- _Failed during health check_.  Any exception thrown during the process of executing the health check.
- _Failed to vaildate DataFactory service_.  Any non-successful responses returned when calling the rest endpoint.