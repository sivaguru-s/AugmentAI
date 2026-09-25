namespace Chatbot_Onbase.Models;

// =====================================================================
// Structured result contracts for the "Finance Query Services / Tool Set"
// defined in OnBase_Chatbot_Business_Flow_v7.md (section 7).
// Each contract carries display values + validation/source metadata so a
// future Trusted Results Gateway can validate before an LLM formats text.
// =====================================================================

/// <summary>get_vendor_spend tool result.</summary>
public class VendorSpendResult
{
    public string VendorCode { get; set; } = string.Empty;
    public int InvoiceCount { get; set; }
    public decimal TotalSpend { get; set; }
    public decimal AverageInvoiceAmount { get; set; }
    public DateTime? EarliestInvoiceDate { get; set; }
    public DateTime? LatestInvoiceDate { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public string Currency { get; set; } = "USD";
    public string SourceReference { get; set; } = "hsi.itemdata + hsi.keyitem293 (itemtypenum 743,914)";
}

/// <summary>get_invoice_count_and_list tool result.</summary>
public class InvoiceListResult
{
    public int TotalCount { get; set; }
    public List<Invoice> Invoices { get; set; } = new();
    public Dictionary<string, object?> AppliedFilters { get; set; } = new();
    public string SourceReference { get; set; } = "hsi.itemdata + hsi.keyitem293 (itemtypenum 743,914)";
}

/// <summary>compare_vendor_spend tool result.</summary>
public class VendorComparisonResult
{
    public VendorSpendResult VendorA { get; set; } = new();
    public VendorSpendResult VendorB { get; set; } = new();
    public decimal SpendDelta { get; set; }
    public decimal? PercentageVariance { get; set; }
    public string SourceReference { get; set; } = "hsi.itemdata + hsi.keyitem293 (itemtypenum 743,914)";
}

/// <summary>Single month row for get_vendor_trend.</summary>
public class VendorTrendPoint
{
    public int Year { get; set; }
    public int Month { get; set; }
    public int InvoiceCount { get; set; }
    public decimal TotalSpend { get; set; }
}

/// <summary>get_vendor_trend tool result.</summary>
public class VendorTrendResult
{
    public string VendorCode { get; set; } = string.Empty;
    public List<VendorTrendPoint> TrendPoints { get; set; } = new();
    public string SourceReference { get; set; } = "hsi.itemdata + hsi.keyitem293 (itemtypenum 743,914)";
}

/// <summary>Single ranked invoice row for get_top_invoices.</summary>
public class TopInvoiceItem
{
    public long InvoiceItemNum { get; set; }
    public string? VendorCode { get; set; }
    public decimal Amount { get; set; }
    public DateTime? InvoiceDate { get; set; }
    public bool PossibleDuplicateFlag { get; set; }
}

/// <summary>get_top_invoices tool result.</summary>
public class TopInvoicesResult
{
    public List<TopInvoiceItem> Invoices { get; set; } = new();
    public string SourceReference { get; set; } = "hsi.itemdata + hsi.keyitem293 (itemtypenum 743,914)";
}

/// <summary>
/// Returned by tools that have no governed data source on this schema
/// (find_tax_anomalies, get_recon_summary, get_recon_details). Per section 8
/// "Zero-result handling" / governance rules, the backend must state that no
/// governed result exists rather than let the LLM infer or fabricate one.
/// </summary>
public class UnsupportedToolResult
{
    public bool IsSupported { get; set; } = false;
    public string Reason { get; set; } = string.Empty;
    public string Recommendation { get; set; } = string.Empty;
}
