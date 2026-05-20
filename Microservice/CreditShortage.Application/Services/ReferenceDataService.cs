using CreditShortage.Application.DTOs;
using CreditShortage.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CreditShortage.Application.Services;

/// <summary>
/// Implementation of reference data service
/// </summary>
public class ReferenceDataService : IReferenceDataService
{
    private readonly IShortageValidationRepository _repository;
    private readonly ILogger<ReferenceDataService> _logger;
    private readonly IConfiguration _configuration;

    public ReferenceDataService(
        IShortageValidationRepository repository,
        ILogger<ReferenceDataService> logger,
        IConfiguration configuration)
    {
        _repository = repository;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<List<DefectCodeDto>> GetActiveDefectCodesAsync()
    {
        _logger.LogInformation("Fetching active defect codes from database");

        var defectCodes = await _repository.GetActiveDefectCodesAsync();

        var defaultCode = _configuration["ShortageValidationSettings:DefaultDefectCode"] ?? "XP";

        return defectCodes.Select(dc => new DefectCodeDto
        {
            Code = dc.Code,
            Description = dc.Description,
            IsActive = dc.IsActive,
            IsDefault = dc.Code == defaultCode
        }).ToList();
    }

    public async Task<List<LocationCodeDto>> GetActiveLocationCodesAsync()
    {
        _logger.LogInformation("Fetching active location codes from database");

        var locationCodes = await _repository.GetActiveLocationCodesAsync();

        var defaultCode = _configuration["ShortageValidationSettings:DefaultLocationCode"] ?? "WU";

        return locationCodes.Select(lc => new LocationCodeDto
        {
            Code = lc.Code,
            Description = lc.Description,
            IsActive = lc.IsActive,
            IsDefault = lc.Code == defaultCode
        }).ToList();
    }

    public DefaultSettingsDto GetDefaultSettings()
    {
        _logger.LogInformation("Fetching default settings from configuration");

        return new DefaultSettingsDto
        {
            DefaultDefectCode = _configuration["ShortageValidationSettings:DefaultDefectCode"] ?? "XP",
            DefaultLocationCode = _configuration["ShortageValidationSettings:DefaultLocationCode"] ?? "WU",
            DefaultEnvironment = _configuration["ShortageValidationSettings:DefaultEnvironment"] ?? "AFI",
            MaxBatchSize = int.Parse(_configuration["ShortageValidationSettings:MaxBatchSize"] ?? "500")
        };
    }
}
