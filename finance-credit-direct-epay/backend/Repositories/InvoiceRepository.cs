using EPay.Api.Exceptions;
using EPay.Api.Models.DTOs;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace EPay.Api.Repositories
{
    /// <summary>
    /// Repository implementation for invoice data access operations.
    /// Communicates with the database using stored procedures for invoice queries.
    /// </summary>
    /// <remarks>
    /// This repository uses the following stored procedures:
    /// <list type="bullet">
    /// <item>Datawhse.dbo.usp_OrderAndInvoiceReportingOpenInvoices3 - Main invoice search</item>
    /// <item>Ashley.dbo.usp_GetEpayDefaultDateSpanInDays - Get default date span configuration</item>
    /// </list>
    /// </remarks>
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<InvoiceRepository> _logger;

        /// <summary>
        /// Default date span in days - 6 months (approximately 180 days).
        /// Used when the stored procedure returns null or fails.
        /// </summary>
        private const int DefaultDateSpanDays = 180;

        /// <summary>
        /// Maximum allowed date age in years for validation.
        /// </summary>
        private const int MaxDateAgeYears = 200;

        /// <summary>
        /// Valid sort column names that match database columns.
        /// Used to prevent SQL injection through the sort column parameter.
        /// </summary>
        private static readonly HashSet<string> ValidSortColumns = new(StringComparer.OrdinalIgnoreCase)
        {
            "opidagedt",    // Invoice Date
            "opiinvno",     // Invoice Number
            "opicrmnr",     // Credit Number
            "opishpno",     // Ship To Number
            "opiordno",     // Order Number
            "inhTripNo",    // Trip Number
            "[RPP #]",      // RPP Number
            "opiponum",     // PO Number
            "opiInvam",     // Invoice Amount
            "opiTtlcr",     // Amount Paid
            "opiOpamt",     // Balance
            "opicatcd",     // Category Code
            "[Days]"        // Days
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="InvoiceRepository"/> class.
        /// </summary>
        /// <param name="configuration">The application configuration containing connection strings.</param>
        /// <param name="logger">The logger instance for diagnostic logging.</param>
        /// <exception cref="ArgumentNullException">Thrown when configuration is null or AFI_Batch connection string is missing.</exception>
        public InvoiceRepository(IConfiguration configuration, ILogger<InvoiceRepository> logger)
        {
            ArgumentNullException.ThrowIfNull(configuration);
            ArgumentNullException.ThrowIfNull(logger);

            _connectionString = configuration.GetConnectionString("AFI_Batch")
                ?? throw new ArgumentNullException(nameof(configuration), "AFI_Batch connection string not found");
            _logger = logger;
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is null.</exception>
        /// <exception cref="ValidationException">Thrown when request validation fails.</exception>
        /// <exception cref="DataAccessException">Thrown when database operation fails.</exception>
        public async Task<DataTable> SearchInvoicesAsync(InvoiceSearchRequest request, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request);

            ValidateSearchRequest(request);

            var dataTable = new DataTable();

            try
            {
                using var connection = new SqlConnection(_connectionString);
                using var command = new SqlCommand("Datawhse.dbo.usp_OrderAndInvoiceReportingOpenInvoices3", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 120 // 2 minutes timeout
                };

                // Determine if consumer PO search should be used
                bool consumerPOSearch = !string.IsNullOrEmpty(request.PONumber)
                    && await DetermineIfToUseConsumerSearchAsync(request.CustomerNumber, cancellationToken);

                // Validate and sanitize sort column - only allow known column names
                var sortColumn = GetValidSortColumn(request.SortColumn);

                // Determine sort ascending string - empty if no sort column, otherwise " ASC" or " DESC"
                var sortAscendingStr = string.IsNullOrEmpty(sortColumn)
                    ? string.Empty
                    : (request.SortAscending ? " ASC" : " DESC");

                // Determine page parameters - 0 means show all
                var pageNum = request.ShowAll ? 0 : request.PageNumber;
                var pageSize = request.ShowAll ? 0 : request.PageSize;

                _logger.LogInformation(
                    "SearchInvoicesAsync - Customer: {CustomerNumber}, FromDate: {FromDate}, ToDate: {ToDate}, " +
                    "SortColumn: {SortColumn}, SortAscending: {SortAscending}, PageNum: {PageNum}, PageSize: {PageSize}, " +
                    "SecurityMHS: {SecurityMHS}, AllShipTos: {AllShipTos}, ConsumerPOSearch: {ConsumerPOSearch}",
                    request.CustomerNumber, request.FromDate, request.ToDate,
                    sortColumn, sortAscendingStr, pageNum, pageSize,
                    request.SecurityMHS, request.AllShipTos, consumerPOSearch);

                // Add parameters matching the stored procedure signature exactly
                // Reference: Common.vb LoadInvoices() -> Datawhse.dbo.usp_OrderAndInvoiceReportingOpenInvoices3
                // IMPORTANT: The stored procedure has REVERSED naming convention:
                //   @FromDate = END date (later date) - e.g., '2025-07-31'
                //   @ToDate = START date (earlier date) - e.g., '2025-01-27'
                // This was confirmed by SSMS testing showing @FromDate='2025-07-31', @ToDate='2025-01-27'
                command.Parameters.AddWithValue("@customerNumber", request.CustomerNumber ?? string.Empty);
                command.Parameters.AddWithValue("@shiptoNumber", request.ShipToNumber ?? string.Empty);
                command.Parameters.AddWithValue("@allShiptos", request.AllShipTos ? 1 : 0);
                command.Parameters.AddWithValue("@securityMHS", request.SecurityMHS ?? "MASTERXX");
                command.Parameters.AddWithValue("@FromDate", request.FromDate);    // SP expects END date (later date from UI)
                command.Parameters.AddWithValue("@ToDate", request.ToDate);    // SP expects START date (earlier date from UI)
                command.Parameters.AddWithValue("@searchInvoice", SanitizeInput(request.InvoiceNumber));
                command.Parameters.AddWithValue("@searchCredit", SanitizeInput(request.CreditNumber));
                command.Parameters.AddWithValue("@searchPo", SanitizePoNumber(request.PONumber));
                command.Parameters.AddWithValue("@sortBy", sortColumn);
                command.Parameters.AddWithValue("@sortAscending", sortAscendingStr);
                command.Parameters.AddWithValue("@pageNum", pageNum);
                command.Parameters.AddWithValue("@pageSize", pageSize);
                command.Parameters.AddWithValue("@ConsumerPOSearch", consumerPOSearch ? 1 : 0);
                command.Parameters.AddWithValue("@Application", "EPAY");

                await connection.OpenAsync(cancellationToken);

                using var adapter = new SqlDataAdapter(command);
                adapter.Fill(dataTable);

                _logger.LogInformation("Found {RowCount} invoices for customer {CustomerNumber}",
                    dataTable.Rows.Count, request.CustomerNumber);

                return dataTable;
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("SearchInvoicesAsync was cancelled for customer {CustomerNumber}", request.CustomerNumber);
                throw;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error searching invoices for customer {CustomerNumber}: {Message}",
                    request.CustomerNumber, ex.Message);
                throw new DataAccessException($"Failed to search invoices for customer {request.CustomerNumber}", ex);
            }
            catch (Exception ex) when (ex is not ValidationException and not DataAccessException)
            {
                _logger.LogError(ex, "Unexpected error searching invoices for customer {CustomerNumber}: {Message}",
                    request.CustomerNumber, ex.Message);
                throw new DataAccessException($"An unexpected error occurred while searching invoices", ex);
            }
        }

        /// <summary>
        /// Validates and returns a safe sort column name.
        /// Returns empty string if the column is invalid (stored procedure will use default sorting).
        /// </summary>
        /// <param name="sortColumn">The sort column name to validate.</param>
        /// <returns>The validated column name or empty string if invalid.</returns>
        private string GetValidSortColumn(string? sortColumn)
        {
            if (string.IsNullOrWhiteSpace(sortColumn))
            {
                return string.Empty;
            }

            // Check if it's a valid column name
            if (ValidSortColumns.Contains(sortColumn))
            {
                return sortColumn;
            }

            // Log warning for invalid column names
            _logger.LogWarning("Invalid sort column '{SortColumn}' - using default sorting", sortColumn);
            return string.Empty;
        }

        /// <inheritdoc />
        public async Task<int> GetDefaultDateSpanInDaysAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                using var command = new SqlCommand("Ashley.dbo.usp_GetEpayDefaultDateSpanInDays", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                await connection.OpenAsync(cancellationToken);

                var result = await command.ExecuteScalarAsync(cancellationToken);
                var days = result != null ? Convert.ToInt32(result) : DefaultDateSpanDays;

                _logger.LogDebug("Default date span: {Days} days", days);
                return days;
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("GetDefaultDateSpanInDaysAsync was cancelled");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting default date span, using fallback of {Days} days", DefaultDateSpanDays);
                return DefaultDateSpanDays;
            }
        }

        /// <summary>
        /// Determines if consumer search should be used for this customer.
        /// </summary>
        /// <param name="customerNumber">The customer number to check.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
        /// <returns>True if consumer search should be used; otherwise, false.</returns>
        /// <remarks>
        /// This logic is from Common.vb - DetermineIfToUseConsumerSearch.
        /// Currently returns false - implement actual logic if needed.
        /// </remarks>
        private Task<bool> DetermineIfToUseConsumerSearchAsync(string customerNumber, CancellationToken cancellationToken = default)
        {
            // This logic is from Common.vb - DetermineIfToUseConsumerSearch
            // For now, return false - implement actual logic if needed
            return Task.FromResult(false);
        }

        /// <summary>
        /// Validates the search request parameters.
        /// </summary>
        /// <param name="request">The search request to validate.</param>
        /// <exception cref="ValidationException">Thrown when validation fails.</exception>
        private void ValidateSearchRequest(InvoiceSearchRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.CustomerNumber))
            {
                throw new ValidationException("Customer number is required.", nameof(request.CustomerNumber));
            }

            // Validate date range - fromDate should be before or equal to toDate
            if (request.FromDate > request.ToDate)
            {
                throw new InvalidDateRangeException(
                    "From date must be before or equal to To date.",
                    request.FromDate,
                    request.ToDate);
            }

            // Validate date range - prevent dates older than MaxDateAgeYears years
            var minAllowedDate = DateTime.Today.AddYears(-MaxDateAgeYears);
            if (request.FromDate < minAllowedDate || request.ToDate < minAllowedDate)
            {
                throw new InvalidDateRangeException(
                    $"Dates cannot be older than {MaxDateAgeYears} years.",
                    request.FromDate,
                    request.ToDate);
            }

            // Validate page number
            if (request.PageNumber < 0)
            {
                throw new ValidationException("Page number must be non-negative.", nameof(request.PageNumber));
            }

            // Validate page size
            if (request.PageSize < 0 || request.PageSize > 1000)
            {
                throw new ValidationException("Page size must be between 0 and 1000.", nameof(request.PageSize));
            }
        }

        /// <summary>
        /// Sanitizes input by removing single quotes to prevent SQL injection.
        /// </summary>
        /// <param name="input">The input string to sanitize.</param>
        /// <returns>The sanitized string or empty string if null.</returns>
        private static string SanitizeInput(string? input)
        {
            return (input ?? string.Empty).Replace("'", string.Empty);
        }

        /// <summary>
        /// Sanitizes PO number by escaping single quotes.
        /// </summary>
        /// <param name="poNumber">The PO number to sanitize.</param>
        /// <returns>The sanitized PO number or empty string if null.</returns>
        private static string SanitizePoNumber(string? poNumber)
        {
            return (poNumber ?? string.Empty).Replace("'", "''");
        }
    }
}

