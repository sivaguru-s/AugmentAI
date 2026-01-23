namespace EPay.Backend.Models.DTOs
{
    /// <summary>
    /// Response DTO for invoice search operations
    /// </summary>
    public class InvoiceSearchResponse
    {
        /// <summary>
        /// Paged result of invoices
        /// </summary>
        public PagedResult<InvoiceDto> Result { get; set; } = new PagedResult<InvoiceDto>();

        /// <summary>
        /// From date used in search
        /// </summary>
        public DateTime FromDate { get; set; }

        /// <summary>
        /// To date used in search
        /// </summary>
        public DateTime ToDate { get; set; }

        /// <summary>
        /// Whether user has EPAYANLYST authorization
        /// </summary>
        public bool IsAnalyst { get; set; }
    }
}

