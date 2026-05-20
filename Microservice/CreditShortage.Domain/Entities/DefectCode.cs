namespace CreditShortage.Domain.Entities;

/// <summary>
/// Domain entity for defect code
/// Maps to SQL table: tblDefectCodes
/// </summary>
public class DefectCode
{
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
