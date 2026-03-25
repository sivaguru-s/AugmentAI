using FinanceBudget.Models;

namespace FinanceBudget.Services;

public interface IUnitBudgetService
{
    Task<List<UnitBudget>> GetUnitBudgetDataAsync(string? unitCode = null, string? natureId = null);
    Task<List<UnitBudget>> GetUnitBudgetByTransactionStatusAsync(string unitCode, string transactionStatus);
    Task<List<Unit>> GetAllUnitsAsync();
    Task<List<Nature>> GetNaturesByUnitAsync(string unitCode);
}

