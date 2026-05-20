using CreditShortage.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text.Json;

namespace CreditShortage.Infrastructure.ExternalServices;

/// <summary>
/// Implementation of IWS (Inventory Warehouse System) integration service
/// </summary>
public class IWSIntegrationService : IIWSIntegrationService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<IWSIntegrationService> _logger;
    private readonly string _apiKey;

    public IWSIntegrationService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<IWSIntegrationService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _apiKey = configuration["IWSIntegration:ApiKey"] 
            ?? throw new InvalidOperationException("IWS API Key not configured");

        var baseUrl = configuration["IWSIntegration:BaseUrl"] 
            ?? throw new InvalidOperationException("IWS Base URL not configured");

        _httpClient.BaseAddress = new Uri(baseUrl);
        _httpClient.DefaultRequestHeaders.Add("X-API-Key", _apiKey);
    }

    public async Task<string> GetSerialNumberAsync(string customerNumber, int invoiceNumber, string itemNumber)
    {
        _logger.LogInformation("Calling IWS to get serial number for Customer: {Customer}, Invoice: {Invoice}, Item: {Item}",
            customerNumber, invoiceNumber, itemNumber);

        try
        {
            // Construct the IWS API endpoint (adjust based on actual IWS API)
            var endpoint = $"/api/v1/orders/serial?customer={customerNumber}&invoice={invoiceNumber}&item={itemNumber}";

            var response = await _httpClient.GetAsync(endpoint);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("IWS API returned error status {StatusCode}: {Error}",
                    response.StatusCode, errorContent);
                throw new HttpRequestException($"IWS API error: {response.StatusCode}");
            }

            var result = await response.Content.ReadFromJsonAsync<IWSSerialNumberResponse>();

            if (result == null || string.IsNullOrWhiteSpace(result.SerialNumber))
            {
                throw new InvalidOperationException("IWS returned empty serial number");
            }

            _logger.LogInformation("Successfully obtained serial number {Serial} from IWS", result.SerialNumber);

            return result.SerialNumber;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error calling IWS API");
            throw new InvalidOperationException("Failed to communicate with IWS. " + ex.Message, ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error parsing IWS response");
            throw new InvalidOperationException("Invalid response from IWS. " + ex.Message, ex);
        }
    }

    public async Task<IWSOrderDetail> GetOrderDetailAsync(string customerNumber, int invoiceNumber)
    {
        _logger.LogInformation("Calling IWS to get order details for Customer: {Customer}, Invoice: {Invoice}",
            customerNumber, invoiceNumber);

        try
        {
            var endpoint = $"/api/v1/orders/details?customer={customerNumber}&invoice={invoiceNumber}";

            var response = await _httpClient.GetAsync(endpoint);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("IWS API returned error status {StatusCode}: {Error}",
                    response.StatusCode, errorContent);
                throw new HttpRequestException($"IWS API error: {response.StatusCode}");
            }

            var result = await response.Content.ReadFromJsonAsync<IWSOrderDetailResponse>();

            if (result == null)
            {
                throw new InvalidOperationException("IWS returned empty order details");
            }

            return new IWSOrderDetail
            {
                SerialNumber = result.SerialNumber,
                OrderNumber = result.OrderNumber,
                ItemNumber = result.ItemNumber,
                OrderedQuantity = result.OrderedQuantity,
                OrderDate = result.OrderDate
            };
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error calling IWS API");
            throw new InvalidOperationException("Failed to communicate with IWS. " + ex.Message, ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error parsing IWS response");
            throw new InvalidOperationException("Invalid response from IWS. " + ex.Message, ex);
        }
    }

    // Internal DTOs for IWS API responses
    private class IWSSerialNumberResponse
    {
        public string SerialNumber { get; set; } = string.Empty;
    }

    private class IWSOrderDetailResponse
    {
        public string SerialNumber { get; set; } = string.Empty;
        public int OrderNumber { get; set; }
        public string ItemNumber { get; set; } = string.Empty;
        public int OrderedQuantity { get; set; }
        public DateTime OrderDate { get; set; }
    }
}
