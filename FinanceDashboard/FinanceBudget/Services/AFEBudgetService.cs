using FinanceBudget.Models;
using Microsoft.Data.SqlClient;
using Dapper;

namespace FinanceBudget.Services;

/// <summary>
/// Service for AFE Budget data management
/// Handles AFE budget information and links to unit nature costs
/// Note: This implementation uses in-memory storage for demo purposes
/// In production, this should be backed by a database table
/// </summary>
public class AFEBudgetService : IAFEBudgetService
{
    private readonly string _connectionString;
    private readonly ILogger<AFEBudgetService> _logger;
    
    // In-memory storage for demo - replace with database in production
    private static readonly List<AFEBudget> _afeBudgets = new();
    private static readonly List<AFEUnitNatureMapping> _afeMappings = new();
    private static readonly List<BusinessVertical> _businessVerticals = new();
    private static readonly List<ProjectVerticalMapping> _projectMappings = new();

    public AFEBudgetService(IConfiguration configuration, ILogger<AFEBudgetService> logger)
    {
        _connectionString = configuration.GetConnectionString("AS400Database") 
            ?? throw new ArgumentNullException("AS400Database connection string is not configured");
        _logger = logger;
        
        // Initialize sample data if empty
        InitializeSampleData();
    }

    private void InitializeSampleData()
    {
        if (_businessVerticals.Count == 0)
        {
            _businessVerticals.AddRange(new[]
            {
                new BusinessVertical { Id = 1, VerticalCode = "IT", VerticalName = "Information Technology", Description = "IT Department", IsActive = true },
                new BusinessVertical { Id = 2, VerticalCode = "FIN", VerticalName = "Finance", Description = "Finance Department", IsActive = true },
                new BusinessVertical { Id = 3, VerticalCode = "OPS", VerticalName = "Operations", Description = "Operations Department", IsActive = true },
                new BusinessVertical { Id = 4, VerticalCode = "HR", VerticalName = "Human Resources", Description = "HR Department", IsActive = true },
                new BusinessVertical { Id = 5, VerticalCode = "MKT", VerticalName = "Marketing", Description = "Marketing Department", IsActive = true }
            });
        }
    }

    public async Task<List<AFEBudget>> GetAFEBudgetsAsync(string? project = null, string? afeNumber = null, string? unit = null)
    {
        try
        {
            await Task.CompletedTask; // Placeholder for async operation
            
            var query = _afeBudgets.AsEnumerable();

            if (!string.IsNullOrEmpty(project))
            {
                query = query.Where(a => a.Project.Equals(project, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(afeNumber))
            {
                query = query.Where(a => a.AFENumber.Equals(afeNumber, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(unit))
            {
                query = query.Where(a => a.Unit.Equals(unit, StringComparison.OrdinalIgnoreCase));
            }

            return query.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching AFE budgets");
            return new List<AFEBudget>();
        }
    }

    public async Task<AFEBudget?> GetAFEBudgetByNumberAsync(string afeNumber)
    {
        try
        {
            await Task.CompletedTask;
            return _afeBudgets.FirstOrDefault(a => a.AFENumber.Equals(afeNumber, StringComparison.OrdinalIgnoreCase));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching AFE budget by number");
            return null;
        }
    }

    public async Task<List<AFEUnitNatureMapping>> GetAFEUnitNatureMappingsAsync(string? afeNumber = null, string? unit = null)
    {
        try
        {
            await Task.CompletedTask;
            
            var query = _afeMappings.AsEnumerable();

            if (!string.IsNullOrEmpty(afeNumber))
            {
                query = query.Where(m => m.AFENumber.Equals(afeNumber, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(unit))
            {
                query = query.Where(m => m.Unit.Equals(unit, StringComparison.OrdinalIgnoreCase));
            }

            return query.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching AFE unit nature mappings");
            return new List<AFEUnitNatureMapping>();
        }
    }

    public async Task<List<BusinessVertical>> GetBusinessVerticalsAsync()
    {
        try
        {
            await Task.CompletedTask;
            return _businessVerticals.Where(v => v.IsActive).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching business verticals");
            return new List<BusinessVertical>();
        }
    }

    public async Task<List<ProjectVerticalMapping>> GetProjectVerticalMappingsAsync(string? project = null, string? verticalCode = null)
    {
        try
        {
            await Task.CompletedTask;
            
            var query = _projectMappings.AsEnumerable();

            if (!string.IsNullOrEmpty(project))
            {
                query = query.Where(m => m.Project.Equals(project, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(verticalCode))
            {
                query = query.Where(m => m.BusinessVerticalCode.Equals(verticalCode, StringComparison.OrdinalIgnoreCase));
            }

            return query.Where(m => !m.EndDate.HasValue || m.EndDate.Value > DateTime.Now).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching project vertical mappings");
            return new List<ProjectVerticalMapping>();
        }
    }

    public async Task<int> ImportAFEBudgetsAsync(List<AFEBudget> budgets)
    {
        try
        {
            await Task.CompletedTask;

            int importedCount = 0;
            foreach (var budget in budgets)
            {
                // Check if AFE already exists
                var existing = _afeBudgets.FirstOrDefault(a => a.AFENumber.Equals(budget.AFENumber, StringComparison.OrdinalIgnoreCase));

                if (existing != null)
                {
                    // Update existing
                    var index = _afeBudgets.IndexOf(existing);
                    _afeBudgets[index] = budget;
                }
                else
                {
                    // Add new
                    _afeBudgets.Add(budget);
                }

                importedCount++;
            }

            _logger.LogInformation("Imported {Count} AFE budgets", importedCount);
            return importedCount;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error importing AFE budgets");
            return 0;
        }
    }

    public async Task<bool> LinkAFEToUnitNatureAsync(AFEUnitNatureMapping mapping)
    {
        try
        {
            await Task.CompletedTask;

            // Check if mapping already exists
            var existing = _afeMappings.FirstOrDefault(m =>
                m.AFENumber.Equals(mapping.AFENumber, StringComparison.OrdinalIgnoreCase) &&
                m.Unit.Equals(mapping.Unit, StringComparison.OrdinalIgnoreCase) &&
                m.Nature.Equals(mapping.Nature, StringComparison.OrdinalIgnoreCase));

            if (existing != null)
            {
                // Update existing
                var index = _afeMappings.IndexOf(existing);
                mapping.Id = existing.Id;
                mapping.ModifiedDate = DateTime.Now;
                _afeMappings[index] = mapping;
            }
            else
            {
                // Add new
                mapping.Id = _afeMappings.Count > 0 ? _afeMappings.Max(m => m.Id) + 1 : 1;
                mapping.CreatedDate = DateTime.Now;
                _afeMappings.Add(mapping);
            }

            _logger.LogInformation("Linked AFE {AFENumber} to Unit {Unit} and Nature {Nature}",
                mapping.AFENumber, mapping.Unit, mapping.Nature);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error linking AFE to unit nature");
            return false;
        }
    }
}

