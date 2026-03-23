# Logging Guide

## Overview

The Onbase Invoice Chatbot application uses **Serilog** for comprehensive logging to text files. All application logs are written to the `Logs/` directory.

## Configuration

### Log File Location
- **Directory**: `Logs/`
- **File Pattern**: `chatbot-YYYYMMDD.log`
- **Example**: `chatbot-20260313.log`

### Log Retention
- **Rolling Interval**: Daily (new file created each day)
- **Retention Period**: 30 days
- **Old logs are automatically deleted** after 30 days

### Log Levels
The application logs at different levels:
- **Information**: General application flow (queries, results, startup/shutdown)
- **Warning**: Potential issues (deprecated features, unusual conditions)
- **Error**: Errors that don't stop the application
- **Fatal**: Critical errors that cause application termination
- **Debug**: Detailed diagnostic information (only in Development mode)

## Log Format

Each log entry includes:
```
2026-03-13 11:45:34.123 +05:30 [INF] Processing query: vendor V207
2026-03-13 11:45:34.456 +05:30 [INF] Searching invoices for vendor: V207
2026-03-13 11:45:35.789 +05:30 [INF] Found 296 invoices for vendor V207 in 1234ms
```

**Format**: `{Timestamp} [{Level}] {Message} {Exception}`

## What Gets Logged

### Application Startup/Shutdown
- Application start
- Configuration loading
- Service registration
- Application shutdown
- Fatal errors

### Query Processing
- User queries received
- Intent detection
- Search parameters
- Query execution time
- Results count

### Database Operations
- Vendor searches (vendor name, search type, duration)
- Invoice searches
- Query performance metrics

### Errors
- Database connection errors
- Query timeout errors
- Parsing errors
- Unexpected exceptions

## Viewing Logs

### View Latest Log
```powershell
Get-Content Logs\chatbot-*.log -Tail 50
```

### View Specific Date
```powershell
Get-Content Logs\chatbot-20260313.log
```

### Monitor Live Logs
```powershell
Get-Content Logs\chatbot-*.log -Wait -Tail 20
```

### Search for Errors
```powershell
Select-String -Path "Logs\*.log" -Pattern "\[ERR\]|\[FTL\]"
```

### Search for Specific Vendor
```powershell
Select-String -Path "Logs\*.log" -Pattern "V207"
```

## Example Log Entries

### Successful Query
```
2026-03-13 11:45:34.123 +05:30 [INF] Processing query: vendor V207
2026-03-13 11:45:34.234 +05:30 [INF] Detected intent: VendorSearch
2026-03-13 11:45:34.345 +05:30 [INF] Searching invoices for vendor: V207
2026-03-13 11:45:34.456 +05:30 [DBG] Vendor search type: VendorCode
2026-03-13 11:45:35.789 +05:30 [INF] Found 296 invoices for vendor V207 in 1333ms
```

### Error Example
```
2026-03-13 11:50:12.345 +05:30 [ERR] Database query timeout
System.Data.SqlClient.SqlException: Timeout expired
   at Chatbot_Onbase.Data.OnbaseRepository.GetInvoicesByVendorAsync(String vendorName)
```

## Configuration File

The logging configuration is in `appsettings.json`:

```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "Microsoft.AspNetCore": "Warning"
      }
    },
    "WriteTo": [
      {
        "Name": "File",
        "Args": {
          "path": "Logs/chatbot-.log",
          "rollingInterval": "Day",
          "retainedFileCountLimit": 30
        }
      }
    ]
  }
}
```

## Troubleshooting

### No Logs Directory
If the `Logs/` directory doesn't exist, it will be created automatically when the application starts.

### Permission Issues
Ensure the application has write permissions to the application directory.

### Disk Space
Monitor disk space as logs can grow over time. The 30-day retention helps manage this.

## Best Practices

1. **Regular Monitoring**: Check logs daily for errors or warnings
2. **Performance Analysis**: Use query duration logs to identify slow queries
3. **Error Tracking**: Monitor error patterns to identify recurring issues
4. **Disk Space**: Ensure adequate disk space for 30 days of logs
5. **Security**: Logs may contain sensitive data - restrict access appropriately

