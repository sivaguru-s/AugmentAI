using FinanceBudget.Models;

namespace FinanceBudget.Services;

/// <summary>
/// Service interface for Planful data integration
/// Fetches Planful Summary and Detail data from DB2 tables
/// </summary>
public interface IPlanfulService
{
    /// <summary>
    /// Get Planful Summary data - Nature-wise total spend (monthly, by type)
    /// </summary>
    /// <param name="year">Filter by year (optional)</param>
    /// <param name="month">Filter by month (optional)</param>
    /// <param name="nature">Filter by nature code (optional)</param>
    /// <returns>List of Planful Summary records</returns>
    Task<List<PlanfulSummary>> GetPlanfulSummaryAsync(int? year = null, int? month = null, string? nature = null);

    /// <summary>
    /// Get Planful Detail data - Unit-wise costs split from nature-wise data
    /// </summary>
    /// <param name="unitCode">Filter by unit code (optional)</param>
    /// <param name="natureId">Filter by nature code (optional)</param>
    /// <param name="year">Filter by year (optional)</param>
    /// <param name="month">Filter by month (optional)</param>
    /// <returns>List of Planful Detail records</returns>
    Task<List<PlanfulDetail>> GetPlanfulDetailAsync(string? unitCode = null, string? natureId = null, int? year = null, int? month = null);

    /// <summary>
    /// Get integrated Planful summary combining summary and detail data
    /// </summary>
    /// <param name="unitCode">Filter by unit code (optional)</param>
    /// <param name="natureId">Filter by nature code (optional)</param>
    /// <param name="year">Filter by year (optional)</param>
    /// <param name="month">Filter by month (optional)</param>
    /// <returns>List of integrated Planful summaries</returns>
    Task<List<PlanfulIntegrationSummary>> GetPlanfulIntegrationSummaryAsync(string? unitCode = null, string? natureId = null, int? year = null, int? month = null);

    /// <summary>
    /// Get Planful to AS400 mappings
    /// </summary>
    /// <returns>List of Planful to AS400 mappings</returns>
    Task<List<PlanfulAS400Mapping>> GetPlanfulAS400MappingsAsync();
}

