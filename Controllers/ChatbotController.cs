using Microsoft.AspNetCore.Mvc;
using Chatbot_Onbase.Models;
using Chatbot_Onbase.Services;
using Serilog;

namespace Chatbot_Onbase.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatbotController : ControllerBase
{
    private readonly IChatbotService _chatbotService;
    private readonly ILogger<ChatbotController> _logger;

    public ChatbotController(IChatbotService chatbotService, ILogger<ChatbotController> logger)
    {
        _chatbotService = chatbotService;
        _logger = logger;
    }

    /// <summary>
    /// Process a natural language query to search for invoices
    /// </summary>
    /// <param name="request">The chat request containing the user's prompt</param>
    /// <returns>Chat response with invoice results</returns>
    [HttpPost("query")]
    public async Task<ActionResult<ChatResponse>> Query([FromBody] ChatRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Prompt))
        {
            return BadRequest(new { error = "Prompt cannot be empty" });
        }

        try
        {
            _logger.LogInformation("Chatbot|Query method started");
            var response = await _chatbotService.ProcessQueryAsync(request.Prompt);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing chat query");
            return StatusCode(500, new { error = "An error occurred while processing your request" });
        }
    }

    /// <summary>
    /// Health check endpoint
    /// </summary>
    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
    }
}

