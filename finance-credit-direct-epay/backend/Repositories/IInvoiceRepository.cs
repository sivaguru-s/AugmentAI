using EPay.Api.Models.DTOs;
using System.Data;

namespace EPay.Api.Repositories
{
    /// <summary>
    /// Repository interface for invoice data access operations.
    /// Provides methods for searching invoices and retrieving configuration.
    /// </summary>
    public interface IInvoiceRepository
    {
        /// <summary>
        /// Searches invoices using the stored procedure usp_OrderAndInvoiceReportingOpenInvoices3.
        /// </summary>
        /// <param name="request">The search criteria including customer number, date range, and pagination.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
        /// <returns>
        /// A <see cref="DataTable"/> containing invoice data with columns matching the stored procedure output.
        /// Includes a PageCount column for pagination support.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is null.</exception>
        /// <exception cref="EPay.Api.Exceptions.ValidationException">Thrown when request validation fails.</exception>
        /// <exception cref="EPay.Api.Exceptions.DataAccessException">Thrown when database operation fails.</exception>
        Task<DataTable> SearchInvoicesAsync(InvoiceSearchRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the default date span in days for invoice search from the database configuration.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
        /// <returns>
        /// The default date span in days. Returns 180 (6 months) if the stored procedure fails or returns null.
        /// </returns>
        Task<int> GetDefaultDateSpanInDaysAsync(CancellationToken cancellationToken = default);
    }
}

