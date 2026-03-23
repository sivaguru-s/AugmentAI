using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Chatbot_Onbase.Models;

namespace Chatbot_Onbase.Tests.Controllers;

/// <summary>
/// Integration tests for ChatbotController
/// Tests end-to-end API functionality
/// </summary>
public class ChatbotControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public ChatbotControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    #region Health Check Tests

    [Fact]
    public async Task HealthCheck_ReturnsOk()
    {
        // Act
        var response = await _client.GetAsync("/api/chatbot/health");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("healthy", content.ToLower());
    }

    #endregion

    #region Query Endpoint Tests

    [Fact]
    public async Task Query_WithValidRequest_ReturnsOk()
    {
        // Arrange
        var request = new ChatRequest
        {
            Prompt = "Show all invoices"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/chatbot/query", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var result = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.True(result.TryGetProperty("answer", out _));
        Assert.True(result.TryGetProperty("invoices", out _));
    }

    [Fact]
    public async Task Query_WithEmptyPrompt_ReturnsBadRequest()
    {
        // Arrange
        var request = new ChatRequest
        {
            Prompt = ""
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/chatbot/query", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Query_WithNullPrompt_ReturnsBadRequest()
    {
        // Arrange
        var request = new ChatRequest
        {
            Prompt = null!
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/chatbot/query", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Query_VendorSearch_ReturnsResults()
    {
        // Arrange
        var request = new ChatRequest
        {
            Prompt = "Show invoices from vendor ABC"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/chatbot/query", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var result = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.True(result.TryGetProperty("resultCount", out var count));
    }

    [Fact]
    public async Task Query_StatusSearch_ReturnsResults()
    {
        // Arrange
        var request = new ChatRequest
        {
            Prompt = "Find all pending invoices"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/chatbot/query", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var result = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.True(result.TryGetProperty("answer", out _));
    }

    [Fact]
    public async Task Query_AmountSearch_ReturnsResults()
    {
        // Arrange
        var request = new ChatRequest
        {
            Prompt = "Show invoices greater than 1000"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/chatbot/query", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Query_DateSearch_ReturnsResults()
    {
        // Arrange
        var request = new ChatRequest
        {
            Prompt = "Find invoices from this month"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/chatbot/query", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    #endregion
}

