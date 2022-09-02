# CloudFit.Azure.HealthChecks

This package provides an simple and opinionated setup for HealthChecks in any ASP.NET Application.

Adding HealthChecks to an existing can be time consuming and often times multiple projects require the same healthchecks with extremely similar configuration.  This package uses a configuration to drive health checks and runs health checks in a standardized method to Log Analytics, App Insights, etc.

The logs output from the configured health checks can be easily consumed by CloudFit's flagship product, CFS.

Other things to include:

  - **Technology stack**: C#, JSON, .NET
  - **Status**: Incubation
  - **Links to production or demo instances**
  - If you are looking for an an easy button to setup monitoroing in your .NET application that runs on Azure, this is the package for you.  If you want complete control over the log formatting and health check code, you may be better suited with a custom solution.

---

## Installation

In the vscode terminal:

    dotnet add package CloudFit.Azure.HealthChecks

---

## Configuration

For incorporating into your application, see [CONFIGURATION](/docs/configuration.md)

---

## Getting involved

General instructions on _how_ to contribute should be stated with a link to [CONTRIBUTING](CONTRIBUTING.md).

----

## Open source licensing info
1. [LICENSE](LICENSE)


----

## Credits and references

1. Configure your Azure Alerts with [cfs-integration-hub](https://github.com/CloudFitSoftware/cfs-integration-hub)
2. Learn how [CloudFit Software](https://www.cloudfitsoftware.com/) can help your organization.

3. Microsoft Health Check [ref](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks?view=aspnetcore-6.0)
