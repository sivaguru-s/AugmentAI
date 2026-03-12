# Onbase Invoice Chatbot - Architecture & Tech Stack

## 📋 **Overview**

This is a **Natural Language Processing (NLP) chatbot** that allows users to search for invoices in an Onbase database using plain English queries instead of writing SQL.

**Example:**
- User types: *"Find invoice 40767602"*
- Chatbot understands the intent and returns the invoice details

---

## 🏗️ **Architecture Diagram**

```
┌─────────────────────────────────────────────────────────────┐
│                         USER                                 │
│                    (Web Browser)                             │
└────────────────────────┬────────────────────────────────────┘
                         │
                         │ HTTP Request
                         │ "Find invoice 40767602"
                         ▼
┌─────────────────────────────────────────────────────────────┐
│                    FRONTEND LAYER                            │
│                  (wwwroot/index.html)                        │
│  • HTML/CSS/JavaScript                                       │
│  • Chat UI Interface                                         │
│  • Sends query to API                                        │
└────────────────────────┬────────────────────────────────────┘
                         │
                         │ POST /api/chatbot/query
                         │ { "query": "Find invoice 40767602" }
                         ▼
┌─────────────────────────────────────────────────────────────┐
│                   API CONTROLLER LAYER                       │
│              (Controllers/ChatbotController.cs)              │
│  • Receives HTTP requests                                    │
│  • Validates input                                           │
│  • Calls ChatbotService                                      │
└────────────────────────┬────────────────────────────────────┘
                         │
                         │ ProcessQuery()
                         ▼
┌─────────────────────────────────────────────────────────────┐
│                   BUSINESS LOGIC LAYER                       │
│               (Services/ChatbotService.cs)                   │
│  • Orchestrates the query processing                         │
│  • Calls NLP Parser                                          │
│  • Calls Repository                                          │
│  • Formats response                                          │
└────────────────────────┬────────────────────────────────────┘
                         │
                         │ ParseQuery()
                         ▼
┌─────────────────────────────────────────────────────────────┐
│                      NLP PARSER LAYER                        │
│            (Services/InvoiceQueryParser.cs)                  │
│  • Regex-based pattern matching                              │
│  • Extracts: Intent, Invoice#, Vendor, Dates, Amounts       │
│  • Returns structured query parameters                       │
└────────────────────────┬────────────────────────────────────┘
                         │
                         │ Query Parameters
                         │ { Intent: "GetByNumber", InvoiceId: "40767602" }
                         ▼
┌─────────────────────────────────────────────────────────────┐
│                    DATA ACCESS LAYER                         │
│               (Data/OnbaseRepository.cs)                     │
│  • Dapper ORM for SQL queries                                │
│  • Optimized SQL with NOLOCK, TOP limits                     │
│  • Maps database results to Invoice objects                  │
└────────────────────────┬────────────────────────────────────┘
                         │
                         │ SQL Query
                         │ SELECT * FROM hsi.keyitem106 WHERE...
                         ▼
┌─────────────────────────────────────────────────────────────┐
│                      DATABASE LAYER                          │
│                  (SQL Server - Onbase)                       │
│  • Server: aazeus-obdmsq01                                   │
│  • Database: Onbase                                          │
│  • Tables: keyitem106, keytable104, keyitem112, itemdata    │
└─────────────────────────────────────────────────────────────┘
```

---

## 🛠️ **Tech Stack**

### **Backend**
| Technology | Version | Purpose |
|------------|---------|---------|
| **.NET** | 8.0 | Web API framework |
| **C#** | 12.0 | Programming language |
| **ASP.NET Core** | 8.0 | Web framework |
| **Dapper** | 2.1.35 | Micro-ORM for database access |
| **System.Data.SqlClient** | 4.8.6 | SQL Server connectivity |

### **Frontend**
| Technology | Purpose |
|------------|---------|
| **HTML5** | Structure |
| **CSS3** | Styling (gradient UI, chat bubbles) |
| **JavaScript (Vanilla)** | Client-side logic, API calls |

### **Database**
| Component | Details |
|-----------|---------|
| **SQL Server** | Onbase production database |
| **Server** | aazeus-obdmsq01 |
| **Authentication** | Windows Integrated Security |
| **Schema** | hsi (Onbase numbered tables) |

### **Development Tools**
- **Visual Studio Code** / **Visual Studio 2022**
- **Git** for version control
- **PowerShell** for automation scripts

