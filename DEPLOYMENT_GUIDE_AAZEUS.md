# Deployment Guide for AAZEUS-FNUKAP01 Server

## Server Information
- **Server Name:** AAZEUS-FNUKAP01
- **Application URL:** http://aazeus-fnukap01/chatbotonbase/
- **IIS Virtual Directory:** chatbotonbase
- **Database Server:** AE1DCVPSQ23407
- **Database:** Onbase

## Pre-Deployment Checklist

### 1. Verify Database Connectivity
From the AAZEUS-FNUKAP01 server, test SQL connection:

```powershell
# Test SQL Server connectivity
sqlcmd -S AE1DCVPSQ23407 -d Onbase -E -Q "SELECT @@VERSION"

# Test specific table access
sqlcmd -S AE1DCVPSQ23407 -d Onbase -E -Q "SELECT TOP 1 * FROM hsi.itemdata"
```

### 2. Check IIS Application Pool Identity
The application pool needs database access:

```sql
-- Run on SQL Server AE1DCVPSQ23407
-- Replace 'ChatbotOnbaseAppPool' with your actual app pool name

CREATE LOGIN [IIS APPPOOL\ChatbotOnbaseAppPool] FROM WINDOWS;
USE Onbase;
CREATE USER [IIS APPPOOL\ChatbotOnbaseAppPool] FOR LOGIN [IIS APPPOOL\ChatbotOnbaseAppPool];
GRANT SELECT ON SCHEMA::hsi TO [IIS APPPOOL\ChatbotOnbaseAppPool];
```

## Deployment Steps

### Step 1: Build the Application

On your development machine:

```powershell
cd C:\Chatbot-Onbase

# Build in Release mode
dotnet publish -c Release -o ./publish

# Verify build output
dir ./publish
```

### Step 2: Copy Files to Server

Copy the published files to the server:

```powershell
# Option 1: If you have network access to the server
Copy-Item -Path ./publish/* -Destination "\\aazeus-fnukap01\c$\inetpub\wwwroot\chatbotonbase\" -Recurse -Force

# Option 2: Manual copy
# 1. Zip the ./publish folder
# 2. Copy to server
# 3. Extract to C:\inetpub\wwwroot\chatbotonbase\
```

### Step 3: Configure IIS on AAZEUS-FNUKAP01

On the server (AAZEUS-FNUKAP01):

```powershell
# Import IIS module
Import-Module WebAdministration

# Create Application Pool (if not exists)
New-WebAppPool -Name "ChatbotOnbaseAppPool"

# Configure Application Pool
Set-ItemProperty IIS:\AppPools\ChatbotOnbaseAppPool -Name "managedRuntimeVersion" -Value ""
Set-ItemProperty IIS:\AppPools\ChatbotOnbaseAppPool -Name "enable32BitAppOnWin64" -Value $false

# Create/Update Application
New-WebApplication -Name "chatbotonbase" -Site "Default Web Site" -PhysicalPath "C:\inetpub\wwwroot\chatbotonbase" -ApplicationPool "ChatbotOnbaseAppPool" -Force

# Verify
Get-WebApplication -Name "chatbotonbase"
```

### Step 4: Verify Configuration Files

Ensure these files exist in `C:\inetpub\wwwroot\chatbotonbase\`:

1. **appsettings.json** - Check connection string:
```json
{
  "ConnectionStrings": {
    "OnBaseConnection": "Server=AE1DCVPSQ23407;Database=Onbase;Integrated Security=true;TrustServerCertificate=true;"
  }
}
```

2. **web.config** - Should be auto-generated, verify it exists

### Step 5: Set Permissions

```powershell
# Grant IIS_IUSRS read access
icacls "C:\inetpub\wwwroot\chatbotonbase" /grant "IIS_IUSRS:(OI)(CI)R" /T

# Grant App Pool identity full access to Logs folder
icacls "C:\inetpub\wwwroot\chatbotonbase\Logs" /grant "IIS APPPOOL\ChatbotOnbaseAppPool:(OI)(CI)F" /T
```

## Post-Deployment Testing

### Step 1: Test from Server

On AAZEUS-FNUKAP01, run these PowerShell commands:

```powershell
# Test health endpoint
Invoke-WebRequest -Uri "http://localhost/chatbotonbase/api/chatbot/health" -UseBasicParsing | 
    Select-Object -ExpandProperty Content | ConvertFrom-Json | Format-List

# Expected output:
# status      : healthy
# timestamp   : 2026-03-13T...
# environment : Production
# machineName : AAZEUS-FNUKAP01
# version     : 1.0.0
```

```powershell
# Test database connection
Invoke-WebRequest -Uri "http://localhost/chatbotonbase/api/chatbot/dbtest" -UseBasicParsing | 
    Select-Object -ExpandProperty Content | ConvertFrom-Json | Format-List

