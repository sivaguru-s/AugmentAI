namespace FinanceBudget.Models;

public class UnitBudget
{
    public string Unit { get; set; } = string.Empty;
    public string UnitDescription { get; set; } = string.Empty;
    public string Nature { get; set; } = string.Empty;
    public string NatureDescription { get; set; } = string.Empty;
    public string TransactionId { get; set; } = string.Empty;
    public string TransactionNumber { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}

