# Finance Budget Dashboard - Implementation Guide

## Overview
This Finance Budget Dashboard integrates three critical data sources to provide a comprehensive view of financial data:

1. **AS400 Unit Budget** - Real-time budget data via SQL Server linked server
2. **ServiceNow AFE Data** - Authorization for Expenditure data mapped to units
3. **JIRA Unit Costing** - Project costing and estimates from JIRA issues

## Architecture

### Technology Stack
- **Framework**: ASP.NET Core 8.0 MVC
- **Database**: SQL Server (AS400 via linked server)
- **APIs**: ServiceNow REST API, JIRA REST API
- **ORM**: Dapper for database operations
- **UI**: Bootstrap 5, Razor Views

### Project Structure
```
FinanceBudget/
├── Controllers/
│   ├── DashboardController.cs    # Main dashboard controller
│   └── HomeController.cs
├── Models/
│   ├── UnitBudget.cs             # AS400 budget data model
│   ├── AFEData.cs                # ServiceNow AFE model
│   ├── JiraCosting.cs            # JIRA costing model
│   └── DashboardViewModel.cs     # Dashboard view model
├── Services/
│   ├── IUnitBudgetService.cs     # AS400 service interface
│   ├── UnitBudgetService.cs      # AS400 service implementation
│   ├── IAFEDataService.cs        # ServiceNow service interface
│   ├── AFEDataService.cs         # ServiceNow service implementation
│   ├── IJiraCostingService.cs    # JIRA service interface
│   └── JiraCostingService.cs     # JIRA service implementation
└── Views/
    └── Dashboard/
        └── Index.cshtml          # Main dashboard view
```

## Configuration

### 1. Database Connection (AS400)
Update `appsettings.json` with your AS400 SQL Server connection:

```json
"ConnectionStrings": {
  "AS400Database": "Server=AFIDynamicSQLStage\\AFI_QDYNAMIC;Database=master;Integrated Security=true;TrustServerCertificate=true;"
}
```

**Note**: The connection uses Windows Authentication as specified.

### 2. ServiceNow Configuration
Update the ServiceNow settings in `appsettings.json`:

```json
"ServiceNow": {
  "BaseUrl": "https://your-instance.service-now.com",
  "Username": "your-username",
  "Password": "your-password",
  "AFETableName": "u_afe_data"
}
```

**Required ServiceNow Table Fields**:
- `u_afe_number` - AFE Number
- `u_unit` - Unit Code (maps to AS400 Unit)
- `u_description` - AFE Description
- `u_approved_amount` - Approved Amount
- `u_spent_amount` - Spent Amount
- `u_remaining_amount` - Remaining Amount
- `u_status` - Status
- `u_approval_date` - Approval Date
- `u_requestor` - Requestor Name

### 3. JIRA Configuration
Update the JIRA settings in `appsettings.json`:

```json
"Jira": {
  "BaseUrl": "https://your-domain.atlassian.net",
  "Username": "your-email@company.com",
  "ApiToken": "your-api-token",
  "ProjectKey": "FINANCE"
}
```

**Required JIRA Custom Fields**:
- `customfield_10001` - Unit Code (maps to AS400 Unit)
- `customfield_10002` - Estimated Cost
- `customfield_10003` - Actual Cost

**Note**: Update the custom field IDs in `JiraCostingService.cs` to match your JIRA instance.

## AS400 SQL Query

The application uses the following SQL query to fetch Unit Budget data:

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
WHERE UNIT.asalcd = @UnitCode AND TRANSACTION.AREDST = @TransactionStatus
```

## Features

### Dashboard Features
1. **Summary Cards** - Display total amounts from all three sources
2. **Unit-wise Summary** - Aggregated view by unit code
3. **Detailed Data Tabs** - Separate tabs for each data source
4. **Filtering** - Filter by unit code
5. **Export** - Export data in JSON format
6. **Responsive Design** - Mobile-friendly Bootstrap UI

### API Endpoints
- `GET /Dashboard/Index?unitCode={code}` - Main dashboard view
- `GET /Dashboard/GetUnitDetails?unitCode={code}` - Get AS400 unit details
- `GET /Dashboard/GetAFEDetails?unitCode={code}` - Get ServiceNow AFE details
- `GET /Dashboard/GetJiraDetails?unitCode={code}` - Get JIRA costing details
- `GET /Dashboard/ExportData?unitCode={code}&format=json` - Export all data

## Running the Application

### Prerequisites
- .NET 8.0 SDK
- Access to AS400 via SQL Server linked server
- ServiceNow instance with API access
- JIRA instance with API access

### Steps
1. Clone the repository
2. Update `appsettings.json` with your configuration
3. Restore NuGet packages:
   ```bash
   dotnet restore
   ```
4. Build the project:
   ```bash
   dotnet build
   ```
5. Run the application:
   ```bash
   dotnet run --project FinanceBudget
   ```
6. Navigate to `https://localhost:7188` or `http://localhost:5275`

## Security Considerations

1. **Credentials**: Store sensitive credentials in Azure Key Vault or User Secrets for production
2. **Windows Authentication**: Ensure the application pool identity has access to AS400
3. **API Tokens**: Use environment variables or secure vaults for API tokens
4. **HTTPS**: Always use HTTPS in production
5. **Input Validation**: Unit codes are parameterized to prevent SQL injection

## Troubleshooting

### AS400 Connection Issues
- Verify SQL Server linked server is configured correctly
- Check Windows Authentication permissions
- Test connection using SQL Server Management Studio

### ServiceNow API Issues
- Verify credentials and base URL
- Check table name and field mappings
- Ensure API user has read permissions

### JIRA API Issues
- Verify API token is valid
- Check custom field IDs match your JIRA instance
- Ensure project key is correct

## Future Enhancements

1. Add data caching for improved performance
2. Implement real-time data refresh
3. Add charting/visualization (Chart.js, D3.js)
4. Export to Excel/CSV formats
5. Add user authentication and authorization
6. Implement audit logging
7. Add data validation and error handling improvements
8. Create scheduled reports
9. Add budget variance analysis
10. Implement predictive analytics

## Support

For issues or questions, please contact the development team.

