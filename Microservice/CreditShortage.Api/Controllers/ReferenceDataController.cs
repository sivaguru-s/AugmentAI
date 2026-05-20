using CreditShortage.Application.DTOs;
using CreditShortage.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CreditShortage.Api.Controllers;

/// <summary>
/// API Controller for reference data (defect codes, locations, etc.)
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class ReferenceDataController : ControllerBase
{
    private readonly IReferenceDataService _referenceDataService;
    private readonly ILogger<ReferenceDataController> _logger;

    public ReferenceDataController(
        IReferenceDataService referenceDataService,
        ILogger<ReferenceDataController> logger)
    {
        _referenceDataService = referenceDataService;
        _logger = logger;
    }

    /// <summary>
    /// Gets all active defect codes
    /// </summary>
    /// <returns>List of active defect codes</returns>
    /// <response code="200">Returns list of defect codes</response>
    [HttpGet("defect-codes")]
    [ProducesResponseType(typeof(List<DefectCodeDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<DefectCodeDto>>> GetDefectCodes()
    {
        _logger.LogInformation("Fetching active defect codes");
        var codes = await _referenceDataService.GetActiveDefectCodesAsync();
        return Ok(codes);
    }

    /// <summary>
    /// Gets all active location codes
    /// </summary>
    /// <returns>List of active location/warehouse codes</returns>
    /// <response code="200">Returns list of location codes</response>
    [HttpGet("location-codes")]
    [ProducesResponseType(typeof(List<LocationCodeDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<LocationCodeDto>>> GetLocationCodes()
    {
        _logger.LogInformation("Fetching active location codes");
        var codes = await _referenceDataService.GetActiveLocationCodesAsync();
        return Ok(codes);
    }

    /// <summary>
    /// Gets default configuration settings
    /// </summary>
    /// <returns>Default defect code and location code</returns>
    /// <response code="200">Returns default settings</response>
    [HttpGet("defaults")]
    [ProducesResponseType(typeof(DefaultSettingsDto), StatusCodes.Status200OK)]
    public ActionResult<DefaultSettingsDto> GetDefaults()
    {
        _logger.LogInformation("Fetching default settings");
        var defaults = _referenceDataService.GetDefaultSettings();
        return Ok(defaults);
    }
}
