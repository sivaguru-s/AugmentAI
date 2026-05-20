using CreditShortage.Application.DTOs;
using CreditShortage.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CreditShortage.Api.Controllers;

/// <summary>
/// API Controller for validating shortage items before credit entry
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class ShortageValidationController : ControllerBase
{
    private readonly IShortageValidationService _validationService;
    private readonly ILogger<ShortageValidationController> _logger;

    public ShortageValidationController(
        IShortageValidationService validationService,
        ILogger<ShortageValidationController> logger)
    {
        _validationService = validationService;
        _logger = logger;
    }

    /// <summary>
    /// Validates a single shortage item
    /// </summary>
    /// <param name="request">Shortage item to validate</param>
    /// <returns>Validation result with detailed feedback</returns>
    /// <response code="200">Validation completed (check IsValid flag in response)</response>
    /// <response code="400">Invalid request data</response>
    /// <response code="500">Internal server error</response>
    [HttpPost("validate")]
    [ProducesResponseType(typeof(ShortageValidationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ShortageValidationResponse>> ValidateSingleItem(
        [FromBody] ShortageItemRequest request)
    {
        _logger.LogInformation("Validating single shortage item for Customer: {Customer}, Invoice: {Invoice}, Item: {Item}",
            request.CustomerNumber, request.InvoiceNumber, request.ItemNumber);

        try
        {
            var result = await _validationService.ValidateSingleItemAsync(request);
            
            _logger.LogInformation("Validation completed for item {Item}. IsValid: {IsValid}",
                request.ItemNumber, result.IsValid);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating shortage item for Customer: {Customer}, Invoice: {Invoice}",
                request.CustomerNumber, request.InvoiceNumber);
            return StatusCode(500, new ProblemDetails
            {
                Title = "Validation Error",
                Detail = ex.Message,
                Status = 500
            });
        }
    }

    /// <summary>
    /// Validates multiple shortage items in a batch
    /// </summary>
    /// <param name="request">Batch of shortage items to validate</param>
    /// <returns>Validation results for each item</returns>
    /// <response code="200">Batch validation completed</response>
    /// <response code="400">Invalid request data</response>
    /// <response code="500">Internal server error</response>
    [HttpPost("validate-batch")]
    [ProducesResponseType(typeof(BatchShortageValidationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<BatchShortageValidationResponse>> ValidateBatch(
        [FromBody] BatchShortageValidationRequest request)
    {
        _logger.LogInformation("Validating batch of {Count} shortage items", request.Items.Count);

        try
        {
            var result = await _validationService.ValidateBatchAsync(request);
            
            _logger.LogInformation("Batch validation completed. Valid: {Valid}, Invalid: {Invalid}",
                result.ValidItemsCount, result.InvalidItemsCount);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating shortage item batch");
            return StatusCode(500, new ProblemDetails
            {
                Title = "Batch Validation Error",
                Detail = ex.Message,
                Status = 500
            });
        }
    }

    /// <summary>
    /// Validates a shortage item with IWS integration to obtain serial number
    /// </summary>
    /// <param name="request">Shortage item request (serial will be fetched from IWS)</param>
    /// <returns>Validation result with IWS-obtained serial number</returns>
    /// <response code="200">Validation completed with IWS integration</response>
    /// <response code="400">Invalid request data</response>
    /// <response code="500">Internal server error or IWS integration failure</response>
    [HttpPost("validate-with-iws")]
    [ProducesResponseType(typeof(ShortageValidationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ShortageValidationResponse>> ValidateWithIWS(
        [FromBody] ShortageItemIWSRequest request)
    {
        _logger.LogInformation("Validating shortage item with IWS integration for Customer: {Customer}, Invoice: {Invoice}",
            request.CustomerNumber, request.InvoiceNumber);

        try
        {
            var result = await _validationService.ValidateWithIWSAsync(request);
            
            _logger.LogInformation("Validation with IWS completed. Serial obtained: {Serial}, IsValid: {IsValid}",
                result.SerialNumber, result.IsValid);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating shortage item with IWS integration");
            return StatusCode(500, new ProblemDetails
            {
                Title = "IWS Integration Error",
                Detail = ex.Message,
                Status = 500
            });
        }
    }

    /// <summary>
    /// Gets validation statistics
    /// </summary>
    /// <returns>Statistics about recent validations</returns>
    [HttpGet("statistics")]
    [ProducesResponseType(typeof(ValidationStatistics), StatusCodes.Status200OK)]
    public async Task<ActionResult<ValidationStatistics>> GetStatistics()
    {
        var stats = await _validationService.GetValidationStatisticsAsync();
        return Ok(stats);
    }
}
