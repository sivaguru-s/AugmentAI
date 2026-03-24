# Configuration Checklist

## Pre-Deployment Configuration Steps

### 1. AS400 Database Configuration ✓

**Connection String Location**: `appsettings.json` → `ConnectionStrings` → `AS400Database`

**Current Configuration**:
```
Server: AFIDynamicSQLStage\AFI_QDYNAMIC
Authentication: Windows Authentication (Integrated Security=true)
```

**Verification Steps**:
- [ ] Verify SQL Server linked server is configured for AS400
- [ ] Test connection using SQL Server Management Studio
- [ ] Confirm application pool identity has necessary permissions
- [ ] Test the following query returns data:

```sql
SELECT 
    UNIT.ASALCD as Unit,
    UNIT.ASAQNA as UnitDescription,
    NATURE.AHAFCD as Nature,
    Nature.AHADNA as NatureDescription,
    GLFile.A7AKNB as TransactionId, 
    TRANSACTION.ARALNB as TransactionNumber,
    GLFile.A7ADZZ as Amount
FROM AMFLIBA.YAASREP as UNIT 
INNER JOIN AMFLIBA.YAC4REP as UNITNATURE ON UNIT.asalcd = UNITNATURE.C4ALCD 
INNER JOIN AMFLIBA.YAAHREP as NATURE ON NATURE.AHAFCD = UNITNATURE.C4AFCD
INNER JOIN AMFLIBA.YAA7REP as GLFile ON GLFile.A7KMCD = UNIT.asalcd AND GLFile.A7KNCD = NATURE.AHAFCD
INNER JOIN AMFLIBA.YAARREP as TRANSACTION ON TRANSACTION.ARAKNB = GLFile.A7AKNB
WHERE UNIT.asalcd = '9075' AND TRANSACTION.AREDST = '5'
```

### 2. ServiceNow Configuration

**Configuration Location**: `appsettings.json` → `ServiceNow`

**Required Settings**:
- [ ] Update `BaseUrl` with your ServiceNow instance URL
- [ ] Update `Username` with ServiceNow API user
- [ ] Update `Password` with ServiceNow API password
- [ ] Verify `AFETableName` matches your ServiceNow table name

**ServiceNow Table Requirements**:
The AFE table must have the following fields:
- [ ] `u_afe_number` (String) - AFE Number
- [ ] `u_unit` (String) - Unit Code
- [ ] `u_description` (String) - Description
- [ ] `u_approved_amount` (Decimal) - Approved Amount
- [ ] `u_spent_amount` (Decimal) - Spent Amount
- [ ] `u_remaining_amount` (Decimal) - Remaining Amount
- [ ] `u_status` (String) - Status
- [ ] `u_approval_date` (Date) - Approval Date
- [ ] `u_requestor` (String) - Requestor

**Test API Access**:
```bash
curl -u username:password https://your-instance.service-now.com/api/now/table/u_afe_data?sysparm_limit=1
```

### 3. JIRA Configuration

**Configuration Location**: `appsettings.json` → `Jira`

**Required Settings**:
- [ ] Update `BaseUrl` with your JIRA instance URL (e.g., https://yourcompany.atlassian.net)
- [ ] Update `Username` with your JIRA email
- [ ] Update `ApiToken` with your JIRA API token
- [ ] Update `ProjectKey` with your finance project key

**Generate JIRA API Token**:
1. Go to https://id.atlassian.com/manage-profile/security/api-tokens
2. Click "Create API token"
3. Copy the token and update `appsettings.json`

**JIRA Custom Fields**:
The following custom fields must be configured in your JIRA project:
- [ ] Custom field for Unit Code (default: `customfield_10001`)
- [ ] Custom field for Estimated Cost (default: `customfield_10002`)
- [ ] Custom field for Actual Cost (default: `customfield_10003`)

**Find Your Custom Field IDs**:
```bash
curl -u email@company.com:api_token https://yourcompany.atlassian.net/rest/api/3/field
```

**Update Custom Field IDs**:
If your custom field IDs are different, update them in:
- File: `FinanceBudget/Services/JiraCostingService.cs`
- Lines: 48, 49, 50 (in GetJiraCostingDataAsync method)
- Lines: 85, 86, 87 (in GetJiraIssueAsync method)

**Test JIRA API Access**:
```bash
curl -u email@company.com:api_token https://yourcompany.atlassian.net/rest/api/3/project/FINANCE
```

### 4. Security Configuration (Production)

**For Production Deployment**:
- [ ] Move sensitive credentials to Azure Key Vault or User Secrets
- [ ] Enable HTTPS only
- [ ] Configure CORS if needed
- [ ] Set up authentication/authorization
- [ ] Configure logging and monitoring

**Using User Secrets (Development)**:
```bash
dotnet user-secrets init --project FinanceBudget
dotnet user-secrets set "ServiceNow:Username" "your-username" --project FinanceBudget
dotnet user-secrets set "ServiceNow:Password" "your-password" --project FinanceBudget
dotnet user-secrets set "Jira:Username" "your-email" --project FinanceBudget
dotnet user-secrets set "Jira:ApiToken" "your-token" --project FinanceBudget
```

### 5. Testing Checklist

**Unit Testing**:
- [ ] Test AS400 connection and query execution
- [ ] Test ServiceNow API integration
- [ ] Test JIRA API integration
- [ ] Test dashboard data aggregation

**Integration Testing**:
- [ ] Test dashboard with real data
- [ ] Test filtering by unit code
- [ ] Test export functionality
- [ ] Test error handling for each integration

**Performance Testing**:
- [ ] Test with large datasets
- [ ] Monitor query execution times
- [ ] Check API response times

### 6. Deployment Steps

1. **Build the Application**:
   ```bash
   dotnet build --configuration Release
   ```

2. **Publish the Application**:
   ```bash
   dotnet publish --configuration Release --output ./publish
   ```

3. **Configure IIS** (if using IIS):
   - [ ] Create application pool with .NET CLR version: No Managed Code
   - [ ] Set application pool identity with AS400 access
   - [ ] Configure bindings (HTTP/HTTPS)
   - [ ] Set appropriate permissions

4. **Verify Deployment**:
   - [ ] Access home page
   - [ ] Navigate to dashboard
   - [ ] Test filtering
   - [ ] Test export
   - [ ] Check logs for errors

### 7. Monitoring and Maintenance

**Setup Monitoring**:
- [ ] Configure application logging
- [ ] Set up error notifications
- [ ] Monitor API rate limits (ServiceNow, JIRA)
- [ ] Monitor database connection pool

**Regular Maintenance**:
- [ ] Review and rotate API credentials
- [ ] Monitor and optimize slow queries
- [ ] Update NuGet packages
- [ ] Review and archive old data

## Quick Start Command

After configuration, run:
```bash
dotnet run --project FinanceBudget
```

Then navigate to: `https://localhost:7188` or `http://localhost:5275`

## Support Contacts

- **AS400 Database**: [Your DBA Team]
- **ServiceNow**: [Your ServiceNow Admin]
- **JIRA**: [Your JIRA Admin]
- **Application Support**: [Your Dev Team]

