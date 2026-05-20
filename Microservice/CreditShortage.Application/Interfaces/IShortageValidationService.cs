using CreditShortage.Application.DTOs;

namespace CreditShortage.Application.Interfaces;

/// <summary>
/// Service interface for shortage item validation
/// </summary>
public interface IShortageValidationService
{
    /// <summary>
    /// Validates a single shortage item
    /// </summary>
    Task<ShortageValidationResponse> ValidateSingleItemAsync(ShortageItemRequest request);

    /// <summary>
    /// Validates multiple shortage items in a batch
    /// </summary>
    Task<BatchShortageValidationResponse> ValidateBatchAsync(BatchShortageValidationRequest request);

    /// <summary>
    /// Validates a shortage item with IWS integration to obtain serial number
    /// </summary>
    Task<ShortageValidationResponse> ValidateWithIWSAsync(ShortageItemIWSRequest request);

    /// <summary>
    /// Gets validation statistics
    /// </summary>
    Task<ValidationStatistics> GetValidationStatisticsAsync();
}
