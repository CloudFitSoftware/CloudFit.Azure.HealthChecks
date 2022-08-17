namespace CloudFit.Azure.HealthChecks;

public interface IConfigureHealthCheck
{
    void SetHealthCheckProperties(IDictionary<string, string> props);
}