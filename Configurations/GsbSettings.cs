namespace GSB.Test.Api.Configurations;

public class GsbSettings
{
    public string BaseUrl { get; set; } = string.Empty;

    public string ApiKey { get; set; } = string.Empty;

    public string SecretKey { get; set; } = string.Empty;

    public string IV { get; set; } = string.Empty;

    [ConfigurationKeyName("User_Key")]
    public string KeyUser { get; set; } = string.Empty;

    [ConfigurationKeyName("Bundle-Id")]
    public string BundleId { get; set; } = string.Empty;

    public string RuleId { get; set; } = string.Empty;
}