# Pagination Support - Skip and Take

## ❌ **Problem**

When searching for invoices, the chatbot would return all matching results but **always show the same invoices** at the top. Users couldn't navigate through large result sets or view different pages of results.

### **Example:**
```
User Query: "Find invoices between 2025-03-01 and 2025-03-30 skip first 5 invoices and take next 5 invoices"
Expected: Show invoices 6-10 from the result set
Actual: Showed invoices 1-5 (pagination was ignored) ❌
```

The chatbot found 249 invoices but always displayed the same first batch because:
1. The NLP parser didn't understand "skip" and "take" keywords
2. No pagination logic was applied to the results
3. Users saw the same invoices every time they searched

---

## ✅ **Solution**

Added **pagination support** to the NLP parser and service layer:

### **1. New Pagination Parser**
```csharp
private (int skip, int take) ExtractPagination(string prompt)
{
    // Extract "skip X" or "skip first X"
    var skipPattern = @"skip\s+(?:first\s+)?(\d+)";
    
    // Extract "take X" or "take next X" or "show X" or "limit X"
    var takePattern = @"(?:take|show|limit)\s+(?:next\s+)?(\d+)";
    
    // Extract "first X" (without skip)
    var firstPattern = @"first\s+(\d+)";
}
```

### **2. Apply Pagination in Service**
```csharp
// Apply pagination if specified
var skip = Convert.ToInt32(intent.Parameters.GetValueOrDefault("skip") ?? 0);
var take = Convert.ToInt32(intent.Parameters.GetValueOrDefault("take") ?? 0);
var totalCount = invoices.Count;

if (skip > 0)
{
    invoices = invoices.Skip(skip).ToList();
}

if (take > 0)
{
    invoices = invoices.Take(take).ToList();
}
```

### **3. Enhanced Response Message**
```csharp
// Show pagination info in response
if (totalCount > 0 && (skip > 0 || take > 0))
{
    var startIndex = skip + 1;
    var endIndex = skip + invoices.Count;
    summary.AppendLine($"I found {totalCount} invoice(s) matching your query. Showing {startIndex} to {endIndex}:");
}
```

---

## 🎯 **Supported Pagination Syntax**

The chatbot now understands these pagination patterns:

### **Skip Patterns:**
- `skip 5` → Skip first 5 results
- `skip first 10` → Skip first 10 results

### **Take Patterns:**
- `take 5` → Take 5 results
- `take next 10` → Take next 10 results
- `show 20` → Show 20 results
- `limit 15` → Limit to 15 results

### **First Pattern:**
- `first 10` → Take first 10 results (same as `take 10`)

---

## 🧪 **Example Queries**

### **Example 1: Skip and Take**
```
Query: "Find invoices between 2025-03-01 and 2025-03-30 skip first 5 and take next 5"
Result: Shows invoices 6-10 out of 249 total
Response: "I found 249 invoice(s) matching your query. Showing 6 to 10:"
```

### **Example 2: Just Take**
```
Query: "Show invoices from March 2025 take 10"
Result: Shows first 10 invoices out of 249 total
Response: "I found 249 invoice(s) matching your query. Showing 1 to 10:"
```

### **Example 3: Just Skip**
```
Query: "Find invoices in March 2025 skip 100"
Result: Shows invoices 101-249 (all remaining)
Response: "I found 249 invoice(s) matching your query. Showing 101 to 249:"
```

### **Example 4: First X**
```
Query: "Show first 20 invoices from March 2025"
Result: Shows first 20 invoices
Response: "I found 249 invoice(s) matching your query. Showing 1 to 20:"
```

### **Example 5: Pagination with Amount Range**
```
Query: "Find invoices between 1000 and 10000 skip 10 take 5"
Result: Shows invoices 11-15 from the amount range
```

---

## 📝 **Changes Made**

### **File: `Services/InvoiceQueryParser.cs`**

#### **1. Added Pagination Extraction (Line 24-26)**
```csharp
// Extract pagination parameters first (applies to all queries)
var pagination = ExtractPagination(prompt);
intent.Parameters["skip"] = pagination.skip;
intent.Parameters["take"] = pagination.take;
```

#### **2. Added ExtractPagination Method (Lines 308-338)**
```csharp
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
```

### **File: `Services/ChatbotService.cs`**

#### **1. Apply Pagination to Results (Lines 68-80)**
```csharp
// Apply pagination if specified
var skip = Convert.ToInt32(intent.Parameters.GetValueOrDefault("skip") ?? 0);
var take = Convert.ToInt32(intent.Parameters.GetValueOrDefault("take") ?? 0);
var totalCount = invoices.Count;

if (skip > 0)
{
    invoices = invoices.Skip(skip).ToList();
}

if (take > 0)
{
    invoices = invoices.Take(take).ToList();
}
```

#### **2. Updated Response Generation (Lines 120-128)**
```csharp
// If pagination was applied, show different message
if (totalCount > 0 && (skip > 0 || take > 0))
{
    var startIndex = skip + 1;
    var endIndex = skip + invoices.Count;
    summary.AppendLine($"I found {totalCount} invoice(s) matching your query. Showing {startIndex} to {endIndex}:");
}
```

---

## ✅ **Summary**

| Item | Status |
|------|--------|
| **Problem** | Always showed same invoices, no pagination |
| **Solution** | Added skip/take support to NLP parser |
| **Files Changed** | `InvoiceQueryParser.cs`, `ChatbotService.cs` |
| **Build Status** | ✅ Success |
| **Application** | ✅ Running on port 5001 |

**The chatbot now supports pagination with skip and take commands!** 🎉