# Expected output:
# status           : connected
# server           : AE1DCVPSQ23407
# database         : Onbase
# serverVersion    : 15.00.4445
# connectionTimeMs : 1234.56
```

```powershell
# Test a query
$body = @{ prompt = "show all invoices" } | ConvertTo-Json
Invoke-WebRequest -Uri "http://localhost/chatbotonbase/api/chatbot/query" `
    -Method POST -Body $body -ContentType "application/json" -UseBasicParsing | 
    Select-Object -ExpandProperty Content | ConvertFrom-Json | Format-List

# Expected output:
# resultCount : 100
# message     : Found 100 invoices...
# invoices    : {...}
```

### Step 2: Test from Browser

1. **Open browser** and navigate to:
   ```
   http://aazeus-fnukap01/chatbotonbase/
   ```

2. **Press F12** to open Developer Tools

3. **Check Console tab** for diagnostic output:
   ```
   === API Connection Test ===
   Base Path: /chatbotonbase/
   API URL: /chatbotonbase/api/chatbot/query
   Current Location: http://aazeus-fnukap01/chatbotonbase/index.html
   Testing health endpoint...
   Health Status: 200
   Testing database connection...
   DB Test Status: 200
   === All Tests Passed ===
   ```

4. **Try a query:**
   - Type: "vendor V207"
   - Click Send
   - Should see results

### Step 3: Check Log Files

On the server:

```powershell
# View latest log
Get-Content "C:\inetpub\wwwroot\chatbotonbase\Logs\chatbot-*.log" -Tail 50

# Monitor in real-time
Get-Content "C:\inetpub\wwwroot\chatbotonbase\Logs\chatbot-*.log" -Wait -Tail 20
```

Look for:
```
[INF] Starting Onbase Invoice Chatbot application
[INF] Environment: Production
[INF] Machine: AAZEUS-FNUKAP01
[INF] Database Connection String: Server=AE1DCVPSQ23407;...
[INF] Application is ready to accept requests
```

## Troubleshooting

### Issue: 404 Not Found

**Check:**
```powershell
# Verify application exists in IIS
Get-WebApplication -Name "chatbotonbase"

# Check physical path
Test-Path "C:\inetpub\wwwroot\chatbotonbase\Chatbot-Onbase.dll"
```

### Issue: 500 Internal Server Error

**Check:**
1. Application logs: `C:\inetpub\wwwroot\chatbotonbase\Logs\`
2. Windows Event Viewer → Application logs
3. Enable detailed errors in web.config:
   ```xml
   <aspNetCore stdoutLogEnabled="true" stdoutLogFile=".\logs\stdout" />
   ```

### Issue: Database Connection Failed

**Check:**
```powershell
# From server, test SQL connection
sqlcmd -S AE1DCVPSQ23407 -d Onbase -E -Q "SELECT @@VERSION"

# Check app pool identity has access
# Run SQL query to verify login exists
```

**Navigate to:**
```
http://aazeus-fnukap01/chatbotonbase/api/chatbot/dbtest
```

Look for error details in the response.

## URL Path Verification

The application now automatically detects the base path `/chatbotonbase/` and constructs all API URLs correctly:

- ✅ Health: `http://aazeus-fnukap01/chatbotonbase/api/chatbot/health`
- ✅ DB Test: `http://aazeus-fnukap01/chatbotonbase/api/chatbot/dbtest`
- ✅ Query: `http://aazeus-fnukap01/chatbotonbase/api/chatbot/query`

No manual configuration needed!

## Monitoring

### Check Application Status
```powershell
# Check if app pool is running
Get-WebAppPoolState -Name "ChatbotOnbaseAppPool"

# Restart if needed
Restart-WebAppPool -Name "ChatbotOnbaseAppPool"
```

### View Recent Logs
```powershell
# Last 100 lines
Get-Content "C:\inetpub\wwwroot\chatbotonbase\Logs\chatbot-*.log" -Tail 100

# Search for errors
Select-String -Path "C:\inetpub\wwwroot\chatbotonbase\Logs\*.log" -Pattern "\[ERR\]|\[FTL\]" | Select-Object -Last 10
```

## Success Criteria

✅ Health endpoint returns 200 OK
✅ Database test shows "connected"
✅ Browser console shows "All Tests Passed"
✅ Log file shows "Application is ready to accept requests"
✅ Queries return invoice results
✅ No errors in browser console
✅ No [ERR] entries in log files

## Support

If you encounter issues:

1. Check browser console (F12)
2. Check log files in `Logs/` directory
3. Test diagnostic endpoints
4. Review troubleshooting guides:
   - IIS_DEPLOYMENT_TROUBLESHOOTING.md
   - QUICK_DIAGNOSTIC_GUIDE.md
   - IIS_URL_FIX_EXPLANATION.md

