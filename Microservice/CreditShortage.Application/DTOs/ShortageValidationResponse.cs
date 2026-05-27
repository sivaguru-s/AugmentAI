namespace CreditShortage.Application.DTOs;

/// <summary>
/// Response DTO for shortage item validation
/// </summary>
public class ShortageValidationResponse
{
    /// <summary>
    /// Customer number
    /// </summary>
    public string CustomerNumber { get; set; } = string.Empty;

    /// <summary>
    /// Ship-to number
    /// </summary>
    public string ShipToNumber { get; set; } = string.Empty;

    /// <summary>
    /// Invoice number
    /// </summary>
    public int InvoiceNumber { get; set; }

    /// <summary>
    /// Item number
    /// </summary>
    public string ItemNumber { get; set; } = string.Empty;

    /// <summary>
    /// Serial number
    /// </summary>
    public string SerialNumber { get; set; } = string.Empty;

    /// <summary>
    /// Requested shortage quantity
    /// </summary>
    public int ShortageQuantity { get; set; }

    /// <summary>
    /// Credit amount for the shortage
    /// </summary>
    public decimal? Amount { get; set; }

    /// <summary>
    /// Defect code (with defaults applied)
    /// </summary>
    public string DefectCode { get; set; } = string.Empty;

    /// <summary>
    /// Location code (with defaults applied)
    /// </summary>
    public string LocationCode { get; set; } = string.Empty;

    /// <summary>
    /// Order number (alphanumeric, max 10 characters)
    /// </summary>
    public string? OrderNumber { get; set; }

    /// <summary>
    /// Order item sequence
    /// </summary>
    public int? OrderItemSeq { get; set; }

    /// <summary>
    /// Indicates if all validations passed (true) or failed (false)
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// Detailed validation error messages or success message
    /// </summary>
    public string ValidationErrors { get; set; } = string.Empty;

    /// <summary>
    /// Original ordered quantity
    /// </summary>
    public int OrderedQuantity { get; set; }

    /// <summary>
    /// Quantity already credited in previous transactions
    /// </summary>
    public decimal AlreadyCreditedQuantity { get; set; }

    /// <summary>
    /// Remaining quantity available for credit
    /// </summary>
    public decimal RemainingCreditableQuantity { get; set; }

    /// <summary>
    /// Individual validation results
    /// </summary>
    public ValidationFlags Flags { get; set; } = new();

    /// <summary>
    /// Timestamp when validation was performed
    /// </summary>
    public DateTime ValidatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Indicates if serial number was obtained from IWS
    /// </summary>
    public bool SerialFromIWS { get; set; }
}

/// <summary>
/// Individual validation flags
/// </summary>
public class ValidationFlags
{
    /// <summary>
    /// Item exists in item master (true/false)
    /// </summary>
    public bool ItemExists { get; set; }

    /// <summary>
    /// Customer/Serial/Item combination is valid (true/false)
    /// </summary>
    public bool CustomerSerialItemValid { get; set; }

    /// <summary>
    /// Defect code is valid and active (true/false)
    /// </summary>
    public bool DefectCodeValid { get; set; }

    /// <summary>
    /// Location code is valid and active (true/false)
    /// </summary>
    public bool LocationCodeValid { get; set; }
}

/// <summary>
/// Batch validation response
/// </summary>
public class BatchShortageValidationResponse
{
    /// <summary>
    /// List of validation results (one per item)
    /// </summary>
    public List<ShortageValidationResponse> Results { get; set; } = new();

    /// <summary>
    /// Total number of items validated
    /// </summary>
    public int TotalItemsCount => Results.Count;

    /// <summary>
    /// Number of items that passed validation
    /// </summary>
    public int ValidItemsCount => Results.Count(r => r.IsValid);

    /// <summary>
    /// Number of items that failed validation
    /// </summary>
    public int InvalidItemsCount => Results.Count(r => !r.IsValid);

    /// <summary>
    /// Overall success flag (true if ALL items are valid)
    /// </summary>
    public bool AllValid => Results.All(r => r.IsValid);

    /// <summary>
    /// Timestamp when batch validation was performed
    /// </summary>
    public DateTime ValidatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Validation statistics
/// </summary>
public class ValidationStatistics
{
    public int TotalValidationsToday { get; set; }
    public int SuccessfulValidationsToday { get; set; }
    public int FailedValidationsToday { get; set; }
    public decimal SuccessRate { get; set; }
    public Dictionary<string, int> TopFailureReasons { get; set; } = new();
}
