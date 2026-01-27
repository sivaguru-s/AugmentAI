using EPay.Api.Models.DTOs;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace EPay.Api.Repositories
{
    /// <summary>
    /// Repository implementation for invoice data access
    /// </summary>
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<InvoiceRepository> _logger;

        // Valid sort column names that match database columns
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

        public InvoiceRepository(IConfiguration configuration, ILogger<InvoiceRepository> logger)
        {
            _connectionString = configuration.GetConnectionString("AFI_Batch")
                ?? throw new ArgumentNullException(nameof(configuration), "AFI_Batch connection string not found");
            _logger = logger;
        }

        /// <summary>
        /// Search invoices using stored procedure usp_OrderAndInvoiceReportingOpenInvoices3
        /// </summary>
        public async Task<DataTable> SearchInvoicesAsync(InvoiceSearchRequest request)
        {
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
                    && await DetermineIfToUseConsumerSearchAsync(request.CustomerNumber);

                // Validate and sanitize sort column - only allow known column names
                var sortColumn = GetValidSortColumn(request.SortColumn);

                _logger.LogInformation(
                    "SearchInvoicesAsync - Customer: {CustomerNumber}, FromDate: {FromDate}, ToDate: {ToDate}, " +
                    "SortColumn: {SortColumn} (validated: {ValidatedSortColumn}), PageNum: {PageNum}, PageSize: {PageSize}, " +
                    "SecurityMHS: {SecurityMHS}, AllShipTos: {AllShipTos}",
                    request.CustomerNumber, request.FromDate, request.ToDate,
                    request.SortColumn, sortColumn, request.PageNumber, request.PageSize,
                    request.SecurityMHS, request.AllShipTos);

                // Add parameters matching the stored procedure signature
                // Note: Parameters match the VB.NET Common.LoadInvoices() method
                command.Parameters.AddWithValue("@customerNumber", request.CustomerNumber ?? string.Empty);
                command.Parameters.AddWithValue("@shiptoNumber", request.ShipToNumber ?? string.Empty);
                command.Parameters.AddWithValue("@allShiptos", request.AllShipTos);
                command.Parameters.AddWithValue("@securityMHS", request.SecurityMHS ?? string.Empty);
                command.Parameters.AddWithValue("@FromDate", request.FromDate);
                command.Parameters.AddWithValue("@ToDate", request.ToDate);
                command.Parameters.AddWithValue("@searchInvoice", (request.InvoiceNumber ?? string.Empty).Replace("'", string.Empty));
                command.Parameters.AddWithValue("@searchCredit", (request.CreditNumber ?? string.Empty).Replace("'", string.Empty));
                command.Parameters.AddWithValue("@searchPo", (request.PONumber ?? string.Empty).Replace("'", "''"));
                command.Parameters.AddWithValue("@sortBy", sortColumn);
                command.Parameters.AddWithValue("@sortAscending", request.SortAscending ? " ASC" : " DESC");
                command.Parameters.AddWithValue("@pageNum", request.ShowAll ? 0 : request.PageNumber);
                command.Parameters.AddWithValue("@pageSize", request.ShowAll ? 0 : request.PageSize);
                command.Parameters.AddWithValue("@ConsumerPOSearch", consumerPOSearch);
                command.Parameters.AddWithValue("@Application", "EPAY");

                await connection.OpenAsync();

                using var adapter = new SqlDataAdapter(command);
                adapter.Fill(dataTable);

                _logger.LogInformation("Found {RowCount} invoices for customer {CustomerNumber}",
                    dataTable.Rows.Count, request.CustomerNumber);

                return dataTable;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching invoices for customer {CustomerNumber}: {Message}",
                    request.CustomerNumber, ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Validates and returns a safe sort column name.
        /// Returns empty string if the column is invalid (stored procedure will use default sorting).
        /// </summary>
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

        /// <summary>
        /// Get default date span in days from stored procedure
        /// </summary>
        public async Task<int> GetDefaultDateSpanInDaysAsync()
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                using var command = new SqlCommand("Ashley.dbo.usp_GetEpayDefaultDateSpanInDays", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                await connection.OpenAsync();

                var result = await command.ExecuteScalarAsync();
                return result != null ? Convert.ToInt32(result) : 90; // Default to 90 days if null
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting default date span");
                return 90; // Default fallback
            }
        }

        /// <summary>
        /// Determine if consumer search should be used for this customer
        /// </summary>
        private async Task<bool> DetermineIfToUseConsumerSearchAsync(string customerNumber)
        {
            // This logic is from Common.vb - DetermineIfToUseConsumerSearch
            // For now, return false - implement actual logic if needed
            return await Task.FromResult(false);
        }
    }
}