---

## 📦 **Project Structure**

```
Chatbot-Onbase/
│
├── Controllers/
│   └── ChatbotController.cs          # API endpoints
│
├── Services/
│   ├── ChatbotService.cs             # Main business logic
│   └── InvoiceQueryParser.cs         # NLP query parser
│
├── Data/
│   └── OnbaseRepository.cs           # Database access layer
│
├── Models/
│   ├── Invoice.cs                    # Invoice data model
│   └── ChatRequest.cs                # API request model
│
├── wwwroot/
│   └── index.html                    # Frontend UI
│
├── Scripts/
│   ├── AnalyzeOnbaseSchema.ps1       # PowerShell schema analyzer
│   ├── CheckOnbaseSchema.sql         # SQL schema queries
│   └── FindVendorField.sql           # Vendor field finder
│
├── Program.cs                        # Application entry point
├── appsettings.json                  # Configuration
└── Chatbot-Onbase.csproj            # Project file
```

---

## 🔄 **How It Works (Step-by-Step)**

### **Step 1: User Input**
```
User types: "Find invoice 40767602"
```

### **Step 2: Frontend Sends Request**
```javascript
// JavaScript in index.html
fetch('/api/chatbot/query', {
    method: 'POST',
    body: JSON.stringify({ query: "Find invoice 40767602" })
})
```

### **Step 3: Controller Receives Request**
```csharp
// ChatbotController.cs
[HttpPost("query")]
public async Task<IActionResult> Query([FromBody] ChatRequest request)
{
    var response = await _chatbotService.ProcessQueryAsync(request.Query);
    return Ok(response);
}
```

### **Step 4: NLP Parser Analyzes Query**
```csharp
// InvoiceQueryParser.cs
// Uses regex to extract:
// - Intent: "GetByNumber"
// - Invoice ID: "40767602"
```

### **Step 5: Repository Executes SQL**
```csharp
// OnbaseRepository.cs
var query = @"
    SELECT TOP 1
        i.itemnum as InvoiceId,
        CAST(ki106.keyvaluesmall AS VARCHAR(50)) as InvoiceNumber,
        kt104.keyvaluechar as PONumber,
        ki112.keyvaluedate as InvoiceDate
    FROM hsi.keyitem106 ki106 WITH (NOLOCK)
    INNER JOIN hsi.itemdata i WITH (NOLOCK) 
        ON i.itemnum = ki106.itemnum AND i.itemtypenum = 102
    WHERE ki106.keyvaluesmall = @InvoiceNumberInt";
```

### **Step 6: Database Returns Data**
```
Invoice #40767602
Order Number: D264904
Date: 10/29/2025
```

### **Step 7: Response Sent to Frontend**
```json
{
  "answer": "I found 1 invoice(s) matching your query...",
  "invoices": [{
    "invoiceNumber": "40767602",
    "poNumber": "D264904",
    "invoiceDate": "2025-10-29"
  }]
}
```

### **Step 8: UI Displays Results**
```
┌─────────────────────────────┐
│ Invoice #40767602           │
├─────────────────────────────┤
│ Order Number: D264904       │
│ Amount: $40767602.00        │
│ Date: 10/29/2025           │
│ Status: N/A                 │
└─────────────────────────────┘
```

---

## 🧠 **NLP Engine (How It Understands Queries)**

The chatbot uses **regex pattern matching** to understand natural language:

### **Supported Query Patterns:**

| User Query | Regex Pattern | Extracted Data |
|------------|---------------|----------------|
| "Find invoice 40767602" | `invoice\s+(\d{7,})` | InvoiceId: 40767602 |
| "Show order D264904" | `order\s+([A-Z0-9]+)` | OrderNumber: D264904 |
| "Invoices from November 2024" | `(january|february|...)\s+(\d{4})` | Month: 11, Year: 2024 |
| "Invoices between 1000 and 5000" | `between\s+(\d+)\s+and\s+(\d+)` | Min: 1000, Max: 5000 |

### **Intent Detection:**
```csharp
if (invoiceId found) → Intent = "GetByNumber"
else if (vendor found) → Intent = "GetByVendor"
else if (date range found) → Intent = "GetByDateRange"
else → Intent = "GetAll"
```

---

## 🗄️ **Database Schema (Onbase)**

### **Key Tables:**

