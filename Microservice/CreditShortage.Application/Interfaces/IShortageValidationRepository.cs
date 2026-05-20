using CreditShortage.Domain.Entities;

namespace CreditShortage.Application.Interfaces;

/// <summary>
/// Repository interface for shortage validation data access
/// </summary>
public interface IShortageValidationRepository
{
    /// <summary>
    /// Executes the stored procedure to validate shortage items
    /// </summary>
    Task<List<ShortageValidationResult>> ValidateShortageItemsAsync(
        List<ShortageValidationInput> items,
        string environment,
        string defaultDefectCode,
        string defaultLocationCode);

    /// <summary>
    /// Gets active defect codes from the database
    /// </summary>
    Task<List<DefectCode>> GetActiveDefectCodesAsync();

    /// <summary>
    /// Gets active location codes from the database
    /// </summary>
    Task<List<LocationCode>> GetActiveLocationCodesAsync();
}
