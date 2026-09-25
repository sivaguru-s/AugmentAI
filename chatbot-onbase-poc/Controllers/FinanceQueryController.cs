using Microsoft.AspNetCore.Mvc;
using Chatbot_Onbase.Services;

namespace Chatbot_Onbase.Controllers;

/// <summary>
/// Exposes the approved Finance Query Services tool set from
/// OnBase_Chatbot_Business_Flow_v7.md (section 7) as read-only HTTP endpoints.
/// This is the boundary an AI Orchestrator would call - each endpoint maps
/// 1:1 to a logical tool name and returns a typed, structured result. No
/// endpoint here accepts or executes arbitrary SQL; all retrieval and
/// calculation happens in FinanceQueryService / FinanceQueryRepository
/// (OnBase DB Access Layer, read-only views/queries only).
/// </summary>
[ApiController]
[Route("api/financequery")]
public class FinanceQueryController : ControllerBase
{
    private readonly IFinanceQueryService _financeQueryService;
    private readonly ILogger<FinanceQueryController> _logger;

    public FinanceQueryController(IFinanceQueryService financeQueryService, ILogger<FinanceQueryController> logger)
    {
        _financeQueryService = financeQueryService;
        _logger = logger;
    }

    /// <summary>Tool: get_vendor_spend(vendor, period)</summary>
    [HttpGet("vendor-spend")]
    public async Task<IActionResult> GetVendorSpend(
        [FromQuery] string vendorCode, [FromQuery] DateTime periodStart, [FromQuery] DateTime periodEnd)
    {
        if (string.IsNullOrWhiteSpace(vendorCode))
            return BadRequest(new { error = "vendorCode is required" });

        var result = await _financeQueryService.GetVendorSpendAsync(vendorCode, periodStart, periodEnd);
        return Ok(result);
    }

    /// <summary>Tool: get_invoice_count_and_list(vendor?, period, limit?)</summary>
    [HttpGet("invoice-list")]
    public async Task<IActionResult> GetInvoiceCountAndList(
        [FromQuery] string? vendorCode, [FromQuery] DateTime periodStart, [FromQuery] DateTime periodEnd, [FromQuery] int limit = 100)
    {
        var result = await _financeQueryService.GetInvoiceCountAndListAsync(vendorCode, periodStart, periodEnd, limit);
        return Ok(result);
    }

    /// <summary>Tool: compare_vendor_spend(vendor_a, vendor_b, period)</summary>
    [HttpGet("compare-vendors")]
    public async Task<IActionResult> CompareVendorSpend(
        [FromQuery] string vendorCodeA, [FromQuery] string vendorCodeB,
        [FromQuery] DateTime periodStart, [FromQuery] DateTime periodEnd)
    {
        if (string.IsNullOrWhiteSpace(vendorCodeA) || string.IsNullOrWhiteSpace(vendorCodeB))
            return BadRequest(new { error = "vendorCodeA and vendorCodeB are required" });

        var result = await _financeQueryService.CompareVendorSpendAsync(vendorCodeA, vendorCodeB, periodStart, periodEnd);
        return Ok(result);
    }

    /// <summary>Tool: get_vendor_trend(vendor, year)</summary>
    [HttpGet("vendor-trend")]
    public async Task<IActionResult> GetVendorTrend([FromQuery] string vendorCode, [FromQuery] int year)
    {
        if (string.IsNullOrWhiteSpace(vendorCode))
            return BadRequest(new { error = "vendorCode is required" });

        var result = await _financeQueryService.GetVendorTrendAsync(vendorCode, year);
        return Ok(result);
    }

    /// <summary>Tool: get_top_invoices(period, n)</summary>
    [HttpGet("top-invoices")]
    public async Task<IActionResult> GetTopInvoices(
        [FromQuery] DateTime periodStart, [FromQuery] DateTime periodEnd, [FromQuery] int topN = 10)
    {
        var result = await _financeQueryService.GetTopInvoicesAsync(periodStart, periodEnd, topN);
        return Ok(result);
    }

    /// <summary>Tool: find_tax_anomalies(period, vendor?) - not supported on current schema.</summary>
    [HttpGet("tax-anomalies")]
    public IActionResult FindTaxAnomalies()
    {
        return Ok(_financeQueryService.FindTaxAnomalies());
    }

    /// <summary>Tool: get_recon_summary(period) - not supported on current schema.</summary>
    [HttpGet("recon-summary")]
    public IActionResult GetReconciliationSummary()
    {
        return Ok(_financeQueryService.GetReconciliationSummary());
    }

    /// <summary>Tool: get_recon_details(recon_id) - not supported on current schema.</summary>
    [HttpGet("recon-details")]
    public IActionResult GetReconciliationDetails()
    {
        return Ok(_financeQueryService.GetReconciliationDetails());
    }
}
