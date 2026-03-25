namespace FinanceBudget.Models;

public class DashboardViewModel
{
    public List<UnitBudget> UnitBudgets { get; set; } = new();
    public List<AFEData> AFEDataList { get; set; } = new();
    public List<JiraCosting> JiraCostings { get; set; } = new();

    // New: AFE Budget and Planful data
    public List<AFEBudget> AFEBudgets { get; set; } = new();
    public List<PlanfulSummary> PlanfulSummaries { get; set; } = new();
    public List<PlanfulDetail> PlanfulDetails { get; set; } = new();
    public List<PlanfulIntegrationSummary> PlanfulIntegrationSummaries { get; set; } = new();

    // Business Vertical data
    public List<BusinessVertical> BusinessVerticals { get; set; } = new();
    public List<ProjectVerticalMapping> ProjectVerticalMappings { get; set; } = new();
    public List<AFEUnitNatureMapping> AFEUnitNatureMappings { get; set; } = new();

    // Dropdown data
    public List<Unit> AvailableUnits { get; set; } = new();
    public List<Nature> AvailableNatures { get; set; } = new();

    // Summary Statistics
    public decimal TotalBudgetAmount { get; set; }
    public decimal TotalBudgetCredit { get; set; }
    public decimal TotalBudgetDebit { get; set; }
    public decimal TotalAFEApproved { get; set; }
    public decimal TotalAFESpent { get; set; }
    public decimal TotalJiraEstimated { get; set; }
    public decimal TotalJiraActual { get; set; }

    // New: Planful and AFE Budget statistics
    public decimal TotalPlanfulBudget { get; set; }
    public decimal TotalPlanfulActual { get; set; }
    public decimal TotalPlanfulForecast { get; set; }
    public decimal TotalAFEBudgetApproved { get; set; }
    public decimal TotalAFEBudgetCommitted { get; set; }
    public decimal TotalAFEBudgetForecast { get; set; }

    // Unit-wise aggregation
    public Dictionary<string, UnitSummary> UnitSummaries { get; set; } = new();

    // Business Vertical aggregation
    public Dictionary<string, VerticalSummary> VerticalSummaries { get; set; } = new();
}

public class UnitSummary
{
    public string Unit { get; set; } = string.Empty;
    public string UnitDescription { get; set; } = string.Empty;
    public decimal BudgetAmount { get; set; }
    public decimal AFEAmount { get; set; }
    public decimal JiraAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public int TransactionCount { get; set; }
    public int AFECount { get; set; }
    public int JiraIssueCount { get; set; }

    // New: Planful and AFE Budget amounts
    public decimal PlanfulBudget { get; set; }
    public decimal PlanfulActual { get; set; }
    public decimal AFEBudgetApproved { get; set; }
    public decimal AFEBudgetCommitted { get; set; }
}

/// <summary>
/// Business Vertical Summary
/// Aggregated costs mapped to business verticals instead of individual cost centers
/// </summary>
public class VerticalSummary
{
    public string VerticalCode { get; set; } = string.Empty;
    public string VerticalName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    // Financial metrics
    public decimal TotalBudget { get; set; }
    public decimal TotalActual { get; set; }
    public decimal TotalForecast { get; set; }
    public decimal TotalVariance { get; set; }

    // Breakdown by source
    public decimal AS400Budget { get; set; }
    public decimal PlanfulBudget { get; set; }
    public decimal AFEBudget { get; set; }
    public decimal JiraBudget { get; set; }

    // Counts
    public int ProjectCount { get; set; }
    public int UnitCount { get; set; }
    public int AFECount { get; set; }

    // Calculated properties
    public decimal VariancePercentage => TotalBudget != 0 ? (TotalVariance / TotalBudget) * 100 : 0;
    public decimal UtilizationPercentage => TotalBudget != 0 ? (TotalActual / TotalBudget) * 100 : 0;
}

