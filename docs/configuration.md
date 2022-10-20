# Configuring Alerts in Your Application

Once the package is referenced within your application, incorporate the below code and add the configuration to the appsettings.json file.

## Code

Use the InjectHealthChecks extension method for the services object.
```c#
public void ConfigureServices(IServiceCollection services)
{
  :
  services.InjectHealthChecks();
}
```

Use the ConfigureHealthChecks extension method for the application object
```c#
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
  IConfiguration configuration = ...
  :
  app.ConfigureHealthChecks(configuration);
}
```
---

## appSettings.json
Health checks configuration is driven out of the appSettings json file.

### **HealthCheckSettings**
This is the required root configuration.  
```json
{
    "HealthCheckSettings": {
        "Path": "/api/health",
        "ResourceGroupName": "NameOfResourceGroup",
        "SubscriptionId": "SubscriptionGuid",
        "IncludeDbContext": [
          "MyCustomDBContextClassName1",
          "MyCustomDBContextClassName2"
        ],
        "HealthCheckConfigs": [
          {
            "Name": "Example Health Check",
            "Type": "ExampleConfig",
            "Props": {
              "ExampleProp1": "ExamplePropValue",
              "ExampleProp2": "ExamplePropValue",
              "ExampleProp3": "::PathToExisting:ConfigKey"
            }
          }
        ]
    }
}
```
**Path:** This is the relative url to direct health data.  Default show in example above.  
**ResourceGroupName** _and_ **SubscriptionId:** (_optional_) These two properties represent the Azure target location of resources (the id/guid of the subscription and the name of the resource group).  
  * If either one exists at the root, it will be cascaded down into the individual HealthCheckConfigs.Props collections that _DO NOT_ have the property(-ies).  

**IncludeDbContext:** This is a collection of strings to identify the _Microsoft.EntityFrameworkCore.DbContext_ to wrap in health checks.  
  * Leaving this as an empty array includes all DbContexts add to the _IServiceCollection_.
  * Removing this from configuration will exclude all DbContexts from health checks via this process.

### **HealthCheckConfigs**
For each health check desired, use the below to add to the HealthCheckConfigs collection.

> [_WARNING_]:  HealthCheckConfig is unique by name.  When multiple of the same check defined in configuration, remember to give them different names.  

<br />

> [_NOTE_]:  To reference the value of another configuration key (in order to prevent duplication), use the key's full name, pre-pended with a double colon (::).
```json
"Props": {
  "ExampleProp1": "A value to used by this property",
  "ExampleProp2": "::PathToExisting:ConfigKey:WhoseValue:WillBe:UsedHere"
}
```

<br />

> [_TIP_]: You can have multiple instances of the same type of health check.  For example, if you have more than one Key Vaults, create a KeyVaultHealthCheck config to match to each one.

<br />

### **Health Checks**
- [DataFactoryHealthCheck](/docs/DataFactoryHealthCheck.md)
- [EventGridHealthCheck](/docs/EventGridHealthCheck.md)
- [GraphApiHealthCheck](/docs/GraphApiHealthCheck.md)
- [KeyVaultHealthCheck](/docs/KeyVaultHealthCheck.md)
- [StorageHealthCheck](/docs/StorageHealthCheck.md)
