using System.ComponentModel.DataAnnotations;

namespace EPay.Api.Models.DTOs
{
    /// <summary>
    /// Request DTO for invoice search operations
    /// </summary>
    public class InvoiceSearchRequest
    {
        /// <summary>
        /// Customer number (from session)
        /// </summary>
        [Required]
        [StringLength(10)]
        public string CustomerNumber { get; set; } = string.Empty;

        /// <summary>
        /// Ship-to number (from session)
        /// </summary>
        [StringLength(4)]
        public string ShipToNumber { get; set; } = string.Empty;

        /// <summary>
        /// Whether to search all ship-tos (from session)
        /// </summary>
        public bool AllShipTos { get; set; }

        /// <summary>
        /// Security MHS code (from session)
        /// </summary>
        [StringLength(25)]
        public string SecurityMHS { get; set; } = string.Empty;

        /// <summary>
        /// Invoice number search filter
        /// </summary>
        [StringLength(9)]
        public string? InvoiceNumber { get; set; }

        /// <summary>
        /// Credit number search filter
        /// </summary>
        [StringLength(15)]
        public string? CreditNumber { get; set; }

        /// <summary>
        /// PO number search filter
        /// </summary>
        [StringLength(25)]
        public string? PONumber { get; set; }

        /// <summary>
        /// Invoice date range start
        /// </summary>
        [Required]
        public DateTime FromDate { get; set; }

        /// <summary>
        /// Invoice date range end
        /// </summary>
        [Required]
        public DateTime ToDate { get; set; }

        /// <summary>
        /// Column to sort by
        /// </summary>
        [StringLength(20)]
        public string? SortColumn { get; set; }

        /// <summary>
        /// Sort direction (true = ascending, false = descending)
        /// </summary>
        public bool SortAscending { get; set; } = true;

        /// <summary>
        /// Page number (1-based)
        /// </summary>
        [Range(0, int.MaxValue)]
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// Page size (0 = all records)
        /// </summary>
        [Range(0, 1000)]
        public int PageSize { get; set; } = 500;

        /// <summary>
        /// Whether to show all invoices (no pagination)
        /// </summary>
        public bool ShowAll { get; set; } = false;
    }
}