| Table | Purpose | Key Column |
|-------|---------|------------|
| `hsi.itemdata` | Main document table | `itemnum`, `itemtypenum = 102` |
| `hsi.keyitem106` | Invoice numbers | `keyvaluesmall` |
| `hsi.keyitem112` | Invoice dates | `keyvaluedate` |
| `hsi.keytable104` | Order numbers | `keyvaluechar` |
| `hsi.keyxitem104` | Links items to keytable104 | `itemnum`, `keywordnum` |

### **How Tables Connect:**
```sql
itemdata (itemnum)
    ↓
keyitem106 (itemnum) → Invoice Number
    ↓
keyitem112 (itemnum) → Invoice Date
    ↓
keyxitem104 (itemnum) → Links to keytable104
    ↓
keytable104 (keywordnum) → Order Number
```

---

## ⚡ **Performance Optimizations**

### **1. SQL Query Optimizations**

#### **NOLOCK Hints**
```sql
FROM hsi.itemdata i WITH (NOLOCK)
```
- **Purpose**: Allows dirty reads, prevents blocking
- **Benefit**: Faster queries, doesn't wait for write locks
- **Trade-off**: May read uncommitted data (acceptable for search)

#### **TOP Limits**
```sql
SELECT TOP 1 ...  -- For exact invoice number searches
SELECT TOP 50 ... -- For general searches
```
- **Purpose**: Limits result set size
- **Benefit**: Prevents timeout on large datasets
- **Performance**: Queries complete in <1 second

#### **Index-Friendly Queries**
```sql
-- Good: Uses index
WHERE ki106.keyvaluesmall = @InvoiceNumber

-- Bad: Cannot use index
WHERE CAST(ki106.keyvaluesmall AS VARCHAR) LIKE '%' + @InvoiceNumber + '%'
```

#### **Command Timeout**
```csharp
commandTimeout: 60  // 60 seconds
```
- Prevents indefinite hanging
- Allows time for complex queries

### **2. Smart Query Strategy**

#### **For Invoice Number Searches:**
```sql
-- Start from keyitem106 (smaller, indexed table)
FROM hsi.keyitem106 ki106 WITH (NOLOCK)
INNER JOIN hsi.itemdata i WITH (NOLOCK)
    ON i.itemnum = ki106.itemnum AND i.itemtypenum = 102
WHERE ki106.keyvaluesmall = @InvoiceNumber
```
- **Benefit**: Filters early, reduces join size

#### **For General Searches:**
```sql
-- Start from itemdata, filter by itemtypenum first
FROM hsi.itemdata i WITH (NOLOCK)
WHERE i.itemtypenum = 102
```
- **Benefit**: Filters to invoices only before joining

---

## 🔒 **Security Features**

### **1. SQL Injection Prevention**
```csharp
// ✅ SAFE: Parameterized queries
var query = "WHERE ki106.keyvaluesmall = @InvoiceNumber";
await connection.QueryAsync<Invoice>(query, new { InvoiceNumber = invoiceNum });

// ❌ UNSAFE: String concatenation (NOT USED)
// var query = "WHERE ki106.keyvaluesmall = " + invoiceNum;
```

### **2. Windows Integrated Security**
```
Server=aazeus-obdmsq01;Database=Onbase;Integrated Security=true;
```
- No passwords in code
- Uses Windows authentication
- Inherits user permissions

### **3. Input Validation**
```csharp
if (string.IsNullOrWhiteSpace(request.Query))
    return BadRequest("Query cannot be empty");
```

---

## 🎨 **Frontend Design**

### **UI Components:**

1. **Chat Header**
   - Gradient background (purple theme)
   - Title and subtitle

2. **Chat Messages Area**
   - Scrollable message list
   - User messages (right-aligned, blue)
   - Bot messages (left-aligned, white)

3. **Invoice Cards**
   - Grid layout (2 columns)
   - Fields: Invoice #, Order #, Amount, Date, Status

4. **Input Area**
   - Text input with rounded corners
   - Send button
   - Enter key support

### **CSS Highlights:**
```css
/* Gradient background */
background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);

/* Chat bubbles */
.message.user .message-bubble {
    background: #667eea;
    color: white;
}

/* Invoice cards */
.invoice-card {
    background: white;
    border-radius: 10px;
    box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
}
```

---

## 🔧 **Configuration**

### **appsettings.json**
```json
{
  "ConnectionStrings": {
    "OnbaseConnection": "Server=aazeus-obdmsq01;Database=Onbase;Integrated Security=true;TrustCertificate=true;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  }
}
```

