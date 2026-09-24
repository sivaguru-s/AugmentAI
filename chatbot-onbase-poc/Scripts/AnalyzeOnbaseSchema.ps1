# PowerShell script to analyze Onbase schema and find vendor field
# This script queries the database to find all fields for a specific invoice

param(
    [string]$InvoiceNumber = "61304208",
    [string]$Server = "aazeus-obdmsq01",
    [string]$Database = "Onbase"
)

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Onbase Schema Analyzer" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Connection string
$connectionString = "Server=$Server;Database=$Database;Integrated Security=true;TrustServerCertificate=true;"

# Function to run query
function Run-Query {
    param([string]$query, [string]$description)
    
    Write-Host "Running: $description" -ForegroundColor Yellow
    
    try {
        $connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
        $connection.Open()
        
        $command = $connection.CreateCommand()
        $command.CommandText = $query
        $command.CommandTimeout = 30
        
        $adapter = New-Object System.Data.SqlClient.SqlDataAdapter($command)
        $dataset = New-Object System.Data.DataSet
        $adapter.Fill($dataset) | Out-Null
        
        $connection.Close()
        
        if ($dataset.Tables[0].Rows.Count -gt 0) {
            Write-Host "Found $($dataset.Tables[0].Rows.Count) rows" -ForegroundColor Green
            return $dataset.Tables[0]
        } else {
            Write-Host "No data found" -ForegroundColor Gray
            return $null
        }
    }
    catch {
        Write-Host "Error: $_" -ForegroundColor Red
        return $null
    }
}

# Step 1: Get itemnum for the invoice
Write-Host ""
Write-Host "Step 1: Finding itemnum for invoice $InvoiceNumber..." -ForegroundColor Cyan
$query1 = "SELECT itemnum, keyvaluesmall as InvoiceNumber FROM hsi.keyitem106 WHERE keyvaluesmall = $InvoiceNumber"
$result1 = Run-Query -query $query1 -description "Get itemnum"

if ($result1 -eq $null) {
    Write-Host "Invoice not found!" -ForegroundColor Red
    exit
}

$itemnum = $result1.Rows[0]["itemnum"]
Write-Host "ItemNum: $itemnum" -ForegroundColor Green
Write-Host ""

# Step 2: Check all keyitem tables (100-120)
Write-Host "Step 2: Checking keyitem tables for itemnum $itemnum..." -ForegroundColor Cyan
$keyitemTables = @(100, 101, 102, 103, 104, 105, 107, 108, 109, 110, 111, 113, 114, 115, 116, 117, 118, 119, 120)

foreach ($num in $keyitemTables) {
    $tableName = "keyitem$num"
    $query = "SELECT * FROM hsi.$tableName WHERE itemnum = $itemnum"
    $result = Run-Query -query $query -description "Check $tableName"
    
    if ($result -ne $null) {
        Write-Host "  Table: $tableName" -ForegroundColor Green
        $result | Format-Table -AutoSize | Out-String | Write-Host
    }
}

Write-Host ""

# Step 3: Check keytable104 (we know this has Order Number)
Write-Host "Step 3: Checking keytable104 (Order Number)..." -ForegroundColor Cyan
$query3 = @"
SELECT kt.keywordnum, kt.keyvaluechar
FROM hsi.keytable104 kt
INNER JOIN hsi.keyxitem104 kx ON kt.keywordnum = kx.keywordnum
WHERE kx.itemnum = $itemnum
"@
$result3 = Run-Query -query $query3 -description "Get Order Number"
if ($result3 -ne $null) {
    $result3 | Format-Table -AutoSize | Out-String | Write-Host
}

Write-Host ""

# Step 4: List all available keyitem tables
Write-Host "Step 4: Listing all available keyitem tables..." -ForegroundColor Cyan
$query4 = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'hsi' AND TABLE_NAME LIKE 'keyitem%' ORDER BY TABLE_NAME"
$result4 = Run-Query -query $query4 -description "List keyitem tables"
if ($result4 -ne $null) {
    $result4 | Format-Table -AutoSize | Out-String | Write-Host
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Analysis Complete!" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Please review the output above and look for:" -ForegroundColor Yellow
Write-Host "  - Any keyitem table with text/varchar data that might be vendor name" -ForegroundColor Yellow
Write-Host "  - Any keyitem table with numeric data that might be amount" -ForegroundColor Yellow
Write-Host ""
Write-Host "Share the results and I'll update the application accordingly!" -ForegroundColor Green

