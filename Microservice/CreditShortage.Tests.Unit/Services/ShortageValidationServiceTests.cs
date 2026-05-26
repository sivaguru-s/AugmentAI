using CreditShortage.Application.DTOs;
using CreditShortage.Application.Interfaces;
using CreditShortage.Application.Services;
using CreditShortage.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CreditShortage.Tests.Unit.Services;

/// <summary>
/// Unit tests for ShortageValidationService
/// </summary>
public class ShortageValidationServiceTests
{
    private readonly Mock<IShortageValidationRepository> _repositoryMock;
    private readonly Mock<IIWSIntegrationService> _iwsServiceMock;
    private readonly Mock<ILogger<ShortageValidationService>> _loggerMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly ShortageValidationService _sut;

    public ShortageValidationServiceTests()
    {
        _repositoryMock = new Mock<IShortageValidationRepository>();
        _iwsServiceMock = new Mock<IIWSIntegrationService>();
        _loggerMock = new Mock<ILogger<ShortageValidationService>>();
        _configurationMock = new Mock<IConfiguration>();

        // Setup default configuration
        _configurationMock.Setup(c => c["ShortageValidationSettings:DefaultDefectCode"]).Returns("XP");
        _configurationMock.Setup(c => c["ShortageValidationSettings:DefaultLocationCode"]).Returns("WU");
        _configurationMock.Setup(c => c["ShortageValidationSettings:DefaultEnvironment"]).Returns("AFI");

        _sut = new ShortageValidationService(
            _repositoryMock.Object,
            _iwsServiceMock.Object,
            _loggerMock.Object,
            _configurationMock.Object);
    }

    #region ValidateSingleItemAsync Tests

