# EPay Application Logging Guide

## Overview

The EPay application uses a custom file-based logging system that writes to a shared network path. The logger automatically creates log files if they don't exist and handles rotation based on date and file size.

---

## Configuration

### Web.config Settings

```xml
<appSettings>
  <!-- Shared network path (Production) -->
  <add key="LogFilePath" value="\\shared-server\logs\EPay"/>
  
  <!-- Local path (Development) -->
  <!-- <add key="LogFilePath" value="C:\Logs\EPay"/> -->
  
  <!-- Log level: Debug, Info, Warning, Error, Fatal -->
  <add key="LogLevel" value="Info"/>
</appSettings>
```

---

## Log Levels

| Level | Description | Use Case |
|-------|-------------|----------|
| **Debug** | Detailed diagnostic information | Development/troubleshooting only |
| **Info** | General informational messages | Normal application flow |
| **Warning** | Potentially harmful situations | Recoverable errors, deprecated features |
| **Error** | Error events that might still allow the application to continue | Exceptions, failed operations |
| **Fatal** | Very severe error events that will presumably lead the application to abort | Critical failures |

---

## Usage Examples

### Basic Logging

```vb
' Import the logger
Imports EPay.Logger

' Info logging
Logger.Info("User logged in successfully", "Login.aspx")
Logger.Info("Invoice search completed: 25 results found", "main.aspx")

' Warning logging
Logger.Warning("Database connection slow: 5 seconds", "DataAccess")
Logger.Warning("Customer has no open invoices", "main.aspx")

' Error logging
Logger.Error("Failed to load invoices", "main.aspx")

' Debug logging (development only)
Logger.Debug("Search parameters: CustomerNo=12345, FromDate=2025-01-01", "main.aspx")
```

### Logging with Exceptions

```vb
Try
    ' Your code here
    Dim invoices As DataTable = LoadInvoices()
    Logger.Info("Successfully loaded " & invoices.Rows.Count & " invoices", "main.aspx")
    
Catch ex As SqlException
    ' Log error with exception details
    Logger.Error("Database error while loading invoices", ex, "main.aspx")
    
Catch ex As Exception
    ' Log fatal error
    Logger.Fatal("Unexpected error in invoice loading", ex, "main.aspx")
    Throw
End Try
```

### Logging in Page Events

```vb
Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
    Try
        Logger.Info("Page_Load started", "main.aspx")
        
        If Not IsPostBack Then
            LoadDefaultData()
            Logger.Info("Default data loaded successfully", "main.aspx")
        End If
        
    Catch ex As Exception
        Logger.Error("Error in Page_Load", ex, "main.aspx")
        lblErrorMsg.Text = "An error occurred. Please try again."
    End Try
End Sub
```

### Logging Button Click Events

```vb
Protected Sub btnSearch_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSearch.Click
    Try
        Logger.Info("Search button clicked", "main.aspx")
        
        Dim customerNo As String = txtCustomerNumber.Text
        Logger.Debug("Search parameters: CustomerNo=" & customerNo, "main.aspx")
        
        Dim results As DataTable = SearchInvoices(customerNo)
        Logger.Info("Search completed: " & results.Rows.Count & " results", "main.aspx")
        
    Catch ex As Exception
        Logger.Error("Error during invoice search", ex, "main.aspx")
        ShowErrorMessage("Search failed. Please try again.")
    End Try
End Sub
```

### Logging Payment Processing

```vb
Protected Sub ProcessPayment(ByVal referenceNumber As Integer, ByVal amount As Decimal)
    Try
        Logger.Info(String.Format("Processing payment: RefNo={0}, Amount={1:C}", referenceNumber, amount), "Payment")
        
        ' Submit to US Bank
        Dim response As String = SubmitToUSBank(referenceNumber, amount)
        
        If response.Contains("SUCCESS") Then
            Logger.Info("Payment submitted successfully: RefNo=" & referenceNumber, "Payment")
        Else
            Logger.Warning("Payment submission returned non-success: " & response, "Payment")
        End If
        
    Catch ex As Exception
        Logger.Error("Payment processing failed: RefNo=" & referenceNumber, ex, "Payment")
        Throw
    End Try
End Sub
```

