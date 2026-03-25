# Finance Budget Dashboard

A comprehensive ASP.NET Core MVC application that integrates financial data from multiple sources:
- **AS400 Unit Budget** via SQL Server
- **ServiceNow AFE Data** via REST API
- **JIRA Unit Costing** via REST API

## 🚀 Quick Start

### Prerequisites
- .NET 8.0 SDK
- Access to AS400 via SQL Server linked server
- ServiceNow instance with API access
- JIRA instance with API access

### Initial Setup

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd FinanceDashboard
   ```

2. **Set up environment configuration**
   
   **Option A: Using PowerShell Script (Recommended)**
   ```powershell
   .\setup-environment.ps1 -Environment Development
   ```
   
   **Option B: Manual Setup**
   ```bash
   # Copy template to create your configuration file
   cp FinanceBudget/appsettings.Development.json.template FinanceBudget/appsettings.Development.json
   
   # Edit the file and update with your credentials
   notepad FinanceBudget/appsettings.Development.json
   ```

3. **Update configuration settings**
   
   Edit `appsettings.Development.json` and update:
   - SQL Server connection string
   - ServiceNow URL and credentials
   - JIRA URL and API token

4. **Restore dependencies**
   ```bash
   dotnet restore
   ```

5. **Run the application**
   ```bash
   dotnet run --project FinanceBudget
   ```

6. **Access the dashboard**
   
   Navigate to: `https://localhost:7188/Dashboard`

## 📁 Project Structure

```
FinanceDashboard/
├── FinanceBudget/
│   ├── Controllers/
│   │   ├── DashboardController.cs    # Main dashboard controller
│   │   └── HomeController.cs
│   ├── Models/
│   │   ├── UnitBudget.cs             # AS400 data model
│   │   ├── AFEData.cs                # ServiceNow data model
│   │   ├── JiraCosting.cs            # JIRA data model
│   │   └── DashboardViewModel.cs     # Dashboard view model
│   ├── Services/
│   │   ├── UnitBudgetService.cs      # AS400 integration
│   │   ├── AFEDataService.cs         # ServiceNow integration
│   │   └── JiraCostingService.cs     # JIRA integration
│   ├── Views/
│   │   └── Dashboard/
│   │       └── Index.cshtml          # Dashboard UI
│   ├── appsettings.json              # Base configuration
│   ├── appsettings.*.json.template   # Configuration templates
│   └── Program.cs
├── IMPLEMENTATION_GUIDE.md           # Detailed implementation docs
├── ENVIRONMENT_CONFIGURATION_GUIDE.md # Environment setup guide
├── CONFIGURATION_CHECKLIST.md        # Pre-deployment checklist
└── README.md                         # This file
```

## 🔧 Configuration

### Environment-Based Configuration

The application supports multiple environments with separate configuration files:

| Environment | Configuration File | SQL Server |
|-------------|-------------------|------------|
| Development | `appsettings.Development.json` | AFIDynamicSQLStage\AFI_QDYNAMIC |
| Staging | `appsettings.Staging.json` | AFIDynamicSQLStage\AFI_QDYNAMIC |
| Production | `appsettings.Production.json` | AFIDynamicSQLProd\AFI_QPROD |

### Running Different Environments

**Development:**
```bash
dotnet run --project FinanceBudget --launch-profile https
```

**Staging:**
```bash
dotnet run --project FinanceBudget --launch-profile https-staging
```

**Production:**
```bash
dotnet run --project FinanceBudget --launch-profile https-production
```

### Secure Credential Management

**Development (Recommended):**
Use User Secrets to avoid storing credentials in files:
```bash
dotnet user-secrets init --project FinanceBudget
dotnet user-secrets set "ServiceNow:Username" "your-username" --project FinanceBudget
dotnet user-secrets set "ServiceNow:Password" "your-password" --project FinanceBudget
dotnet user-secrets set "Jira:ApiToken" "your-token" --project FinanceBudget
```

**Production (Recommended):**
Use Azure Key Vault or environment variables for production credentials.

See [ENVIRONMENT_CONFIGURATION_GUIDE.md](ENVIRONMENT_CONFIGURATION_GUIDE.md) for detailed instructions.

## 📊 Features

### Dashboard Features
- **Real-time Data Integration** - Fetches data from all three sources in parallel
- **Summary Cards** - Display total amounts from AS400, ServiceNow, and JIRA
- **Unit-wise Summary** - Aggregated view by unit code
- **Detailed Data Tabs** - Separate tabs for each data source
- **Filtering** - Filter by unit code (e.g., "9075")
- **Export** - Export data in JSON format
- **Responsive Design** - Mobile-friendly Bootstrap UI

### API Endpoints
- `GET /Dashboard/Index?unitCode={code}` - Main dashboard view
- `GET /Dashboard/GetUnitDetails?unitCode={code}` - AS400 unit details
- `GET /Dashboard/GetAFEDetails?unitCode={code}` - ServiceNow AFE details
- `GET /Dashboard/GetJiraDetails?unitCode={code}` - JIRA costing details
- `GET /Dashboard/ExportData?unitCode={code}` - Export all data

## 🔌 Data Source Configuration

### 1. AS400 Unit Budget

**Connection:** SQL Server with Windows Authentication

**SQL Query:**
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
INNER JOIN AMFLIBA.YAA7REP as GLFile ON GLFile.A7KMCD = UNIT.asalcd 
    AND GLFile.A7KNCD = NATURE.AHAFCD
INNER JOIN AMFLIBA.YAARREP as TRANSACTION ON TRANSACTION.ARAKNB = GLFile.A7AKNB
WHERE UNIT.asalcd = @UnitCode
```

### 2. ServiceNow AFE Data

**Authentication:** Basic Authentication

**Required Table Fields:**
- `u_afe_number` - AFE Number
- `u_unit` - Unit Code (maps to AS400)
- `u_description` - Description
- `u_approved_amount` - Approved Amount
- `u_spent_amount` - Spent Amount
- `u_remaining_amount` - Remaining Amount
- `u_status` - Status
- `u_approval_date` - Approval Date
- `u_requestor` - Requestor

### 3. JIRA Unit Costing

**Authentication:** Basic Authentication (Email + API Token)

**Required Custom Fields:**
- `customfield_10001` - Unit Code (maps to AS400)
- `customfield_10002` - Estimated Cost
- `customfield_10003` - Actual Cost

**Note:** Update custom field IDs in `JiraCostingService.cs` to match your JIRA instance.

## 📚 Documentation

- [IMPLEMENTATION_GUIDE.md](IMPLEMENTATION_GUIDE.md) - Detailed architecture and implementation
- [ENVIRONMENT_CONFIGURATION_GUIDE.md](ENVIRONMENT_CONFIGURATION_GUIDE.md) - Environment setup guide
- [CONFIGURATION_CHECKLIST.md](CONFIGURATION_CHECKLIST.md) - Pre-deployment checklist

## 🔒 Security

- ✅ SQL injection protection via parameterized queries
- ✅ Windows Authentication for AS400 connection
- ✅ HTTPS enforcement
- ✅ Environment-based configuration
- ✅ Support for User Secrets and Azure Key Vault
- ⚠️ Update credentials in environment-specific files
- ⚠️ Never commit real credentials to source control

## 🧪 Testing

```bash
# Build the project
dotnet build

# Run tests (when available)
dotnet test

# Run the application
dotnet run --project FinanceBudget
```

## 📝 License

[Your License Here]

## 👥 Support

For issues or questions, please contact the development team.

