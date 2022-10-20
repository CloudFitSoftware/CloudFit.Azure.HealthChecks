using CloudFit.Azure.HealthChecks.Base;

namespace CloudFit.Azure.HealthChecks;

public class DataFactoryHealthCheck : ManagementHealthCheckBase
{
    // Property keys
    private const string _factNameKey = "FactoryName";

    // Local variables
    private const string _apiVersion = "api-version=2018-06-01";
    private const string _provider = "Microsoft.DataFactory";
    private const string _providerGroup = "factories";

    public DataFactoryHealthCheck() : base()
    {
        this.Props.Add(_factNameKey, string.Empty);

        this.ItemNameKey = _factNameKey;
        this.ApiVersion = _apiVersion;
        this.Provider = _provider;
        this.ProviderGroup = _providerGroup;
    }
}