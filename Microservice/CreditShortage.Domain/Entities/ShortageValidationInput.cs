namespace CreditShortage.Domain.Entities;

/// <summary>
/// Domain entity representing a shortage item to be validated
/// Maps to SQL Table Type: typCEShortageItemValidation
/// </summary>
public class ShortageValidationInput
{
    public string CustomerNumber { get; set; } = string.Empty;
    public string ShipToNumber { get; set; } = string.Empty;
    public int InvoiceNumber { get; set; }
    public string ItemNumber { get; set; } = string.Empty;
    public string SerialNumber { get; set; } = string.Empty;
    public int ShortageQuantity { get; set; }
    public decimal? Amount { get; set; }  // Credit amount for the shortage
    public string? DefectCode { get; set; }
    public string? LocationCode { get; set; }
    public string? OrderNumber { get; set; }  // Changed from int? to string? to match SP (VARCHAR(10))
    public int? OrderItemSeq { get; set; }
}
