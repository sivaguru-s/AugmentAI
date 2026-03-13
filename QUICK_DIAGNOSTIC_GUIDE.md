# Quick Diagnostic Guide for IIS Deployment

## 🚀 Quick Start - Testing Your IIS Deployment

### Step 1: Open the Application
Navigate to your IIS application URL:
```
http://localhost/chatbotonbase/
```

### Step 2: Open Browser Console (F12)
Look for automatic diagnostic output:
```
Page loaded. Running diagnostics...
=== API Connection Test ===
Testing health endpoint...
Health Status: 200
Testing database connection...
DB Test Status: 200
=== All Tests Passed ===
```

### Step 3: Test Diagnostic Endpoints

#### Health Check
```
http://localhost/chatbotonbase/api/chatbot/health
```
**Expected Response:**
```json
{
  "status": "healthy",
  "timestamp": "2026-03-13T12:00:00Z",
  "environment": "Production",
  "machineName": "YOUR-SERVER",
  "version": "1.0.0"
}
```

#### Database Test
```
http://localhost/chatbotonbase/api/chatbot/dbtest
```
**Expected Response (Success):**
```json
{
  "status": "connected",
  "server": "AE1DCVPSQ23407",
  "database": "Onbase",
  "serverVersion": "15.00.4445",
  "connectionTimeMs": 123.45
}
```

**Response (Failure):**
```json
{
  "status": "connection_failed",
  "error": "Login failed for user...",
  "errorNumber": 18456,
  "state": 1,
  "server": "AE1DCVPSQ23407"
}
```

### Step 4: Check Log Files

Navigate to the Logs directory:
```
C:\inetpub\wwwroot\ChatbotOnbase\Logs\
```

View the latest log:
```powershell
Get-Content "C:\inetpub\wwwroot\ChatbotOnbase\Logs\chatbot-*.log" -Tail 50
```

Look for startup messages:
```
[INF] Starting Onbase Invoice Chatbot application
[INF] Environment: Production
[INF] Database Connection String: Server=AE1DCVPSQ23407;...
[INF] Application is ready to accept requests
```

### Step 5: Test a Query

Try a simple query in the UI:
```
show all invoices
```

Check browser console for detailed request/response logging.

## 🔍 Common Issues and Quick Fixes

### Issue: "Sorry, I encountered an error"

**Check:**
1. Browser Console (F12) - Look for detailed error message
2. Network Tab - Check response status and body
3. Log file - Look for [ERR] entries

**Example Error in Console:**
```
❌ Error (500): Database error occurred
Details: SQL Error 53: A network-related error...
Type: SqlException
```

### Issue: Database Connection Failed

**Quick Test:**
```powershell
# From IIS server
sqlcmd -S AE1DCVPSQ23407 -d Onbase -E -Q "SELECT @@VERSION"
```

**Fix:**
Grant database access to IIS App Pool:
```sql
CREATE LOGIN [IIS APPPOOL\YourAppPoolName] FROM WINDOWS;
USE Onbase;
CREATE USER [IIS APPPOOL\YourAppPoolName] FOR LOGIN [IIS APPPOOL\YourAppPoolName];
GRANT SELECT ON SCHEMA::hsi TO [IIS APPPOOL\YourAppPoolName];
```

### Issue: 404 Not Found

**Check:**
- IIS application path
- web.config exists
- Application pool is running

**Test:**
```powershell
Invoke-WebRequest -Uri "http://localhost/chatbotonbase/api/chatbot/health"
```

## 📊 Understanding Error Messages

### Frontend Error Format
```
❌ Error (500): Database error occurred

Details: SQL Error 53: A network-related error...

Type: SqlException

Time: 3/13/2026, 12:00:00 PM
```

### Log File Error Format
```
[ERR] Database error processing query: show all invoices. 
Error Number: 53, State: 0, Server: AE1DCVPSQ23407
System.Data.SqlClient.SqlException: A network-related error...
   at Chatbot_Onbase.Data.OnbaseRepository...
```

## 🛠️ PowerShell Testing Commands

### Test All Endpoints
```powershell
# Health Check
Invoke-WebRequest -Uri "http://localhost/chatbotonbase/api/chatbot/health" | 
    Select-Object -ExpandProperty Content | ConvertFrom-Json

# Database Test
Invoke-WebRequest -Uri "http://localhost/chatbotonbase/api/chatbot/dbtest" | 
    Select-Object -ExpandProperty Content | ConvertFrom-Json

# Query Test
$body = @{ prompt = "show all invoices" } | ConvertTo-Json
Invoke-WebRequest -Uri "http://localhost/chatbotonbase/api/chatbot/query" `
    -Method POST -Body $body -ContentType "application/json" | 
    Select-Object -ExpandProperty Content | ConvertFrom-Json
```

### Monitor Logs in Real-Time
```powershell
Get-Content "C:\inetpub\wwwroot\ChatbotOnbase\Logs\chatbot-*.log" -Wait -Tail 20
```

### Search for Errors
```powershell
Select-String -Path "C:\inetpub\wwwroot\ChatbotOnbase\Logs\*.log" `
    -Pattern "\[ERR\]|\[FTL\]" | Select-Object -Last 10
```

## ✅ Success Indicators

### Healthy Application
- ✅ Health endpoint returns 200
- ✅ Database test shows "connected"
- ✅ Browser console shows "All Tests Passed"
- ✅ Log file shows "Application is ready to accept requests"
- ✅ Queries return results without errors

### Unhealthy Application
- ❌ Health endpoint returns 500
- ❌ Database test shows "connection_failed"
- ❌ Browser console shows connection errors
- ❌ Log file shows [ERR] or [FTL] entries
- ❌ Queries return error messages

## 📞 Getting Help

If issues persist:

1. **Collect Information:**
   - Browser console output (F12)
   - Network tab response (F12 → Network)
   - Last 50 lines of log file
   - Database test endpoint response

2. **Check Documentation:**
   - IIS_DEPLOYMENT_TROUBLESHOOTING.md
   - ENHANCED_LOGGING_SUMMARY.md
   - LOGGING_GUIDE.md

3. **Common Fixes:**
   - Restart IIS Application Pool
   - Verify database connectivity
   - Check file permissions
   - Review connection string

