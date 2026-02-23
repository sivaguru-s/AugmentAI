using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using IncidentResolution.Core.Configuration;
using IncidentResolution.Core.Interfaces;
using IncidentResolution.Core.Models;
using Microsoft.Extensions.Options;

namespace IncidentResolution.API.Services;

/// <summary>
/// ServiceNow API integration service
/// </summary>
public class ServiceNowService : IServiceNowService
{
    private readonly HttpClient _httpClient;
    private readonly ServiceNowSettings _settings;
    private readonly ILogger<ServiceNowService> _logger;

    // Filter for Finance Systems assignment group only
    private const string FinanceSystemsAssignmentGroup = "Finance Systems";

    public ServiceNowService(HttpClient httpClient, IOptions<ServiceNowSettings> settings, ILogger<ServiceNowService> logger)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
        _logger = logger;

        // Configure basic auth
        var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_settings.Username}:{_settings.Password}"));
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<List<Incident>> SearchIncidentsAsync(string description, string? category = null, int limit = 100)
    {
        try
        {
            // Filter for last 365 days
            var dateFilter = DateTime.UtcNow.AddDays(-365).ToString("yyyy-MM-dd");

            // Build query: resolved/closed incidents from last 365 days for Finance Systems only
            var query = $"resolved_at>={dateFilter}";
            query += "^state=6^ORstate=7"; // Resolved or Closed
            query += $"^assignment_group.name={Uri.EscapeDataString(FinanceSystemsAssignmentGroup)}"; // Finance Systems only

            if (!string.IsNullOrEmpty(category))
            {
                query += $"^category={Uri.EscapeDataString(category)}";
            }

            // Fetch all resolved incidents from last 365 days - we'll do keyword matching locally
            var url = $"{_settings.InstanceUrl}/api/now/{_settings.ApiVersion}/table/incident?sysparm_query={query}&sysparm_limit={limit}&sysparm_fields=number,short_description,description,category,priority,state,assignment_group,close_code,close_notes,opened_at,resolved_at,closed_at";

            _logger.LogInformation("Fetching Finance Systems incidents from ServiceNow (last 365 days)");

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ServiceNowResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var incidents = result?.Result?.Select(MapToIncident).ToList() ?? new List<Incident>();
            _logger.LogInformation("Retrieved {Count} resolved Finance Systems incidents from ServiceNow", incidents.Count);

            return incidents;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching ServiceNow incidents");
            return new List<Incident>();
        }
    }

    public async Task<List<Incident>> GetResolvedIncidentsAsync(string? category = null, int limit = 500)
    {
        try
        {
            // Filter for last 365 days
            var dateFilter = DateTime.UtcNow.AddDays(-365).ToString("yyyy-MM-dd");

            var query = $"resolved_at>={dateFilter}";
            query += "^state=6^ORstate=7"; // Resolved or Closed
            query += $"^assignment_group.name={Uri.EscapeDataString(FinanceSystemsAssignmentGroup)}"; // Finance Systems only

            if (!string.IsNullOrEmpty(category))
            {
                query += $"^category={Uri.EscapeDataString(category)}";
            }

            var url = $"{_settings.InstanceUrl}/api/now/{_settings.ApiVersion}/table/incident?sysparm_query={query}&sysparm_limit={limit}&sysparm_order_by=resolved_at&sysparm_fields=number,short_description,description,category,priority,state,assignment_group,close_code,close_notes,opened_at,resolved_at,closed_at";

            _logger.LogInformation("Fetching resolved Finance Systems incidents from last 365 days");

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ServiceNowResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var incidents = result?.Result?.Select(MapToIncident).ToList() ?? new List<Incident>();
            _logger.LogInformation("Retrieved {Count} resolved Finance Systems incidents", incidents.Count);

            return incidents;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching resolved incidents from ServiceNow");
            return new List<Incident>();
        }
    }

    private static Incident MapToIncident(ServiceNowIncident snow) => new()
    {
        Number = snow.Number ?? string.Empty,
        ShortDescription = snow.Short_Description ?? string.Empty,
        Description = snow.Description ?? string.Empty,
        Category = snow.Category ?? string.Empty,
        Priority = snow.Priority ?? string.Empty,
        State = snow.State ?? string.Empty,
        AssignmentGroup = snow.Assignment_Group ?? string.Empty,
        Resolution = snow.Close_Code ?? string.Empty,
        ResolutionNotes = snow.Close_Notes ?? string.Empty,
        OpenedAt = ParseDateTime(snow.Opened_At),
        ResolvedAt = ParseDateTime(snow.Resolved_At),
        ClosedAt = ParseDateTime(snow.Closed_At)
    };

    private static DateTime? ParseDateTime(string? dateStr) =>
        DateTime.TryParse(dateStr, out var dt) ? dt : null;

    private class ServiceNowResponse
    {
        public List<ServiceNowIncident>? Result { get; set; }
    }

    private class ServiceNowIncident
    {
        public string? Number { get; set; }
        public string? Short_Description { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public string? Priority { get; set; }
        public string? State { get; set; }
        public string? Assignment_Group { get; set; }
        public string? Close_Code { get; set; }
        public string? Close_Notes { get; set; }
        public string? Opened_At { get; set; }
        public string? Resolved_At { get; set; }
        public string? Closed_At { get; set; }
    }
}

