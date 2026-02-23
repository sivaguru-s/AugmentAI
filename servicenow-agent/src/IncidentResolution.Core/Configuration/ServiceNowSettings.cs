namespace IncidentResolution.Core.Configuration;

/// <summary>
/// ServiceNow API configuration settings
/// </summary>
public class ServiceNowSettings
{
    public const string SectionName = "ServiceNow";

    public string InstanceUrl { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ApiVersion { get; set; } = "v2";
}

