using CreditShortage.Application.Interfaces;
using CreditShortage.Application.Services;
using CreditShortage.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CreditShortage.Tests.Unit.Services;

/// <summary>
/// Unit tests for ReferenceDataService
/// </summary>
public class ReferenceDataServiceTests
{
    private readonly Mock<IShortageValidationRepository> _repositoryMock;
    private readonly Mock<ILogger<ReferenceDataService>> _loggerMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly ReferenceDataService _sut;

    public ReferenceDataServiceTests()
    {
        _repositoryMock = new Mock<IShortageValidationRepository>();
        _loggerMock = new Mock<ILogger<ReferenceDataService>>();
        _configurationMock = new Mock<IConfiguration>();

        // Setup default configuration
        _configurationMock.Setup(c => c["ShortageValidationSettings:DefaultDefectCode"]).Returns("XP");
        _configurationMock.Setup(c => c["ShortageValidationSettings:DefaultLocationCode"]).Returns("WU");
        _configurationMock.Setup(c => c["ShortageValidationSettings:DefaultEnvironment"]).Returns("AFI");
        _configurationMock.Setup(c => c["ShortageValidationSettings:MaxBatchSize"]).Returns("500");

        _sut = new ReferenceDataService(
            _repositoryMock.Object,
            _loggerMock.Object,
            _configurationMock.Object);
    }

    #region GetActiveDefectCodesAsync Tests

    [Fact]
    public async Task GetActiveDefectCodesAsync_ReturnsDefectCodes_WithDefaultMarked()
    {
        // Arrange
        var defectCodes = new List<DefectCode>
        {
            new() { Code = "XP", Description = "Excessive Packaging", IsActive = true },
            new() { Code = "DM", Description = "Damaged", IsActive = true },
            new() { Code = "MI", Description = "Missing Item", IsActive = true }
        };

        _repositoryMock
            .Setup(r => r.GetActiveDefectCodesAsync())
            .ReturnsAsync(defectCodes);

        // Act
        var result = await _sut.GetActiveDefectCodesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.Should().Contain(dc => dc.Code == "XP" && dc.IsDefault);
        result.Should().Contain(dc => dc.Code == "DM" && !dc.IsDefault);
        result.Should().Contain(dc => dc.Code == "MI" && !dc.IsDefault);

        _repositoryMock.Verify(r => r.GetActiveDefectCodesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetActiveDefectCodesAsync_WhenNoCodesExist_ReturnsEmptyList()
    {
        // Arrange
        _repositoryMock
            .Setup(r => r.GetActiveDefectCodesAsync())
            .ReturnsAsync(new List<DefectCode>());

        // Act
        var result = await _sut.GetActiveDefectCodesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetActiveDefectCodesAsync_MapsAllProperties_Correctly()
    {
        // Arrange
        var defectCodes = new List<DefectCode>
        {
            new() { Code = "XP", Description = "Excessive Packaging", IsActive = true }
        };

        _repositoryMock
            .Setup(r => r.GetActiveDefectCodesAsync())
            .ReturnsAsync(defectCodes);

        // Act
        var result = await _sut.GetActiveDefectCodesAsync();

        // Assert
        var dto = result.First();
        dto.Code.Should().Be("XP");
        dto.Description.Should().Be("Excessive Packaging");
        dto.IsActive.Should().BeTrue();
        dto.IsDefault.Should().BeTrue();
    }

    #endregion

    #region GetActiveLocationCodesAsync Tests

    [Fact]
    public async Task GetActiveLocationCodesAsync_ReturnsLocationCodes_WithDefaultMarked()
    {
        // Arrange
        var locationCodes = new List<LocationCode>
        {
            new() { Code = "WU", Description = "Warehouse Unit 1", IsActive = true },
            new() { Code = "W2", Description = "Warehouse Unit 2", IsActive = true },
            new() { Code = "W3", Description = "Warehouse Unit 3", IsActive = true }
        };

        _repositoryMock
            .Setup(r => r.GetActiveLocationCodesAsync())
            .ReturnsAsync(locationCodes);

        // Act
        var result = await _sut.GetActiveLocationCodesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.Should().Contain(lc => lc.Code == "WU" && lc.IsDefault);
        result.Should().Contain(lc => lc.Code == "W2" && !lc.IsDefault);
        result.Should().Contain(lc => lc.Code == "W3" && !lc.IsDefault);

        _repositoryMock.Verify(r => r.GetActiveLocationCodesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetActiveLocationCodesAsync_WhenNoCodesExist_ReturnsEmptyList()
    {
        // Arrange
        _repositoryMock
            .Setup(r => r.GetActiveLocationCodesAsync())
            .ReturnsAsync(new List<LocationCode>());

        // Act
        var result = await _sut.GetActiveLocationCodesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    #endregion

    #region GetDefaultSettings Tests

    [Fact]
    public void GetDefaultSettings_ReturnsConfiguredDefaults()
    {
        // Act
        var result = _sut.GetDefaultSettings();

        // Assert
        result.Should().NotBeNull();
        result.DefaultDefectCode.Should().Be("XP");
        result.DefaultLocationCode.Should().Be("WU");
        result.DefaultEnvironment.Should().Be("AFI");
        result.MaxBatchSize.Should().Be(500);
    }

    [Fact]
    public void GetDefaultSettings_UsesHardcodedDefaults_WhenConfigMissing()
    {
        // Arrange
        var configMock = new Mock<IConfiguration>();
        configMock.Setup(c => c["ShortageValidationSettings:DefaultDefectCode"]).Returns((string?)null);
        configMock.Setup(c => c["ShortageValidationSettings:DefaultLocationCode"]).Returns((string?)null);
        configMock.Setup(c => c["ShortageValidationSettings:DefaultEnvironment"]).Returns((string?)null);
        configMock.Setup(c => c["ShortageValidationSettings:MaxBatchSize"]).Returns((string?)null);

        var service = new ReferenceDataService(_repositoryMock.Object, _loggerMock.Object, configMock.Object);

        // Act
        var result = service.GetDefaultSettings();

        // Assert
        result.DefaultDefectCode.Should().Be("XP");
        result.DefaultLocationCode.Should().Be("WU");
        result.DefaultEnvironment.Should().Be("AFI");
        result.MaxBatchSize.Should().Be(500);
    }

    #endregion
}
