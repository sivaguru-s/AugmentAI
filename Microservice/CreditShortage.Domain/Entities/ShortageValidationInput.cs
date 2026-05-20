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
    public string? DefectCode { get; set; }
    public string? LocationCode { get; set; }
    public int? OrderNumber { get; set; }
    public int? OrderItemSeq { get; set; }
}
