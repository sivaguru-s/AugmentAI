using IncidentResolution.Core.Models;

namespace IncidentResolution.Core.Interfaces;

/// <summary>
/// Interface for email notification service
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Send email notification with resolution recommendations
    /// </summary>
    Task<bool> SendResolutionEmailAsync(string incidentDescription, List<Resolution> resolutions);
}

