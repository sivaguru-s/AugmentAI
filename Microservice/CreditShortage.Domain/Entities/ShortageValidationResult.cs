namespace CreditShortage.Domain.Entities;

/// <summary>
/// Domain entity representing the result of shortage validation
/// Maps to SQL stored procedure output: usp_CE_ValidateShortageItems
/// </summary>
public class ShortageValidationResult
{
    public string CustomerNumber { get; set; } = string.Empty;
    public string ShipToNumber { get; set; } = string.Empty;
    public int InvoiceNumber { get; set; }
    public string ItemNumber { get; set; } = string.Empty;
    public string SerialNumber { get; set; } = string.Empty;
    public int ShortageQuantity { get; set; }
    public decimal? Amount { get; set; }  // Credit amount for the shortage
    public string DefectCode { get; set; } = string.Empty;
    public string LocationCode { get; set; } = string.Empty;
    public string? OrderNumber { get; set; }  // Changed from int? to string? to match SP (VARCHAR(10))
    public int? OrderItemSeq { get; set; }
    public bool IsValid { get; set; }
    public string ValidationErrors { get; set; } = string.Empty;
    public int OrderedQuantity { get; set; }
    public decimal AlreadyCreditedQuantity { get; set; }
    public decimal RemainingCreditableQuantity { get; set; }
    public bool ItemExists { get; set; }
    public bool CustomerSerialItemValid { get; set; }
    public bool DefectCodeValid { get; set; }
    public bool LocationCodeValid { get; set; }
}
