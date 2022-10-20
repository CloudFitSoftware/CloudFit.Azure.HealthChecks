
namespace CloudFit.Azure.HealthChecks.Attributes;

[System.AttributeUsage(System.AttributeTargets.Class)]
public class NugetPackageAttribute : System.Attribute
{
    private string _nugetPackage;

    public NugetPackageAttribute(string nugetPackage)
    {
        _nugetPackage = nugetPackage;
    }

    public string NugetPackage { get { return _nugetPackage; } }
}