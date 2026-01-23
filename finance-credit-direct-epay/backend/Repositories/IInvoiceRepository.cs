using EPay.Backend.Models.DTOs;
using System.Data;

namespace EPay.Backend.Repositories
{
    /// <summary>
    /// Repository interface for invoice data access
    /// </summary>
    public interface IInvoiceRepository
    {
        /// <summary>
        /// Search invoices with pagination
        /// </summary>
        /// <param name="request">Search criteria</param>
        /// <returns>DataTable with invoice data and PageCount column</returns>
        Task<DataTable> SearchInvoicesAsync(InvoiceSearchRequest request);

        /// <summary>
        /// Get default date span in days for invoice search
        /// </summary>
        /// <returns>Number of days</returns>
        Task<int> GetDefaultDateSpanInDaysAsync();
    }
}

