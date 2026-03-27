# Backend Setup Guide - EPay API

Complete step-by-step guide to configure and run the ASP.NET Core 8.0 backend on your local machine.

---

## 📋 Prerequisites

### Required Software

1. **.NET 8.0 SDK** - [Download](https://dotnet.microsoft.com/download/dotnet/8.0)
2. **Visual Studio 2022** (any edition)
3. **SQL Server** (2019 or later)
4. **Git** (already installed)

### Required Database Access

- **Datawhse** database with stored procedure: `usp_OrderAndInvoiceReportingOpenInvoices3`
- **Ashley** database with stored procedure: `usp_GetEpayDefaultDateSpanInDays`

---

## 🚀 Step-by-Step Setup

### Step 1: Verify .NET Installation

Open PowerShell or Command Prompt:

```powershell
dotnet --version
```

**Expected Output:** `8.0.x` or higher

If not installed, download and install .NET 8.0 SDK from the link above.

---

### Step 2: Navigate to Backend Directory

```powershell
cd C:\AugmentAI\finance-credit-direct-epay\backend
```

---

### Step 3: Configure Database Connection Strings

Edit `appsettings.Development.json` and update the connection strings:

```json
{
  "ConnectionStrings": {
    "AFI_Batch": "Server=YOUR_SERVER_NAME;Database=Datawhse;Integrated Security=true;TrustServerCertificate=true;",
    "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=Datawhse;Integrated Security=true;TrustServerCertificate=true;",
    "AshleyDatabase": "Server=YOUR_SERVER_NAME;Database=Ashley;Integrated Security=true;TrustServerCertificate=true;"
  }
}
```

**Replace `YOUR_SERVER_NAME` with your actual SQL Server instance name.**

#### Common SQL Server Names:
- `localhost` - Local SQL Server
- `(localdb)\MSSQLLocalDB` - LocalDB instance
- `YOUR_COMPUTER_NAME\SQLEXPRESS` - SQL Server Express
- `YOUR_COMPUTER_NAME` - Default instance

#### Example:
```json
"AFI_Batch": "Server=localhost;Database=Datawhse;Integrated Security=true;TrustServerCertificate=true;"
```

---

### Step 4: Restore NuGet Packages

```powershell
dotnet restore
```

**Expected Output:**
```
Restore succeeded.
```

This will download all required NuGet packages:
- Microsoft.AspNetCore.OpenApi
- Swashbuckle.AspNetCore
- Microsoft.Data.SqlClient
- Serilog.AspNetCore
- ClosedXML

---

### Step 5: Build the Solution

```powershell
dotnet build
```

**Expected Output:**
```
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

If you see errors, check:
- All namespaces are `EPay.Api.*`
- Connection strings are configured
- .NET 8.0 SDK is installed

---

### Step 6: Run the Application

```powershell
dotnet run
```

**Expected Output:**
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:5001
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
```

The API is now running!

---

## 🌐 Access the Application

### Swagger UI (API Documentation)

Open your browser and navigate to:

**https://localhost:5001**

You should see the Swagger UI with 3 endpoints:

1. **POST /api/invoice/search** - Search invoices with pagination
2. **GET /api/invoice/default-date-span** - Get default date range
3. **POST /api/invoice/export** - Export invoices to Excel

---

## 🔧 Alternative: Run with Visual Studio 2022

### Option A: Open Solution File

1. Open Visual Studio 2022
2. File → Open → Project/Solution
3. Navigate to: `C:\AugmentAI\finance-credit-direct-epay\backend`
4. Select: `EPay.Api.sln`
5. Press **F5** or click **Start** button

### Option B: Open Folder

1. Open Visual Studio 2022
2. File → Open → Folder
3. Navigate to: `C:\AugmentAI\finance-credit-direct-epay\backend`
4. Visual Studio will detect the project automatically
5. Press **F5** or click **Start** button

---

## ✅ Verify Installation

### Test 1: Check Swagger UI

Navigate to: https://localhost:5001

You should see the Swagger documentation page.

### Test 2: Test Default Date Span Endpoint

In Swagger UI:
1. Click on **GET /api/invoice/default-date-span**
2. Click **Try it out**
3. Click **Execute**

**Expected Response:**
```json
90
```

(or whatever value is configured in your database)

---

## 🔍 Troubleshooting

### Issue 1: "Connection string 'AFI_Batch' not found"

**Solution:** Make sure `appsettings.Development.json` has the `AFI_Batch` connection string:

```json
"ConnectionStrings": {
  "AFI_Batch": "Server=YOUR_SERVER;Database=Datawhse;..."
}
```

### Issue 2: "Cannot connect to SQL Server"

**Possible Causes:**
1. SQL Server is not running
2. Wrong server name
3. Database doesn't exist
4. Windows Authentication not configured

**Solutions:**

**Check SQL Server is running:**
```powershell
# Open Services
services.msc
# Look for "SQL Server (MSSQLSERVER)" or "SQL Server (SQLEXPRESS)"
# Make sure it's running
```

**Test connection with SQLCMD:**
```powershell
sqlcmd -S localhost -E -Q "SELECT @@VERSION"
```

**Verify database exists:**
```sql
SELECT name FROM sys.databases WHERE name IN ('Datawhse', 'Ashley')
```

### Issue 3: "Stored procedure not found"

**Error:** `Could not find stored procedure 'Datawhse.dbo.usp_OrderAndInvoiceReportingOpenInvoices3'`

**Solution:** Verify the stored procedure exists:

```sql
USE Datawhse
GO
SELECT * FROM sys.procedures WHERE name = 'usp_OrderAndInvoiceReportingOpenInvoices3'
```

If it doesn't exist, you need to create it or restore the database.

### Issue 4: Build Errors - Namespace Issues

**Error:** `The type or namespace name 'Backend' does not exist`

**Solution:** All namespaces should be `EPay.Api.*`, not `EPay.Backend.*`

Check these files:
- Controllers/InvoiceController.cs
- Services/InvoiceService.cs
- Repositories/InvoiceRepository.cs
- Models/DTOs/*.cs

### Issue 5: Port Already in Use

**Error:** `Failed to bind to address https://127.0.0.1:5001: address already in use`

**Solution:** Change the port in `Properties/launchSettings.json`:

```json
"applicationUrl": "https://localhost:5002;http://localhost:5003"
```

### Issue 6: SSL Certificate Error

**Error:** `The SSL connection could not be established`

**Solution:** Trust the development certificate:

```powershell
dotnet dev-certs https --trust
```

---

## 📁 Project Structure

```
backend/
├── Controllers/
│   └── InvoiceController.cs          # API endpoints
├── Services/
│   ├── IInvoiceService.cs            # Service interface
│   └── InvoiceService.cs             # Business logic
├── Repositories/
│   ├── IInvoiceRepository.cs         # Repository interface
│   └── InvoiceRepository.cs          # Data access
├── Models/DTOs/
│   ├── InvoiceDto.cs                 # Invoice model
│   ├── InvoiceSearchRequest.cs       # Search request
│   ├── InvoiceSearchResponse.cs      # Search response
│   └── PagedResult.cs                # Pagination wrapper
├── Properties/
│   └── launchSettings.json           # Development settings
├── Program.cs                        # Application entry point
├── appsettings.json                  # Production config
├── appsettings.Development.json      # Development config
├── EPay.Api.csproj                   # Project file
└── EPay.Api.sln                      # Solution file
```

---

## 🔐 Configuration Files Explained

### appsettings.json (Production)

Used when running in production mode.

```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information"  // Log level for production
    }
  },
  "ConnectionStrings": {
    "AFI_Batch": "Server=PROD_SERVER;..."  // Production database
  }
}
```

### appsettings.Development.json (Development)

Used when running in development mode (default).

```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Debug"  // More verbose logging
    }
  },
  "ConnectionStrings": {
    "AFI_Batch": "Server=localhost;..."  // Local database
  }
}
```

---

## 🛠️ Useful Commands

### Build Commands

```powershell
# Clean build artifacts
dotnet clean

# Restore packages
dotnet restore

# Build solution
dotnet build

# Build in Release mode
dotnet build --configuration Release

# Run the application
dotnet run

# Run with specific environment
dotnet run --environment Production

# Watch mode (auto-restart on file changes)
dotnet watch run
```

### Database Commands

```powershell
# Test connection
sqlcmd -S localhost -E -Q "SELECT @@VERSION"

# List databases
sqlcmd -S localhost -E -Q "SELECT name FROM sys.databases"

# Check stored procedure
sqlcmd -S localhost -d Datawhse -E -Q "SELECT * FROM sys.procedures WHERE name LIKE '%Invoice%'"
```

---

## 📊 API Endpoints

### 1. Search Invoices

**Endpoint:** `POST /api/invoice/search`

**Request Body:**
```json
{
  "customerNumber": "123456",
  "shipToNumber": "",
  "allShipTos": false,
  "fromDate": "2024-01-01",
  "toDate": "2024-12-31",
  "invoiceNumber": "",
  "creditNumber": "",
  "poNumber": "",
  "sortColumn": "InvoiceDate",
  "sortAscending": false,
  "pageNumber": 1,
  "pageSize": 500,
  "showAll": false
}
```

**Response:**
```json
{
  "result": {
    "items": [...],
    "pageNumber": 1,
    "pageSize": 500,
    "totalPages": 5,
    "totalRecords": 2500
  },
  "fromDate": "2024-01-01",
  "toDate": "2024-12-31",
  "isAnalyst": false
}
```

### 2. Get Default Date Span

**Endpoint:** `GET /api/invoice/default-date-span`

**Response:**
```json
90
```

### 3. Export to Excel

**Endpoint:** `POST /api/invoice/export`

**Request Body:** Same as search endpoint

**Response:** Excel file download

---

## 🔒 Security Notes

### Authentication

Currently, the API uses `[Authorize]` attribute but authentication is not fully configured.

For development, you may need to:
1. Comment out `[Authorize]` attribute in `InvoiceController.cs`
2. Or configure JWT authentication in `Program.cs`

### CORS

CORS is configured to allow requests from Angular frontend:
- http://localhost:4200
- https://localhost:4200

---

## 📝 Logging

Logs are written to:
- **Console** - Real-time output
- **File** - `logs/epay-YYYYMMDD.txt` (daily rolling)

Log levels:
- **Debug** - Development only
- **Information** - General information
- **Warning** - Warnings
- **Error** - Errors and exceptions

---

## ✅ Next Steps

After successfully running the backend:

1. ✅ Test all 3 endpoints in Swagger UI
2. ✅ Verify database connectivity
3. ✅ Check logs for any errors
4. ✅ Set up the Angular frontend
5. ✅ Test end-to-end integration

---

## 📞 Support

If you encounter issues:

1. Check the **Troubleshooting** section above
2. Review the logs in `logs/` directory
3. Verify all prerequisites are installed
4. Check database connectivity

---

**Last Updated:** 2026-01-25
**Version:** 1.0
**Framework:** ASP.NET Core 8.0


