namespace FinanceBudget.Models;

/// <summary>
/// Planful Summary - Nature-wise total spend (monthly, by type such as salary, software)
/// Represents aggregated spending data from Planful
/// </summary>
public class PlanfulSummary
{
    public string Nature { get; set; } = string.Empty;
    public string NatureDescription { get; set; } = string.Empty;
    public string SpendType { get; set; } = string.Empty; // e.g., Salary, Software, Hardware
    public int Year { get; set; }
    public int Month { get; set; }
    public decimal TotalSpend { get; set; }
    public decimal BudgetAmount { get; set; }
    public decimal Variance { get; set; }
    public string Currency { get; set; } = "USD";
    
    // Calculated properties
    public decimal VariancePercentage => BudgetAmount != 0 ? (Variance / BudgetAmount) * 100 : 0;
    public string MonthName => new DateTime(Year, Month, 1).ToString("MMMM");
    public string Period => $"{MonthName} {Year}";
}

/// <summary>
/// Planful Detail - Unit-wise costs split from the nature-wise data
/// Represents detailed unit-level spending from Planful
/// </summary>
public class PlanfulDetail
{
    public string Unit { get; set; } = string.Empty;
    public string UnitDescription { get; set; } = string.Empty;
    public string Nature { get; set; } = string.Empty;
    public string NatureDescription { get; set; } = string.Empty;
    public string SpendType { get; set; } = string.Empty;
    public int Year { get; set; }
    public int Month { get; set; }
    public decimal Amount { get; set; }
    public decimal BudgetAmount { get; set; }
    public decimal ForecastAmount { get; set; }
    public decimal ActualAmount { get; set; }
    public string CostCenter { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string BusinessVertical { get; set; } = string.Empty;
    
    // Calculated properties
    public decimal Variance => ActualAmount - BudgetAmount;
    public decimal VariancePercentage => BudgetAmount != 0 ? (Variance / BudgetAmount) * 100 : 0;
    public string Period => $"{new DateTime(Year, Month, 1):MMMM yyyy}";
}

/// <summary>
/// Planful Integration Summary
/// Aggregated view combining Planful Summary and Detail data
/// </summary>
public class PlanfulIntegrationSummary
{
    public string Unit { get; set; } = string.Empty;
    public string UnitDescription { get; set; } = string.Empty;
    public string Nature { get; set; } = string.Empty;
    public string NatureDescription { get; set; } = string.Empty;
    public string BusinessVertical { get; set; } = string.Empty;
    
    // Financial metrics
    public decimal TotalBudget { get; set; }
    public decimal TotalActual { get; set; }
    public decimal TotalForecast { get; set; }
    public decimal TotalVariance { get; set; }
    
    // Breakdown by spend type
    public decimal SalarySpend { get; set; }
    public decimal SoftwareSpend { get; set; }
    public decimal HardwareSpend { get; set; }
    public decimal OtherSpend { get; set; }
    
    // Period information
    public int Year { get; set; }
    public int Month { get; set; }
    public string Period => $"{new DateTime(Year, Month, 1):MMMM yyyy}";
    
    // Calculated properties
    public decimal VariancePercentage => TotalBudget != 0 ? (TotalVariance / TotalBudget) * 100 : 0;
    public decimal UtilizationPercentage => TotalBudget != 0 ? (TotalActual / TotalBudget) * 100 : 0;
}

/// <summary>
/// Planful to AS400 mapping
/// Maps Planful data to AS400 Unit and Nature codes
/// </summary>
public class PlanfulAS400Mapping
{
    public int Id { get; set; }
    public string PlanfulAccountCode { get; set; } = string.Empty;
    public string PlanfulAccountName { get; set; } = string.Empty;
    public string AS400Unit { get; set; } = string.Empty;
    public string AS400Nature { get; set; } = string.Empty;
    public string MappingType { get; set; } = string.Empty; // Direct, Allocated, Derived
    public decimal AllocationPercentage { get; set; }
    public bool IsActive { get; set; }
    public DateTime EffectiveDate { get; set; }
    public DateTime? EndDate { get; set; }
}

