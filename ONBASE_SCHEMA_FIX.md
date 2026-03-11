# Onbase Database Schema Fix

## Problem
The initial SQL queries were using column names that don't exist in the Onbase database schema. Onbase stores document metadata as keyword-value pairs in the `hsi.KeyItem` table, not as direct columns.

## Solution Applied
I've updated all SQL queries in `Data/OnbaseRepository.cs` to use the proper Onbase schema structure:

### Old (Incorrect) Approach
```sql
SELECT InvoiceNumber, VendorName, Amount 
FROM hsi.ItemData
WHERE VendorName LIKE @VendorName
```

### New (Correct) Approach
```sql
SELECT 
    i.ItemNum as InvoiceId,
    kt1.KeyValueChar as InvoiceNumber,
    kt2.KeyValueChar as VendorName,
    kt3.KeyValueNumeric as Amount
FROM hsi.ItemData i
INNER JOIN hsi.ItemType dt ON i.ItemTypeNum = dt.ItemTypeNum
LEFT JOIN hsi.KeyItem kt1 ON i.ItemNum = kt1.ItemNum 
    AND kt1.KeywordNum = (SELECT KeywordNum FROM hsi.Keyword WHERE KeywordName = 'Invoice Number')
LEFT JOIN hsi.KeyItem kt2 ON i.ItemNum = kt2.ItemNum 
    AND kt2.KeywordNum = (SELECT KeywordNum FROM hsi.Keyword WHERE KeywordName = 'Vendor Name')
```

## Important: Verify Keyword Names

The queries assume the following keyword names exist in your Onbase database:
- **Invoice Number**
- **Vendor Name**
- **Amount**
- **Invoice Date**
- **Due Date**
- **Status**
- **PO Number**

### Step 1: Check Your Actual Keyword Names

Run the SQL script `Scripts/CheckOnbaseSchema.sql` against your Onbase database to see the actual keyword names:

```sql
SELECT KeywordNum, KeywordName, KeywordTypeName, DataType
FROM hsi.Keyword
WHERE 
    KeywordName LIKE '%Invoice%' OR
    KeywordName LIKE '%Vendor%' OR
    KeywordName LIKE '%Amount%' OR
    KeywordName LIKE '%Date%' OR
    KeywordName LIKE '%PO%' OR
    KeywordName LIKE '%Status%'
ORDER BY KeywordName;
```

### Step 2: Update Configuration

If your keyword names are different, update them in `appsettings.json`:

```json
{
  "OnbaseKeywords": {
    "InvoiceNumber": "Your_Actual_Invoice_Number_Keyword",
    "VendorName": "Your_Actual_Vendor_Keyword",
    "Amount": "Your_Actual_Amount_Keyword",
    "InvoiceDate": "Your_Actual_Invoice_Date_Keyword",
    "DueDate": "Your_Actual_Due_Date_Keyword",
    "Status": "Your_Actual_Status_Keyword",
    "PONumber": "Your_Actual_PO_Number_Keyword"
  }
}
```

### Step 3: Update Repository Queries

If the keyword names in your database are different, you'll need to update the keyword names in `Data/OnbaseRepository.cs`. 

For example, if your keyword is called "Inv Number" instead of "Invoice Number", change:
```sql
WHERE KeywordName = 'Invoice Number'
```
to:
```sql
WHERE KeywordName = 'Inv Number'
```

## Alternative: Simplified Query

If you don't know the exact keyword names or want a simpler approach, you can use this query that searches across all keywords:

```sql
SELECT DISTINCT TOP 50
    i.ItemNum as InvoiceId,
    i.ItemName as Description,
    dt.ItemTypeName as DocumentType,
    i.ItemDate as CreatedDate
FROM hsi.ItemData i
INNER JOIN hsi.ItemType dt ON i.ItemTypeNum = dt.ItemTypeNum
LEFT JOIN hsi.KeyItem ki ON i.ItemNum = ki.ItemNum
WHERE 
    ki.KeyValueChar LIKE @SearchTerm OR
    i.ItemName LIKE @SearchTerm
ORDER BY i.ItemDate DESC
```

This will search across all keyword values but won't return specific fields like InvoiceNumber, VendorName, etc.

## Testing the Connection

### Option 1: Test with SQL Management Studio
1. Open SQL Server Management Studio
2. Connect to: `aazeus-obdmsq01`
3. Database: `Onbase`
4. Run the queries from `Scripts/CheckOnbaseSchema.sql`

### Option 2: Test with PowerShell
```powershell
$connectionString = "Server=aazeus-obdmsq01;Database=Onbase;Integrated Security=true;TrustServerCertificate=true;"
$connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
$connection.Open()
$command = $connection.CreateCommand()
$command.CommandText = "SELECT TOP 5 KeywordName FROM hsi.Keyword"
$reader = $command.ExecuteReader()
while ($reader.Read()) {
    Write-Host $reader["KeywordName"]
}
$connection.Close()
```

## Next Steps

1. **Stop any running instances** of the application (close browser, stop dotnet processes)
2. **Run the schema check script** to identify actual keyword names
3. **Update the queries** in `Data/OnbaseRepository.cs` with correct keyword names
4. **Rebuild**: `dotnet build`
5. **Run**: `dotnet run`
6. **Test**: Navigate to http://localhost:5050

## Common Onbase Keyword Name Variations

Your database might use different naming conventions:
- Invoice Number → `Inv Num`, `Invoice_Number`, `InvoiceNo`
- Vendor Name → `Vendor`, `Supplier`, `Vendor_Name`
- Amount → `Invoice Amount`, `Total`, `Amt`
- Invoice Date → `Inv Date`, `Date`, `Invoice_Date`
- PO Number → `PO`, `Purchase Order`, `PO_Number`

## Need Help?

If you're still getting errors:
1. Share the output from `Scripts/CheckOnbaseSchema.sql`
2. I can update the queries with the exact keyword names from your database
3. Or we can create a more generic version that works without specific keywords

## Files Modified
- `Data/OnbaseRepository.cs` - All SQL queries updated
- `appsettings.json` - Added OnbaseKeywords configuration section
- `Scripts/CheckOnbaseSchema.sql` - New diagnostic script

