using Xunit;
using Chatbot_Onbase.Services;

namespace Chatbot_Onbase.Tests.Services;

/// <summary>
/// Unit tests for InvoiceQueryParser service
/// Tests natural language query parsing and intent detection
/// </summary>
public class InvoiceQueryParserTests
{
    private readonly InvoiceQueryParser _parser;

    public InvoiceQueryParserTests()
    {
        _parser = new InvoiceQueryParser();
    }

    #region Vendor Search Tests

    [Fact]
    public void ParseQuery_VendorByName_ReturnsVendorSearchIntent()
    {
        // Arrange
        var query = "Show me all invoices from vendor ABC Corp";

        // Act
        var result = _parser.ParseQuery(query);

        // Assert
        Assert.Equal("VendorSearch", result.IntentType);
        Assert.True(result.Parameters.ContainsKey("vendorName"));
        Assert.Equal("ABC Corp", result.Parameters["vendorName"]);
    }

    [Fact]
    public void ParseQuery_VendorByCode_ReturnsVendorSearchIntent()
    {
        // Arrange
        var query = "Find invoices for vendor V207";

        // Act
        var result = _parser.ParseQuery(query);

        // Assert
        Assert.Equal("VendorSearch", result.IntentType);
        Assert.True(result.Parameters.ContainsKey("vendorName"));
    }

    #endregion

    #region Invoice Number Tests

    [Fact]
    public void ParseQuery_InvoiceNumber_ReturnsInvoiceNumberIntent()
    {
        // Arrange
        var query = "Find invoice number INV-12345";

        // Act
        var result = _parser.ParseQuery(query);

        // Assert
        Assert.Equal("InvoiceNumber", result.IntentType);
        Assert.True(result.Parameters.ContainsKey("invoiceNumber"));
        Assert.Equal("INV-12345", result.Parameters["invoiceNumber"]);
    }

    #endregion

    #region Status Search Tests

    [Fact]
    public void ParseQuery_StatusPending_ReturnsStatusSearchIntent()
    {
        // Arrange
        var query = "Show all pending invoices";

        // Act
        var result = _parser.ParseQuery(query);

        // Assert
        Assert.Equal("StatusSearch", result.IntentType);
        Assert.True(result.Parameters.ContainsKey("status"));
        Assert.Equal("pending", result.Parameters["status"].ToString().ToLower());
    }

    [Fact]
    public void ParseQuery_StatusApproved_ReturnsStatusSearchIntent()
    {
        // Arrange
        var query = "Find approved invoices";

        // Act
        var result = _parser.ParseQuery(query);

        // Assert
        Assert.Equal("StatusSearch", result.IntentType);
        Assert.True(result.Parameters.ContainsKey("status"));
    }

    #endregion

    #region Amount Search Tests

    [Fact]
    public void ParseQuery_AmountGreaterThan_ReturnsAmountSearchIntent()
    {
        // Arrange
        var query = "Show invoices greater than 5000";

        // Act
        var result = _parser.ParseQuery(query);

        // Assert
        Assert.Equal("AmountSearch", result.IntentType);
        Assert.True(result.Parameters.ContainsKey("minAmount"));
        Assert.Equal(5000m, result.Parameters["minAmount"]);
    }

    [Fact]
    public void ParseQuery_AmountBetween_ReturnsAmountSearchIntent()
    {
        // Arrange
        var query = "Find invoices between 1000 and 10000";

        // Act
        var result = _parser.ParseQuery(query);

        // Assert
        Assert.Equal("AmountSearch", result.IntentType);
        Assert.True(result.Parameters.ContainsKey("minAmount"));
        Assert.True(result.Parameters.ContainsKey("maxAmount"));
        Assert.Equal(1000m, result.Parameters["minAmount"]);
        Assert.Equal(10000m, result.Parameters["maxAmount"]);
    }

    [Fact]
    public void ParseQuery_AmountLessThan_ReturnsAmountSearchIntent()
    {
        // Arrange
        var query = "Get invoices less than 500";

        // Act
        var result = _parser.ParseQuery(query);

        // Assert
        Assert.Equal("AmountSearch", result.IntentType);
        Assert.True(result.Parameters.ContainsKey("maxAmount"));
        Assert.Equal(500m, result.Parameters["maxAmount"]);
    }

    #endregion

    #region Date Search Tests

    [Fact]
    public void ParseQuery_DateThisMonth_ReturnsDateSearchIntent()
    {
        // Arrange
        var query = "Show invoices from this month";

        // Act
        var result = _parser.ParseQuery(query);

        // Assert
        Assert.Equal("DateSearch", result.IntentType);
        Assert.True(result.Parameters.ContainsKey("startDate"));
        Assert.True(result.Parameters.ContainsKey("endDate"));
    }

    #endregion
}

