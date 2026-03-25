using FinanceBudget.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace FinanceBudget.Services;

public class JiraCostingService : IJiraCostingService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<JiraCostingService> _logger;
    private readonly string _baseUrl;
    private readonly string _projectKey;

    public JiraCostingService(HttpClient httpClient, IConfiguration configuration, ILogger<JiraCostingService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
        
        _baseUrl = configuration["Jira:BaseUrl"] ?? throw new ArgumentNullException("Jira:BaseUrl");
        _projectKey = configuration["Jira:ProjectKey"] ?? "FINANCE";
        
        var username = configuration["Jira:Username"];
        var apiToken = configuration["Jira:ApiToken"];
        
        if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(apiToken))
        {
            var authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{apiToken}"));
            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authToken);
        }
        
        _httpClient.DefaultRequestHeaders.Accept.Add(
            new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<List<JiraCosting>> GetJiraCostingDataAsync(string? unitCode = null)
    {
        try
        {
            // Build JQL query
            var jql = $"project = {_projectKey}";
            if (!string.IsNullOrEmpty(unitCode))
            {
                jql += $" AND cf[10001] = '{unitCode}'"; // Assuming custom field for Unit
            }

            var url = $"{_baseUrl}/rest/api/3/search?jql={Uri.EscapeDataString(jql)}&maxResults=1000&fields=summary,status,assignee,created,duedate,priority,customfield_10001,customfield_10002,customfield_10003";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var jiraResponse = JsonConvert.DeserializeObject<JiraSearchResponse>(content);

            if (jiraResponse?.Issues == null)
                return new List<JiraCosting>();

            return jiraResponse.Issues.Select(issue => new JiraCosting
            {
                IssueKey = issue.Key ?? "",
                Unit = GetCustomFieldValue(issue.Fields, "customfield_10001"),
                Summary = issue.Fields?.Summary ?? "",
                Status = issue.Fields?.Status?.Name ?? "",
                EstimatedCost = ParseDecimal(GetCustomFieldValue(issue.Fields, "customfield_10002")),
                ActualCost = ParseDecimal(GetCustomFieldValue(issue.Fields, "customfield_10003")),
                Assignee = issue.Fields?.Assignee?.DisplayName ?? "Unassigned",
                CreatedDate = issue.Fields?.Created,
                DueDate = issue.Fields?.DueDate,
                Priority = issue.Fields?.Priority?.Name ?? ""
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching JIRA costing data");
            return new List<JiraCosting>();
        }
    }

    public async Task<JiraCosting?> GetJiraIssueAsync(string issueKey)
    {
        try
        {
            var url = $"{_baseUrl}/rest/api/3/issue/{issueKey}?fields=summary,status,assignee,created,duedate,priority,customfield_10001,customfield_10002,customfield_10003";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var issue = JsonConvert.DeserializeObject<JiraIssue>(content);

            if (issue == null)
                return null;

            return new JiraCosting
            {
                IssueKey = issue.Key ?? "",
                Unit = GetCustomFieldValue(issue.Fields, "customfield_10001"),
                Summary = issue.Fields?.Summary ?? "",
                Status = issue.Fields?.Status?.Name ?? "",
                EstimatedCost = ParseDecimal(GetCustomFieldValue(issue.Fields, "customfield_10002")),
                ActualCost = ParseDecimal(GetCustomFieldValue(issue.Fields, "customfield_10003")),
                Assignee = issue.Fields?.Assignee?.DisplayName ?? "Unassigned",
                CreatedDate = issue.Fields?.Created,
                DueDate = issue.Fields?.DueDate,
                Priority = issue.Fields?.Priority?.Name ?? ""
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching JIRA issue");
            return null;
        }
    }

    private string GetCustomFieldValue(JiraFields? fields, string fieldName)
    {
        if (fields == null) return "";
        
        try
        {
            var fieldValue = fields.GetType().GetProperty(fieldName)?.GetValue(fields);
            return fieldValue?.ToString() ?? "";
        }
        catch
        {
            return "";
        }
    }

    private decimal ParseDecimal(string value)
    {
        return decimal.TryParse(value, out var result) ? result : 0;
    }

    private class JiraSearchResponse
    {
        [JsonProperty("issues")]
        public List<JiraIssue>? Issues { get; set; }
    }

    private class JiraIssue
    {
        [JsonProperty("key")]
        public string? Key { get; set; }
        
        [JsonProperty("fields")]
        public JiraFields? Fields { get; set; }
    }

    private class JiraFields
    {
        [JsonProperty("summary")]
        public string? Summary { get; set; }
        
        [JsonProperty("status")]
        public JiraStatus? Status { get; set; }
        
        [JsonProperty("assignee")]
        public JiraUser? Assignee { get; set; }
        
        [JsonProperty("created")]
        public DateTime? Created { get; set; }
        
        [JsonProperty("duedate")]
        public DateTime? DueDate { get; set; }
        
        [JsonProperty("priority")]
        public JiraPriority? Priority { get; set; }
    }

    private class JiraStatus
    {
        [JsonProperty("name")]
        public string? Name { get; set; }
    }

    private class JiraUser
    {
        [JsonProperty("displayName")]
        public string? DisplayName { get; set; }
    }

    private class JiraPriority
    {
        [JsonProperty("name")]
        public string? Name { get; set; }
    }
}

