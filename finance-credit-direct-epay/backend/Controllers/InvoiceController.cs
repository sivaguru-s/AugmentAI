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
    [Authorize] // Requires JWT authentication
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
                // Get user context from claims
                var customerNumber = User.FindFirst("CustomerNumber")?.Value;
                var shipToNumber = User.FindFirst("ShipToNumber")?.Value;
                var allShipTos = bool.Parse(User.FindFirst("AllShipTos")?.Value ?? "false");
                var securityMHS = User.FindFirst("SecurityMHS")?.Value;

                if (string.IsNullOrEmpty(customerNumber))
                {
                    _logger.LogWarning("No customer number found in user claims");
                    return BadRequest(new { error = "No account selected. Please select an account." });
                }

                // Override request with session data
                request.CustomerNumber = customerNumber;
                request.ShipToNumber = shipToNumber ?? string.Empty;
                request.AllShipTos = allShipTos;
                request.SecurityMHS = securityMHS ?? string.Empty;

                _logger.LogInformation("Searching invoices for customer {CustomerNumber}", customerNumber);

                var response = await _invoiceService.SearchInvoicesAsync(request);

                // Check if user has EPAYANLYST role
                response.IsAnalyst = User.IsInRole("EPAYANLYST");

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching invoices");
                return StatusCode(500, new { error = "An error occurred while searching invoices" });
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
                // Get user context from claims
                var customerNumber = User.FindFirst("CustomerNumber")?.Value;
                var shipToNumber = User.FindFirst("ShipToNumber")?.Value;
                var allShipTos = bool.Parse(User.FindFirst("AllShipTos")?.Value ?? "false");
                var securityMHS = User.FindFirst("SecurityMHS")?.Value;

                if (string.IsNullOrEmpty(customerNumber))
                {
                    return BadRequest(new { error = "No account selected" });
                }

                // Override request with session data
                request.CustomerNumber = customerNumber;
                request.ShipToNumber = shipToNumber ?? string.Empty;
                request.AllShipTos = allShipTos;
                request.SecurityMHS = securityMHS ?? string.Empty;

                _logger.LogInformation("Exporting invoices to Excel for customer {CustomerNumber}", customerNumber);

                var excelData = await _invoiceService.ExportToExcelAsync(request);

                return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
                    "EpaymentCustomerInvoices.xlsx");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting invoices to Excel");
                return StatusCode(500, new { error = "An error occurred while exporting invoices" });
            }
        }
    }
}

