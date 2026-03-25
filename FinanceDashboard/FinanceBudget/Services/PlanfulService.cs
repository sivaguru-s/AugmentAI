using FinanceBudget.Models;
using Microsoft.Data.SqlClient;
using Dapper;

namespace FinanceBudget.Services;

/// <summary>
/// Service for Planful data integration from DB2 tables
/// Fetches Planful Summary (nature-wise) and Detail (unit-wise) data
/// </summary>
public class PlanfulService : IPlanfulService
{
    private readonly string _connectionString;
    private readonly ILogger<PlanfulService> _logger;

    public PlanfulService(IConfiguration configuration, ILogger<PlanfulService> logger)
    {
        _connectionString = configuration.GetConnectionString("AS400Database") 
            ?? throw new ArgumentNullException("AS400Database connection string is not configured");
        _logger = logger;
    }

    public async Task<List<PlanfulSummary>> GetPlanfulSummaryAsync(int? year = null, int? month = null, string? nature = null)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            // TODO: Update table names and column names based on actual Planful DB2 tables
            // This is a template query - adjust based on your actual Planful schema
            var query = @"
                EXEC ('SELECT
                    NATURE.AHAFCD as Nature,
                    NATURE.AHADNA as NatureDescription,
                    PLANFUL.SPEND_TYPE as SpendType,
                    PLANFUL.FISCAL_YEAR as Year,
                    PLANFUL.FISCAL_MONTH as Month,
                    PLANFUL.TOTAL_SPEND as TotalSpend,
                    PLANFUL.BUDGET_AMOUNT as BudgetAmount,
                    (PLANFUL.TOTAL_SPEND - PLANFUL.BUDGET_AMOUNT) as Variance
                FROM PLANFUL_SUMMARY as PLANFUL
                INNER JOIN AMFLIBA.YAAHREP as NATURE ON NATURE.AHAFCD = PLANFUL.NATURE_CODE
                WHERE 1=1";

            if (!string.IsNullOrEmpty(nature))
            {
                query += " AND NATURE.AHAFCD = ''" + nature + "''";
            }

            if (year.HasValue)
            {
                query += " AND PLANFUL.FISCAL_YEAR = " + year.Value;
            }

            if (month.HasValue)
            {
                query += " AND PLANFUL.FISCAL_MONTH = " + month.Value;
            }

            query += "') at DB2";

            var result = await connection.QueryAsync<PlanfulSummary>(query);
            return result.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching Planful Summary data from DB2");
            // Return empty list instead of throwing to allow dashboard to load with partial data
            return new List<PlanfulSummary>();
        }
    }

    public async Task<List<PlanfulDetail>> GetPlanfulDetailAsync(string? unitCode = null, string? natureId = null, int? year = null, int? month = null)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            // TODO: Update table names and column names based on actual Planful DB2 tables
            // This is a template query - adjust based on your actual Planful schema
            var query = @"
                EXEC ('SELECT
                    UNIT.ASALCD as Unit,
                    UNIT.ASAQNA as UnitDescription,
                    NATURE.AHAFCD as Nature,
                    NATURE.AHADNA as NatureDescription,
                    PLANFUL.SPEND_TYPE as SpendType,
                    PLANFUL.FISCAL_YEAR as Year,
                    PLANFUL.FISCAL_MONTH as Month,
                    PLANFUL.AMOUNT as Amount,
                    PLANFUL.BUDGET_AMOUNT as BudgetAmount,
                    PLANFUL.FORECAST_AMOUNT as ForecastAmount,
                    PLANFUL.ACTUAL_AMOUNT as ActualAmount,
                    PLANFUL.COST_CENTER as CostCenter,
                    PLANFUL.DEPARTMENT as Department,
                    PLANFUL.BUSINESS_VERTICAL as BusinessVertical
                FROM PLANFUL_DETAIL as PLANFUL
                INNER JOIN AMFLIBA.YAASREP as UNIT ON UNIT.ASALCD = PLANFUL.UNIT_CODE
                INNER JOIN AMFLIBA.YAAHREP as NATURE ON NATURE.AHAFCD = PLANFUL.NATURE_CODE
                WHERE 1=1";

            if (!string.IsNullOrEmpty(unitCode))
            {
                query += " AND UNIT.ASALCD = ''" + unitCode + "''";
            }

            if (!string.IsNullOrEmpty(natureId))
            {
                query += " AND NATURE.AHAFCD = ''" + natureId + "''";
            }

            if (year.HasValue)
            {
                query += " AND PLANFUL.FISCAL_YEAR = " + year.Value;
            }

            if (month.HasValue)
            {
                query += " AND PLANFUL.FISCAL_MONTH = " + month.Value;
            }

            query += "') at DB2";

            var result = await connection.QueryAsync<PlanfulDetail>(query);
            return result.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching Planful Detail data from DB2");
            return new List<PlanfulDetail>();
        }
    }

    public async Task<List<PlanfulIntegrationSummary>> GetPlanfulIntegrationSummaryAsync(string? unitCode = null, string? natureId = null, int? year = null, int? month = null)
    {
        try
        {
            // Get detail data
            var details = await GetPlanfulDetailAsync(unitCode, natureId, year, month);

            // Group and aggregate
            var summaries = details
                .GroupBy(d => new { d.Unit, d.UnitDescription, d.Nature, d.NatureDescription, d.BusinessVertical, d.Year, d.Month })
                .Select(g => new PlanfulIntegrationSummary
                {
                    Unit = g.Key.Unit,
                    UnitDescription = g.Key.UnitDescription,
                    Nature = g.Key.Nature,
                    NatureDescription = g.Key.NatureDescription,
                    BusinessVertical = g.Key.BusinessVertical,
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    TotalBudget = g.Sum(d => d.BudgetAmount),
                    TotalActual = g.Sum(d => d.ActualAmount),
                    TotalForecast = g.Sum(d => d.ForecastAmount),
                    TotalVariance = g.Sum(d => d.Variance),
                    SalarySpend = g.Where(d => d.SpendType.Equals("Salary", StringComparison.OrdinalIgnoreCase)).Sum(d => d.ActualAmount),
                    SoftwareSpend = g.Where(d => d.SpendType.Equals("Software", StringComparison.OrdinalIgnoreCase)).Sum(d => d.ActualAmount),
                    HardwareSpend = g.Where(d => d.SpendType.Equals("Hardware", StringComparison.OrdinalIgnoreCase)).Sum(d => d.ActualAmount),
                    OtherSpend = g.Where(d => !new[] { "Salary", "Software", "Hardware" }.Contains(d.SpendType, StringComparer.OrdinalIgnoreCase)).Sum(d => d.ActualAmount)
                })
                .ToList();

            return summaries;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating Planful Integration Summary");
            return new List<PlanfulIntegrationSummary>();
        }
    }

    public async Task<List<PlanfulAS400Mapping>> GetPlanfulAS400MappingsAsync()
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            // TODO: Update table name based on actual mapping table in DB2
            // This assumes there's a mapping table - adjust based on your schema
            var query = @"
                EXEC ('SELECT
                    MAPPING.ID as Id,
                    MAPPING.PLANFUL_ACCOUNT_CODE as PlanfulAccountCode,
                    MAPPING.PLANFUL_ACCOUNT_NAME as PlanfulAccountName,
                    MAPPING.AS400_UNIT as AS400Unit,
                    MAPPING.AS400_NATURE as AS400Nature,
                    MAPPING.MAPPING_TYPE as MappingType,
                    MAPPING.ALLOCATION_PCT as AllocationPercentage,
                    MAPPING.IS_ACTIVE as IsActive,
                    MAPPING.EFFECTIVE_DATE as EffectiveDate,
                    MAPPING.END_DATE as EndDate
                FROM PLANFUL_AS400_MAPPING as MAPPING
                WHERE MAPPING.IS_ACTIVE = 1') at DB2";

            var result = await connection.QueryAsync<PlanfulAS400Mapping>(query);
            return result.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching Planful AS400 mappings from DB2");
            return new List<PlanfulAS400Mapping>();
        }
    }
}
