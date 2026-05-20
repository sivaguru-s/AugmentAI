namespace CreditShortage.Application.Interfaces;

/// <summary>
/// Service interface for IWS (Inventory Warehouse System) integration
/// </summary>
public interface IIWSIntegrationService
{
    /// <summary>
    /// Gets serial number from IWS for a given customer and invoice
    /// </summary>
    /// <param name="customerNumber">Customer number</param>
    /// <param name="invoiceNumber">Invoice number</param>
    /// <param name="itemNumber">Item number</param>
    /// <returns>Serial number obtained from IWS</returns>
    Task<string> GetSerialNumberAsync(string customerNumber, int invoiceNumber, string itemNumber);

    /// <summary>
    /// Gets order details from IWS
    /// </summary>
    Task<IWSOrderDetail> GetOrderDetailAsync(string customerNumber, int invoiceNumber);
}

/// <summary>
/// IWS order detail response
/// </summary>
public class IWSOrderDetail
{
    public string SerialNumber { get; set; } = string.Empty;
    public int OrderNumber { get; set; }
    public string ItemNumber { get; set; } = string.Empty;
    public int OrderedQuantity { get; set; }
    public DateTime OrderDate { get; set; }
}
