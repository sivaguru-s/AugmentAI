# Enhanced Logging and Diagnostics Summary

## Overview
Added comprehensive logging and diagnostic features to help troubleshoot IIS deployment issues and monitor application health.

## Changes Made

### 1. Controllers/ChatbotController.cs

#### Enhanced Error Handling
- **SQL Exceptions**: Captures error number, state, and server information
- **Timeout Exceptions**: Specific handling for query timeouts
- **General Exceptions**: Logs exception type and full details

#### New Diagnostic Endpoints

**Health Check** (`GET /api/chatbot/health`):
- Returns application status
- Shows environment, machine name, version
- Logs all health check requests

**Database Test** (`GET /api/chatbot/dbtest`):
- Tests database connectivity
- Measures connection time
- Returns server version and database name
- Provides detailed SQL error information on failure

#### Enhanced Logging
- Logs client IP address for all requests
- Logs query prompts and result counts
- Logs all errors with full context

### 2. Program.cs

#### Startup Logging
Added detailed startup information:
- Environment name
- Machine name
- Operating system
- Current directory
- Connection string (sanitized)
- Application URLs
- Available endpoints

#### Configuration Validation
- Checks for connection string presence
- Logs configuration issues at startup

### 3. wwwroot/index.html

#### Enhanced Error Display
Frontend now shows:
- HTTP status codes
- Detailed error messages from server
- Exception types
- Timestamps
- Connection diagnostics

#### Automatic Diagnostics
- Runs API connectivity test on page load
- Tests health endpoint
- Tests database connection
- Logs all results to browser console

#### Detailed Console Logging
- Request/response logging
- Error type identification
- Connection troubleshooting hints

## Error Response Format

### Standard Error Response
```json
{
  "error": "Error message",
  "details": "Detailed error information",
  "exceptionType": "SqlException",
  "timestamp": "2026-03-13T12:00:00Z"
}
```

### SQL Error Response
```json
{
  "error": "Database error occurred",
  "details": "SQL Error 53: A network-related error...",
  "timestamp": "2026-03-13T12:00:00Z"
}
```

### Database Test Response
```json
{
  "status": "connection_failed",
  "error": "Login failed for user...",
  "errorNumber": 18456,
  "state": 1,
  "server": "AE1DCVPSQ23407",
  "timestamp": "2026-03-13T12:00:00Z"
}
```

## Log File Output

### Startup Logs
```
========================================
[INF] Starting Onbase Invoice Chatbot application
[INF] Environment: Production
[INF] Machine: SERVER-NAME
[INF] OS: Microsoft Windows NT 10.0.17763.0
[INF] Current Directory: C:\inetpub\wwwroot\ChatbotOnbase
========================================
[INF] Database Connection String: Server=AE1DCVPSQ23407;Database=Onbase;...
[INF] Application built successfully
[INF] Configuring HTTP request pipeline...
========================================
[INF] Application configured successfully
[INF] Listening on: http://localhost:5000
[INF] Swagger UI: /swagger
[INF] API Endpoint: /api/chatbot/query
[INF] Health Check: /api/chatbot/health
[INF] DB Test: /api/chatbot/dbtest
========================================
[INF] Application is ready to accept requests
```

### Query Logs
```
[INF] Received query request from 192.168.1.100
[INF] Processing query: vendor V207
[INF] Detected intent: VendorSearch
[INF] Searching invoices for vendor: V207
[DBG] Vendor search type: VendorCode
[INF] Found 296 invoices for vendor V207 in 1333ms
[INF] Query processed successfully. Result count: 296
```

### Error Logs
```
[ERR] Database error processing query: show all invoices. Error Number: 53, State: 0, Server: AE1DCVPSQ23407
System.Data.SqlClient.SqlException (0x80131904): A network-related or instance-specific error occurred while establishing a connection to SQL Server...
   at Chatbot_Onbase.Data.OnbaseRepository.GetAllInvoicesAsync(Int32 limit, String documentType)
   at Chatbot_Onbase.Services.ChatbotService.ProcessQueryAsync(String userPrompt)
```

## Browser Console Output

### Successful Request
```
Page loaded. Running diagnostics...
=== API Connection Test ===
API URL: /api/chatbot/query
Current Location: http://localhost/chatbotonbase/
Base URL: http://localhost
Testing health endpoint...
Health Status: 200
Health Data: {status: "healthy", timestamp: "2026-03-13T12:00:00Z", ...}
Testing database connection...
DB Test Status: 200
DB Test Data: {status: "connected", server: "AE1DCVPSQ23407", ...}
=== All Tests Passed ===
```

### Failed Request
```
=== API Connection Test Failed ===
Error: TypeError: Failed to fetch
```

## Troubleshooting Workflow

1. **Deploy to IIS**
2. **Open browser to application URL**
3. **Press F12 and check Console tab**
   - Look for diagnostic test results
   - Check for any errors
4. **Navigate to `/api/chatbot/health`**
   - Verify application is running
5. **Navigate to `/api/chatbot/dbtest`**
   - Verify database connectivity
6. **Check log files in `Logs/` directory**
   - Look for startup errors
   - Check connection string
   - Review any error messages
7. **Test a query**
   - Monitor browser console
   - Check response in Network tab
   - Review log file for query execution

## Files Modified

- ✅ **Controllers/ChatbotController.cs** - Enhanced error handling and diagnostic endpoints
- ✅ **Program.cs** - Detailed startup logging
- ✅ **wwwroot/index.html** - Enhanced error display and automatic diagnostics
- ✅ **IIS_DEPLOYMENT_TROUBLESHOOTING.md** - Comprehensive troubleshooting guide
- ✅ **ENHANCED_LOGGING_SUMMARY.md** - This file

## Benefits

1. **Faster Troubleshooting**: Detailed error messages pinpoint exact issues
2. **Proactive Monitoring**: Automatic diagnostics on page load
3. **Complete Audit Trail**: All requests and errors logged
4. **Database Diagnostics**: Dedicated endpoint for connection testing
5. **User-Friendly Errors**: Clear error messages in the UI
6. **Developer-Friendly Logs**: Detailed technical information in log files

## Next Steps

1. Build and publish the application
2. Deploy to IIS
3. Test diagnostic endpoints
4. Review log files
5. Monitor application health

