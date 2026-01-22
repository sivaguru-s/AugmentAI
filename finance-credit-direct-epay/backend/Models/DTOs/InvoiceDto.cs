namespace EPay.Backend.Models.DTOs
{
    /// <summary>
    /// Invoice data transfer object
    /// </summary>
    public class InvoiceDto
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
        public string InvoiceNumber { get; set; } = string.Empty;

        /// <summary>
        /// Credit number
        /// </summary>
        public int CreditNumber { get; set; }

        /// <summary>
        /// Invoice date
        /// </summary>
        public DateTime InvoiceDate { get; set; }

        /// <summary>
        /// Invoice amount
        /// </summary>
        public decimal InvoiceAmount { get; set; }

        /// <summary>
        /// Amount paid
        /// </summary>
        public decimal AmountPaid { get; set; }

        /// <summary>
        /// Balance due
        /// </summary>
        public decimal Balance { get; set; }

        /// <summary>
        /// Category code
        /// </summary>
        public string CategoryCode { get; set; } = string.Empty;

        /// <summary>
        /// PO number
        /// </summary>
        public string PONumber { get; set; } = string.Empty;

        /// <summary>
        /// Order number
        /// </summary>
        public string OrderNumber { get; set; } = string.Empty;

        /// <summary>
        /// Order date
        /// </summary>
        public DateTime? OrderDate { get; set; }

        /// <summary>
        /// RPP number
        /// </summary>
        public string RPPNumber { get; set; } = string.Empty;

        /// <summary>
        /// Days since invoice date
        /// </summary>
        public int Days { get; set; }

        /// <summary>
        /// Trip number
        /// </summary>
        public int? TripNumber { get; set; }

        /// <summary>
        /// EPay status (Sent, Verifying, Paid, etc.)
        /// </summary>
        public string? EPayStatus { get; set; }

        /// <summary>
        /// EPay reference number
        /// </summary>
        public int? EPayReferenceNumber { get; set; }

        /// <summary>
        /// Whether to hide detail link
        /// </summary>
        public bool HideDetail { get; set; }

        /// <summary>
        /// Whether this invoice can be selected for payment
        /// </summary>
        public bool CanSelect { get; set; } = true;
    }
}

