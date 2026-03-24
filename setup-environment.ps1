# Finance Dashboard - Environment Setup Script
# This script helps set up environment-specific configuration files

param(
    [Parameter(Mandatory=$false)]
    [ValidateSet('Development', 'Staging', 'Production')]
    [string]$Environment = 'Development'
)

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Finance Dashboard - Environment Setup" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$templateFile = "FinanceBudget\appsettings.$Environment.json.template"
$targetFile = "FinanceBudget\appsettings.$Environment.json"

# Check if template exists
if (-not (Test-Path $templateFile)) {
    Write-Host "ERROR: Template file not found: $templateFile" -ForegroundColor Red
    exit 1
}

# Check if target file already exists
if (Test-Path $targetFile) {
    Write-Host "WARNING: Configuration file already exists: $targetFile" -ForegroundColor Yellow
    $overwrite = Read-Host "Do you want to overwrite it? (y/N)"
    if ($overwrite -ne 'y' -and $overwrite -ne 'Y') {
        Write-Host "Setup cancelled." -ForegroundColor Yellow
        exit 0
    }
}

# Copy template to target
Write-Host "Creating configuration file for $Environment environment..." -ForegroundColor Green
Copy-Item $templateFile $targetFile

Write-Host ""
Write-Host "Configuration file created: $targetFile" -ForegroundColor Green
Write-Host ""
Write-Host "NEXT STEPS:" -ForegroundColor Cyan
Write-Host "1. Edit $targetFile" -ForegroundColor White
Write-Host "2. Update the following settings:" -ForegroundColor White
Write-Host "   - ConnectionStrings:AS400Database (SQL Server connection)" -ForegroundColor Gray
Write-Host "   - ServiceNow:BaseUrl, Username, Password" -ForegroundColor Gray
Write-Host "   - Jira:BaseUrl, Username, ApiToken" -ForegroundColor Gray
Write-Host ""
Write-Host "SECURITY RECOMMENDATIONS:" -ForegroundColor Yellow
Write-Host "- For Development: Consider using User Secrets instead" -ForegroundColor Gray
Write-Host "  Run: dotnet user-secrets init --project FinanceBudget" -ForegroundColor Gray
Write-Host "- For Production: Use Azure Key Vault or Environment Variables" -ForegroundColor Gray
Write-Host ""
Write-Host "To run the application in $Environment mode:" -ForegroundColor Cyan
Write-Host "  dotnet run --project FinanceBudget --launch-profile https-$($Environment.ToLower())" -ForegroundColor White
Write-Host ""

# Offer to open the file in default editor
$openFile = Read-Host "Do you want to open the configuration file now? (y/N)"
if ($openFile -eq 'y' -or $openFile -eq 'Y') {
    Start-Process $targetFile
}

Write-Host "Setup complete!" -ForegroundColor Green

