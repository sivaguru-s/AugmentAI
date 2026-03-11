using System.Text.RegularExpressions;

namespace Chatbot_Onbase.Services;

public class QueryIntent
{
    public string IntentType { get; set; } = string.Empty;
    public Dictionary<string, object> Parameters { get; set; } = new();
}

public interface IInvoiceQueryParser
{
    QueryIntent ParseQuery(string userPrompt);
}

public class InvoiceQueryParser : IInvoiceQueryParser
{
    public QueryIntent ParseQuery(string userPrompt)
    {
        var prompt = userPrompt.ToLower().Trim();
        var intent = new QueryIntent();

        // Detect intent type and extract parameters
        if (ContainsAny(prompt, new[] { "vendor", "supplier", "company" }))
        {
            intent.IntentType = "SearchByVendor";
            intent.Parameters["vendorName"] = ExtractVendorName(prompt);
        }
        else if (ContainsAny(prompt, new[] { "invoice number", "invoice #", "inv#", "invoice no" }))
        {
            intent.IntentType = "SearchByInvoiceNumber";
            intent.Parameters["invoiceNumber"] = ExtractInvoiceNumber(prompt);
        }
        else if (ContainsAny(prompt, new[] { "po number", "purchase order", "po#", "po " }))
        {
            intent.IntentType = "SearchByPO";
            intent.Parameters["poNumber"] = ExtractPONumber(prompt);
        }
        else if (ContainsAny(prompt, new[] { "status", "pending", "approved", "paid", "rejected" }))
        {
            intent.IntentType = "SearchByStatus";
            intent.Parameters["status"] = ExtractStatus(prompt);
        }
        else if (ContainsAny(prompt, new[] { "amount", "total", "value", "cost", "price" }))
        {
            intent.IntentType = "SearchByAmount";
            var amounts = ExtractAmounts(prompt);
            intent.Parameters["minAmount"] = amounts.min;
            intent.Parameters["maxAmount"] = amounts.max;
        }
        else if (ContainsAny(prompt, new[] { "date", "month", "year", "between", "from", "to", "during" }))
        {
            intent.IntentType = "SearchByDate";
            var dates = ExtractDateRange(prompt);
            intent.Parameters["startDate"] = dates.start;
            intent.Parameters["endDate"] = dates.end;
        }
        else if (ContainsAny(prompt, new[] { "all", "list", "show", "get", "find", "search" }))
        {
            intent.IntentType = "GeneralSearch";
            intent.Parameters["searchTerm"] = ExtractSearchTerm(prompt);
        }
        else
        {
            intent.IntentType = "GeneralSearch";
            intent.Parameters["searchTerm"] = prompt;
        }

        return intent;
    }

    private bool ContainsAny(string text, string[] keywords)
    {
        return keywords.Any(keyword => text.Contains(keyword));
    }

    private string ExtractVendorName(string prompt)
    {
        var patterns = new[]
        {
            @"vendor\s+(?:named?\s+)?[""']?([^""']+)[""']?",
            @"from\s+[""']?([^""']+)[""']?",
            @"supplier\s+[""']?([^""']+)[""']?",
            @"company\s+[""']?([^""']+)[""']?"
        };

        foreach (var pattern in patterns)
        {
            var match = Regex.Match(prompt, pattern, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return match.Groups[1].Value.Trim();
            }
        }

        return string.Empty;
    }

    private string ExtractInvoiceNumber(string prompt)
    {
        var patterns = new[]
        {
            @"invoice\s+(?:number|#|no\.?)\s*:?\s*['""]?([A-Z0-9-]+)['""]?",
            @"invoice\s+['""]?(\d{7,})['""]?",  // "find invoice 40767602" pattern
            @"inv#?\s*:?\s*['""]?([A-Z0-9-]+)['""]?",
            @"#\s*['""]?([A-Z0-9-]+)['""]?"
        };

        foreach (var pattern in patterns)
        {
            var match = Regex.Match(prompt, pattern, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return match.Groups[1].Value.Trim();
            }
        }

        return string.Empty;
    }

