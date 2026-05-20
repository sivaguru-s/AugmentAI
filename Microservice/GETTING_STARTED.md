# Getting Started - Credit Shortage Validation Microservice

## Prerequisites

Before you begin, ensure you have the following installed:

- ✅ [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- ✅ [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)
- ✅ SQL Server access (Ashley/Datawhse databases)
- ✅ Git (for version control)

## Step 1: Clone or Navigate to the Project

```bash
cd c:\source\GitHub\Credit_Shortage\Microservice
```

## Step 2: Restore Dependencies

```bash
# Restore NuGet packages for all projects
dotnet restore CreditShortage.sln
```

## Step 3: Configure Database Connections

1. Open `CreditShortage.Api/appsettings.json`
2. Update the connection strings:

```json
{
  "ConnectionStrings": {
    "AshleyDatabase": "Server=YOUR_SQL_SERVER;Database=Ashley;Integrated Security=true;TrustServerCertificate=true;",
    "DatawhseDatabase": "Server=YOUR_SQL_SERVER;Database=Datawhse;Integrated Security=true;TrustServerCertificate=true;",
    "ArchiveDatabase": "Server=YOUR_SQL_SERVER;Database=Archive;Integrated Security=true;TrustServerCertificate=true;"
  }
}
```

Replace `YOUR_SQL_SERVER` with your actual SQL Server instance name.

## Step 4: Configure IWS Integration (Optional)

If you want to test IWS integration, update these settings:

```json
{
  "IWSIntegration": {
    "BaseUrl": "https://your-iws-api.ashley.com",
    "ApiKey": "YOUR_ACTUAL_API_KEY",
    "Timeout": 30,
    "RetryCount": 3,
    "RetryDelaySeconds": 2
  }
}
```

## Step 5: Verify Database Objects

Ensure the following database objects exist:

### Stored Procedure
- ✅ `usp_CE_ValidateShortageItems` (in Datawhse database)

### Table Type
- ✅ `typCEShortageItemValidation` (User-Defined Table Type)

### Reference Tables
- ✅ `tblDefectCodes` (for defect codes)
- ✅ `tblWarehouse` (for location codes)

**Note**: See `Database/StoredProcedures/usp_CE_ValidateShortageItems.sql` for the stored procedure.

## Step 6: Build the Solution

```bash
# Build in Debug mode
dotnet build CreditShortage.sln

# Or build in Release mode
dotnet build CreditShortage.sln -c Release
```

## Step 7: Run the API

```bash
# Navigate to the API project
cd CreditShortage.Api

# Run the application
dotnet run
```

The API will start and display:

```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:5001
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
```

## Step 8: Access Swagger UI

Open your browser and navigate to:

```
https://localhost:5001
```

You should see the Swagger UI with all available endpoints.

## Step 9: Test the API

### Option 1: Using Swagger UI

1. Click on `POST /api/v1/ShortageValidation/validate`
2. Click "Try it out"
3. Enter sample request data:

```json
{
  "customerNumber": "12345678",
  "shipToNumber": "0001",
  "invoiceNumber": 123456,
  "itemNumber": "ITEM001",
  "serialNumber": "SER001",
  "shortageQuantity": 1,
  "defectCode": "XP",
  "locationCode": "WU"
}
```

4. Click "Execute"

### Option 2: Using PowerShell

```powershell
$body = @{
    customerNumber = "12345678"
    shipToNumber = "0001"
    invoiceNumber = 123456
    itemNumber = "ITEM001"
    serialNumber = "SER001"
    shortageQuantity = 1
    defectCode = "XP"
    locationCode = "WU"
} | ConvertTo-Json

Invoke-RestMethod -Uri "https://localhost:5001/api/v1/ShortageValidation/validate" `
    -Method Post `
    -Body $body `
    -ContentType "application/json"
```

### Option 3: Using cURL

```bash
curl -X POST "https://localhost:5001/api/v1/ShortageValidation/validate" \
  -H "Content-Type: application/json" \
  -d '{
    "customerNumber": "12345678",
    "shipToNumber": "0001",
    "invoiceNumber": 123456,
    "itemNumber": "ITEM001",
    "serialNumber": "SER001",
    "shortageQuantity": 1,
    "defectCode": "XP",
    "locationCode": "WU"
  }'
```

## Step 10: Check Health Status

```
GET https://localhost:5001/health
```

Should return:
```json
{
  "status": "Healthy"
}
```

## Common Issues & Solutions

### Issue 1: Database Connection Failed

**Error**: `Cannot connect to SQL Server`

**Solution**: 
- Verify SQL Server is running
- Check connection string format
- Ensure you have permissions to the databases
- Test connection using SQL Server Management Studio

### Issue 2: Stored Procedure Not Found

**Error**: `Could not find stored procedure 'usp_CE_ValidateShortageItems'`

**Solution**:
- Run the SQL script in `Database/StoredProcedures/`
- Verify it exists in the Datawhse database
- Check you're using the correct database

### Issue 3: Port Already in Use

**Error**: `Failed to bind to address https://localhost:5001`

**Solution**:
- Change the port in `appsettings.json` or `launchSettings.json`
- Or stop the process using that port

### Issue 4: NuGet Package Restore Failed

**Solution**:
```bash
# Clear NuGet cache
dotnet nuget locals all --clear

# Restore again
dotnet restore
```

## Development Tips

### Hot Reload
When running in development, the app supports hot reload:
```bash
dotnet watch run --project CreditShortage.Api
```

### View Logs
Logs are written to:
- Console (structured output)
- File: `logs/credit-shortage-YYYYMMDD.txt`

### Environment Variables
Override settings with environment variables:
```bash
$env:ShortageValidationSettings__DefaultDefectCode = "DF"
dotnet run
```

## Next Steps

1. ✅ Review the API documentation: `API_DOCUMENTATION.md`
2. ✅ Understand the architecture: `MICROSERVICE_ARCHITECTURE.md`
3. ✅ Write tests for your use cases
4. ✅ Integrate with your client application
5. ✅ Deploy to development environment

## Support

For questions or issues:
- **Developer**: Sivaguru Sampanthamoorthy
- **Email**: SSampanthamoorthy@ashleyfurnitureindia.com

---

**Happy Coding! 🚀**
