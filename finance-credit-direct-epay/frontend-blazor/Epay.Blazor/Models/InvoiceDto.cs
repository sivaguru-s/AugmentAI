namespace Epay.Blazor.Models;

/// <summary>
/// Invoice data transfer object (mirrors backend)
/// </summary>
public class InvoiceDto
{
    public string CustomerNumber { get; set; } = string.Empty;
    public string ShipToNumber { get; set; } = string.Empty;
    public string InvoiceNumber { get; set; } = string.Empty;
    public int CreditNumber { get; set; }
    public DateTime InvoiceDate { get; set; }
    public decimal InvoiceAmount { get; set; }
    public decimal AmountPaid { get; set; }
    public decimal Balance { get; set; }
    public string CategoryCode { get; set; } = string.Empty;
    public string PONumber { get; set; } = string.Empty;
    public string OrderNumber { get; set; } = string.Empty;
    public DateTime? OrderDate { get; set; }
    public string RPPNumber { get; set; } = string.Empty;
    public int Days { get; set; }
    public int? TripNumber { get; set; }
    public string? EPayStatus { get; set; }
    public int? EPayReferenceNumber { get; set; }
    public bool HideDetail { get; set; }
    public bool CanSelect { get; set; } = true;
}
