namespace Chatbot_Onbase.Models;

public class InvoiceAnalytics
{
    public string VendorName { get; set; } = string.Empty;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int TotalInvoiceCount { get; set; }
    public decimal TotalCost { get; set; }
    public decimal AverageCostPerInvoice { get; set; }
    public decimal MinInvoiceAmount { get; set; }
    public decimal MaxInvoiceAmount { get; set; }
    public List<PricingTrend> PricingTrends { get; set; } = new();
    public List<Invoice> Invoices { get; set; } = new();
}

public class PricingTrend
{
    public string Period { get; set; } = string.Empty;
    public decimal AverageAmount { get; set; }
    public decimal PercentageChange { get; set; }
    public string TrendDirection { get; set; } = string.Empty; // "Increase", "Decrease", "Stable"
}

