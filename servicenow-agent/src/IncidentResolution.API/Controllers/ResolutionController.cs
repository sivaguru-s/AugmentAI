using IncidentResolution.Core.Interfaces;
using IncidentResolution.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace IncidentResolution.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ResolutionController : ControllerBase
{
    private readonly IServiceNowService _serviceNowService;
    private readonly IAIRecommendationService _aiService;
    private readonly IEmailService _emailService;
    private readonly ILogger<ResolutionController> _logger;

    public ResolutionController(
        IServiceNowService serviceNowService,
        IAIRecommendationService aiService,
        IEmailService emailService,
        ILogger<ResolutionController> logger)
    {
        _serviceNowService = serviceNowService;
        _aiService = aiService;
        _emailService = emailService;
        _logger = logger;
    }

    /// <summary>
    /// Get top 3 resolution recommendations for an incident
    /// </summary>
    [HttpPost("recommend")]
    public async Task<ActionResult<ResolutionResponse>> GetRecommendations([FromBody] ResolutionRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.IncidentDescription))
        {
            return BadRequest("Incident description is required");
        }

        _logger.LogInformation("Processing resolution request for: {Description}", request.IncidentDescription);

        // 1. Search ServiceNow for similar resolved incidents
        var historicalIncidents = await _serviceNowService.SearchIncidentsAsync(
            request.IncidentDescription, 
            request.Category);

        // If no similar incidents found, get recent resolved incidents
        if (!historicalIncidents.Any())
        {
            historicalIncidents = await _serviceNowService.GetResolvedIncidentsAsync(request.Category);
        }

        // 2. Generate AI recommendations
        var resolutions = await _aiService.GetRecommendationsAsync(
            request.IncidentDescription, 
            historicalIncidents, 
            topN: 3);

        var response = new ResolutionResponse
        {
            OriginalQuery = request.IncidentDescription,
            Resolutions = resolutions,
            ProcessedAt = DateTime.UtcNow
        };

        // 3. Send email if requested
        if (request.SendEmail && resolutions.Any())
        {
            try
            {
                response.EmailSent = await _emailService.SendResolutionEmailAsync(
                    request.IncidentDescription, 
                    resolutions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email");
                response.EmailSent = false;
                response.EmailError = ex.Message;
            }
        }

        return Ok(response);
    }

    /// <summary>
    /// Health check endpoint
    /// </summary>
    [HttpGet("health")]
    public IActionResult Health() => Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow });
}

