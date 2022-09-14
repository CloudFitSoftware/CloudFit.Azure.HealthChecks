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
              "ExampleProp2": "ExamplePropValue"
            },
            "KeyRefs": [
              "Existing:Config:Key",
              "Existing:Config:Key"
            ]
          }
        ]
    }
}
```
**Path:** This is the relative url to direct health data.  Default show in example above.  
**IncludeDbContext:** This is a collection of strings to identify the _Microsoft.EntityFrameworkCore.DbContext_ to wrap in health checks.  
  * Leaving this as an empty array includes all DbContexts add to the _IServiceCollection_.
  * Removing this from configuration will exclude all DbContexts from health checks via this process.

### **HealthCheckConfigs**
For each health check desired, use the below to add to the HealthCheckConfigs collection.

> [_WARNING_]:  HealthCheckConfig is unique by name.  When multiple of the same check defined in configuration, remember to give them different names.  

<br />

> [_NOTE_]:  For a better understanding of setting an individual health check properties:  
>> **Props:** Set the value of a property for that health check.  Name directly matches the property name on the health check class.  
**KeyRefs:** An array of strings to reference another configuration key.  These values will be parsed based on the ':', and the last part will be used to match against the property name on the health check class.  _use these in replacement of the name/value pair under Props._

<br />

> [_TIP_]: You can have multiple instances of the same type of health check.  For example, if you have more than one Key Vaults, create a KeyVaultHealthCheck config to match to each one.

<br />

### **Health Checks**
- [DataFactoryHealthCheck](/docs/DataFactoryHealthCheck.md)
- [GraphApiHealthCheck](/docs/GraphApiHealthCheck.md)
- [KeyVaultHealthCheck](/docs/KeyVaultHealthCheck.md)
