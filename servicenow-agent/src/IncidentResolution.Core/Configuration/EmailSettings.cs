namespace IncidentResolution.Core.Configuration;

/// <summary>
/// SMTP Email service configuration settings
/// </summary>
public class EmailSettings
{
    public const string SectionName = "Email";

    public string SmtpServer { get; set; } = string.Empty;
    public int SmtpPort { get; set; } = 587;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool EnableSsl { get; set; } = true;
    public string SenderEmail { get; set; } = string.Empty;
    public string SenderDisplayName { get; set; } = "Incident Resolution Assistant";
    public string FinanceDistributionList { get; set; } = "DL_AFI_ARC_Financial_Systems_Group_IT@Ashleyfurniture.com";
}

