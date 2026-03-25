using FinanceBudget.Models;

namespace FinanceBudget.Services;

public interface IJiraCostingService
{
    Task<List<JiraCosting>> GetJiraCostingDataAsync(string? unitCode = null);
    Task<JiraCosting?> GetJiraIssueAsync(string issueKey);
}

