# PowerShell Script to Generate Demo Document
# This script converts the DEMO_PRESENTATION.md to a Word document

Write-Host "==================================================================" -ForegroundColor Cyan
Write-Host "  Onbase Invoice Chatbot - Demo Document Generator" -ForegroundColor Cyan
Write-Host "==================================================================" -ForegroundColor Cyan
Write-Host ""

# Check if Pandoc is installed
$pandocInstalled = Get-Command pandoc -ErrorAction SilentlyContinue

if (-not $pandocInstalled) {
    Write-Host "❌ Pandoc is not installed." -ForegroundColor Red
    Write-Host ""
    Write-Host "To generate a Word document, you need to install Pandoc:" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Option 1: Install via Chocolatey" -ForegroundColor Green
    Write-Host "  choco install pandoc" -ForegroundColor White
    Write-Host ""
    Write-Host "Option 2: Download from https://pandoc.org/installing.html" -ForegroundColor Green
    Write-Host ""
    Write-Host "After installation, run this script again." -ForegroundColor Yellow
    Write-Host ""
    Write-Host "==================================================================" -ForegroundColor Cyan
    Write-Host "  Alternative: Manual Conversion" -ForegroundColor Cyan
    Write-Host "==================================================================" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "You can manually convert DEMO_PRESENTATION.md to Word:" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "1. Open DEMO_PRESENTATION.md in VS Code" -ForegroundColor White
    Write-Host "2. Install 'Markdown PDF' extension" -ForegroundColor White
    Write-Host "3. Right-click → 'Markdown PDF: Export (pdf)'" -ForegroundColor White
    Write-Host "4. Or copy content to Word and format manually" -ForegroundColor White
    Write-Host ""
    Write-Host "The markdown file is ready to view at:" -ForegroundColor Green
    Write-Host "  C:\Chatbot-Onbase\DEMO_PRESENTATION.md" -ForegroundColor White
    Write-Host ""
    exit
}

Write-Host "✅ Pandoc is installed" -ForegroundColor Green
Write-Host ""

# Set paths
$markdownFile = "C:\Chatbot-Onbase\DEMO_PRESENTATION.md"
$wordFile = "C:\Chatbot-Onbase\Onbase_Invoice_Chatbot_Demo.docx"
$pdfFile = "C:\Chatbot-Onbase\Onbase_Invoice_Chatbot_Demo.pdf"

# Check if markdown file exists
if (-not (Test-Path $markdownFile)) {
    Write-Host "❌ DEMO_PRESENTATION.md not found!" -ForegroundColor Red
    exit
}

Write-Host "📄 Converting Markdown to Word..." -ForegroundColor Yellow

# Convert to Word
try {
    pandoc $markdownFile -o $wordFile --toc --toc-depth=2 --highlight-style=tango
    Write-Host "✅ Word document created successfully!" -ForegroundColor Green
    Write-Host "   Location: $wordFile" -ForegroundColor White
    Write-Host ""
} catch {
    Write-Host "❌ Error creating Word document: $_" -ForegroundColor Red
    Write-Host ""
}

# Optionally convert to PDF
Write-Host "📄 Converting Markdown to PDF..." -ForegroundColor Yellow

try {
    pandoc $markdownFile -o $pdfFile --toc --toc-depth=2 --highlight-style=tango --pdf-engine=xelatex
    Write-Host "✅ PDF document created successfully!" -ForegroundColor Green
    Write-Host "   Location: $pdfFile" -ForegroundColor White
    Write-Host ""
} catch {
    Write-Host "⚠️  PDF conversion failed (requires LaTeX)" -ForegroundColor Yellow
    Write-Host "   You can install MiKTeX from https://miktex.org/" -ForegroundColor White
    Write-Host ""
}

Write-Host "==================================================================" -ForegroundColor Cyan
Write-Host "  Summary" -ForegroundColor Cyan
Write-Host "==================================================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "✅ Markdown file: DEMO_PRESENTATION.md" -ForegroundColor Green
if (Test-Path $wordFile) {
    Write-Host "✅ Word document: Onbase_Invoice_Chatbot_Demo.docx" -ForegroundColor Green
}
if (Test-Path $pdfFile) {
    Write-Host "✅ PDF document: Onbase_Invoice_Chatbot_Demo.pdf" -ForegroundColor Green
}
Write-Host ""
Write-Host "You can now open the Word document and customize it further!" -ForegroundColor Yellow
Write-Host ""

# Open the Word document
if (Test-Path $wordFile) {
    Write-Host "Opening Word document..." -ForegroundColor Cyan
    Start-Process $wordFile
}

