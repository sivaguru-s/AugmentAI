using Xunit;
using Moq;
using Chatbot_Onbase.Services;
using Chatbot_Onbase.Data;
using Chatbot_Onbase.Models;
using Microsoft.Extensions.Logging;

namespace Chatbot_Onbase.Tests.Services;

/// <summary>
/// Unit tests for ChatbotService
/// Tests business logic and service orchestration
/// </summary>
public class ChatbotServiceTests
{
    private readonly Mock<IOnbaseRepository> _mockRepository;
    private readonly Mock<ILogger<ChatbotService>> _mockLogger;
    private readonly ChatbotService _service;

    public ChatbotServiceTests()
    {
        _mockRepository = new Mock<IOnbaseRepository>();
        _mockLogger = new Mock<ILogger<ChatbotService>>();
        _service = new ChatbotService(_mockRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task ProcessQuery_WithValidQuery_ReturnsResponse()
    {
        // Arrange
        var query = "Show all invoices";
        var mockInvoices = new List<Invoice>
        {
            new Invoice
            {
                InvoiceId = 1,
                InvoiceNumber = "INV-001",
                VendorName = "Test Vendor",
                Amount = 1000.00m,
                InvoiceDate = DateTime.Now,
                Status = "Pending"
            }
        };

        _mockRepository
            .Setup(r => r.SearchInvoicesAsync(It.IsAny<string>(), It.IsAny<Dictionary<string, object>>()))
            .ReturnsAsync(mockInvoices);

        // Act
        var result = await _service.ProcessQueryAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.Answer);
        Assert.Single(result.Invoices);
        Assert.Equal(1, result.ResultCount);
    }

    [Fact]
    public async Task ProcessQuery_WithEmptyQuery_ThrowsArgumentException()
    {
        // Arrange
        var query = "";

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.ProcessQueryAsync(query));
    }

    [Fact]
    public async Task ProcessQuery_WithNullQuery_ThrowsArgumentNullException()
    {
        // Arrange
        string query = null!;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _service.ProcessQueryAsync(query));
    }

    [Fact]
    public async Task ProcessQuery_VendorSearch_CallsRepositoryWithCorrectParameters()
    {
        // Arrange
        var query = "Show invoices from vendor ABC Corp";
        var mockInvoices = new List<Invoice>();

        _mockRepository
            .Setup(r => r.SearchInvoicesAsync("VendorSearch", It.IsAny<Dictionary<string, object>>()))
            .ReturnsAsync(mockInvoices);

        // Act
        await _service.ProcessQueryAsync(query);

        // Assert
        _mockRepository.Verify(
            r => r.SearchInvoicesAsync("VendorSearch", It.Is<Dictionary<string, object>>(
                p => p.ContainsKey("vendorName")
            )),
            Times.Once
        );
    }

    [Fact]
    public async Task ProcessQuery_NoResults_ReturnsEmptyList()
    {
        // Arrange
        var query = "Show invoices from nonexistent vendor";
        var mockInvoices = new List<Invoice>();

        _mockRepository
            .Setup(r => r.SearchInvoicesAsync(It.IsAny<string>(), It.IsAny<Dictionary<string, object>>()))
            .ReturnsAsync(mockInvoices);

        // Act
        var result = await _service.ProcessQueryAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Invoices);
        Assert.Equal(0, result.ResultCount);
        Assert.Contains("no invoices", result.Answer.ToLower());
    }

    [Fact]
    public async Task ProcessQuery_RepositoryThrowsException_PropagatesException()
    {
        // Arrange
        var query = "Show all invoices";

        _mockRepository
            .Setup(r => r.SearchInvoicesAsync(It.IsAny<string>(), It.IsAny<Dictionary<string, object>>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.ProcessQueryAsync(query));
    }

    [Fact]
    public async Task ProcessQuery_MultipleResults_ReturnsAllResults()
    {
        // Arrange
        var query = "Show all invoices";
        var mockInvoices = new List<Invoice>
        {
            new Invoice { InvoiceId = 1, InvoiceNumber = "INV-001", Amount = 1000m },
            new Invoice { InvoiceId = 2, InvoiceNumber = "INV-002", Amount = 2000m },
            new Invoice { InvoiceId = 3, InvoiceNumber = "INV-003", Amount = 3000m }
        };

        _mockRepository
            .Setup(r => r.SearchInvoicesAsync(It.IsAny<string>(), It.IsAny<Dictionary<string, object>>()))
            .ReturnsAsync(mockInvoices);

        // Act
        var result = await _service.ProcessQueryAsync(query);

        // Assert
        Assert.Equal(3, result.ResultCount);
        Assert.Equal(3, result.Invoices.Count);
    }
}

