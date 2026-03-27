using EPay.Api.Exceptions;
using EPay.Api.Models.DTOs;
using EPay.Api.Repositories;
using System.Data;

namespace EPay.Api.Services
{
    /// <summary>
    /// Service implementation for invoice business logic operations.
    /// Provides validation, business rules, and data transformation for invoice operations.
    /// </summary>
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _repository;
        private readonly ILogger<InvoiceService> _logger;

        /// <summary>
        /// Maximum allowed date age in years for validation.
        /// </summary>
        private const int MaxDateAgeYears = 200;

        /// <summary>
        /// Initializes a new instance of the <see cref="InvoiceService"/> class.
        /// </summary>
        /// <param name="repository">The invoice repository for data access.</param>
        /// <param name="logger">The logger instance for diagnostic logging.</param>
        /// <exception cref="ArgumentNullException">Thrown when repository or logger is null.</exception>
        public InvoiceService(IInvoiceRepository repository, ILogger<InvoiceService> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is null.</exception>
        /// <exception cref="ValidationException">Thrown when request validation fails.</exception>
        public async Task<InvoiceSearchResponse> SearchInvoicesAsync(InvoiceSearchRequest request, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request);

            _logger.LogInformation("Searching invoices for customer {CustomerNumber}", request.CustomerNumber);

            // Apply default date range if dates are not specified
            await ApplyDefaultDateRangeIfNeededAsync(request, cancellationToken);

            // Get data from repository
            var dataTable = await _repository.SearchInvoicesAsync(request, cancellationToken);

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

            _logger.LogDebug("Search completed for customer {CustomerNumber}: {Count} invoices found",
                request.CustomerNumber, invoices.Count);

            return response;
        }

        /// <inheritdoc />
        public async Task<int> GetDefaultDateSpanInDaysAsync(CancellationToken cancellationToken = default)
        {
            return await _repository.GetDefaultDateSpanInDaysAsync(cancellationToken);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is null.</exception>
        public async Task<byte[]> ExportToExcelAsync(InvoiceSearchRequest request, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request);

            _logger.LogInformation("Exporting invoices to Excel for customer {CustomerNumber}", request.CustomerNumber);

            // Get all invoices (no pagination)
            request.ShowAll = true;
            request.PageNumber = 0;
            request.PageSize = 0;

            var dataTable = await _repository.SearchInvoicesAsync(request, cancellationToken);

            _logger.LogInformation("Exporting {Count} invoices to Excel for customer {CustomerNumber}",
                dataTable.Rows.Count, request.CustomerNumber);

            // TODO: Implement Excel export using EPPlus or ClosedXML
            // For now, return empty byte array
            return Array.Empty<byte>();
        }

        /// <summary>
        /// Applies default date range if dates are not specified or are invalid.
        /// </summary>
        /// <param name="request">The request to modify.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        private async Task ApplyDefaultDateRangeIfNeededAsync(InvoiceSearchRequest request, CancellationToken cancellationToken)
        {
            var minAllowedDate = DateTime.Today.AddYears(-MaxDateAgeYears);

            // Apply default dates if not specified
            if (request.FromDate == default || request.ToDate == default)
            {
                var defaultDays = await GetDefaultDateSpanInDaysAsync(cancellationToken);
                request.FromDate = DateTime.Today.AddDays(-defaultDays);
                request.ToDate = DateTime.Today;
                _logger.LogDebug("Applied default date range: {FromDate} to {ToDate}", request.FromDate, request.ToDate);
            }
            // Reset dates if they are too old
            else if (request.FromDate < minAllowedDate || request.ToDate < minAllowedDate)
            {
                var defaultDays = await GetDefaultDateSpanInDaysAsync(cancellationToken);
                request.FromDate = DateTime.Today.AddDays(-defaultDays);
                request.ToDate = DateTime.Today;
                _logger.LogWarning("Date range was too old, reset to default: {FromDate} to {ToDate}",
                    request.FromDate, request.ToDate);
            }
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

