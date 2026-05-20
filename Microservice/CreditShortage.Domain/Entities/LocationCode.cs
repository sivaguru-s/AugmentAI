namespace CreditShortage.Domain.Entities;

/// <summary>
/// Domain entity for location/warehouse code
/// Maps to SQL table: tblWarehouse
/// </summary>
public class LocationCode
{
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
