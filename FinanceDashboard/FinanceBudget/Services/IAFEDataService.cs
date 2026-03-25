using FinanceBudget.Models;

namespace FinanceBudget.Services;

public interface IAFEDataService
{
    Task<List<AFEData>> GetAFEDataAsync(string? unitCode = null);
    Task<AFEData?> GetAFEByNumberAsync(string afeNumber);
}

