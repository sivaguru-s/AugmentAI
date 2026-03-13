using Chatbot_Onbase.Models;

namespace Chatbot_Onbase.Services;

public interface IInvoiceAnalyticsService
{
    InvoiceAnalytics AnalyzeInvoices(List<Invoice> invoices, string vendorName, DateTime? startDate, DateTime? endDate);
}

public class InvoiceAnalyticsService : IInvoiceAnalyticsService
{
    private readonly ILogger<InvoiceAnalyticsService> _logger;

    public InvoiceAnalyticsService(ILogger<InvoiceAnalyticsService> logger)
    {
        _logger = logger;
    }

    public InvoiceAnalytics AnalyzeInvoices(List<Invoice> invoices, string vendorName, DateTime? startDate, DateTime? endDate)
    {
        _logger.LogInformation($"Analyzing {invoices.Count} invoices for vendor: {vendorName}");

        var analytics = new InvoiceAnalytics
        {
            VendorName = vendorName,
            StartDate = startDate,
            EndDate = endDate,
            Invoices = invoices,
            TotalInvoiceCount = invoices.Count
        };

        if (invoices.Count == 0)
        {
            return analytics;
        }

        // Calculate cost statistics
        // Note: Amount field is currently NULL in the database
        // This will work once the Amount field is properly mapped
        var invoicesWithAmount = invoices.Where(i => i.Amount.HasValue && i.Amount.Value > 0).ToList();

        if (invoicesWithAmount.Any())
        {
            analytics.TotalCost = invoicesWithAmount.Sum(i => i.Amount!.Value);
            analytics.AverageCostPerInvoice = invoicesWithAmount.Average(i => i.Amount!.Value);
            analytics.MinInvoiceAmount = invoicesWithAmount.Min(i => i.Amount!.Value);
            analytics.MaxInvoiceAmount = invoicesWithAmount.Max(i => i.Amount!.Value);

            // Calculate pricing trends
            analytics.PricingTrends = CalculatePricingTrends(invoicesWithAmount);
        }
        else
        {
            _logger.LogWarning("No invoices with amount data found. Amount field may not be mapped in the database.");
            // Provide a message indicating amount data is not available
            analytics.TotalCost = 0;
            analytics.AverageCostPerInvoice = 0;
            analytics.MinInvoiceAmount = 0;
            analytics.MaxInvoiceAmount = 0;
        }

        return analytics;
    }

    private List<PricingTrend> CalculatePricingTrends(List<Invoice> invoices)
    {
        var trends = new List<PricingTrend>();

        // Group invoices by month
        var monthlyGroups = invoices
            .Where(i => i.InvoiceDate.HasValue)
            .GroupBy(i => new { Year = i.InvoiceDate!.Value.Year, Month = i.InvoiceDate!.Value.Month })
            .OrderBy(g => g.Key.Year)
            .ThenBy(g => g.Key.Month)
            .ToList();

        if (monthlyGroups.Count < 2)
        {
            // Not enough data for trend analysis
            return trends;
        }

        PricingTrend? previousTrend = null;

        foreach (var group in monthlyGroups)
        {
            var monthInvoices = group.Where(i => i.Amount.HasValue).ToList();
            if (!monthInvoices.Any()) continue;

            var avgAmount = monthInvoices.Average(i => i.Amount!.Value);
            var period = $"{group.Key.Year}-{group.Key.Month:D2}";

            var trend = new PricingTrend
            {
                Period = period,
                AverageAmount = avgAmount
            };

            if (previousTrend != null)
            {
                var change = avgAmount - previousTrend.AverageAmount;
                trend.PercentageChange = previousTrend.AverageAmount != 0
                    ? (change / previousTrend.AverageAmount) * 100
                    : 0;

                trend.TrendDirection = trend.PercentageChange > 5 ? "Increase"
                    : trend.PercentageChange < -5 ? "Decrease"
                    : "Stable";
            }
            else
            {
                trend.PercentageChange = 0;
                trend.TrendDirection = "Baseline";
            }

            trends.Add(trend);
            previousTrend = trend;
        }

        return trends;
    }
}

