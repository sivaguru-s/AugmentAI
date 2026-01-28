using EPay.Api.Models.DTOs;

namespace EPay.Api.Services
{
    /// <summary>
    /// Service interface for invoice business logic operations.
    /// Provides methods for searching, retrieving, and exporting invoice data.
    /// </summary>
    public interface IInvoiceService
    {
        /// <summary>
        /// Searches invoices with pagination and applies business logic validations.
        /// </summary>
        /// <param name="request">The search criteria including customer number, date range, and pagination.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
        /// <returns>
        /// An <see cref="InvoiceSearchResponse"/> containing the paged invoice list and metadata.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is null.</exception>
        /// <exception cref="EPay.Api.Exceptions.ValidationException">Thrown when request validation fails.</exception>
        Task<InvoiceSearchResponse> SearchInvoicesAsync(InvoiceSearchRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the default date span in days for invoice search from the database configuration.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
        /// <returns>
        /// The default date span in days. Returns 180 (6 months) if the stored procedure fails or returns null.
        /// </returns>
        Task<int> GetDefaultDateSpanInDaysAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Exports invoices to an Excel file based on the search criteria.
        /// </summary>
        /// <param name="request">The search criteria for the invoices to export.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
        /// <returns>
        /// A byte array containing the Excel file data in XLSX format.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is null.</exception>
        /// <exception cref="EPay.Api.Exceptions.ValidationException">Thrown when request validation fails.</exception>
        Task<byte[]> ExportToExcelAsync(InvoiceSearchRequest request, CancellationToken cancellationToken = default);
    }
}

