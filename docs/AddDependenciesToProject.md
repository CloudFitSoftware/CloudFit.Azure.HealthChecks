[<-- configuration](/docs/configuration.md)
<br />
<br />
# Adding Dependencies to Your Project

Some health checks have additional dependencies that are not included with the NuGet package.  This is because if the health check is not being used, the extra "weight" of additional dependencies are not included with a project.

This page covers some notes on adding these additional dependencies.

### <u>HealthCheck Custom Attribute</u>
Custom attribute is used in code to mark individual HealthCheck classes.  One or more may be applied to a class.  If the dependency is missing, a health check will return _Degraded_ with a message that includes the name of the dependency.

examples:
```c#
[NugetPackageAttribute("AspNetCore.HealthChecks.Redis")]
public class RedisHealthCheck
```

### <u>Adding to Project</u>
Additional dependencies need to be added to the project where you added CloudFit.Azure.HealthChecks.

At this time, the additional dependencies can all be found on Nuget.org.  So to add them, just simply use your favortie approach.

example:  vscode
```
PS z:\sln\prj\dotnet add package <package-name>
```

### <u>Mdifying the Project File</u>
With dotnet projects, you need an additional step.  In the project file, add the [CopyLocalLockFileAssemblies](https://learn.microsoft.com/en-us/dotnet/core/project-sdk/msbuild-props#copylocallockfileassemblies) property.

```xml
<PropertyGroup>
    <CopyLocalLockFileAssemblies>true</CopyLocalLockFileAssemblies>
</PropertyGroup>
```

### <u>Package References</u>

__[AspNetCore.Diagnostics.HealthChecks](https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks)__  
This package already has some health checks fully coded and maintained.  The thin wrapper we use around it is to be able to leverage the use of our configuration scheme.  Currently, packages used from here include:
- AspNetCore.HealthChecks.AzureKeyVault
- AspNetCore.HealthChecks.Redis