# Environment-Based Configuration Guide

## Overview
The Finance Dashboard now supports environment-based configuration, allowing different settings for Development, Staging, and Production environments.

## Configuration Files

### File Hierarchy
ASP.NET Core loads configuration files in the following order (later files override earlier ones):

1. **appsettings.json** - Base configuration (shared across all environments)
2. **appsettings.{Environment}.json** - Environment-specific overrides
3. **User Secrets** (Development only)
4. **Environment Variables**
5. **Command-line arguments**

### Available Configuration Files

#### 1. `appsettings.json` (Base Configuration)
- Contains default/placeholder values
- Should NOT contain sensitive data
- Safe to commit to source control
- Values are overridden by environment-specific files

#### 2. `appsettings.Development.json`
- Used when `ASPNETCORE_ENVIRONMENT=Development`
- Contains development/local settings
- Points to development databases and APIs
- Can contain test credentials (but consider User Secrets instead)

#### 3. `appsettings.Staging.json`
- Used when `ASPNETCORE_ENVIRONMENT=Staging`
- Contains staging environment settings
- Points to staging databases and APIs
- Should use staging credentials

#### 4. `appsettings.Production.json`
- Used when `ASPNETCORE_ENVIRONMENT=Production`
- Contains production settings
- Points to production databases and APIs
- **IMPORTANT**: Should use secure credential management (Azure Key Vault, etc.)

## Environment-Specific Settings

### Development Environment
**SQL Server**: `AFIDynamicSQLStage\AFI_QDYNAMIC`
```json
{
  "ConnectionStrings": {
    "AS400Database": "Server=AFIDynamicSQLStage\\AFI_QDYNAMIC;Database=master;Integrated Security=true;TrustServerCertificate=true;"
  },
  "ServiceNow": {
    "BaseUrl": "https://dev-instance.service-now.com",
    "Username": "dev-username",
    "Password": "dev-password"
  },
  "Jira": {
    "BaseUrl": "https://your-domain.atlassian.net",
    "Username": "dev-email@company.com",
    "ApiToken": "dev-api-token"
  }
}
```

### Staging Environment
**SQL Server**: `AFIDynamicSQLStage\AFI_QDYNAMIC` (same as dev, or use staging server)
```json
{
  "ConnectionStrings": {
    "AS400Database": "Server=AFIDynamicSQLStage\\AFI_QDYNAMIC;Database=master;Integrated Security=true;TrustServerCertificate=true;"
  },
  "ServiceNow": {
    "BaseUrl": "https://staging-instance.service-now.com",
    "Username": "staging-username",
    "Password": "staging-password"
  }
}
```

### Production Environment
**SQL Server**: `AFIDynamicSQLProd\AFI_QPROD` (example production server)
```json
{
  "ConnectionStrings": {
    "AS400Database": "Server=AFIDynamicSQLProd\\AFI_QPROD;Database=master;Integrated Security=true;TrustServerCertificate=true;"
  },
  "ServiceNow": {
    "BaseUrl": "https://prod-instance.service-now.com",
    "Username": "prod-username",
    "Password": "prod-password"
  }
}
```

## Running the Application

### Using Visual Studio
1. Open the project in Visual Studio
2. Select the launch profile from the dropdown:
   - **https** - Development environment
   - **https-staging** - Staging environment
   - **https-production** - Production environment
3. Press F5 to run

### Using .NET CLI

#### Development
```bash
dotnet run --project FinanceBudget --launch-profile https
# OR
$env:ASPNETCORE_ENVIRONMENT="Development"
dotnet run --project FinanceBudget
```

#### Staging
```bash
dotnet run --project FinanceBudget --launch-profile https-staging
# OR
$env:ASPNETCORE_ENVIRONMENT="Staging"
dotnet run --project FinanceBudget
```

#### Production
```bash
dotnet run --project FinanceBudget --launch-profile https-production
# OR
$env:ASPNETCORE_ENVIRONMENT="Production"
dotnet run --project FinanceBudget
```

### Using IIS
Set the environment variable in web.config:
```xml
<aspNetCore processPath="dotnet" arguments=".\FinanceBudget.dll">
  <environmentVariables>
    <environmentVariable name="ASPNETCORE_ENVIRONMENT" value="Production" />
  </environmentVariables>
</aspNetCore>
```

## Secure Credential Management

### Development: User Secrets (Recommended)
For local development, use User Secrets instead of storing credentials in appsettings.Development.json:

