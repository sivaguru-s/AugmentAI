using System.ComponentModel.DataAnnotations;

namespace CreditShortage.Application.DTOs;

/// <summary>
/// Request DTO for validating a shortage item
/// </summary>
public class ShortageItemRequest
{
    /// <summary>
    /// Customer number (8 characters)
    /// </summary>
    [Required]
    [StringLength(8, MinimumLength = 1)]
    public string CustomerNumber { get; set; } = string.Empty;

    /// <summary>
    /// Ship-to location number (4 characters)
    /// </summary>
    [Required]
    [StringLength(4, MinimumLength = 1)]
    public string ShipToNumber { get; set; } = string.Empty;

    /// <summary>
    /// Invoice number where shortage occurred
    /// </summary>
    [Required]
    [Range(1, 999999)]
    public int InvoiceNumber { get; set; }

    /// <summary>
    /// Item/SKU number (15 characters max)
    /// </summary>
    [Required]
    [StringLength(15, MinimumLength = 1)]
    public string ItemNumber { get; set; } = string.Empty;

    /// <summary>
    /// Serial number (obtained from IWS) (10 characters max)
    /// </summary>
    [Required]
    [StringLength(10, MinimumLength = 1)]
    public string SerialNumber { get; set; } = string.Empty;

    /// <summary>
    /// Quantity of items short
    /// </summary>
    [Required]
    [Range(1, 9999999)]
    public int ShortageQuantity { get; set; }

    /// <summary>
    /// Defect code (4 characters max). If null, defaults to 'XP'
    /// </summary>
    [StringLength(4)]
    public string? DefectCode { get; set; }

    /// <summary>
    /// Location/warehouse code (2 characters max). If null, defaults to 'WU'
    /// </summary>
    [StringLength(2)]
    public string? LocationCode { get; set; }

    /// <summary>
    /// Original order number (optional)
    /// </summary>
    [Range(1, 9999999)]
    public int? OrderNumber { get; set; }

    /// <summary>
    /// Order item sequence number (optional)
    /// </summary>
    public int? OrderItemSeq { get; set; }

    /// <summary>
    /// Environment code (AFI, WVF, etc.). If null, will be auto-detected
    /// </summary>
    [StringLength(3)]
    public string? Environment { get; set; }
}

/// <summary>
/// Request DTO for shortage item validation with IWS integration
/// Serial number will be obtained from IWS, so it's not required in the request
/// </summary>
public class ShortageItemIWSRequest
{
    /// <summary>
    /// Customer number (8 characters)
    /// </summary>
    [Required]
    [StringLength(8, MinimumLength = 1)]
    public string CustomerNumber { get; set; } = string.Empty;

    /// <summary>
    /// Ship-to location number (4 characters)
    /// </summary>
    [Required]
    [StringLength(4, MinimumLength = 1)]
    public string ShipToNumber { get; set; } = string.Empty;

    /// <summary>
    /// Invoice number where shortage occurred
    /// </summary>
    [Required]
    [Range(1, 999999)]
    public int InvoiceNumber { get; set; }

    /// <summary>
    /// Item/SKU number (15 characters max)
    /// </summary>
    [Required]
    [StringLength(15, MinimumLength = 1)]
    public string ItemNumber { get; set; } = string.Empty;

    /// <summary>
    /// Quantity of items short
    /// </summary>
    [Required]
    [Range(1, 9999999)]
    public int ShortageQuantity { get; set; }

    /// <summary>
    /// Defect code (4 characters max). If null, defaults to 'XP'
    /// </summary>
    [StringLength(4)]
    public string? DefectCode { get; set; }

    /// <summary>
    /// Location/warehouse code (2 characters max). If null, defaults to 'WU'
    /// </summary>
    [StringLength(2)]
    public string? LocationCode { get; set; }

    /// <summary>
    /// Original order number (optional)
    /// </summary>
    [Range(1, 9999999)]
    public int? OrderNumber { get; set; }

    /// <summary>
    /// Order item sequence number (optional)
    /// </summary>
    public int? OrderItemSeq { get; set; }

    /// <summary>
    /// Environment code (AFI, WVF, etc.). If null, will be auto-detected
    /// </summary>
    [StringLength(3)]
    public string? Environment { get; set; }
}

/// <summary>
/// Batch validation request
/// </summary>
public class BatchShortageValidationRequest
{
    /// <summary>
    /// List of shortage items to validate
    /// </summary>
    [Required]
    [MinLength(1)]
    [MaxLength(500)]
    public List<ShortageItemRequest> Items { get; set; } = new();

    /// <summary>
    /// Environment code for all items (optional, can be overridden per item)
    /// </summary>
    [StringLength(3)]
    public string? Environment { get; set; }
}
