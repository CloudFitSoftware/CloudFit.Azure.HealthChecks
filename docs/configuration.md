# Configuring Alerts in Your Application

Once the package is referenced within your application, incorporate the below code and add the configuration to the appsettings.json file.

## Code

Use the InjectHealthChecks extension method for the services object.
```c#
public void ConfigureServices(IServiceCollection services)
{
  IConfiguration configuration = ...
  :
  services.InjectHealthChecks(configuration);
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

### HealthCheckSettings
This is the required root configuration.  
```json
{
    "HealthCheckSettings": {
        "Path": "/api/health",
        "HealthCheckConfigs": [
          {
            "Name": "Example Health Check",
            "Type": "ExampleConfig",
            "Props": {
              "ExampleProp1": "ExamplePropValue",
              "ExampleProp2": "ExamplePropValue"
            }
          }
        ]
    }
}
```
For each health check desired, use the below to add to the HealthCheckConfigs collection.

### KeyVaultCheck
```json
{
    "Name": "KeyVault Health Check",
    "Type": "KeyVaultConfig",
    "Props": {
        "KeyVaultName": "name-of-key-vault"
    }
}
```