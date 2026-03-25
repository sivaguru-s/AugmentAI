namespace FinanceBudget.Models;

/// <summary>
/// AFE Budget model representing project budget information from Excel
/// Links AFE budgets to Unit and Nature cost details
/// </summary>
public class AFEBudget
{
    public string Project { get; set; } = string.Empty;
    public string AFENumber { get; set; } = string.Empty;
    public string ProjectDescription { get; set; } = string.Empty;
    public string Owner { get; set; } = string.Empty;
    public string AFEListing { get; set; } = string.Empty;
    public decimal ApprovedAmount { get; set; }
    public DateTime? EstimatedCompletion { get; set; }
    public decimal CommittedAmount { get; set; }
    public decimal Forecast { get; set; }
    public decimal PercentSpent { get; set; }
    public decimal LinkToCost { get; set; }
    public decimal Other { get; set; }
    public string CPApproved { get; set; } = string.Empty;
    public decimal RevisedAFE { get; set; }
    public decimal Cap { get; set; }
    public decimal Opex { get; set; }
    public string Notes { get; set; } = string.Empty;
    public string UnitDescription { get; set; } = string.Empty;
    
    // Linking properties
    public string Unit { get; set; } = string.Empty;
    public string Nature { get; set; } = string.Empty;
    public string BusinessVertical { get; set; } = string.Empty;
    
    // Calculated properties
    public decimal RemainingAmount => ApprovedAmount - CommittedAmount;
    public decimal VarianceToForecast => ApprovedAmount - Forecast;
}

/// <summary>
/// AFE Budget to Unit Nature mapping
/// Links AFE projects to specific unit and nature combinations
/// </summary>
public class AFEUnitNatureMapping
{
    public int Id { get; set; }
    public string AFENumber { get; set; } = string.Empty;
    public string Project { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public string Nature { get; set; } = string.Empty;
    public decimal AllocationPercentage { get; set; }
    public string BusinessVertical { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
}

/// <summary>
/// Business Vertical definition
/// Represents organizational business verticals for cost mapping
/// </summary>
public class BusinessVertical
{
    public int Id { get; set; }
    public string VerticalCode { get; set; } = string.Empty;
    public string VerticalName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

/// <summary>
/// Project to Business Vertical mapping
/// Maps projects to business verticals instead of individual cost centers
/// </summary>
public class ProjectVerticalMapping
{
    public int Id { get; set; }
    public string Project { get; set; } = string.Empty;
    public string AFENumber { get; set; } = string.Empty;
    public string BusinessVerticalCode { get; set; } = string.Empty;
    public decimal AllocationPercentage { get; set; }
    public DateTime EffectiveDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Notes { get; set; } = string.Empty;
}

