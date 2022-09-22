using CloudFit.Azure.HealthChecks.Base;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace CloudFit.Azure.HealthChecks;

public class StorageHealthCheck : RestApiHealthCheckBase, IHealthCheck, IConfigureHealthCheck
{
    // Property keys
    private static string _accountNameKey = "AccountName";
    private static string _storageTypeKey = "StorageType";

    private static string _storageBaseUri = ".core.windows.net/";
    

    private enum StorageAccountTypes
    {
        Blob,
        Queue,
        Table
    }

    private string uriQuery = "restype=service&comp=properties";


    public StorageHealthCheck() : base()
    {
        this.Props.Add(_accountNameKey, string.Empty);
        this.Props.Add(_storageTypeKey, string.Empty);
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            StorageAccountTypes acctType;
            if (Enum.TryParse<StorageAccountTypes>(this.Props[_storageTypeKey], false, out acctType))
            {
                var token = await this.GetTokenAsync("https://storage.azure.com/.default");

                if (!string.IsNullOrEmpty(token))
                {
                    using (var client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, token);
                        client.DefaultRequestHeaders.Add("x-ms-version", "2021-08-06");
                        client.DefaultRequestHeaders.Add("x-ms-date", $"{DateTime.UtcNow.ToString("ddd, dd MMM yyyy HH:mm:ss")} GMT");

                        var response = await client.GetAsync($"{this.RestBaseUri}?{this.uriQuery}");
                        var content = response.Content.ReadAsStringAsync();
                        if (!response.IsSuccessStatusCode)
                        {
                            return HealthCheckResult.Unhealthy($"Failed to vaildate [{this.Props[_storageTypeKey]}] storage account.  ({this.Props[_accountNameKey]}).  ({content.Result})");
                        }
                    }
                }
            }
            else
            {
                HealthCheckResult.Unhealthy($"Storage account type is invalid.  ({this.Props[_storageTypeKey]})");
            }
        }
        catch (Exception e)
        {
            return HealthCheckResult.Unhealthy($"Failed during health check.  {e.Message}", e);
        }

        return HealthCheckResult.Healthy($"Successfully connected to [{this.Props[_storageTypeKey]}] storage with account: {this.Props[_accountNameKey]})");

    }

    public override void SetHealthCheckProperties(IDictionary<string, object>? props)
    {
        base.SetHealthCheckProperties(props);

        // reset base url on inputted properties of Account Name and Storage Account Type
        if (this.Props.ContainsKey(_accountNameKey) && this.Props.ContainsKey(_storageTypeKey))
        {
            this.RestBaseUri = $"https://{this.Props[_accountNameKey]}.{this.Props[_storageTypeKey]}{_storageBaseUri}";
        }
        else
        {
            this.RestBaseUri = string.Empty;
        }
    }
}