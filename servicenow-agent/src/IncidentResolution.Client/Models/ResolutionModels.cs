namespace IncidentResolution.Client.Models;

/// <summary>
/// Resolution recommendation model
/// </summary>
public class Resolution
{
    public int Rank { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> Steps { get; set; } = new();
    public double ConfidenceScore { get; set; }
    public string SourceIncidentNumber { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
}

/// <summary>
/// Request model for resolution lookup
/// </summary>
public class ResolutionRequest
{
    public string IncidentDescription { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public bool SendEmail { get; set; } = true;
}

/// <summary>
/// Response model containing top resolutions
/// </summary>
public class ResolutionResponse
{
    public string OriginalQuery { get; set; } = string.Empty;
    public List<Resolution> Resolutions { get; set; } = new();
    public bool EmailSent { get; set; }
    public string? EmailError { get; set; }
    public DateTime ProcessedAt { get; set; }
}

