using CreditShortage.Application.DTOs;

namespace CreditShortage.Application.Interfaces;

/// <summary>
/// Service interface for reference data (defect codes, locations, etc.)
/// </summary>
public interface IReferenceDataService
{
    /// <summary>
    /// Gets all active defect codes
    /// </summary>
    Task<List<DefectCodeDto>> GetActiveDefectCodesAsync();

    /// <summary>
    /// Gets all active location codes
    /// </summary>
    Task<List<LocationCodeDto>> GetActiveLocationCodesAsync();

    /// <summary>
    /// Gets default settings
    /// </summary>
    DefaultSettingsDto GetDefaultSettings();
}
