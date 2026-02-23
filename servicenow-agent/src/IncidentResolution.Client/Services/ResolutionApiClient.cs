using System.Net.Http.Json;
using IncidentResolution.Client.Models;

namespace IncidentResolution.Client.Services;

/// <summary>
/// Client for the Resolution API
/// </summary>
public class ResolutionApiClient
{
    private readonly HttpClient _httpClient;

    public ResolutionApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Get resolution recommendations for an incident
    /// </summary>
    public async Task<ResolutionResponse?> GetRecommendationsAsync(ResolutionRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/resolution/recommend", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ResolutionResponse>();
    }

    /// <summary>
    /// Check API health
    /// </summary>
    public async Task<bool> HealthCheckAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/resolution/health");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}

