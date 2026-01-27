using EPay.Api.Models.DTOs;
using EPay.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EPay.Api.Controllers
{
    /// <summary>
    /// API controller for invoice operations
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    // [Authorize] // TODO: Enable authentication when ready - Commented out for development
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;
        private readonly ILogger<InvoiceController> _logger;

        public InvoiceController(IInvoiceService invoiceService, ILogger<InvoiceController> logger)
        {
            _invoiceService = invoiceService;
            _logger = logger;
        }

        /// <summary>
        /// Search invoices with pagination
        /// </summary>
        /// <param name="request">Search criteria</param>
        /// <returns>Paged invoice search results</returns>
        /// <response code="200">Returns the paged invoice list</response>
        /// <response code="400">If the request is invalid</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="500">If an error occurs</response>
        [HttpPost("search")]
        [ProducesResponseType(typeof(InvoiceSearchResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<InvoiceSearchResponse>> SearchInvoices([FromBody] InvoiceSearchRequest request)
        {
            try
            {
                // For development: Use customer number from request body directly
                // TODO: When authentication is enabled, get customer number from user claims
                if (string.IsNullOrEmpty(request.CustomerNumber))
                {
                    _logger.LogWarning("No customer number provided in request");
                    return BadRequest(new { error = "No account selected. Please provide a customer number." });
                }

                _logger.LogInformation("Searching invoices for customer {CustomerNumber}, FromDate: {FromDate}, ToDate: {ToDate}",
                    request.CustomerNumber, request.FromDate, request.ToDate);

                var response = await _invoiceService.SearchInvoicesAsync(request);

                // For development: Set IsAnalyst to true to show all features
                response.IsAnalyst = true;

                _logger.LogInformation("Found {Count} invoices", response.Result?.Items?.Count ?? 0);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching invoices: {Message}", ex.Message);
                return StatusCode(500, new { error = $"An error occurred while searching invoices: {ex.Message}" });
            }
        }

        /// <summary>
        /// Get default date span in days
        /// </summary>
        /// <returns>Number of days</returns>
        /// <response code="200">Returns the default date span</response>
        /// <response code="500">If an error occurs</response>
        [HttpGet("default-date-span")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<int>> GetDefaultDateSpan()
        {
            try
            {
                var days = await _invoiceService.GetDefaultDateSpanInDaysAsync();
                return Ok(days);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting default date span");
                return StatusCode(500, new { error = "An error occurred while getting default date span" });
            }
        }

        /// <summary>
        /// Export invoices to Excel
        /// </summary>
        /// <param name="request">Search criteria</param>
        /// <returns>Excel file</returns>
        /// <response code="200">Returns the Excel file</response>
        /// <response code="400">If the request is invalid</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="500">If an error occurs</response>
        [HttpPost("export")]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ExportToExcel([FromBody] InvoiceSearchRequest request)
        {
            try
            {
                // For development: Use customer number from request body directly
                if (string.IsNullOrEmpty(request.CustomerNumber))
                {
                    return BadRequest(new { error = "No account selected. Please provide a customer number." });
                }

                _logger.LogInformation("Exporting invoices to Excel for customer {CustomerNumber}", request.CustomerNumber);

                var excelData = await _invoiceService.ExportToExcelAsync(request);

                return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "EpaymentCustomerInvoices.xlsx");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting invoices to Excel: {Message}", ex.Message);
                return StatusCode(500, new { error = $"An error occurred while exporting invoices: {ex.Message}" });
            }
        }
    }
}