    private string ExtractPONumber(string prompt)
    {
        var patterns = new[]
        {
            @"po\s+(?:number|#|no\.?)\s*:?\s*([A-Z0-9-]+)",
            @"purchase\s+order\s+([A-Z0-9-]+)",
            @"po#?\s*:?\s*([A-Z0-9-]+)"
        };

        foreach (var pattern in patterns)
        {
            var match = Regex.Match(prompt, pattern, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return match.Groups[1].Value.Trim();
            }
        }

        return string.Empty;
    }

    private string ExtractStatus(string prompt)
    {
        var statuses = new[] { "pending", "approved", "paid", "rejected", "processing", "completed", "cancelled" };
        
        foreach (var status in statuses)
        {
            if (prompt.Contains(status))
            {
                return status;
            }
        }

        return string.Empty;
    }

    private (decimal min, decimal max) ExtractAmounts(string prompt)
    {
        var amountPattern = @"(\d+(?:\.\d{2})?)";
        var matches = Regex.Matches(prompt, amountPattern);

        if (matches.Count >= 2)
        {
            return (decimal.Parse(matches[0].Value), decimal.Parse(matches[1].Value));
        }
        else if (matches.Count == 1)
        {
            var amount = decimal.Parse(matches[0].Value);
            if (prompt.Contains("greater") || prompt.Contains("more") || prompt.Contains("above"))
            {
                return (amount, decimal.MaxValue);
            }
            else if (prompt.Contains("less") || prompt.Contains("below") || prompt.Contains("under"))
            {
                return (0, amount);
            }
            return (amount, amount);
        }

        return (0, decimal.MaxValue);
    }

    private (DateTime start, DateTime end) ExtractDateRange(string prompt)
    {
        var today = DateTime.Today;

        // Check for relative dates
        if (prompt.Contains("today"))
        {
            return (today, today.AddDays(1));
        }
        else if (prompt.Contains("yesterday"))
        {
            return (today.AddDays(-1), today);
        }
        else if (prompt.Contains("this week"))
        {
            var startOfWeek = today.AddDays(-(int)today.DayOfWeek);
            return (startOfWeek, today.AddDays(1));
        }
        else if (prompt.Contains("this month"))
        {
            var startOfMonth = new DateTime(today.Year, today.Month, 1);
            return (startOfMonth, today.AddDays(1));
        }
        else if (prompt.Contains("this year"))
        {
            var startOfYear = new DateTime(today.Year, 1, 1);
            return (startOfYear, today.AddDays(1));
        }
        else if (prompt.Contains("last month"))
        {
            var lastMonth = today.AddMonths(-1);
            var startOfLastMonth = new DateTime(lastMonth.Year, lastMonth.Month, 1);
            var endOfLastMonth = startOfLastMonth.AddMonths(1);
            return (startOfLastMonth, endOfLastMonth);
        }
        else if (prompt.Contains("last year"))
        {
            var lastYear = today.Year - 1;
            return (new DateTime(lastYear, 1, 1), new DateTime(lastYear, 12, 31));
        }

        // Try to extract specific dates
        var datePattern = @"(\d{1,2})[/-](\d{1,2})[/-](\d{2,4})";
        var matches = Regex.Matches(prompt, datePattern);

        if (matches.Count >= 2)
        {
            var date1 = ParseDate(matches[0].Value);
            var date2 = ParseDate(matches[1].Value);
            return date1 < date2 ? (date1, date2) : (date2, date1);
        }
        else if (matches.Count == 1)
        {
            var date = ParseDate(matches[0].Value);
            return (date, date.AddDays(1));
        }

        // Default to last 30 days
        return (today.AddDays(-30), today.AddDays(1));
    }

    private DateTime ParseDate(string dateStr)
    {
        if (DateTime.TryParse(dateStr, out var date))
        {
            return date;
        }
        return DateTime.Today;
    }

    private string ExtractSearchTerm(string prompt)
    {
        // Remove common query words
        var stopWords = new[] { "show", "get", "find", "search", "list", "all", "me", "the", "for", "invoices", "invoice" };
        var words = prompt.Split(' ').Where(w => !stopWords.Contains(w.ToLower())).ToArray();
        return string.Join(" ", words).Trim();
    }
}
