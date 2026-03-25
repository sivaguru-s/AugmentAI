using FinanceBudget.Models;
using Newtonsoft.Json;
using System.Text;

namespace FinanceBudget.Services;

public class AFEDataService : IAFEDataService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AFEDataService> _logger;
    private readonly string _baseUrl;
    private readonly string _tableName;

    public AFEDataService(HttpClient httpClient, IConfiguration configuration, ILogger<AFEDataService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
        
        _baseUrl = configuration["ServiceNow:BaseUrl"] ?? throw new ArgumentNullException("ServiceNow:BaseUrl");
        _tableName = configuration["ServiceNow:AFETableName"] ?? "u_afe_data";
        
        var username = configuration["ServiceNow:Username"];
        var password = configuration["ServiceNow:Password"];
        
        if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
        {
            var authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}"));
            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authToken);
        }
        
        _httpClient.DefaultRequestHeaders.Accept.Add(
            new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<List<AFEData>> GetAFEDataAsync(string? unitCode = null)
    {
        try
        {
            var url = $"{_baseUrl}/api/now/table/{_tableName}";
            
            if (!string.IsNullOrEmpty(unitCode))
            {
                url += $"?sysparm_query=u_unit={unitCode}";
            }
            
            url += string.IsNullOrEmpty(unitCode) ? "?" : "&";
            url += "sysparm_limit=1000";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var serviceNowResponse = JsonConvert.DeserializeObject<ServiceNowResponse>(content);

            if (serviceNowResponse?.Result == null)
                return new List<AFEData>();

            return serviceNowResponse.Result.Select(r => new AFEData
            {
                AFENumber = r.GetValueOrDefault("u_afe_number", ""),
                Unit = r.GetValueOrDefault("u_unit", ""),
                Description = r.GetValueOrDefault("u_description", ""),
                ApprovedAmount = decimal.TryParse(r.GetValueOrDefault("u_approved_amount", "0"), out var approved) ? approved : 0,
                SpentAmount = decimal.TryParse(r.GetValueOrDefault("u_spent_amount", "0"), out var spent) ? spent : 0,
                RemainingAmount = decimal.TryParse(r.GetValueOrDefault("u_remaining_amount", "0"), out var remaining) ? remaining : 0,
                Status = r.GetValueOrDefault("u_status", ""),
                ApprovalDate = DateTime.TryParse(r.GetValueOrDefault("u_approval_date", ""), out var date) ? date : null,
                Requestor = r.GetValueOrDefault("u_requestor", "")
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching AFE data from ServiceNow");
            return new List<AFEData>();
        }
    }

    public async Task<AFEData?> GetAFEByNumberAsync(string afeNumber)
    {
        try
        {
            var url = $"{_baseUrl}/api/now/table/{_tableName}?sysparm_query=u_afe_number={afeNumber}&sysparm_limit=1";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var serviceNowResponse = JsonConvert.DeserializeObject<ServiceNowResponse>(content);

            if (serviceNowResponse?.Result == null || !serviceNowResponse.Result.Any())
                return null;

            var r = serviceNowResponse.Result.First();
            return new AFEData
            {
                AFENumber = r.GetValueOrDefault("u_afe_number", ""),
                Unit = r.GetValueOrDefault("u_unit", ""),
                Description = r.GetValueOrDefault("u_description", ""),
                ApprovedAmount = decimal.TryParse(r.GetValueOrDefault("u_approved_amount", "0"), out var approved) ? approved : 0,
                SpentAmount = decimal.TryParse(r.GetValueOrDefault("u_spent_amount", "0"), out var spent) ? spent : 0,
                RemainingAmount = decimal.TryParse(r.GetValueOrDefault("u_remaining_amount", "0"), out var remaining) ? remaining : 0,
                Status = r.GetValueOrDefault("u_status", ""),
                ApprovalDate = DateTime.TryParse(r.GetValueOrDefault("u_approval_date", ""), out var date) ? date : null,
                Requestor = r.GetValueOrDefault("u_requestor", "")
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching AFE by number from ServiceNow");
            return null;
        }
    }

    private class ServiceNowResponse
    {
        [JsonProperty("result")]
        public List<Dictionary<string, string>>? Result { get; set; }
    }
}

public static class DictionaryExtensions
{
    public static string GetValueOrDefault(this Dictionary<string, string> dict, string key, string defaultValue)
    {
        return dict.TryGetValue(key, out var value) ? value : defaultValue;
    }
}

