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
**Props:** Set the value of a property for that health check.  Name directly matches the property name on the health check class.  
**KeyRefs:** An array of strings to reference another configuration key.  These values will be parsed based on the ':', and the last part will be used to match against the property name on the health check class.  

For each health check desired, use the below to add to the HealthCheckConfigs collection.

### KeyVaultCheck
```json
{
  "Name": "KeyVault Health Check",
  "Type": "KeyVaultHealthCheck",
  "Props": {
      "KeyVaultName": "name-of-key-vault"
  }
}
```
**KeyVaultCheck** checks the ability for the app to reach the specified key vault.

### GraphAPIHealthCheck
```json
,
{
  "Name": "Graph API Health Check",
  "Type": "GraphApiHealthCheck",
  "Props": {
    "GraphScope": "Email.Send",
    "TenantId": "guid"
  },
  "KeyRefs": [
    "Email:ClientId",
    "Email:ClientSecret"
  ]
}
```
**GraphAPIHealthCheck** checks the ability for the app to get an auth token against graph.microsoft.com for the defined scope.  
_GraphScope_:  Default value is:  ".default"