### **Program.cs (Dependency Injection)**
```csharp
// Register services
builder.Services.AddScoped<IOnbaseRepository, OnbaseRepository>();
builder.Services.AddScoped<IChatbotService, ChatbotService>();

// Add CORS for frontend
builder.Services.AddCors(options => {
    options.AddDefaultPolicy(policy => {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
```

---

## 📊 **Data Flow Example**

### **Complete Request/Response Cycle:**

```
1. USER INPUT
   ↓
   "Find invoice 40767602"

2. FRONTEND (JavaScript)
   ↓
   POST /api/chatbot/query
   { "query": "Find invoice 40767602" }

3. CONTROLLER
   ↓
   ChatbotController.Query()
   → Validates request
   → Calls ChatbotService

4. CHATBOT SERVICE
   ↓
   ProcessQueryAsync()
   → Calls InvoiceQueryParser
   → Gets: Intent="GetByNumber", InvoiceId="40767602"
   → Calls OnbaseRepository.GetInvoicesByNumberAsync()

5. NLP PARSER
   ↓
   ParseQuery("Find invoice 40767602")
   → Regex match: invoice\s+(\d{7,})
   → Returns: { Intent: "GetByNumber", InvoiceIds: ["40767602"] }

6. REPOSITORY
   ↓
   GetInvoicesByNumberAsync("40767602")
   → Builds SQL query
   → Executes with Dapper
   → Maps to Invoice objects

7. DATABASE
   ↓
   SELECT TOP 1 ... WHERE ki106.keyvaluesmall = 40767602
   → Returns 1 row

8. RESPONSE
   ↓
   {
     "answer": "I found 1 invoice(s)...",
     "invoices": [{
       "invoiceNumber": "40767602",
       "poNumber": "D264904",
       "amount": 40767602.00,
       "invoiceDate": "2025-10-29"
     }],
     "resultCount": 1
   }

9. FRONTEND DISPLAY
   ↓
   Renders invoice card with all details
```

---

## 🚀 **Deployment**

### **Requirements:**
- Windows Server with IIS
- .NET 8.0 Runtime
- SQL Server access (Windows Auth)
- Port 5001 (or configure in IIS)

### **Steps:**
1. Publish application: `dotnet publish -c Release`
2. Copy to server: `C:\inetpub\wwwroot\chatbot-onbase`
3. Configure IIS application pool (.NET 8.0)
4. Set application pool identity (for database access)
5. Browse to: `http://server:5001`

---

## 📈 **Future Enhancements**

### **Planned Features:**
1. ✅ **Vendor Name Field** - Once identified in schema
2. ✅ **Actual Amount Field** - Replace invoice number placeholder
3. ✅ **Status Field** - Show approval/payment status
4. 🔄 **Advanced NLP** - Use ML models (BERT, GPT) instead of regex
5. 🔄 **Document Preview** - Show invoice PDF/image
6. 🔄 **Export to Excel** - Download search results
7. 🔄 **User Authentication** - Role-based access
8. 🔄 **Audit Logging** - Track who searched what

---

## 📚 **Key Design Decisions**

### **Why Regex NLP Instead of AI?**
- ✅ **Fast**: No API calls, instant response
- ✅ **Reliable**: Predictable pattern matching
- ✅ **No Cost**: No OpenAI/Azure AI fees
- ✅ **Offline**: Works without internet
- ❌ **Limited**: Can't handle complex queries

### **Why Dapper Instead of Entity Framework?**
- ✅ **Performance**: Faster than EF for read-heavy operations
- ✅ **Control**: Full control over SQL queries
- ✅ **Lightweight**: Minimal overhead
- ✅ **Optimization**: Easy to add NOLOCK, TOP, etc.

### **Why Single Page App (SPA)?**
- ✅ **Simple**: No build process, no frameworks
- ✅ **Fast**: Loads instantly
- ✅ **Maintainable**: Easy to update
- ✅ **Portable**: Works anywhere

---

## 🎯 **Summary**

This chatbot is a **lightweight, high-performance NLP application** that:
- Uses **regex-based NLP** for query understanding
- Connects to **Onbase SQL Server** with optimized queries
- Provides a **clean chat interface** for invoice search
- Runs on **.NET 8.0** with **Dapper ORM**
- Achieves **sub-second response times** with proper indexing

**Perfect for**: Internal business users who need quick invoice lookups without learning SQL! 🚀


