using IncidentResolution.Core.Models;

namespace IncidentResolution.Core.Interfaces;

/// <summary>
/// Interface for ServiceNow API operations
/// </summary>
public interface IServiceNowService
{
    /// <summary>
    /// Search for similar incidents based on description
    /// </summary>
    Task<List<Incident>> SearchIncidentsAsync(string description, string? category = null, int limit = 10);

    /// <summary>
    /// Get resolved incidents with resolutions
    /// </summary>
    Task<List<Incident>> GetResolvedIncidentsAsync(string? category = null, int limit = 50);
}

