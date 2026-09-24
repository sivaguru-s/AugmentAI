using Microsoft.AspNetCore.Mvc;
using Chatbot_Onbase.Models;
using Chatbot_Onbase.Services;
using Chatbot_Onbase.Data;
using Microsoft.Data.SqlClient;
using Serilog;

namespace Chatbot_Onbase.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatbotController : ControllerBase
{
    private readonly IChatbotService _chatbotService;
    private readonly ILogger<ChatbotController> _logger;
    private readonly IConfiguration _configuration;

    public ChatbotController(IChatbotService chatbotService, ILogger<ChatbotController> logger, IConfiguration configuration)
    {
        _chatbotService = chatbotService;
        _logger = logger;
        _configuration = configuration;
    }

    /// <summary>
    /// Process a natural language query to search for invoices
    /// </summary>
    /// <param name="request">The chat request containing the user's prompt</param>
    /// <returns>Chat response with invoice results</returns>
    [HttpPost("query")]
    public async Task<ActionResult<ChatResponse>> Query([FromBody] ChatRequest request)
    {
        _logger.LogInformation("Received query request from {IpAddress}", HttpContext.Connection.RemoteIpAddress);

        if (string.IsNullOrWhiteSpace(request.Prompt))
        {
            _logger.LogWarning("Empty prompt received");
            return BadRequest(new { error = "Prompt cannot be empty" });
        }

        try
        {
            _logger.LogInformation("Processing query: {Prompt}", request.Prompt);
            var response = await _chatbotService.ProcessQueryAsync(request.Prompt);
            _logger.LogInformation("Query processed successfully. Result count: {Count}", response.ResultCount);
            return Ok(response);
        }
        catch (Microsoft.Data.SqlClient.SqlException sqlEx)
        {
            _logger.LogError(sqlEx, "Database error processing query: {Prompt}. Error Number: {ErrorNumber}, State: {State}, Server: {Server}",
                request.Prompt, sqlEx.Number, sqlEx.State, sqlEx.Server);
            return StatusCode(500, new {
                error = "Database error occurred",
                details = $"SQL Error {sqlEx.Number}: {sqlEx.Message}",
                timestamp = DateTime.UtcNow
            });
        }
        catch (TimeoutException timeoutEx)
        {
            _logger.LogError(timeoutEx, "Timeout error processing query: {Prompt}", request.Prompt);
            return StatusCode(504, new {
                error = "Query timeout",
                details = "The database query took too long to complete",
                timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error processing query: {Prompt}. Exception Type: {ExceptionType}",
                request.Prompt, ex.GetType().Name);
            return StatusCode(500, new {
                error = "An error occurred while processing your request",
                details = ex.Message,
                exceptionType = ex.GetType().Name,
                timestamp = DateTime.UtcNow
            });
        }
    }

    /// <summary>
    /// Health check endpoint
    /// </summary>
    [HttpGet("health")]
    public IActionResult Health()
    {
        _logger.LogInformation("Health check requested from {IpAddress}", HttpContext.Connection.RemoteIpAddress);

        try
        {
            return Ok(new {
                status = "healthy",
                timestamp = DateTime.UtcNow,
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
                machineName = Environment.MachineName,
                version = "1.0.0"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health check failed");
            return StatusCode(500, new {
                status = "unhealthy",
                error = ex.Message,
                timestamp = DateTime.UtcNow
            });
        }
    }

    /// <summary>
    /// Test database connection
    /// </summary>
    [HttpGet("dbtest")]
    public async Task<IActionResult> DatabaseTest()
    {
        _logger.LogInformation("Database connection test requested from {IpAddress}", HttpContext.Connection.RemoteIpAddress);

        var connectionString = _configuration.GetConnectionString("OnBaseConnection");

        if (string.IsNullOrEmpty(connectionString))
        {
            _logger.LogError("Connection string 'OnBaseConnection' not found in configuration");
            return StatusCode(500, new {
                status = "error",
                message = "Connection string not configured",
                timestamp = DateTime.UtcNow
            });
        }

        try
        {
            using var connection = new SqlConnection(connectionString);
            var startTime = DateTime.UtcNow;
            await connection.OpenAsync();
            var duration = (DateTime.UtcNow - startTime).TotalMilliseconds;

            var serverVersion = connection.ServerVersion;
            var database = connection.Database;
            var dataSource = connection.DataSource;

            await connection.CloseAsync();

            _logger.LogInformation("Database connection successful. Server: {Server}, Database: {Database}, Duration: {Duration}ms",
                dataSource, database, duration);

            return Ok(new {
                status = "connected",
                server = dataSource,
                database = database,
                serverVersion = serverVersion,
                connectionTimeMs = duration,
                timestamp = DateTime.UtcNow
            });
        }
        catch (SqlException sqlEx)
        {
            _logger.LogError(sqlEx, "Database connection failed. Error Number: {ErrorNumber}, State: {State}, Server: {Server}",
                sqlEx.Number, sqlEx.State, sqlEx.Server);

            return StatusCode(500, new {
                status = "connection_failed",
                error = sqlEx.Message,
                errorNumber = sqlEx.Number,
                state = sqlEx.State,
                server = sqlEx.Server,
                timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error testing database connection. Exception Type: {ExceptionType}", ex.GetType().Name);

            return StatusCode(500, new {
                status = "error",
                error = ex.Message,
                exceptionType = ex.GetType().Name,
                timestamp = DateTime.UtcNow
            });
        }
    }
}

