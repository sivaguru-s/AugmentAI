namespace CreditShortage.Application.DTOs;

/// <summary>
/// Defect code reference data
/// </summary>
public class DefectCodeDto
{
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public bool IsDefault { get; set; }
}

/// <summary>
/// Location/warehouse code reference data
/// </summary>
public class LocationCodeDto
{
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public bool IsDefault { get; set; }
}

/// <summary>
/// Default settings
/// </summary>
public class DefaultSettingsDto
{
    public string DefaultDefectCode { get; set; } = "XP";
    public string DefaultLocationCode { get; set; } = "WU";
    public string DefaultEnvironment { get; set; } = "AFI";
    public int MaxBatchSize { get; set; } = 500;
}
