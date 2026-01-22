using EPay.Backend.Models.DTOs;
using EPay.Backend.Repositories;
using System.Data;

namespace EPay.Backend.Services
{
    /// <summary>
    /// Service implementation for invoice business logic
    /// </summary>
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _repository;
        private readonly ILogger<InvoiceService> _logger;

        public InvoiceService(IInvoiceRepository repository, ILogger<InvoiceService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        /// <summary>
        /// Search invoices with pagination
        /// </summary>
        public async Task<InvoiceSearchResponse> SearchInvoicesAsync(InvoiceSearchRequest request)
        {
            _logger.LogInformation("Searching invoices for customer {CustomerNumber}", request.CustomerNumber);

            // Validate dates
            if (request.FromDate == default || request.ToDate == default)
            {
                var defaultDays = await GetDefaultDateSpanInDaysAsync();
                request.FromDate = DateTime.Today.AddDays(-defaultDays);
                request.ToDate = DateTime.Today;
            }

            // Validate date range (prevent dates older than 200 years)
            if (request.FromDate <= DateTime.Today.AddYears(-200) || request.ToDate <= DateTime.Today.AddYears(-200))
            {
                var defaultDays = await GetDefaultDateSpanInDaysAsync();
                request.FromDate = DateTime.Today.AddDays(-defaultDays);
                request.ToDate = DateTime.Today;
            }

            // Get data from repository
            var dataTable = await _repository.SearchInvoicesAsync(request);

            // Map DataTable to DTOs
            var invoices = MapDataTableToInvoiceDtos(dataTable);

            // Get page count from first row
            int totalPages = 1;
            if (dataTable.Rows.Count > 0 && dataTable.Columns.Contains("PageCount"))
            {
                totalPages = Convert.ToInt32(dataTable.Rows[0]["PageCount"]);
            }

            var response = new InvoiceSearchResponse
            {
                Result = new PagedResult<InvoiceDto>
                {
                    Items = invoices,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,
                    TotalPages = totalPages,
                    TotalRecords = totalPages * request.PageSize
                },
                FromDate = request.FromDate,
                ToDate = request.ToDate,
                IsAnalyst = false // Will be set by controller based on user claims
            };

            return response;
        }

        /// <summary>
        /// Get default date span in days
        /// </summary>
        public async Task<int> GetDefaultDateSpanInDaysAsync()
        {
            return await _repository.GetDefaultDateSpanInDaysAsync();
        }

        /// <summary>
        /// Export invoices to Excel
        /// </summary>
        public async Task<byte[]> ExportToExcelAsync(InvoiceSearchRequest request)
        {
            _logger.LogInformation("Exporting invoices to Excel for customer {CustomerNumber}", request.CustomerNumber);

            // Get all invoices (no pagination)
            request.ShowAll = true;
            request.PageNumber = 0;
            request.PageSize = 0;

            var dataTable = await _repository.SearchInvoicesAsync(request);

            // TODO: Implement Excel export using EPPlus or ClosedXML
            // For now, return empty byte array
            return Array.Empty<byte>();
        }

        /// <summary>
        /// Map DataTable to list of InvoiceDto
        /// </summary>
        private List<InvoiceDto> MapDataTableToInvoiceDtos(DataTable dataTable)
        {
            var invoices = new List<InvoiceDto>();

            foreach (DataRow row in dataTable.Rows)
            {
                var invoice = new InvoiceDto
                {
                    CustomerNumber = row["opicusno"].ToString()?.Trim() ?? string.Empty,
                    ShipToNumber = row["opishpno"].ToString()?.Trim() ?? string.Empty,
                    InvoiceNumber = row["opiinvno"].ToString()?.Trim() ?? string.Empty,
                    CreditNumber = Convert.ToInt32(row["opicrmnr"]),
                    InvoiceDate = Convert.ToDateTime(row["opiDagedt"]),
                    InvoiceAmount = Convert.ToDecimal(row["opiInvam"]),
                    AmountPaid = Convert.ToDecimal(row["opiTtlcr"]),
                    Balance = Convert.ToDecimal(row["opiOpamt"]),
                    CategoryCode = row["opicatcd"].ToString()?.Trim() ?? string.Empty,
                    PONumber = row["opiponum"].ToString()?.Trim() ?? string.Empty,
                    OrderNumber = row["opiordno"].ToString()?.Trim() ?? string.Empty,
                    OrderDate = row["inhdordda"] != DBNull.Value ? Convert.ToDateTime(row["inhdordda"]) : null,
                    RPPNumber = row["RPP #"].ToString()?.Trim() ?? string.Empty,
                    Days = Convert.ToInt32(row["Days"]),
                    TripNumber = row["inhTripNo"] != DBNull.Value ? Convert.ToInt32(row["inhTripNo"]) : null,
                    EPayStatus = dataTable.Columns.Contains("EpayStatus") && row["EpayStatus"] != DBNull.Value 
                        ? row["EpayStatus"].ToString()?.Trim() 
                        : null,
                    EPayReferenceNumber = dataTable.Columns.Contains("EpayRefNumber") && row["EpayRefNumber"] != DBNull.Value 
                        ? Convert.ToInt32(row["EpayRefNumber"]) 
                        : null,
                    HideDetail = dataTable.Columns.Contains("HideDetail") && row["HideDetail"] != DBNull.Value 
                        && Convert.ToBoolean(row["HideDetail"]),
                    CanSelect = string.IsNullOrEmpty(row.Table.Columns.Contains("EpayStatus") && row["EpayStatus"] != DBNull.Value 
                        ? row["EpayStatus"].ToString()?.Trim() 
                        : null)
                };

                invoices.Add(invoice);
            }

            return invoices;
        }
    }
}

