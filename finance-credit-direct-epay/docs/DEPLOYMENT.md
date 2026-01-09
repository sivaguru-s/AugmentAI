# Deployment Guide - Finance Credit Direct EPay

**Version:** 1.0  
**Last Updated:** January 9, 2026  
**Target Framework:** .NET Framework 3.5  

---

## Table of Contents

- [Overview](#overview)
- [Prerequisites](#prerequisites)
- [Environment Configuration](#environment-configuration)
- [Deployment Procedures](#deployment-procedures)
- [Post-Deployment Verification](#post-deployment-verification)
- [Rollback Procedures](#rollback-procedures)
- [Troubleshooting](#troubleshooting)

---

## Overview

This document provides step-by-step instructions for deploying the Finance Credit Direct EPay application to Development, Staging, and Production environments.

### Deployment Checklist

- [ ] Review and address all security vulnerabilities (see [SECURITY_AUDIT.md](./SECURITY_AUDIT.md))
- [ ] Build in Release mode
- [ ] Update Web.config for target environment
- [ ] Verify database connectivity
- [ ] Test US Bank integration (Production only)
- [ ] Backup current version
- [ ] Deploy application files
- [ ] Verify deployment
- [ ] Monitor for errors

---

## Prerequisites

### Server Requirements

**Operating System:**
- Windows Server 2008 R2 or later
- Windows Server 2012 R2 or later (recommended)

**IIS:**
- IIS 7.0 or later
- IIS 7.5 or later (recommended)
- ASP.NET 3.5 installed and enabled

**.NET Framework:**
- .NET Framework 3.5 SP1
- .NET Framework 4.0+ (for compatibility)

**Server Resources:**
- CPU: 2+ cores
- RAM: 4GB minimum, 8GB recommended
- Disk: 10GB free space

### Database Requirements

**SQL Server:**
- SQL Server 2008 or later
- Access to databases: Ashley, Datawhse, AFI_Batch, AFI_Dynamic

**IBM DB2:**
- IBM DB2 iSeries Access Client installed
- Connectivity to AS/400 system

### Network Requirements

**Firewall Rules:**
- Inbound: HTTP (80), HTTPS (443)
- Outbound: SQL Server (1433), DB2 (varies), US Bank (443)

**DNS:**
- Internal DNS resolution for database servers
- External DNS for US Bank gateway

### Access Requirements

**Permissions:**
- Local Administrator on web server
- IIS Manager access
- SQL Server database access (db_datareader, db_datawriter)
- File system write access to deployment directory

**Accounts:**
- Service account for IIS application pool
- Database service account (Windows Authentication)

---

## Environment Configuration

### Development Environment (DEV.ASHLEYDIRECT.COM)

**Purpose:** Development and testing

**Configuration:**

**Web.config:**
```xml
<compilation debug="true" targetFramework="3.5">
<customErrors mode="Off"/>
```

**Features:**
- Debug mode enabled
- Detailed error messages
- US Bank integration disabled
- Test data

**Database:**
- Development databases
- Test customer data
- Separate from production

**URL:** `http://dev.ashleydirect.com/EPay`

---

### Staging Environment (STAGE.ASHLEYDIRECT.COM)

**Purpose:** Pre-production testing and UAT

**Configuration:**

**Web.config:**
```xml
<compilation debug="false" targetFramework="3.5">
<customErrors mode="RemoteOnly" defaultRedirect="~/Error.aspx"/>
```

**Features:**
- Production-like configuration
- US Bank integration disabled
- Sanitized production data
- Performance testing

**Database:**
- Staging databases
- Refreshed from production (sanitized)
- Isolated from production

**URL:** `http://stage.ashleydirect.com/EPay`

---

### Production Environment

**Purpose:** Live production system

**Configuration:**

**Web.config:**
```xml
<compilation debug="false" targetFramework="3.5">
<customErrors mode="On" defaultRedirect="~/Error.aspx">
  <error statusCode="404" redirect="~/NotFound.aspx"/>
  <error statusCode="500" redirect="~/Error.aspx"/>
</customErrors>
```

**Features:**
- Debug mode disabled
- Custom errors enabled
- US Bank integration enabled
- Production data
- High availability

**Database:**
- Production databases
- Live customer data
- Backup and replication

**URL:** `https://www.ashleydirect.com/EPay` (or internal URL)

---

## Deployment Procedures

### Pre-Deployment Steps

#### 1. Security Review

⚠️ **CRITICAL:** Before deploying to production, address all security vulnerabilities:

```powershell
# Review security audit
Get-Content .\docs\SECURITY_AUDIT.md

# Verify no hardcoded credentials
Select-String -Path .\Web.config -Pattern "password"

# Verify debug mode is off
Select-String -Path .\Web.config -Pattern 'debug="false"'

# Verify custom errors enabled
Select-String -Path .\Web.config -Pattern 'customErrors mode="On"'
```

#### 2. Build Application

**Using Visual Studio:**
```
1. Open EPay.sln in Visual Studio
2. Set Configuration to "Release"
3. Build > Rebuild Solution
4. Verify no build errors
5. Check Output window for warnings
```

**Using MSBuild (Command Line):**
```powershell
# Set environment
$env:PATH += ";C:\Windows\Microsoft.NET\Framework\v3.5"

# Build solution
msbuild EPay.sln /p:Configuration=Release /p:Platform="Any CPU" /t:Rebuild

# Verify build succeeded
if ($LASTEXITCODE -eq 0) {
    Write-Host "Build succeeded" -ForegroundColor Green
} else {
    Write-Host "Build failed" -ForegroundColor Red
    exit 1
}
```

#### 3. Prepare Deployment Package

```powershell
# Create deployment directory
$deployDir = "C:\Deployments\EPay\$(Get-Date -Format 'yyyyMMdd-HHmmss')"
New-Item -ItemType Directory -Path $deployDir

# Copy application files
Copy-Item -Path ".\*" -Destination $deployDir -Recurse -Exclude @("*.cs", "*.vb", "*.csproj", "*.vbproj", "*.sln", "obj", "Properties")

# Copy bin directory
Copy-Item -Path ".\bin\*" -Destination "$deployDir\bin" -Recurse

# Copy Web.config (will be transformed)
Copy-Item -Path ".\Web.config" -Destination $deployDir

# Create deployment manifest
@{
    Version = "1.0"
    BuildDate = Get-Date
    BuildBy = $env:USERNAME
    Files = (Get-ChildItem $deployDir -Recurse).Count
} | ConvertTo-Json | Out-File "$deployDir\deployment-manifest.json"
```

#### 4. Transform Web.config

**For Production:**
```powershell
# Backup original
Copy-Item "$deployDir\Web.config" "$deployDir\Web.config.bak"

# Apply transformations (manual or using tool)
# Update the following settings:

# 1. Set debug="false"
(Get-Content "$deployDir\Web.config") -replace 'debug="true"', 'debug="false"' | Set-Content "$deployDir\Web.config"

# 2. Enable custom errors
(Get-Content "$deployDir\Web.config") -replace 'customErrors mode="Off"', 'customErrors mode="On"' | Set-Content "$deployDir\Web.config"

# 3. Remove hardcoded credentials (if any)
# Manually review and remove any commented credentials

# 4. Verify changes
Select-String -Path "$deployDir\Web.config" -Pattern 'debug="false"'
Select-String -Path "$deployDir\Web.config" -Pattern 'customErrors mode="On"'
```

---

### Deployment Steps

#### Step 1: Backup Current Version

```powershell
# Define paths
$webRoot = "C:\inetpub\wwwroot\EPay"
$backupRoot = "C:\Backups\EPay"
$timestamp = Get-Date -Format "yyyyMMdd-HHmmss"
$backupPath = "$backupRoot\EPay-$timestamp"

# Create backup directory
New-Item -ItemType Directory -Path $backupPath -Force

# Backup application files
Copy-Item -Path "$webRoot\*" -Destination $backupPath -Recurse

# Backup Web.config separately
Copy-Item -Path "$webRoot\Web.config" -Destination "$backupPath\Web.config.backup"

# Create backup manifest
@{
    BackupDate = Get-Date
    BackupBy = $env:USERNAME
    SourcePath = $webRoot
    BackupPath = $backupPath
    FileCount = (Get-ChildItem $backupPath -Recurse).Count
} | ConvertTo-Json | Out-File "$backupPath\backup-manifest.json"

Write-Host "Backup completed: $backupPath" -ForegroundColor Green
```

#### Step 2: Stop IIS Application Pool

```powershell
# Import IIS module
Import-Module WebAdministration

# Stop application pool
$appPoolName = "EPayAppPool"
Stop-WebAppPool -Name $appPoolName

# Wait for app pool to stop
$timeout = 30
$elapsed = 0
while ((Get-WebAppPoolState -Name $appPoolName).Value -ne "Stopped" -and $elapsed -lt $timeout) {
    Start-Sleep -Seconds 1
    $elapsed++
}

if ((Get-WebAppPoolState -Name $appPoolName).Value -eq "Stopped") {
    Write-Host "Application pool stopped" -ForegroundColor Green
} else {
    Write-Host "Warning: Application pool did not stop within timeout" -ForegroundColor Yellow
}
```

#### Step 3: Deploy Application Files

```powershell
# Define paths
$deployDir = "C:\Deployments\EPay\20260109-120000"  # Use actual deployment directory
$webRoot = "C:\inetpub\wwwroot\EPay"

# Remove old files (except Web.config and App_Data)
Get-ChildItem $webRoot -Exclude @("Web.config", "App_Data") | Remove-Item -Recurse -Force

# Copy new files
Copy-Item -Path "$deployDir\*" -Destination $webRoot -Recurse -Force

# Preserve Web.config if needed (or replace with new one)
# Option 1: Keep existing Web.config
# (Already preserved by excluding from deletion)

# Option 2: Replace with new Web.config
# Copy-Item -Path "$deployDir\Web.config" -Destination "$webRoot\Web.config" -Force

# Set permissions
$acl = Get-Acl $webRoot
$rule = New-Object System.Security.AccessControl.FileSystemAccessRule("IIS_IUSRS", "ReadAndExecute", "ContainerInherit,ObjectInherit", "None", "Allow")
$acl.SetAccessRule($rule)
Set-Acl $webRoot $acl

Write-Host "Application files deployed" -ForegroundColor Green
```

#### Step 4: Update Web.config

```powershell
# Backup current Web.config
Copy-Item "$webRoot\Web.config" "$webRoot\Web.config.pre-deploy"

# Copy new Web.config (if using new one)
Copy-Item "$deployDir\Web.config" "$webRoot\Web.config" -Force

# Verify critical settings
$config = Get-Content "$webRoot\Web.config"

# Check debug mode
if ($config -match 'debug="false"') {
    Write-Host "✓ Debug mode is OFF" -ForegroundColor Green
} else {
    Write-Host "✗ WARNING: Debug mode is ON" -ForegroundColor Red
}

# Check custom errors
if ($config -match 'customErrors mode="(On|RemoteOnly)"') {
    Write-Host "✓ Custom errors enabled" -ForegroundColor Green
} else {
    Write-Host "✗ WARNING: Custom errors disabled" -ForegroundColor Red
}

# Check for hardcoded credentials
if ($config -match 'password=') {
    Write-Host "✗ WARNING: Hardcoded credentials found" -ForegroundColor Red
} else {
    Write-Host "✓ No hardcoded credentials" -ForegroundColor Green
}
```

#### Step 5: Start IIS Application Pool

```powershell
# Start application pool
Start-WebAppPool -Name $appPoolName

# Wait for app pool to start
$timeout = 30
$elapsed = 0
while ((Get-WebAppPoolState -Name $appPoolName).Value -ne "Started" -and $elapsed -lt $timeout) {
    Start-Sleep -Seconds 1
    $elapsed++
}

if ((Get-WebAppPoolState -Name $appPoolName).Value -eq "Started") {
    Write-Host "Application pool started" -ForegroundColor Green
} else {
    Write-Host "ERROR: Application pool failed to start" -ForegroundColor Red
    exit 1
}

# Recycle app pool to ensure clean start
Restart-WebAppPool -Name $appPoolName
Write-Host "Application pool recycled" -ForegroundColor Green
```

#### Step 6: Verify Deployment

```powershell
# Test application URL
$url = "http://localhost/EPay/main.aspx"
try {
    $response = Invoke-WebRequest -Uri $url -UseBasicParsing -TimeoutSec 30
    if ($response.StatusCode -eq 200) {
        Write-Host "✓ Application is responding" -ForegroundColor Green
    } else {
        Write-Host "✗ Application returned status: $($response.StatusCode)" -ForegroundColor Yellow
    }
} catch {
    Write-Host "✗ ERROR: Application is not responding: $_" -ForegroundColor Red
}

# Check IIS logs for errors
$logPath = "C:\inetpub\logs\LogFiles\W3SVC1"
$recentLogs = Get-ChildItem $logPath | Sort-Object LastWriteTime -Descending | Select-Object -First 1
$errors = Select-String -Path $recentLogs.FullName -Pattern " 500 " -SimpleMatch
if ($errors) {
    Write-Host "✗ WARNING: 500 errors found in IIS logs" -ForegroundColor Yellow
    $errors | Select-Object -First 5
} else {
    Write-Host "✓ No 500 errors in recent logs" -ForegroundColor Green
}
```

---

## Post-Deployment Verification

### Smoke Tests

#### 1. Application Availability

```powershell
# Test main page
$urls = @(
    "http://localhost/EPay/main.aspx",
    "http://localhost/EPay/History.aspx",
    "http://localhost/EPay/AnalystReport.aspx"
)

foreach ($url in $urls) {
    try {
        $response = Invoke-WebRequest -Uri $url -UseBasicParsing -TimeoutSec 10
        Write-Host "✓ $url - Status: $($response.StatusCode)" -ForegroundColor Green
    } catch {
        Write-Host "✗ $url - ERROR: $_" -ForegroundColor Red
    }
}
```

#### 2. Database Connectivity

```powershell
# Test SQL Server connection
$connectionString = "Server=SQL_SERVER;Database=Datawhse;Integrated Security=True;"
try {
    $connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
    $connection.Open()
    Write-Host "✓ SQL Server connection successful" -ForegroundColor Green
    $connection.Close()
} catch {
    Write-Host "✗ SQL Server connection failed: $_" -ForegroundColor Red
}

# Test stored procedure
try {
    $connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
    $connection.Open()
    $command = $connection.CreateCommand()
    $command.CommandText = "SELECT COUNT(*) FROM tblEpay"
    $count = $command.ExecuteScalar()
    Write-Host "✓ Database query successful (tblEpay records: $count)" -ForegroundColor Green
    $connection.Close()
} catch {
    Write-Host "✗ Database query failed: $_" -ForegroundColor Red
}
```

#### 3. Functional Tests

**Manual Testing Checklist:**

- [ ] Login to application
- [ ] Select customer account
- [ ] View open invoices
- [ ] Filter invoices by date range
- [ ] Sort invoices by different columns
- [ ] Select invoices for payment
- [ ] Create payment batch
- [ ] View payment confirmation
- [ ] Cancel payment (test only)
- [ ] View payment history
- [ ] Access analyst report
- [ ] Export to Excel

#### 4. Integration Tests

**US Bank Integration (Production Only):**

⚠️ **WARNING:** Only test in production with a small test payment

- [ ] Create test payment batch
- [ ] Verify redirect to US Bank
- [ ] Complete test payment
- [ ] Verify confirmation number
- [ ] Check payment status

**Database Integration:**

- [ ] Verify data in tblEpay
- [ ] Check audit logging
- [ ] Verify stored procedures execute
- [ ] Test DB2 connectivity (if applicable)

---

## Rollback Procedures

### When to Rollback

Rollback if any of the following occur:
- Application fails to start
- Critical functionality broken
- Database connectivity issues
- Security vulnerabilities introduced
- Performance degradation

### Rollback Steps

#### 1. Stop Application Pool

```powershell
Stop-WebAppPool -Name "EPayAppPool"
```

#### 2. Restore Previous Version

```powershell
# Define paths
$webRoot = "C:\inetpub\wwwroot\EPay"
$backupPath = "C:\Backups\EPay\EPay-20260109-120000"  # Use actual backup path

# Remove current files
Get-ChildItem $webRoot | Remove-Item -Recurse -Force

# Restore from backup
Copy-Item -Path "$backupPath\*" -Destination $webRoot -Recurse -Force

Write-Host "Rollback completed" -ForegroundColor Green
```

#### 3. Restore Web.config

```powershell
# Restore Web.config from backup
Copy-Item "$backupPath\Web.config.backup" "$webRoot\Web.config" -Force
```

#### 4. Start Application Pool

```powershell
Start-WebAppPool -Name "EPayAppPool"
Restart-WebAppPool -Name "EPayAppPool"
```

#### 5. Verify Rollback

```powershell
# Test application
$response = Invoke-WebRequest -Uri "http://localhost/EPay/main.aspx" -UseBasicParsing
if ($response.StatusCode -eq 200) {
    Write-Host "✓ Rollback successful" -ForegroundColor Green
} else {
    Write-Host "✗ Rollback verification failed" -ForegroundColor Red
}
```

---

## Troubleshooting

### Common Issues

#### Issue: Application Pool Fails to Start

**Symptoms:**
- HTTP 503 Service Unavailable
- Application pool stops immediately after starting

**Diagnosis:**
```powershell
# Check event logs
Get-EventLog -LogName Application -Source "ASP.NET*" -Newest 10

# Check IIS logs
Get-Content "C:\inetpub\logs\LogFiles\W3SVC1\*.log" | Select-Object -Last 20
```

**Solutions:**
1. Verify .NET Framework 3.5 is installed
2. Check application pool identity has correct permissions
3. Verify Web.config is valid XML
4. Check for missing assemblies in bin folder

#### Issue: Database Connection Failures

**Symptoms:**
- SQL timeout errors
- "Cannot open database" errors
- Authentication failures

**Diagnosis:**
```powershell
# Test connection
$connectionString = "Server=SQL_SERVER;Database=Datawhse;Integrated Security=True;"
$connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
try {
    $connection.Open()
    Write-Host "Connection successful"
} catch {
    Write-Host "Connection failed: $_"
}
```

**Solutions:**
1. Verify SQL Server is running
2. Check firewall rules
3. Verify application pool identity has database access
4. Check connection strings in Ashley.Data.DataAccess

#### Issue: US Bank Integration Not Working

**Symptoms:**
- "Can not call USBank from Test!!" message
- Redirect fails
- Payment not processed

**Diagnosis:**
```powershell
# Check environment detection
$siteName = "PRODUCTION"  # Should not be DEV or STAGE
if ($siteName -eq "DEV.ASHLEYDIRECT.COM" -or $siteName -eq "STAGE.ASHLEYDIRECT.COM") {
    Write-Host "US Bank integration is disabled in this environment"
}
```

**Solutions:**
1. Verify environment is production
2. Check SessionData("SITENAME") value
3. Verify US Bank URL is correct
4. Test network connectivity to US Bank gateway

#### Issue: Performance Degradation

**Symptoms:**
- Slow page loads
- Timeouts
- High CPU/memory usage

**Diagnosis:**
```powershell
# Check application pool
Get-Counter "\Process(w3wp)\% Processor Time"
Get-Counter "\Process(w3wp)\Working Set"

# Check SQL Server
# Run SQL Profiler or check query execution times
```

**Solutions:**
1. Increase application pool memory limit
2. Optimize database queries
3. Add database indexes
4. Enable output caching
5. Review stored procedure performance

---

## Monitoring

### Application Monitoring

**IIS Logs:**
```powershell
# Monitor IIS logs for errors
Get-Content "C:\inetpub\logs\LogFiles\W3SVC1\*.log" -Wait | Where-Object { $_ -match " 500 " }
```

**Event Logs:**
```powershell
# Monitor application event log
Get-EventLog -LogName Application -Source "ASP.NET*" -Newest 10 -EntryType Error
```

**Performance Counters:**
```powershell
# Monitor key performance counters
Get-Counter @(
    "\ASP.NET Applications(__Total__)\Requests/Sec",
    "\ASP.NET Applications(__Total__)\Errors Total/Sec",
    "\Process(w3wp)\% Processor Time",
    "\Process(w3wp)\Working Set"
)
```

### Database Monitoring

**Query Performance:**
```sql
-- Check long-running queries
SELECT
    session_id,
    start_time,
    status,
    command,
    wait_type,
    wait_time,
    cpu_time,
    total_elapsed_time,
    text
FROM sys.dm_exec_requests
CROSS APPLY sys.dm_exec_sql_text(sql_handle)
WHERE session_id > 50
ORDER BY total_elapsed_time DESC
```

**EPay Activity:**
```sql
-- Monitor recent EPay activity
SELECT TOP 100
    epaRefNo,
    epaCusNo,
    epaStatus,
    epaDateAdded,
    epaUserAdded,
    COUNT(*) as InvoiceCount,
    SUM(epaInvAm) as TotalAmount
FROM Datawhse.dbo.tblEpay
WHERE epaDateAdded >= DATEADD(day, -7, GETDATE())
GROUP BY epaRefNo, epaCusNo, epaStatus, epaDateAdded, epaUserAdded
ORDER BY epaDateAdded DESC
```

---

## Maintenance

### Regular Maintenance Tasks

**Weekly:**
- Review IIS logs for errors
- Check application pool health
- Monitor disk space
- Review database performance

**Monthly:**
- Archive old IIS logs
- Review and archive old EPay records
- Update security patches
- Review access logs

**Quarterly:**
- Security audit
- Performance review
- Disaster recovery test
- Documentation review

---

## Appendix

### Deployment Checklist

**Pre-Deployment:**
- [ ] Security audit completed
- [ ] Code review completed
- [ ] Build in Release mode
- [ ] Web.config updated
- [ ] Backup created
- [ ] Change control approved
- [ ] Stakeholders notified

**Deployment:**
- [ ] Application pool stopped
- [ ] Files deployed
- [ ] Web.config updated
- [ ] Permissions set
- [ ] Application pool started
- [ ] Smoke tests passed

**Post-Deployment:**
- [ ] Functional tests passed
- [ ] Integration tests passed
- [ ] Performance acceptable
- [ ] No errors in logs
- [ ] Stakeholders notified
- [ ] Documentation updated

### Contact Information

**Support:**
- Development Team: IT Development
- Database Team: DBA Team
- Infrastructure Team: IT Operations
- Security Team: Information Security

**Escalation:**
- Level 1: Help Desk
- Level 2: Application Support
- Level 3: Development Team
- Level 4: IT Management

---

**Copyright © 2025 Ashley Furniture Industries, Inc. All rights reserved.**


