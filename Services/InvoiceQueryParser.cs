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

        // Extract pagination parameters first (applies to all queries)
        var pagination = ExtractPagination(prompt);
        intent.Parameters["skip"] = pagination.skip;
        intent.Parameters["take"] = pagination.take;

        // Extract document type filter (customer vs vendor)
        var documentType = ExtractDocumentType(prompt);
        intent.Parameters["documentType"] = documentType;

        // Check if this is an analytics query
        bool isAnalyticsQuery = ContainsAny(prompt, new[] { "analyze", "analysis", "summary", "summarize", "average", "pricing changes", "rate increase", "trend", "cost breakdown" });
        intent.Parameters["isAnalytics"] = isAnalyticsQuery;

        // Extract date range if present (for analytics or date-based queries)
        // Note: "from" is excluded because it's commonly used for vendor names (e.g., "from CDW")
        if (ContainsAny(prompt, new[] { "date", "month", "year", "between", "during", "january", "february", "march", "april", "may", "june", "july", "august", "september", "october", "november", "december" }))
        {
            var dates = ExtractDateRange(prompt);
            intent.Parameters["startDate"] = dates.start;
            intent.Parameters["endDate"] = dates.end;
        }

        // Detect intent type and extract parameters
        // Check if this is a vendor-specific search (has vendor name) vs just filtering by document type
        // IMPORTANT: Pass original userPrompt (not lowercased) to preserve capital letters for vendor name matching
        string vendorName = string.Empty;
        if (ContainsAny(prompt, new[] { "vendor", "supplier", "company", "from" }))
        {
            vendorName = ExtractVendorName(userPrompt);
        }

        if (!string.IsNullOrEmpty(vendorName))
        {
            // Specific vendor search
            intent.IntentType = isAnalyticsQuery ? "AnalyzeByVendor" : "SearchByVendor";
            intent.Parameters["vendorName"] = vendorName;
            // If searching by vendor, default to vendor invoices unless customer is specified
            if (documentType == "all")
            {
                intent.Parameters["documentType"] = "vendor";
            }
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

    private string ExtractDocumentType(string prompt)
    {
        // Check for explicit document type keywords
        // Vendor/AP keywords
        if (ContainsAny(prompt, new[] { "vendor invoice", "ap invoice", "payable", "vendor", "supplier" }))
        {
            return "vendor";
        }

        // Customer/AR keywords
        if (ContainsAny(prompt, new[] { "customer invoice", "ar invoice", "receivable", "customer", "sales invoice" }))
        {
            return "customer";
        }

        // Credit memo keywords
        if (ContainsAny(prompt, new[] { "credit memo", "credit note", "cm" }))
        {
            // Check if it's vendor or customer credit memo
            if (ContainsAny(prompt, new[] { "vendor", "ap", "payable", "supplier" }))
            {
                return "vendor_credit";
            }
            else if (ContainsAny(prompt, new[] { "customer", "ar", "receivable" }))
            {
                return "customer_credit";
            }
            return "credit"; // Generic credit memo
        }

        // Default to "all" - search both customer and vendor invoices
        return "all";
    }

    private string ExtractVendorName(string prompt)
    {
        var patterns = new[]
        {
            @"(?:vendor|supplier|company)\s+(?:named?\s+)?[""']([^""']+)[""']",  // Quoted vendor name
            @"from\s+[""']([^""']+)[""']",  // Quoted vendor after "from"
            @"from\s+((?!invoice|credit|memo|document)[A-Z][\w\s&,.-]+?)(?:\s+for|\s+invoice|\s+credit|\s+memo|\s*$)",  // Vendor name after "from" (must start with capital)
            @"(?:vendor|supplier|company)\s+(?:named?\s+)?((?!invoice|credit|memo|document)[A-Z][\w\s&,.-]{2,}?)(?:\s+for|\s+invoice|\s+credit|\s*$)"  // Vendor name (must start with capital, at least 3 chars)
        };

        foreach (var pattern in patterns)
        {
            var match = Regex.Match(prompt, pattern, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                var vendorName = match.Groups[1].Value.Trim();
                // Clean up common trailing words
                vendorName = Regex.Replace(vendorName, @"\s+(invoice|credit|memo|document|for)s?$", "", RegexOptions.IgnoreCase).Trim();

                // Reject if vendor name is a common keyword
                if (Regex.IsMatch(vendorName, @"^(invoice|credit|memo|document|all|list|show|for)s?$", RegexOptions.IgnoreCase))
                {
                    continue;
                }

                return vendorName;
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

        // Check for month name + year pattern (e.g., "February 2025", "March 2024")
        var monthYearPattern = @"(january|february|march|april|may|june|july|august|september|october|november|december)\s+(\d{4})";
        var monthYearMatch = Regex.Match(prompt, monthYearPattern, RegexOptions.IgnoreCase);

        if (monthYearMatch.Success)
        {
            var monthName = monthYearMatch.Groups[1].Value;
            var year = int.Parse(monthYearMatch.Groups[2].Value);
            var month = monthName.ToLower() switch
            {
                "january" => 1,
                "february" => 2,
                "march" => 3,
                "april" => 4,
                "may" => 5,
                "june" => 6,
                "july" => 7,
                "august" => 8,
                "september" => 9,
                "october" => 10,
                "november" => 11,
                "december" => 12,
                _ => today.Month
            };

            var startOfMonth = new DateTime(year, month, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);
            return (startOfMonth, endOfMonth.AddDays(1)); // Add 1 day to include the last day
        }

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

        // Try to extract specific dates - support both ISO (YYYY-MM-DD) and common formats (DD/MM/YYYY, MM-DD-YYYY)
        // First try ISO format (YYYY-MM-DD)
        var isoDatePattern = @"(\d{4})[/-](\d{1,2})[/-](\d{1,2})";
        var isoMatches = Regex.Matches(prompt, isoDatePattern);

        if (isoMatches.Count >= 2)
        {
            var date1 = ParseISODate(isoMatches[0].Value);
            var date2 = ParseISODate(isoMatches[1].Value);
            return date1 < date2 ? (date1, date2.AddDays(1)) : (date2, date1.AddDays(1));
        }
        else if (isoMatches.Count == 1)
        {
            var date = ParseISODate(isoMatches[0].Value);
            return (date, date.AddDays(1));
        }

        // Try common date formats (DD/MM/YYYY, MM-DD-YYYY)
        var datePattern = @"(\d{1,2})[/-](\d{1,2})[/-](\d{2,4})";
        var matches = Regex.Matches(prompt, datePattern);

        if (matches.Count >= 2)
        {
            var date1 = ParseDate(matches[0].Value);
            var date2 = ParseDate(matches[1].Value);
            return date1 < date2 ? (date1, date2.AddDays(1)) : (date2, date1.AddDays(1));
        }
        else if (matches.Count == 1)
        {
            var date = ParseDate(matches[0].Value);
            return (date, date.AddDays(1));
        }

        // Default to last 30 days
        return (today.AddDays(-30), today.AddDays(1));
    }

    private DateTime ParseISODate(string dateStr)
    {
        // Parse ISO format: YYYY-MM-DD or YYYY/MM/DD
        var parts = dateStr.Split(new[] { '-', '/' });
        if (parts.Length == 3 &&
            int.TryParse(parts[0], out var year) &&
            int.TryParse(parts[1], out var month) &&
            int.TryParse(parts[2], out var day))
        {
            try
            {
                return new DateTime(year, month, day);
            }
            catch
            {
                return DateTime.Today;
            }
        }
        return DateTime.Today;
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

    private (int skip, int take) ExtractPagination(string prompt)
    {
        int skip = 0;
        int take = 0; // 0 means no limit

        // Extract "skip X" or "skip first X"
        var skipPattern = @"skip\s+(?:first\s+)?(\d+)";
        var skipMatch = Regex.Match(prompt, skipPattern);
        if (skipMatch.Success)
        {
            skip = int.Parse(skipMatch.Groups[1].Value);
        }

        // Extract "take X" or "take next X" or "show X" or "limit X"
        var takePattern = @"(?:take|show|limit)\s+(?:next\s+)?(\d+)";
        var takeMatch = Regex.Match(prompt, takePattern);
        if (takeMatch.Success)
        {
            take = int.Parse(takeMatch.Groups[1].Value);
        }

        // Extract "first X" (without skip)
        if (take == 0 && !skipMatch.Success)
        {
            var firstPattern = @"first\s+(\d+)";
            var firstMatch = Regex.Match(prompt, firstPattern);
            if (firstMatch.Success)
            {
                take = int.Parse(firstMatch.Groups[1].Value);
            }
        }

        return (skip, take);
    }
}
