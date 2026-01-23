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
            _logger.LogInformation("Searching invoices for customer {CustomerNumber}", request.CustomerNumber);

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

                // Add parameters matching the stored procedure signature
                command.Parameters.AddWithValue("@customerNumber", request.CustomerNumber);
                command.Parameters.AddWithValue("@shiptoNumber", request.ShipToNumber ?? string.Empty);
                command.Parameters.AddWithValue("@allShiptos", request.AllShipTos);
                command.Parameters.AddWithValue("@securityMHS", request.SecurityMHS ?? string.Empty);
                command.Parameters.AddWithValue("@FromDate", request.FromDate);
                command.Parameters.AddWithValue("@ToDate", request.ToDate);
                command.Parameters.AddWithValue("@searchInvoice", (request.InvoiceNumber ?? string.Empty).Replace("'", string.Empty));
                command.Parameters.AddWithValue("@searchCredit", (request.CreditNumber ?? string.Empty).Replace("'", string.Empty));
                command.Parameters.AddWithValue("@searchPo", (request.PONumber ?? string.Empty).Replace("'", "''"));
                command.Parameters.AddWithValue("@sortBy", request.SortColumn ?? string.Empty);
                command.Parameters.AddWithValue("@sortAscending", request.SortAscending ? " ASC" : " DESC");
                command.Parameters.AddWithValue("@pageNum", request.ShowAll ? 0 : request.PageNumber);
                command.Parameters.AddWithValue("@pageSize", request.ShowAll ? 0 : request.PageSize);
                command.Parameters.AddWithValue("@ConsumerPOSearch", consumerPOSearch);
                command.Parameters.AddWithValue("@Application", "EPAY");

                await connection.OpenAsync();

                using var adapter = new SqlDataAdapter(command);
                adapter.Fill(dataTable);

                _logger.LogInformation("Found {RowCount} invoices", dataTable.Rows.Count);

                return dataTable;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching invoices for customer {CustomerNumber}", request.CustomerNumber);
                throw;
            }
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

