using EPay.Backend.Models.DTOs;

namespace EPay.Backend.Services
{
    /// <summary>
    /// Service interface for invoice business logic
    /// </summary>
    public interface IInvoiceService
    {
        /// <summary>
        /// Search invoices with pagination and business logic
        /// </summary>
        /// <param name="request">Search criteria</param>
        /// <returns>Paged invoice search response</returns>
        Task<InvoiceSearchResponse> SearchInvoicesAsync(InvoiceSearchRequest request);

        /// <summary>
        /// Get default date span in days for invoice search
        /// </summary>
        /// <returns>Number of days</returns>
        Task<int> GetDefaultDateSpanInDaysAsync();

        /// <summary>
        /// Export invoices to Excel
        /// </summary>
        /// <param name="request">Search criteria</param>
        /// <returns>Excel file as byte array</returns>
        Task<byte[]> ExportToExcelAsync(InvoiceSearchRequest request);
    }
}