```bash
# Initialize user secrets
dotnet user-secrets init --project FinanceBudget

# Set secrets
dotnet user-secrets set "ConnectionStrings:AS400Database" "Server=..." --project FinanceBudget
dotnet user-secrets set "ServiceNow:Username" "your-username" --project FinanceBudget
dotnet user-secrets set "ServiceNow:Password" "your-password" --project FinanceBudget
dotnet user-secrets set "Jira:Username" "your-email" --project FinanceBudget
dotnet user-secrets set "Jira:ApiToken" "your-token" --project FinanceBudget

# List all secrets
dotnet user-secrets list --project FinanceBudget
```

User Secrets are stored in:
- Windows: `%APPDATA%\Microsoft\UserSecrets\<user_secrets_id>\secrets.json`
- Linux/macOS: `~/.microsoft/usersecrets/<user_secrets_id>/secrets.json`

### Production: Azure Key Vault (Recommended)
For production, use Azure Key Vault:

1. **Install NuGet Package**:
```bash
dotnet add FinanceBudget package Azure.Extensions.AspNetCore.Configuration.Secrets
dotnet add FinanceBudget package Azure.Identity
```

2. **Update Program.cs**:
```csharp
if (builder.Environment.IsProduction())
{
    var keyVaultEndpoint = new Uri(builder.Configuration["KeyVaultEndpoint"]!);
    builder.Configuration.AddAzureKeyVault(
        keyVaultEndpoint,
        new DefaultAzureCredential());
}
```

3. **Store secrets in Azure Key Vault** with names like:
   - `ConnectionStrings--AS400Database`
   - `ServiceNow--Username`
   - `ServiceNow--Password`
   - `Jira--ApiToken`

### Production: Environment Variables
Alternatively, use environment variables:

```bash
# Windows PowerShell
$env:ConnectionStrings__AS400Database="Server=..."
$env:ServiceNow__Username="prod-user"
$env:ServiceNow__Password="prod-password"

# Linux/macOS
export ConnectionStrings__AS400Database="Server=..."
export ServiceNow__Username="prod-user"
export ServiceNow__Password="prod-password"
```

Note: Use double underscores `__` to represent nested configuration (`:` in JSON).

## Verification

### Check Current Environment
Add this to any controller to verify the environment:

```csharp
public IActionResult CheckEnvironment([FromServices] IWebHostEnvironment env)
{
    return Content($"Environment: {env.EnvironmentName}");
}
```

### Check Configuration Values
Add this to verify configuration is loaded correctly:

```csharp
public IActionResult CheckConfig([FromServices] IConfiguration config)
{
    var connectionString = config.GetConnectionString("AS400Database");
    var serviceNowUrl = config["ServiceNow:BaseUrl"];
    return Content($"DB: {connectionString}\nServiceNow: {serviceNowUrl}");
}
```

## Best Practices

1. ✅ **DO** use User Secrets for local development
2. ✅ **DO** use Azure Key Vault or similar for production
3. ✅ **DO** commit appsettings.json with placeholder values
4. ✅ **DO** add appsettings.*.json to .gitignore if they contain real credentials
5. ❌ **DON'T** commit real credentials to source control
6. ❌ **DON'T** use the same credentials across environments
7. ❌ **DON'T** store production credentials in appsettings.Production.json

## Troubleshooting

### Configuration Not Loading
1. Check `ASPNETCORE_ENVIRONMENT` is set correctly
2. Verify file name matches environment exactly (case-sensitive on Linux)
3. Ensure JSON is valid (use JSON validator)
4. Check file is copied to output directory

### Wrong Environment
```bash
# Check current environment
echo $env:ASPNETCORE_ENVIRONMENT  # PowerShell
echo $ASPNETCORE_ENVIRONMENT      # Bash
```

### Connection String Issues
- Verify server name and instance
- Check Windows Authentication permissions
- Test connection using SQL Server Management Studio
- Ensure backslashes are escaped in JSON: `\\`

## Summary

The application now automatically loads the correct configuration based on the `ASPNETCORE_ENVIRONMENT` variable:

- **Development** → `appsettings.Development.json`
- **Staging** → `appsettings.Staging.json`
- **Production** → `appsettings.Production.json`

Update the environment-specific files with your actual connection strings and credentials, and use secure storage (User Secrets, Azure Key Vault) for sensitive data.

