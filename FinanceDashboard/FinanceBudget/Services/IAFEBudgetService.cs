using FinanceBudget.Models;

namespace FinanceBudget.Services;

/// <summary>
/// Service interface for AFE Budget data
/// Handles AFE budget information from Excel and links to unit nature costs
/// </summary>
public interface IAFEBudgetService
{
    /// <summary>
    /// Get all AFE budgets
    /// </summary>
    /// <param name="project">Filter by project (optional)</param>
    /// <param name="afeNumber">Filter by AFE number (optional)</param>
    /// <param name="unit">Filter by unit (optional)</param>
    /// <returns>List of AFE budgets</returns>
    Task<List<AFEBudget>> GetAFEBudgetsAsync(string? project = null, string? afeNumber = null, string? unit = null);

    /// <summary>
    /// Get AFE budget by AFE number
    /// </summary>
    /// <param name="afeNumber">AFE number</param>
    /// <returns>AFE budget or null</returns>
    Task<AFEBudget?> GetAFEBudgetByNumberAsync(string afeNumber);

    /// <summary>
    /// Get AFE to Unit Nature mappings
    /// </summary>
    /// <param name="afeNumber">Filter by AFE number (optional)</param>
    /// <param name="unit">Filter by unit (optional)</param>
    /// <returns>List of AFE to Unit Nature mappings</returns>
    Task<List<AFEUnitNatureMapping>> GetAFEUnitNatureMappingsAsync(string? afeNumber = null, string? unit = null);

    /// <summary>
    /// Get business verticals
    /// </summary>
    /// <returns>List of business verticals</returns>
    Task<List<BusinessVertical>> GetBusinessVerticalsAsync();

    /// <summary>
    /// Get project to business vertical mappings
    /// </summary>
    /// <param name="project">Filter by project (optional)</param>
    /// <param name="verticalCode">Filter by vertical code (optional)</param>
    /// <returns>List of project to vertical mappings</returns>
    Task<List<ProjectVerticalMapping>> GetProjectVerticalMappingsAsync(string? project = null, string? verticalCode = null);

    /// <summary>
    /// Import AFE budgets from Excel data
    /// </summary>
    /// <param name="budgets">List of AFE budgets to import</param>
    /// <returns>Number of records imported</returns>
    Task<int> ImportAFEBudgetsAsync(List<AFEBudget> budgets);

    /// <summary>
    /// Link AFE budget to unit and nature
    /// </summary>
    /// <param name="mapping">AFE to Unit Nature mapping</param>
    /// <returns>Success status</returns>
    Task<bool> LinkAFEToUnitNatureAsync(AFEUnitNatureMapping mapping);
}

