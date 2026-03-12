using Chatbot_Onbase.Data;
using Chatbot_Onbase.Models;

namespace Chatbot_Onbase.Services;

public interface IChatbotService
{
    Task<ChatResponse> ProcessQueryAsync(string userPrompt);
}

public class ChatbotService : IChatbotService
{
    private readonly IOnbaseRepository _repository;
    private readonly IInvoiceQueryParser _queryParser;
    private readonly ILogger<ChatbotService> _logger;

    public ChatbotService(
        IOnbaseRepository repository,
        IInvoiceQueryParser queryParser,
        ILogger<ChatbotService> logger)
    {
        _repository = repository;
        _queryParser = queryParser;
        _logger = logger;
    }

    public async Task<ChatResponse> ProcessQueryAsync(string userPrompt)
    {
        try
        {
            _logger.LogInformation("Processing query: {Query}", userPrompt);

            // Parse the user's intent
            var intent = _queryParser.ParseQuery(userPrompt);
            
            _logger.LogInformation("Detected intent: {Intent}", intent.IntentType);

            // Execute the appropriate query based on intent
            List<Invoice> invoices = intent.IntentType switch
            {
                "SearchByVendor" => await _repository.GetInvoicesByVendorAsync(
                    intent.Parameters.GetValueOrDefault("vendorName")?.ToString() ?? string.Empty),

                "SearchByInvoiceNumber" => await _repository.GetInvoicesByNumberAsync(
                    intent.Parameters.GetValueOrDefault("invoiceNumber")?.ToString() ?? string.Empty),

                "SearchByPO" => await _repository.SearchInvoicesAsync(
                    intent.Parameters.GetValueOrDefault("poNumber")?.ToString() ?? string.Empty),

                "SearchByStatus" => await _repository.GetInvoicesByStatusAsync(
                    intent.Parameters.GetValueOrDefault("status")?.ToString() ?? string.Empty),

                "SearchByAmount" => await _repository.GetInvoicesByAmountRangeAsync(
                    Convert.ToDecimal(intent.Parameters.GetValueOrDefault("minAmount") ?? 0m),
                    Convert.ToDecimal(intent.Parameters.GetValueOrDefault("maxAmount") ?? decimal.MaxValue)),

                "SearchByDate" => await _repository.GetInvoicesByDateRangeAsync(
                    Convert.ToDateTime(intent.Parameters.GetValueOrDefault("startDate") ?? DateTime.Today.AddDays(-30)),
                    Convert.ToDateTime(intent.Parameters.GetValueOrDefault("endDate") ?? DateTime.Today)),

                "GeneralSearch" => await _repository.SearchInvoicesAsync(
                    intent.Parameters.GetValueOrDefault("searchTerm")?.ToString() ?? userPrompt),

                _ => await _repository.GetAllInvoicesAsync(50)
            };

            // Apply pagination if specified
            var skip = Convert.ToInt32(intent.Parameters.GetValueOrDefault("skip") ?? 0);
            var take = Convert.ToInt32(intent.Parameters.GetValueOrDefault("take") ?? 0);
            var totalCount = invoices.Count;

            if (skip > 0)
            {
                invoices = invoices.Skip(skip).ToList();
            }

            if (take > 0)
            {
                invoices = invoices.Take(take).ToList();
            }

            // Generate a natural language response
            var answer = GenerateResponse(intent, invoices, totalCount, skip, take);

            return new ChatResponse
            {
                Answer = answer,
                Invoices = invoices,
                Query = userPrompt,
                ResultCount = invoices.Count
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing query: {Query}", userPrompt);
            return new ChatResponse
            {
                Answer = $"I encountered an error while searching for invoices: {ex.Message}",
                Invoices = new List<Invoice>(),
                Query = userPrompt,
                ResultCount = 0
            };
        }
    }

    private string GenerateResponse(QueryIntent intent, List<Invoice> invoices, int totalCount = 0, int skip = 0, int take = 0)
    {
        if (invoices.Count == 0)
        {
            return intent.IntentType switch
            {
                "SearchByVendor" => $"I couldn't find any invoices for vendor '{intent.Parameters.GetValueOrDefault("vendorName")}'.",
                "SearchByInvoiceNumber" => $"I couldn't find invoice number '{intent.Parameters.GetValueOrDefault("invoiceNumber")}'.",
                "SearchByStatus" => $"I couldn't find any invoices with status '{intent.Parameters.GetValueOrDefault("status")}'.",
                _ => "I couldn't find any invoices matching your search criteria."
            };
        }

        var summary = new System.Text.StringBuilder();

        // If pagination was applied, show different message
        if (totalCount > 0 && (skip > 0 || take > 0))
        {
            var startIndex = skip + 1;
            var endIndex = skip + invoices.Count;
            summary.AppendLine($"I found {totalCount} invoice(s) matching your query. Showing {startIndex} to {endIndex}:");
        }
        else
        {
            summary.AppendLine($"I found {invoices.Count} invoice(s) matching your query:");
        }

        summary.AppendLine();

        // Provide a summary based on intent type
        switch (intent.IntentType)
        {
            case "SearchByVendor":
                var totalAmount = invoices.Sum(i => i.Amount ?? 0);
                summary.AppendLine($"Total amount: ${totalAmount:N2}");
                summary.AppendLine($"Vendor: {invoices.First().VendorName}");
                break;

            case "SearchByStatus":
                var statusCount = invoices.GroupBy(i => i.Status).Select(g => $"{g.Key}: {g.Count()}");
                summary.AppendLine($"Status breakdown: {string.Join(", ", statusCount)}");
                break;

            case "SearchByAmount":
                var avgAmount = invoices.Average(i => i.Amount ?? 0);
                summary.AppendLine($"Average amount: ${avgAmount:N2}");
                summary.AppendLine($"Total amount: ${invoices.Sum(i => i.Amount ?? 0):N2}");
                break;

            case "SearchByDate":
                var dateRange = $"{invoices.Min(i => i.InvoiceDate):d} to {invoices.Max(i => i.InvoiceDate):d}";
                summary.AppendLine($"Date range: {dateRange}");
                break;
        }

        summary.AppendLine();
        summary.AppendLine("Here are the details:");

        return summary.ToString();
    }
}

