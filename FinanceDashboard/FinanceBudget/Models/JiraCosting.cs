namespace FinanceBudget.Models;

public class JiraCosting
{
    public string IssueKey { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal EstimatedCost { get; set; }
    public decimal ActualCost { get; set; }
    public string Assignee { get; set; } = string.Empty;
    public DateTime? CreatedDate { get; set; }
    public DateTime? DueDate { get; set; }
    public string Priority { get; set; } = string.Empty;
}

