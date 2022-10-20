using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace CloudFit.Azure.HealthChecks;

public class ConfigurationHealthCheck : IHealthCheck
{
    private string _message;
    private Exception? _exception;

    public ConfigurationHealthCheck(string? message = null, Exception? exception = null)
    {
        _message = message ?? string.Empty;
        _exception = exception;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        await Task.Delay(10);

        var combined = _message;
        if(_exception != null)
        {
            combined += "\n\n " + _exception.Message;
        }

        return HealthCheckResult.Degraded(combined, _exception);
    }
}