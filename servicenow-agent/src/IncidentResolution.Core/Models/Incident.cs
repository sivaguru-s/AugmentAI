namespace IncidentResolution.Core.Models;

/// <summary>
/// Represents an incident from ServiceNow
/// </summary>
public class Incident
{
    public string Number { get; set; } = string.Empty;
    public string ShortDescription { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string AssignmentGroup { get; set; } = string.Empty;
    public string Resolution { get; set; } = string.Empty;
    public string ResolutionNotes { get; set; } = string.Empty;
    public DateTime? OpenedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public DateTime? ClosedAt { get; set; }
}