    [Fact]
    public async Task ValidateSingleItemAsync_WithValidRequest_ReturnsSuccessResponse()
    {
        // Arrange
        var request = new ShortageItemRequest
        {
            CustomerNumber = "2067300",
            ShipToNumber = "0001",
            ItemNumber = "D425-325",
            SerialNumber = "999999",
            OrderNumber = 12345678,
            InvoiceNumber = 12345678,
            ShortageQuantity = 1
        };

        var validationResult = new ShortageValidationResult
        {
            CustomerNumber = request.CustomerNumber,
            ShipToNumber = request.ShipToNumber,
            ItemNumber = request.ItemNumber,
            SerialNumber = request.SerialNumber,
            OrderNumber = request.OrderNumber,
            InvoiceNumber = request.InvoiceNumber,
            ShortageQuantity = request.ShortageQuantity,
            ItemExists = true,
            CustomerSerialItemValid = true,
            DefectCodeValid = true,
            LocationCodeValid = true,
            IsValid = true,
            ValidationErrors = ""
        };

        _repositoryMock
            .Setup(r => r.ValidateShortageItemsAsync(
                It.IsAny<List<ShortageValidationInput>>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
            .ReturnsAsync(new List<ShortageValidationResult> { validationResult });

        // Act
        var result = await _sut.ValidateSingleItemAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
        result.ItemNumber.Should().Be(request.ItemNumber);
        result.SerialNumber.Should().Be(request.SerialNumber);
        result.Flags.Should().NotBeNull();
        result.Flags.ItemExists.Should().BeTrue();
        result.Flags.CustomerSerialItemValid.Should().BeTrue();

        _repositoryMock.Verify(
            r => r.ValidateShortageItemsAsync(
                It.Is<List<ShortageValidationInput>>(list => list.Count == 1),
                "AFI",
                "XP",
                "WU"),
            Times.Once);
    }

    [Fact]
    public async Task ValidateSingleItemAsync_WithInvalidItem_ReturnsFailureResponse()
    {
        // Arrange
        var request = new ShortageItemRequest
        {
            CustomerNumber = "2067300",
            ShipToNumber = "0001",
            ItemNumber = "INVALID",
            SerialNumber = "999999",
            OrderNumber = 12345678,
            InvoiceNumber = 12345678,
            ShortageQuantity = 1
        };

        var validationResult = new ShortageValidationResult
        {
            CustomerNumber = request.CustomerNumber,
            ShipToNumber = request.ShipToNumber,
            ItemNumber = request.ItemNumber,
            SerialNumber = request.SerialNumber,
            ShortageQuantity = request.ShortageQuantity,
            ItemExists = false, // Item not found
            CustomerSerialItemValid = false,
            DefectCodeValid = true,
            LocationCodeValid = true,
            IsValid = false,
            ValidationErrors = "Item not found in master items"
        };

        _repositoryMock
            .Setup(r => r.ValidateShortageItemsAsync(
                It.IsAny<List<ShortageValidationInput>>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
            .ReturnsAsync(new List<ShortageValidationResult> { validationResult });

        // Act
        var result = await _sut.ValidateSingleItemAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Flags.ItemExists.Should().BeFalse();
        result.ValidationErrors.Should().Contain("Item not found in master items");
    }

    [Fact]
    public async Task ValidateSingleItemAsync_WhenRepositoryReturnsEmpty_ThrowsException()
    {
        // Arrange
        var request = new ShortageItemRequest
        {
            CustomerNumber = "2067300",
            ShipToNumber = "0001",
            ItemNumber = "D425-325",
            SerialNumber = "999999",
            OrderNumber = 12345678,
            InvoiceNumber = 12345678,
            ShortageQuantity = 1
        };

        _repositoryMock
            .Setup(r => r.ValidateShortageItemsAsync(
                It.IsAny<List<ShortageValidationInput>>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
            .ReturnsAsync(new List<ShortageValidationResult>());

        // Act & Assert
        await _sut.Invoking(s => s.ValidateSingleItemAsync(request))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Validation returned no results");
    }

    #endregion

    #region ValidateBatchAsync Tests

    [Fact]
    public async Task ValidateBatchAsync_WithMultipleValidItems_ReturnsAllResults()
    {
        // Arrange
        var request = new BatchShortageValidationRequest
        {
            Items = new List<ShortageItemRequest>
            {
                new() { CustomerNumber = "2067300", ShipToNumber = "0001", ItemNumber = "ITEM001", SerialNumber = "999999", OrderNumber = 12345678, InvoiceNumber = 12345678, ShortageQuantity = 1 },
                new() { CustomerNumber = "2067300", ShipToNumber = "0001", ItemNumber = "ITEM002", SerialNumber = "999999", OrderNumber = 12345679, InvoiceNumber = 12345679, ShortageQuantity = 2 }
            }
        };

        var validationResults = new List<ShortageValidationResult>
        {
            new() { ItemNumber = "ITEM001", ItemExists = true, CustomerSerialItemValid = true, DefectCodeValid = true, LocationCodeValid = true, IsValid = true, ValidationErrors = "" },
            new() { ItemNumber = "ITEM002", ItemExists = true, CustomerSerialItemValid = true, DefectCodeValid = true, LocationCodeValid = true, IsValid = true, ValidationErrors = "" }
        };

        _repositoryMock
            .Setup(r => r.ValidateShortageItemsAsync(
                It.IsAny<List<ShortageValidationInput>>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
            .ReturnsAsync(validationResults);

        // Act
        var result = await _sut.ValidateBatchAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Results.Should().HaveCount(2);
        result.Results.Should().AllSatisfy(r => r.IsValid.Should().BeTrue());
        result.ValidatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));

        _repositoryMock.Verify(
            r => r.ValidateShortageItemsAsync(
                It.Is<List<ShortageValidationInput>>(list => list.Count == 2),
                "AFI",
                "XP",
                "WU"),
            Times.Once);
    }

    [Fact]
    public async Task ValidateBatchAsync_WithMixedResults_ReturnsBothValidAndInvalid()
    {
        // Arrange
        var request = new BatchShortageValidationRequest
        {
            Items = new List<ShortageItemRequest>
            {
                new() { CustomerNumber = "2067300", ShipToNumber = "0001", ItemNumber = "VALID", SerialNumber = "999999", OrderNumber = 12345678, InvoiceNumber = 12345678, ShortageQuantity = 1 },
                new() { CustomerNumber = "2067300", ShipToNumber = "0001", ItemNumber = "INVALID", SerialNumber = "999999", OrderNumber = 12345679, InvoiceNumber = 12345679, ShortageQuantity = 2 }
            }
        };

        var validationResults = new List<ShortageValidationResult>
        {
            new() { ItemNumber = "VALID", ItemExists = true, CustomerSerialItemValid = true, DefectCodeValid = true, LocationCodeValid = true, IsValid = true, ValidationErrors = "" },
            new() { ItemNumber = "INVALID", ItemExists = false, CustomerSerialItemValid = false, DefectCodeValid = true, LocationCodeValid = true, IsValid = false, ValidationErrors = "Item not found" }
        };

        _repositoryMock
            .Setup(r => r.ValidateShortageItemsAsync(
                It.IsAny<List<ShortageValidationInput>>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
            .ReturnsAsync(validationResults);

        // Act
        var result = await _sut.ValidateBatchAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Results.Should().HaveCount(2);
        result.Results.Count(r => r.IsValid).Should().Be(1);
        result.Results.Count(r => !r.IsValid).Should().Be(1);
    }

    #endregion

    #region Configuration Tests

    [Fact]
    public async Task ValidateSingleItemAsync_UsesCustomEnvironment_WhenProvided()
    {
        // Arrange
        var request = new ShortageItemRequest
        {
            CustomerNumber = "2067300",
            ShipToNumber = "0001",
            ItemNumber = "D425-325",
            SerialNumber = "999999",
            OrderNumber = 12345678,
            InvoiceNumber = 12345678,
            ShortageQuantity = 1,
            Environment = "CUSTOM_ENV"
        };

        var validationResult = new ShortageValidationResult
        {
            ItemNumber = request.ItemNumber,
            ItemExists = true,
            CustomerSerialItemValid = true,
            DefectCodeValid = true,
            LocationCodeValid = true,
            IsValid = true,
            ValidationErrors = ""
        };

        _repositoryMock
            .Setup(r => r.ValidateShortageItemsAsync(
                It.IsAny<List<ShortageValidationInput>>(),
                "CUSTOM_ENV",
                It.IsAny<string>(),
                It.IsAny<string>()))
            .ReturnsAsync(new List<ShortageValidationResult> { validationResult });

        // Act
        await _sut.ValidateSingleItemAsync(request);

        // Assert
        _repositoryMock.Verify(
            r => r.ValidateShortageItemsAsync(
                It.IsAny<List<ShortageValidationInput>>(),
                "CUSTOM_ENV",
                "XP",
                "WU"),
            Times.Once);
    }

    #endregion
}