---

## Log File Format

### Log Entry Structure

```
2025-03-02 14:35:22.123 | [INFO   ] | Thread:5    | Source:main.aspx | User:DOMAIN\jsmith | Invoice search completed: 25 results found
2025-03-02 14:35:45.456 | [ERROR  ] | Thread:8    | Source:Payment   | User:DOMAIN\jdoe   | Payment processing failed: RefNo=12345
Exception: Timeout expired. The timeout period elapsed prior to completion of the operation.
StackTrace:    at System.Data.SqlClient.SqlConnection.OnError(SqlException exception, Boolean breakConnection)
```

### Log File Naming

- **Daily logs**: `EPay_20250302.log`
- **Archived logs** (when size > 50MB): `EPay_20250302_143522.log`

---

## Features

### ✅ Automatic File Creation
- Creates log directory if it doesn't exist
- Creates log file automatically on first write
- No manual setup required

### ✅ Thread-Safe
- Multiple concurrent requests can log safely
- Uses locking mechanism to prevent corruption

### ✅ Automatic Rotation
- **Daily rotation**: New file created each day
- **Size-based rotation**: Archives file when > 50MB
- Old files are preserved with timestamp

### ✅ Retry Logic
- Retries up to 3 times on file access errors
- 100ms delay between retries
- Prevents log loss due to temporary issues

### ✅ Fallback to Event Log
- If file logging fails, writes to Windows Event Log
- Ensures critical errors are never lost
- Event source: "EPay Application"

---

## Best Practices

### 1. Use Appropriate Log Levels
```vb
' ✅ Good
Logger.Info("User logged in", "Login")
Logger.Error("Database connection failed", ex, "DataAccess")

' ❌ Bad
Logger.Error("User logged in", "Login")  ' Not an error
Logger.Info("Critical database failure", "DataAccess")  ' Should be Error/Fatal
```

### 2. Include Context
```vb
' ✅ Good
Logger.Info("Invoice search: CustomerNo=12345, Results=25", "main.aspx")

' ❌ Bad
Logger.Info("Search completed", "main.aspx")  ' Missing context
```

### 3. Log Exceptions Properly
```vb
' ✅ Good
Logger.Error("Failed to process payment", ex, "Payment")

' ❌ Bad
Logger.Error(ex.Message, "Payment")  ' Loses stack trace
```

### 4. Don't Log Sensitive Data
```vb
' ✅ Good
Logger.Info("Payment processed: RefNo=12345, Amount=$100.00", "Payment")

' ❌ Bad
Logger.Info("Payment: CardNo=4111111111111111, CVV=123", "Payment")  ' PCI violation!
```

---

## Troubleshooting

### Issue: Logs not appearing in shared path

**Solution:**
1. Check network path permissions
2. Verify IIS application pool identity has write access
3. Check Web.config `LogFilePath` setting
4. Look for logs in Windows Event Viewer (fallback location)

### Issue: "Access Denied" errors

**Solution:**
1. Grant write permissions to IIS application pool identity
2. Use local path for development: `C:\Logs\EPay`
3. Check firewall/network restrictions

### Issue: Log files too large

**Solution:**
- Automatic rotation happens at 50MB
- Adjust `MAX_LOG_FILE_SIZE_MB` in Logger.vb if needed
- Implement log cleanup policy (delete logs older than 30 days)

---

## Copyright

Copyright © 2025 Ashley Furniture Industries, Inc. All rights reserved.  
This software is proprietary and confidential. Unauthorized copying, distribution, or use of this software, via any medium, is strictly prohibited.

