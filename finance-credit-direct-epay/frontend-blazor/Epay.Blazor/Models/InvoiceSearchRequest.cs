using System.ComponentModel.DataAnnotations;

namespace Epay.Blazor.Models;

public class InvoiceSearchRequest
{
    [Required, StringLength(10)]
    public string CustomerNumber { get; set; } = string.Empty;

    [StringLength(4)]
    public string ShipToNumber { get; set; } = string.Empty;

    public bool AllShipTos { get; set; }

    [StringLength(25)]
    public string SecurityMHS { get; set; } = string.Empty;

    [StringLength(9)]
    public string? InvoiceNumber { get; set; }

    [StringLength(15)]
    public string? CreditNumber { get; set; }

    [StringLength(25)]
    public string? PONumber { get; set; }

    [Required]
    public DateTime FromDate { get; set; }

    [Required]
    public DateTime ToDate { get; set; }

    [StringLength(20)]
    public string? SortColumn { get; set; }

    public bool SortAscending { get; set; } = true;

    [Range(0, int.MaxValue)]
    public int PageNumber { get; set; } = 1;

    [Range(0, 1000)]
    public int PageSize { get; set; } = 500;

    public bool ShowAll { get; set; } = false;
}

