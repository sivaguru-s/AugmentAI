# IIS Deployment Troubleshooting Guide

## Enhanced Logging Features

The application now includes comprehensive logging to help diagnose IIS deployment issues:

### 1. **Detailed Error Responses**
All API errors now return detailed information including:
- Error message
- Error details
- Exception type
- SQL error number and state (for database errors)
- Timestamp

### 2. **New Diagnostic Endpoints**

#### Health Check Endpoint
```
GET /api/chatbot/health
```

**Response:**
```json
{
  "status": "healthy",
  "timestamp": "2026-03-13T12:00:00Z",
  "environment": "Production",
  "machineName": "SERVER-NAME",
  "version": "1.0.0"
}
```

#### Database Connection Test
```
GET /api/chatbot/dbtest
```

**Response (Success):**
```json
{
  "status": "connected",
  "server": "AE1DCVPSQ23407",
  "database": "Onbase",
  "serverVersion": "15.00.4153",
  "connectionTimeMs": 123.45,
  "timestamp": "2026-03-13T12:00:00Z"
}
```

**Response (Failure):**
```json
{
  "status": "connection_failed",
  "error": "A network-related or instance-specific error...",
  "errorNumber": 53,
  "state": 0,
  "server": "AE1DCVPSQ23407",
  "timestamp": "2026-03-13T12:00:00Z"
}
```

### 3. **Enhanced Frontend Error Display**

The frontend now shows detailed error information:
- HTTP status codes
- Error details from server
- Exception types
- Connection diagnostics
- Automatic API connectivity test on page load

### 4. **Comprehensive Log Files**

All errors are logged to `Logs/chatbot-YYYYMMDD.log` with:
- Request IP address
- Query details
- SQL error numbers and states
- Full exception stack traces
- Performance metrics

## Troubleshooting Steps for IIS

### Step 1: Check Application Logs

After deploying to IIS, check the log files in the `Logs/` directory:

```powershell
# View latest log
Get-Content "C:\inetpub\wwwroot\ChatbotOnbase\Logs\chatbot-*.log" -Tail 50

# Search for errors
Select-String -Path "C:\inetpub\wwwroot\ChatbotOnbase\Logs\*.log" -Pattern "\[ERR\]|\[FTL\]"
```

### Step 2: Test Health Endpoint

Open browser and navigate to:
```
http://localhost/chatbotonbase/api/chatbot/health
```

Or use PowerShell:
```powershell
Invoke-WebRequest -Uri "http://localhost/chatbotonbase/api/chatbot/health" -UseBasicParsing
```

### Step 3: Test Database Connection

Navigate to:
```
http://localhost/chatbotonbase/api/chatbot/dbtest
```

Or use PowerShell:
```powershell
$response = Invoke-WebRequest -Uri "http://localhost/chatbotonbase/api/chatbot/dbtest" -UseBasicParsing
$response.Content | ConvertFrom-Json | Format-List
```

### Step 4: Check Browser Console

1. Open the application in browser
2. Press F12 to open Developer Tools
3. Go to Console tab
4. Look for diagnostic messages:
   - "Page loaded. Running diagnostics..."
   - Health check results
   - Database test results
   - Any error messages

### Step 5: Test API Directly

Use PowerShell to test the query endpoint:

```powershell
$body = @{ prompt = "show all invoices" } | ConvertTo-Json
$response = Invoke-WebRequest `
    -Uri "http://localhost/chatbotonbase/api/chatbot/query" `
    -Method POST `
    -Body $body `
    -ContentType "application/json" `
    -UseBasicParsing

$response.Content | ConvertFrom-Json | Format-List
```

## Common IIS Issues and Solutions

### Issue 1: 500 Internal Server Error

**Symptoms:**
- Generic 500 error
- No detailed error message

**Solutions:**
1. Check `Logs/chatbot-*.log` for detailed error
2. Enable detailed errors in web.config:
   ```xml
   <aspNetCore stdoutLogEnabled="true" stdoutLogFile=".\logs\stdout" />
   ```
3. Check Windows Event Viewer → Application logs

### Issue 2: Database Connection Failed

**Symptoms:**
- `/api/chatbot/dbtest` returns connection_failed
- SQL error numbers in logs

**Solutions:**
1. Verify connection string in appsettings.json
2. Check if IIS Application Pool identity has database access:
   ```sql
   -- Run on SQL Server
   CREATE LOGIN [IIS APPPOOL\ChatbotOnbaseAppPool] FROM WINDOWS;
   USE Onbase;
   CREATE USER [IIS APPPOOL\ChatbotOnbaseAppPool] FOR LOGIN [IIS APPPOOL\ChatbotOnbaseAppPool];
   GRANT SELECT ON SCHEMA::hsi TO [IIS APPPOOL\ChatbotOnbaseAppPool];
   ```
3. Test connection from IIS server:
   ```powershell
   sqlcmd -S AE1DCVPSQ23407 -d Onbase -E -Q "SELECT @@VERSION"
   ```

### Issue 3: API URL Not Found (404)

**Symptoms:**
- Frontend shows "Cannot connect to server"
- 404 errors in browser console

**Solutions:**
1. Check IIS application path matches API_URL in index.html
2. Verify web.config has correct settings
3. Check IIS URL Rewrite rules
4. Ensure application is in correct IIS site

### Issue 4: CORS Errors

**Symptoms:**
- "Access-Control-Allow-Origin" errors in console
- Requests blocked by browser

**Solutions:**
- Application already has CORS enabled for all origins
- Check if IIS has additional CORS restrictions
- Verify no conflicting CORS headers in web.config

## Log File Analysis

### Startup Logs
Look for these entries to confirm successful startup:
```
[INF] Starting Onbase Invoice Chatbot application
[INF] Database Connection String: Server=AE1DCVPSQ23407;...
[INF] Application configured successfully
[INF] Application is ready to accept requests
```

### Query Logs
Successful query pattern:
```
[INF] Received query request from ::1
[INF] Processing query: show all invoices
[INF] Searching invoices for vendor: ...
[INF] Found 100 invoices for vendor ... in 1234ms
[INF] Query processed successfully. Result count: 100
```

### Error Logs
Database error example:
```
[ERR] Database error processing query: show all invoices. Error Number: 53, State: 0, Server: AE1DCVPSQ23407
System.Data.SqlClient.SqlException: A network-related or instance-specific error...
```

## Next Steps

1. Deploy application to IIS
2. Navigate to health endpoint to verify deployment
3. Test database connection endpoint
4. Check browser console for diagnostic output
5. Review log files for any errors
6. Test a simple query
7. If errors occur, check logs and use diagnostic endpoints to identify the issue

