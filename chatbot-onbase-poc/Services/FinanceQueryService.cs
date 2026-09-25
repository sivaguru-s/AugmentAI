using Chatbot_Onbase.Data;
using Chatbot_Onbase.Models;

namespace Chatbot_Onbase.Services;

/// <summary>
/// "Finance Query Services" + "Analytics + Rules Layer" from
/// OnBase_Chatbot_Business_Flow_v7.md (sections 4, 5, 7). Exposes the approved,
/// typed tool set. All counts, totals, deltas, and percentages are computed
/// here deterministically - never by an LLM - per the "deterministic
/// calculation boundary" governance control (section 8).
/// Backed exclusively by IFinanceQueryRepository (the read-only OnBase DB
/// Access Layer); no direct SQL, no write access.
/// </summary>
public interface IFinanceQueryService
{
    Task<VendorSpendResult> GetVendorSpendAsync(string vendorCode, DateTime periodStart, DateTime periodEnd);
    Task<InvoiceListResult> GetInvoiceCountAndListAsync(string? vendorCode, DateTime periodStart, DateTime periodEnd, int limit);
    Task<VendorComparisonResult> CompareVendorSpendAsync(string vendorCodeA, string vendorCodeB, DateTime periodStart, DateTime periodEnd);
    Task<VendorTrendResult> GetVendorTrendAsync(string vendorCode, int year);
    Task<TopInvoicesResult> GetTopInvoicesAsync(DateTime periodStart, DateTime periodEnd, int topN);
    UnsupportedToolResult FindTaxAnomalies();
    UnsupportedToolResult GetReconciliationSummary();
    UnsupportedToolResult GetReconciliationDetails();
}

public class FinanceQueryService : IFinanceQueryService
{
    private readonly IFinanceQueryRepository _repository;
    private readonly ILogger<FinanceQueryService> _logger;

    public FinanceQueryService(IFinanceQueryRepository repository, ILogger<FinanceQueryService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public Task<VendorSpendResult> GetVendorSpendAsync(string vendorCode, DateTime periodStart, DateTime periodEnd)
    {
        _logger.LogInformation("get_vendor_spend: vendor={Vendor} period={Start:d}-{End:d}", vendorCode, periodStart, periodEnd);
        return _repository.GetVendorSpendAsync(vendorCode, periodStart, periodEnd);
    }

    public Task<InvoiceListResult> GetInvoiceCountAndListAsync(string? vendorCode, DateTime periodStart, DateTime periodEnd, int limit)
    {
        _logger.LogInformation("get_invoice_count_and_list: vendor={Vendor} period={Start:d}-{End:d} limit={Limit}",
            vendorCode, periodStart, periodEnd, limit);
        return _repository.GetInvoiceListAsync(vendorCode, periodStart, periodEnd, limit);
    }

    public async Task<VendorComparisonResult> CompareVendorSpendAsync(string vendorCodeA, string vendorCodeB, DateTime periodStart, DateTime periodEnd)
    {
        _logger.LogInformation("compare_vendor_spend: {A} vs {B} period={Start:d}-{End:d}", vendorCodeA, vendorCodeB, periodStart, periodEnd);

        // Backend computes both sides via the same deterministic tool, then the delta - never the LLM.
        var spendA = await _repository.GetVendorSpendAsync(vendorCodeA, periodStart, periodEnd);
        var spendB = await _repository.GetVendorSpendAsync(vendorCodeB, periodStart, periodEnd);

        var delta = spendA.TotalSpend - spendB.TotalSpend;
        decimal? pctVariance = spendB.TotalSpend == 0 ? null : Math.Round((delta / spendB.TotalSpend) * 100m, 2);

        return new VendorComparisonResult
        {
            VendorA = spendA,
            VendorB = spendB,
            SpendDelta = delta,
            PercentageVariance = pctVariance
        };
    }

    public Task<VendorTrendResult> GetVendorTrendAsync(string vendorCode, int year)
    {
        var yearStart = new DateTime(year, 1, 1);
        var yearEnd = new DateTime(year, 12, 31);
        _logger.LogInformation("get_vendor_trend: vendor={Vendor} year={Year}", vendorCode, year);
        return _repository.GetVendorTrendAsync(vendorCode, yearStart, yearEnd);
    }

    public Task<TopInvoicesResult> GetTopInvoicesAsync(DateTime periodStart, DateTime periodEnd, int topN)
    {
        _logger.LogInformation("get_top_invoices: period={Start:d}-{End:d} topN={TopN}", periodStart, periodEnd, topN);
        return _repository.GetTopInvoicesAsync(periodStart, periodEnd, topN);
    }

    // ------------------------------------------------------------------
    // Tools with no governed data source on the current OnBase schema.
    // Per section 8 "Zero-result handling": the backend must state no
    // governed result is available rather than let the LLM infer one.
    // ------------------------------------------------------------------

    public UnsupportedToolResult FindTaxAnomalies() => new()
    {
        Reason = "No tax amount/rate field exists on AP invoice item types (743, 914, 263, 340, 364, 429, 515) in the OnBase schema.",
        Recommendation = "Requires AS400/ERP as the authoritative tax data source; needs SME confirmation before implementation."
    };

    public UnsupportedToolResult GetReconciliationSummary() => new()
    {
        Reason = "No reconciliation table/status field was found among the tables catalogued in Onbase_DB_Tables.md.",
        Recommendation = "Requires confirmation of the authoritative reconciliation system (likely AS400/ERP) before implementation."
    };

    public UnsupportedToolResult GetReconciliationDetails() => new()
    {
        Reason = "No reconciliation table/status field was found among the tables catalogued in Onbase_DB_Tables.md.",
        Recommendation = "Requires confirmation of the authoritative reconciliation system (likely AS400/ERP) before implementation."
    };
}
