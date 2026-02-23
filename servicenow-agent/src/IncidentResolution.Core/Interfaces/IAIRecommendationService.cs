using IncidentResolution.Core.Models;

namespace IncidentResolution.Core.Interfaces;

/// <summary>
/// Interface for AI-powered recommendation service
/// </summary>
public interface IAIRecommendationService
{
    /// <summary>
    /// Generate top resolution recommendations based on incident description and historical data
    /// </summary>
    Task<List<Resolution>> GetRecommendationsAsync(string incidentDescription, List<Incident> historicalIncidents, int topN = 3);
}

