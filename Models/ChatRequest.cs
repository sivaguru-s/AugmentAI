namespace Chatbot_Onbase.Models;

public class ChatRequest
{
    public string Prompt { get; set; } = string.Empty;
}

public class ChatResponse
{
    public string Answer { get; set; } = string.Empty;
    public List<Invoice>? Invoices { get; set; }
    public string? Query { get; set; }
    public int ResultCount { get; set; }
}

