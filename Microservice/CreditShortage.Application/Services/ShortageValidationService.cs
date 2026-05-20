using CreditShortage.Application.DTOs;
using CreditShortage.Application.Interfaces;
using CreditShortage.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace CreditShortage.Application.Services;

/// <summary>
/// Implementation of shortage validation service
/// </summary>
public class ShortageValidationService : IShortageValidationService
{
    private readonly IShortageValidationRepository _repository;
    private readonly IIWSIntegrationService _iwsService;
    private readonly ILogger<ShortageValidationService> _logger;
    private readonly string _defaultDefectCode;
    private readonly string _defaultLocationCode;
    private readonly string _defaultEnvironment;

    public ShortageValidationService(
        IShortageValidationRepository repository,
        IIWSIntegrationService iwsService,
        ILogger<ShortageValidationService> logger,
        Microsoft.Extensions.Configuration.IConfiguration configuration)
    {
        _repository = repository;
        _iwsService = iwsService;
        _logger = logger;
        _defaultDefectCode = configuration["ShortageValidationSettings:DefaultDefectCode"] ?? "XP";
        _defaultLocationCode = configuration["ShortageValidationSettings:DefaultLocationCode"] ?? "WU";
        _defaultEnvironment = configuration["ShortageValidationSettings:DefaultEnvironment"] ?? "AFI";
    }

    public async Task<ShortageValidationResponse> ValidateSingleItemAsync(ShortageItemRequest request)
    {
        _logger.LogInformation("Validating single shortage item for customer {Customer}, invoice {Invoice}, item {Item}",
            request.CustomerNumber, request.InvoiceNumber, request.ItemNumber);

        var input = MapToValidationInput(request);
        var environment = request.Environment ?? _defaultEnvironment;

        var results = await _repository.ValidateShortageItemsAsync(
            new List<ShortageValidationInput> { input },
            environment,
            _defaultDefectCode,
            _defaultLocationCode);

        if (results.Count == 0)
        {
            throw new InvalidOperationException("Validation returned no results");
        }

        return MapToResponse(results[0], false);
    }

    public async Task<BatchShortageValidationResponse> ValidateBatchAsync(BatchShortageValidationRequest request)
    {
        _logger.LogInformation("Validating batch of {Count} shortage items", request.Items.Count);

        var inputs = request.Items.Select(MapToValidationInput).ToList();
        var environment = request.Environment ?? _defaultEnvironment;

        var results = await _repository.ValidateShortageItemsAsync(
            inputs,
            environment,
            _defaultDefectCode,
            _defaultLocationCode);

        return new BatchShortageValidationResponse
        {
            Results = results.Select(r => MapToResponse(r, false)).ToList(),
            ValidatedAt = DateTime.UtcNow
        };
    }

    public async Task<ShortageValidationResponse> ValidateWithIWSAsync(ShortageItemIWSRequest request)
    {
        _logger.LogInformation("Validating shortage item with IWS integration for customer {Customer}, invoice {Invoice}",
            request.CustomerNumber, request.InvoiceNumber);

        // Step 1: Get serial number from IWS
        string serialNumber;
        try
        {
            serialNumber = await _iwsService.GetSerialNumberAsync(
                request.CustomerNumber,
                request.InvoiceNumber,
                request.ItemNumber);

            _logger.LogInformation("Obtained serial number {Serial} from IWS", serialNumber);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to obtain serial number from IWS");
            throw new InvalidOperationException("Failed to obtain serial number from IWS. " + ex.Message, ex);
        }

        // Step 2: Create validation request with IWS serial number
        var validationRequest = new ShortageItemRequest
        {
            CustomerNumber = request.CustomerNumber,
            ShipToNumber = request.ShipToNumber,
            InvoiceNumber = request.InvoiceNumber,
            ItemNumber = request.ItemNumber,
            SerialNumber = serialNumber,
            ShortageQuantity = request.ShortageQuantity,
            DefectCode = request.DefectCode,
            LocationCode = request.LocationCode,
            OrderNumber = request.OrderNumber,
            OrderItemSeq = request.OrderItemSeq,
            Environment = request.Environment
        };

        // Step 3: Validate
        var response = await ValidateSingleItemAsync(validationRequest);
        response.SerialFromIWS = true;

        return response;
    }

    public async Task<ValidationStatistics> GetValidationStatisticsAsync()
    {
        // This would typically query a validation log table
        // For now, returning placeholder statistics
        _logger.LogInformation("Fetching validation statistics");

        return new ValidationStatistics
        {
            TotalValidationsToday = 0,
            SuccessfulValidationsToday = 0,
            FailedValidationsToday = 0,
            SuccessRate = 0,
            TopFailureReasons = new Dictionary<string, int>()
        };
    }

    private ShortageValidationInput MapToValidationInput(ShortageItemRequest request)
    {
        return new ShortageValidationInput
        {
            CustomerNumber = request.CustomerNumber,
            ShipToNumber = request.ShipToNumber,
            InvoiceNumber = request.InvoiceNumber,
            ItemNumber = request.ItemNumber,
            SerialNumber = request.SerialNumber,
            ShortageQuantity = request.ShortageQuantity,
            DefectCode = request.DefectCode,
            LocationCode = request.LocationCode,
            OrderNumber = request.OrderNumber,
            OrderItemSeq = request.OrderItemSeq
        };
    }

    private ShortageValidationResponse MapToResponse(ShortageValidationResult result, bool serialFromIWS)
    {
        return new ShortageValidationResponse
        {
            CustomerNumber = result.CustomerNumber,
            ShipToNumber = result.ShipToNumber,
            InvoiceNumber = result.InvoiceNumber,
            ItemNumber = result.ItemNumber,
            SerialNumber = result.SerialNumber,
            ShortageQuantity = result.ShortageQuantity,
            DefectCode = result.DefectCode,
            LocationCode = result.LocationCode,
            OrderNumber = result.OrderNumber,
            OrderItemSeq = result.OrderItemSeq,
            IsValid = result.IsValid,
            ValidationErrors = result.ValidationErrors,
            OrderedQuantity = result.OrderedQuantity,
            AlreadyCreditedQuantity = result.AlreadyCreditedQuantity,
            RemainingCreditableQuantity = result.RemainingCreditableQuantity,
            Flags = new ValidationFlags
            {
                ItemExists = result.ItemExists,
                CustomerSerialItemValid = result.CustomerSerialItemValid,
                DefectCodeValid = result.DefectCodeValid,
                LocationCodeValid = result.LocationCodeValid
            },
            ValidatedAt = DateTime.UtcNow,
            SerialFromIWS = serialFromIWS
        };
    }
}
