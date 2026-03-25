namespace FinanceBudget.Models;

public class AFEData
{
    public string AFENumber { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal ApprovedAmount { get; set; }
    public decimal SpentAmount { get; set; }
    public decimal RemainingAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? ApprovalDate { get; set; }
    public string Requestor { get; set; } = string.Empty;
}

