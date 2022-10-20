using CloudFit.Azure.HealthChecks.Base;

namespace CloudFit.Azure.HealthChecks;

public class EventGridHealthCheck : ManagementHealthCheckBase
{
    // Property keys
    private const string _topicNameKey = "TopicName";

    // Local variables
    private const string _apiVersion = "api-version=2022-06-15";
    private const string _provider = "Microsoft.EventGrid";
    private const string _providerGroup = "topics";

    public EventGridHealthCheck() : base()
    {
        this.Props.Add(_topicNameKey, string.Empty);
        
        this.ItemNameKey = _topicNameKey;
        this.ApiVersion = _apiVersion;
        this.Provider = _provider;
        this.ProviderGroup = _providerGroup;
    }
}