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
    private readonly IInvoiceAnalyticsService _analyticsService;
    private readonly ILogger<ChatbotService> _logger;

    public ChatbotService(
        IOnbaseRepository repository,
        IInvoiceQueryParser queryParser,
        IInvoiceAnalyticsService analyticsService,
        ILogger<ChatbotService> logger)
    {
        _repository = repository;
        _queryParser = queryParser;
        _analyticsService = analyticsService;
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

            // Get document type filter
            var documentType = intent.Parameters.GetValueOrDefault("documentType")?.ToString() ?? "all";
            _logger.LogInformation("Document type filter: {DocumentType}", documentType);

            // Execute the appropriate query based on intent
            List<Invoice> invoices = intent.IntentType switch
            {
                "SearchByVendor" or "AnalyzeByVendor" => await _repository.GetInvoicesByVendorAsync(
                    intent.Parameters.GetValueOrDefault("vendorName")?.ToString() ?? string.Empty),

                "SearchByInvoiceNumber" => await _repository.GetInvoicesByNumberAsync(
                    intent.Parameters.GetValueOrDefault("invoiceNumber")?.ToString() ?? string.Empty,
                    documentType),

                "SearchByPO" => await _repository.SearchInvoicesAsync(
                    intent.Parameters.GetValueOrDefault("poNumber")?.ToString() ?? string.Empty,
                    documentType),

                "SearchByStatus" => await _repository.GetInvoicesByStatusAsync(
                    intent.Parameters.GetValueOrDefault("status")?.ToString() ?? string.Empty),

                "SearchByAmount" => await _repository.GetInvoicesByAmountRangeAsync(
                    Convert.ToDecimal(intent.Parameters.GetValueOrDefault("minAmount") ?? 0m),
                    Convert.ToDecimal(intent.Parameters.GetValueOrDefault("maxAmount") ?? decimal.MaxValue),
                    documentType),

                "SearchByDate" => await _repository.GetInvoicesByDateRangeAsync(
                    Convert.ToDateTime(intent.Parameters.GetValueOrDefault("startDate") ?? DateTime.Today.AddDays(-30)),
                    Convert.ToDateTime(intent.Parameters.GetValueOrDefault("endDate") ?? DateTime.Today),
                    documentType),

                "GeneralSearch" => await _repository.SearchInvoicesAsync(
                    intent.Parameters.GetValueOrDefault("searchTerm")?.ToString() ?? userPrompt,
                    documentType),

                _ => await _repository.GetAllInvoicesAsync(50, documentType)
            };

            // Filter by date range if specified (for analytics queries)
            if (intent.Parameters.ContainsKey("startDate") && intent.Parameters.ContainsKey("endDate"))
            {
                var startDate = Convert.ToDateTime(intent.Parameters["startDate"]);
                var endDate = Convert.ToDateTime(intent.Parameters["endDate"]);
                invoices = invoices.Where(i => i.InvoiceDate.HasValue &&
                    i.InvoiceDate.Value >= startDate &&
                    i.InvoiceDate.Value <= endDate).ToList();
            }

            // Check if this is an analytics query
            var isAnalytics = Convert.ToBoolean(intent.Parameters.GetValueOrDefault("isAnalytics") ?? false);

            // Apply pagination if specified (not for analytics)
            var skip = Convert.ToInt32(intent.Parameters.GetValueOrDefault("skip") ?? 0);
            var take = Convert.ToInt32(intent.Parameters.GetValueOrDefault("take") ?? 0);
            var totalCount = invoices.Count;

            List<Invoice> paginatedInvoices = invoices;
            if (!isAnalytics)
            {
                if (skip > 0)
                {
                    paginatedInvoices = paginatedInvoices.Skip(skip).ToList();
                }

                if (take > 0)
                {
                    paginatedInvoices = paginatedInvoices.Take(take).ToList();
                }
            }

            // Generate a natural language response
            string answer;
            if (isAnalytics)
            {
                // Perform analytics
                var vendorName = intent.Parameters.GetValueOrDefault("vendorName")?.ToString() ?? "All Vendors";
                var startDate = intent.Parameters.ContainsKey("startDate")
                    ? Convert.ToDateTime(intent.Parameters["startDate"])
                    : (DateTime?)null;
                var endDate = intent.Parameters.ContainsKey("endDate")
                    ? Convert.ToDateTime(intent.Parameters["endDate"])
                    : (DateTime?)null;

                var analytics = _analyticsService.AnalyzeInvoices(invoices, vendorName, startDate, endDate);
                answer = GenerateAnalyticsResponse(analytics);
            }
            else
            {
                answer = GenerateResponse(intent, paginatedInvoices, totalCount, skip, take);
            }

            return new ChatResponse
            {
                Answer = answer,
                Invoices = isAnalytics ? invoices : paginatedInvoices,
                Query = userPrompt,
                ResultCount = totalCount
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
                if (totalAmount > 0 || invoices.Any(i => i.Amount.HasValue))
                {
                    summary.AppendLine($"Total amount: ${totalAmount:N2}");
                }
                else
                {
                    summary.AppendLine("⚠️ Note: Amount data is not available for these invoices.");
                }
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

    private string GenerateAnalyticsResponse(InvoiceAnalytics analytics)
    {
        var summary = new System.Text.StringBuilder();

        summary.AppendLine($"📊 **Invoice Analytics Report for {analytics.VendorName}**");
        summary.AppendLine();

        if (analytics.StartDate.HasValue && analytics.EndDate.HasValue)
        {
            summary.AppendLine($"📅 **Period:** {analytics.StartDate.Value:MMMM yyyy} ({analytics.StartDate.Value:yyyy-MM-dd} to {analytics.EndDate.Value:yyyy-MM-dd})");
        }
        summary.AppendLine();

        summary.AppendLine($"📄 **Total Invoices:** {analytics.TotalInvoiceCount}");
        summary.AppendLine();

        if (analytics.TotalCost > 0)
        {
            summary.AppendLine("💰 **Cost Summary:**");
            summary.AppendLine($"   - Total Cost: ${analytics.TotalCost:N2}");
            summary.AppendLine($"   - Average Cost per Invoice: ${analytics.AverageCostPerInvoice:N2}");
            summary.AppendLine($"   - Minimum Invoice Amount: ${analytics.MinInvoiceAmount:N2}");
            summary.AppendLine($"   - Maximum Invoice Amount: ${analytics.MaxInvoiceAmount:N2}");
            summary.AppendLine();

            if (analytics.PricingTrends.Any())
            {
                summary.AppendLine("📈 **Pricing Trends:**");
                foreach (var trend in analytics.PricingTrends)
                {
                    var trendIcon = trend.TrendDirection switch
                    {
                        "Increase" => "⬆️",
                        "Decrease" => "⬇️",
                        "Stable" => "➡️",
                        _ => "📍"
                    };

                    summary.AppendLine($"   {trendIcon} {trend.Period}: Avg ${trend.AverageAmount:N2} ({trend.PercentageChange:+0.00;-0.00;0}%)");
                }
                summary.AppendLine();

                // Identify significant pricing changes
                var significantIncreases = analytics.PricingTrends
                    .Where(t => t.TrendDirection == "Increase" && t.PercentageChange > 10)
                    .ToList();

                if (significantIncreases.Any())
                {
                    summary.AppendLine("⚠️ **Significant Rate Increases Detected:**");
                    foreach (var increase in significantIncreases)
                    {
                        summary.AppendLine($"   - {increase.Period}: +{increase.PercentageChange:0.00}% increase");
                    }
                    summary.AppendLine();
                }
            }
        }
        else
        {
            summary.AppendLine("⚠️ **Note:** Amount data is not available in the database.");
            summary.AppendLine("The Amount field needs to be mapped to the correct Onbase keyitem table.");
            summary.AppendLine($"However, I found {analytics.TotalInvoiceCount} invoices matching your criteria.");
            summary.AppendLine();
        }

        summary.AppendLine("✅ **Analysis Complete**");

        return summary.ToString();
    }
}

