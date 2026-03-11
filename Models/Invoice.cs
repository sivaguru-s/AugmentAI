namespace Chatbot_Onbase.Models;

public class Invoice
{
    public int InvoiceId { get; set; }
    public string? InvoiceNumber { get; set; }
    public string? VendorName { get; set; }
    public decimal? Amount { get; set; }
    public DateTime? InvoiceDate { get; set; }
    public DateTime? DueDate { get; set; }
    public string? Status { get; set; }
    public string? PONumber { get; set; }
    public string? Description { get; set; }
    public string? DocumentType { get; set; }
    public DateTime? CreatedDate { get; set; }
}

