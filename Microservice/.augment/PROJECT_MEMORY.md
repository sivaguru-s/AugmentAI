# E-Payment Migration Project Memory

## Project Overview

This is a **legacy VB.NET ASP.NET WebForms application (E-Payment system)** being migrated to a **modern Angular + ASP.NET Core architecture**.

### Repository Structure
- **Legacy Code**: `finance-credit-direct-epay/` (VB.NET WebForms - `.aspx`, `.aspx.vb` files)
- **New Backend**: `finance-credit-direct-epay/backend/` (ASP.NET Core Web API)
- **New Frontend**: `finance-credit-direct-epay/frontend/` (Angular with NgRx)
- **Branch**: `feature/migrate_augment_dotnet`

### Key Files Reference
| Legacy File | Purpose | Migration Status |
|-------------|---------|------------------|
| `main.aspx` / `main.aspx.vb` | Invoice Search Page | ✅ Migrated |
| `AnalystReport.aspx.vb` | Analyst Report | 🔄 Pending |
| `Classes/Common.vb` | Shared utilities, DB calls | Reference for migration |

### Database
- **Connection String**: `AFI_Batch`
- **Key Stored Procedure**: `Datawhse.dbo.usp_OrderAndInvoiceReportingOpenInvoices3` (Invoice search)
- **Date Span SP**: `Ashley.dbo.usp_GetEpayDefaultDateSpanInDays`

---

## Recently Completed Work

### 1. Invoice Search Migration (Complete ✅)
- **Frontend**: Angular component with NgRx state management
- **Backend**: ASP.NET Core Web API with Repository pattern
- **Fixed Issues**:
  - "Invalid column name 'string'" - Added sort column whitelist validation
  - "DataTable Returns 0 Rows" - Set correct `securityMHS` parameter ('MASTERXX') and `allShipTos: true`
  - Date parameter mapping corrected (`@FromDate` = start date, `@ToDate` = end date)

### 2. Default Date Range Update (Complete ✅)
- Changed from 90 days to **180 days (6 months)**
- Updated in both frontend (`invoice.state.ts`) and backend (`InvoiceRepository.cs`)

### 3. Augment Rules Applied (Complete ✅)
All coding standards from `.augment/rules/` applied:

**Backend Enhancements:**
- Created `BusinessExceptions.cs` with custom exception hierarchy (EPayException, ValidationException, DataAccessException, InvalidDateRangeException)
- Added `CancellationToken` to all async methods
- Added comprehensive XML documentation (`<summary>`, `<param>`, `<returns>`, `<exception>`)
- Added input validation (`ValidateSearchRequest()` method)
- Added input sanitization (`SanitizeInput()`, `SanitizePoNumber()`)
- Enhanced error handling with specific exception types
- Used `ArgumentNullException.ThrowIfNull()` for null checks

**Frontend Enhancements:**
- Added custom validators: `dateRangeValidator()`, `maxDateAgeValidator()`, `notFutureDateValidator()`
- Added validation error display in template
- Disabled search button when form is invalid

### 4. Latest Commit
- **Hash**: `0863c65`
- **Message**: `feat(epay): Apply augment rules - comprehensive code quality improvements`
- **Files Changed**: 10 files, +770 lines, -149 deletions

---

## What's Next (Pending Tasks)

### Immediate Next Steps
1. **Build and test the application** to verify all changes work correctly
2. **Migrate `AnalystReport.aspx.vb`** - The user currently has this file open

### Remaining Pages to Migrate
- `AnalystReport.aspx` - Analyst reporting functionality
- `RPPReport.aspx` - RPP reporting
- `CheckStatus.aspx` - Payment status checking
- `AdminMaster.aspx` - Admin functions
- Other `.aspx` pages in the legacy folder

### Technical Debt / Improvements
- Implement Excel export in `InvoiceService.ExportToExcelAsync()` (currently returns empty array)
- Implement `DetermineIfToUseConsumerSearchAsync()` logic from Common.vb
- Enable authentication when ready (currently commented out in controller)
- Write unit tests for the migrated code

---

## Key Technical Patterns

### Backend Architecture
```
Controller -> Service -> Repository -> Database (Stored Procedures)
```

### Frontend Architecture  
```
Component -> NgRx Store (Actions/Effects/Reducers) -> Service -> HTTP -> API
```

### Augment Rules Location
- `.augment/rules/Agent_Backend_Developer.md`
- `.augment/rules/coding_robust-error-handling.md`
- `finance-credit-direct-epay/.augment/rules/` (project-specific rules)

---

## Commands to Run

```powershell
# Backend
cd C:\AugmentAI\finance-credit-direct-epay\backend
dotnet build
dotnet run

# Frontend
cd C:\AugmentAI\finance-credit-direct-epay\frontend
npm install
npm start
```

---

## Important Parameters for Invoice Search

| Parameter | Value | Notes |
|-----------|-------|-------|
| `customerNumber` | e.g., '4031300' | Required |
| `securityMHS` | 'MASTERXX' | Required for query to return results |
| `allShipTos` | true (1) | Search all ship-tos |
| `@FromDate` | Start date (earlier) | Despite confusing name |
| `@ToDate` | End date (later) | Despite confusing name |

