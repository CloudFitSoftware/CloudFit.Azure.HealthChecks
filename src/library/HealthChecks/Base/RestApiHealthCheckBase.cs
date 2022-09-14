using Microsoft.Identity.Web;

namespace CloudFit.Azure.HealthChecks.Base;

public abstract class RestApiHealthCheckBase
{
    public static ITokenAcquisition? TokenAcquisition { get; set; }

    internal protected IDictionary<string, string> Props { get; set; }

    internal protected string RestBaseUri { get; set; }

    internal protected static string _tokenScopeKey = "TokenScope";

    internal RestApiHealthCheckBase()
    {
        this.Props = new Dictionary<string, string>()
        {
            { _tokenScopeKey, ".default" }
        };
    }

    public virtual void SetHealthCheckProperties(IDictionary<string, object>? props)
    {
        if (props != null)
        {
            foreach (var key in props.Keys)
            {
                if (this.Props.ContainsKey(key))
                {
                    this.Props[key] = ((string)props[key]);
                }
            }
        }
    }
}