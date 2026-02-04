namespace Epay.Blazor.Models;

/// <summary>
/// Response for invoice searches (mirrors backend)
/// </summary>
public class InvoiceSearchResponse
{
    public PagedResult<InvoiceDto> Result { get; set; } = new();
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public bool IsAnalyst { get; set; }
}
