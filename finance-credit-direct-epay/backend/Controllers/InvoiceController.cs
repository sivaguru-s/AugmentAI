using EPay.Api.Exceptions;
using EPay.Api.Models.DTOs;
using EPay.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EPay.Api.Controllers
{
    /// <summary>
    /// API controller for invoice operations.
    /// Provides endpoints for searching, retrieving, and exporting invoice data.
    /// </summary>
    /// <remarks>
    /// This controller handles invoice-related operations including:
    /// <list type="bullet">
    /// <item>Searching invoices with pagination and filtering</item>
    /// <item>Retrieving default date span configuration</item>
    /// <item>Exporting invoices to Excel format</item>
    /// </list>
    /// </remarks>
    [ApiController]
    [Route("api/[controller]")]
    // [Authorize] // TODO: Enable authentication when ready - Commented out for development
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;
        private readonly ILogger<InvoiceController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="InvoiceController"/> class.
        /// </summary>
        /// <param name="invoiceService">The invoice service for business logic.</param>
        /// <param name="logger">The logger instance for diagnostic logging.</param>
        /// <exception cref="ArgumentNullException">Thrown when invoiceService or logger is null.</exception>
        public InvoiceController(IInvoiceService invoiceService, ILogger<InvoiceController> logger)
        {
            _invoiceService = invoiceService ?? throw new ArgumentNullException(nameof(invoiceService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Searches invoices with pagination and filtering.
        /// </summary>
        /// <param name="request">The search criteria including customer number, date range, and pagination.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
        /// <returns>Paged invoice search results with metadata.</returns>
        /// <response code="200">Returns the paged invoice list.</response>
        /// <response code="400">If the request is invalid or validation fails.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="500">If an internal error occurs.</response>
        [HttpPost("search")]
        [ProducesResponseType(typeof(InvoiceSearchResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<InvoiceSearchResponse>> SearchInvoices(
            [FromBody] InvoiceSearchRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                // For development: Use customer number from request body directly
                // TODO: When authentication is enabled, get customer number from user claims
                if (string.IsNullOrEmpty(request.CustomerNumber))
                {
                    _logger.LogWarning("No customer number provided in request");
                    return BadRequest(new { error = "No account selected. Please provide a customer number.", errorCode = "MISSING_CUSTOMER" });
                }

                _logger.LogInformation("Searching invoices for customer {CustomerNumber}, FromDate: {FromDate}, ToDate: {ToDate}",
                    request.CustomerNumber, request.FromDate, request.ToDate);

                var response = await _invoiceService.SearchInvoicesAsync(request, cancellationToken);

                // For development: Set IsAnalyst to true to show all features
                response.IsAnalyst = true;

                _logger.LogInformation("Found {Count} invoices for customer {CustomerNumber}",
                    response.Result?.Items?.Count ?? 0, request.CustomerNumber);

                return Ok(response);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validation error searching invoices: {Message}", ex.Message);
                return BadRequest(new { error = ex.Message, errorCode = ex.ErrorCode, field = ex.FieldName });
            }
            catch (InvalidDateRangeException ex)
            {
                _logger.LogWarning(ex, "Invalid date range: {Message}", ex.Message);
                return BadRequest(new { error = ex.Message, errorCode = ex.ErrorCode, fromDate = ex.FromDate, toDate = ex.ToDate });
            }
            catch (DataAccessException ex)
            {
                _logger.LogError(ex, "Data access error searching invoices: {Message}", ex.Message);
                return StatusCode(500, new { error = "A database error occurred while searching invoices.", errorCode = ex.ErrorCode });
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("SearchInvoices request was cancelled");
                return StatusCode(499, new { error = "Request was cancelled." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error searching invoices: {Message}", ex.Message);
                return StatusCode(500, new { error = "An unexpected error occurred while searching invoices.", errorCode = "INTERNAL_ERROR" });
            }
        }

        /// <summary>
        /// Gets the default date span in days for invoice search.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
        /// <returns>The default date span in days.</returns>
        /// <response code="200">Returns the default date span.</response>
        /// <response code="500">If an internal error occurs.</response>
        [HttpGet("default-date-span")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<int>> GetDefaultDateSpan(CancellationToken cancellationToken)
        {
            try
            {
                var days = await _invoiceService.GetDefaultDateSpanInDaysAsync(cancellationToken);
                return Ok(days);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("GetDefaultDateSpan request was cancelled");
                return StatusCode(499, new { error = "Request was cancelled." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting default date span: {Message}", ex.Message);
                return StatusCode(500, new { error = "An error occurred while getting default date span.", errorCode = "INTERNAL_ERROR" });
            }
        }

        /// <summary>
        /// Exports invoices to an Excel file.
        /// </summary>
        /// <param name="request">The search criteria for the invoices to export.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
        /// <returns>An Excel file containing the invoice data.</returns>
        /// <response code="200">Returns the Excel file.</response>
        /// <response code="400">If the request is invalid or validation fails.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="500">If an internal error occurs.</response>
        [HttpPost("export")]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ExportToExcel(
            [FromBody] InvoiceSearchRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                // For development: Use customer number from request body directly
                if (string.IsNullOrEmpty(request.CustomerNumber))
                {
                    return BadRequest(new { error = "No account selected. Please provide a customer number.", errorCode = "MISSING_CUSTOMER" });
                }

                _logger.LogInformation("Exporting invoices to Excel for customer {CustomerNumber}", request.CustomerNumber);

                var excelData = await _invoiceService.ExportToExcelAsync(request, cancellationToken);

                return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "EpaymentCustomerInvoices.xlsx");
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validation error exporting invoices: {Message}", ex.Message);
                return BadRequest(new { error = ex.Message, errorCode = ex.ErrorCode, field = ex.FieldName });
            }
            catch (DataAccessException ex)
            {
                _logger.LogError(ex, "Data access error exporting invoices: {Message}", ex.Message);
                return StatusCode(500, new { error = "A database error occurred while exporting invoices.", errorCode = ex.ErrorCode });
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("ExportToExcel request was cancelled");
                return StatusCode(499, new { error = "Request was cancelled." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error exporting invoices to Excel: {Message}", ex.Message);
                return StatusCode(500, new { error = "An unexpected error occurred while exporting invoices.", errorCode = "INTERNAL_ERROR" });
            }
        }
    }
}

