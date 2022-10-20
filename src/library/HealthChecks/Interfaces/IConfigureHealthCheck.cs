using CloudFit.Azure.HealthChecks.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CloudFit.Azure.HealthChecks;

public interface IConfigureHealthCheck
{
    IHealthChecksBuilder AddHealthCheck(IHealthChecksBuilder builder, HealthCheckConfig config);
}