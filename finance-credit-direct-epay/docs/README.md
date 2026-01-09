# Finance Credit Direct EPay System

**Version:** 1.0  
**Framework:** ASP.NET Web Forms (.NET Framework 3.5)  
**Language:** Visual Basic .NET  
**Database:** SQL Server, IBM DB2 (AS/400)  

---

## Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Architecture](#architecture)
- [Prerequisites](#prerequisites)
- [Installation](#installation)
- [Configuration](#configuration)
- [Usage](#usage)
- [Security](#security)
- [Documentation](#documentation)
- [Support](#support)

---

## Overview

The Finance Credit Direct EPay System is an internal web application for Ashley Furniture Industries that enables customers to make electronic payments for open invoices. The system integrates with US Bank's payment gateway to process ACH (Automated Clearing House) payments.

### Purpose

- Allow customers to view and select open invoices for payment
- Generate payment batches for submission to US Bank
- Track payment status and confirmation
- Provide reporting and analytics for credit analysts
- Maintain payment history and audit trails

### Business Context

This application is part of Ashley Furniture's credit and collections workflow, enabling:
- Faster payment processing
- Reduced manual intervention
- Improved cash flow management
- Enhanced customer self-service capabilities

---

## Features

### Customer Features

1. **Invoice Management**
   - View open invoices with filtering by date range, PO number, invoice number
   - Search invoices and credits
   - Sort by multiple columns (date, amount, status, etc.)
   - Pagination support for large datasets
   - Export to Excel functionality

2. **Payment Processing**
   - Select multiple invoices for batch payment
   - View payment totals before submission
   - Generate payment confirmation reports
   - Integration with US Bank payment gateway
   - Real-time payment status tracking

3. **Payment History**
   - View historical payments
   - Track confirmation numbers
   - Access payment receipts
   - Monitor payment status (Sent, Verifying, Confirmed)

### Analyst Features

1. **User Management**
   - View EPay users by customer, territory, state
   - Filter by terms codes
   - Paginated user lists

2. **Reporting**
   - Analyst reports with custom date ranges
   - Filter by customer, reference number, confirmation number
   - Territory-based reporting
   - Sortable columns

3. **Administrative Functions**
   - Cancel unconfirmed payments
   - Delete EPay records
   - View payment details
   - Access audit logs

---

## Architecture

### High-Level Architecture

```
┌─────────────┐      ┌──────────────┐      ┌─────────────┐
│   Browser   │─────▶│  IIS/ASP.NET │─────▶│ SQL Server  │
│  (Client)   │◀─────│  Web Server  │◀─────│  (Ashley)   │
└─────────────┘      └──────────────┘      └─────────────┘
                            │
                            │
                            ▼
                     ┌──────────────┐
                     │   US Bank    │
                     │   Payment    │
                     │   Gateway    │
                     └──────────────┘
                            │
                            ▼
                     ┌──────────────┐
                     │   IBM DB2    │
                     │   (AS/400)   │
                     └──────────────┘
```

### Technology Stack

**Frontend:**
- ASP.NET Web Forms
- JavaScript (jQuery, custom scripts)
- AJAX Control Toolkit
- GrapeCity ActiveReports 7.1

**Backend:**
- Visual Basic .NET
- ASP.NET 3.5
- IIS 7.0+

**Data Layer:**
- SQL Server (Ashley, Datawhse databases)
- IBM DB2 iSeries (AS/400)
- ADO.NET
- Stored Procedures

**Third-Party Integrations:**
- US Bank EPay Gateway (HTTPS/JHTML)
- Ashley Common Assemblies
- Ashley.Web.Responsive UI Framework

### Key Components

1. **Pages (ASPX)**
   - `main.aspx` - Invoice selection and payment initiation
   - `Confirmation.aspx` - Payment confirmation and US Bank redirect
   - `History.aspx` - Payment history
   - `AnalystReport.aspx` - Analyst reporting
   - `UserList.aspx` - User management
   - `AdminMaintenance.aspx` - Administrative functions
   - `ViewPayment.aspx` - Payment details

2. **Business Logic (Classes)**
   - `Common.vb` - Core business logic and data access
   - `DataAccess.vb` - SQL Server data access layer
   - `Db2DataAccess.vb` - IBM DB2 data access layer
   - `EpayBasePage.vb` - Base page with common functionality

3. **Data Access**
   - Stored procedures in SQL Server
   - DB2 stored procedures for MC2 batch processing
   - Direct SQL queries (⚠️ Security concern - see Security Audit)

4. **Reports**
   - `ReportEpayConfirmation.vb` - Payment confirmation report (ActiveReports)

---

## Prerequisites

### Development Environment

- **Operating System:** Windows Server 2008 R2 or later
- **IDE:** Visual Studio 2010 or later
- **.NET Framework:** 3.5 SP1
- **IIS:** 7.0 or later
- **Database Access:**
  - SQL Server 2008 or later
  - IBM DB2 iSeries Access Client

### Runtime Dependencies

- **Ashley Common Assemblies:**
  - Ashley.Web.LocalUI (1.0.0.0)
  - Ashley.Web.Responsive (1.0.0.22927)
  - Ashley2.Common.Auditing (2.0.0.0)
  - Ashley2.Data (2.0.0.0)
  - Ashley2.Web.UI.BasePage (2.0.0.0)
  - Ashley2.Web.UI.GridViewExportUtil (2.0.5669.23981)

- **Third-Party Components:**
  - GrapeCity ActiveReports 7.1.7671.0
  - AJAX Control Toolkit 3.5.40412.2
  - IBM.Data.DB2.iSeries (11.0.11.2)

### Network Access

- SQL Server databases: `Ashley`, `Datawhse`, `AFI_Batch`, `AFI_Dynamic`
- IBM DB2 AS/400 system
- US Bank EPay Gateway: `https://epayment.epymtservice.com`
- Ashley network file shares for common assemblies

---

## Installation

### 1. Clone Repository

```bash
git clone https://github.com/afi-internal/finance-credit-direct-epay.git
cd finance-credit-direct-epay
```

### 2. Configure IIS

```powershell
# Create application pool
New-WebAppPool -Name "EPayAppPool"
Set-ItemProperty IIS:\AppPools\EPayAppPool -Name "managedRuntimeVersion" -Value "v2.0"

# Create website
New-Website -Name "EPay" -Port 80 -PhysicalPath "C:\inetpub\wwwroot\EPay" -ApplicationPool "EPayAppPool"
```

### 3. Configure Database Connections

Edit `Web.config` and update connection strings (stored in Ashley.Data.DataAccess):

```xml
<!-- Connection strings are managed centrally in Ashley.Data.DataAccess -->
<!-- Ensure the application has access to:
     - SQL_Dynamic (Ashley database)
     - AFI_Batch (Datawhse database)
     - DB2_AFI (IBM DB2 AS/400)
-->
```

### 4. Deploy Common Assemblies

Ensure access to network shares:
```
\\ashleyfurniture.com\afi-dfs\Vaults\Ashley_Common_Assemblies\
```

Or copy assemblies to local `bin` folder.

### 5. Build Solution

```bash
# Open in Visual Studio
devenv EPay.sln

# Or build from command line
msbuild EPay.sln /p:Configuration=Release
```

### 6. Configure Security

⚠️ **CRITICAL:** See [SECURITY_AUDIT.md](./SECURITY_AUDIT.md) for security vulnerabilities that must be addressed before deployment.

---

## Configuration

### Web.config Settings

#### Application Settings

```xml
<appSettings>
  <!-- Maximum HTTP collection keys -->
  <add key="aspnet:MaxHttpCollectionKeys" value="50000"/>

  <!-- ActiveReports License -->
  <add key="DataDynamicsARLic" value="[LICENSE_KEY]"/>
</appSettings>
```

#### Custom Errors

```xml
<!-- ⚠️ SECURITY: Set to RemoteOnly or On in production -->
<customErrors mode="RemoteOnly" defaultRedirect="~/Error.aspx"/>
```

#### Compilation

```xml
<!-- ⚠️ SECURITY: Set debug="false" in production -->
<compilation debug="false">
```

### Environment-Specific Configuration

The application detects environment based on `SessionData("SITENAME")`:

- **DEV.ASHLEYDIRECT.COM** - Development (blocks US Bank calls)
- **STAGE.ASHLEYDIRECT.COM** - Staging (blocks US Bank calls)
- **Production** - Live environment (enables US Bank integration)

### Database Configuration

Database connections are managed through `Ashley.Data.DataAccess` component:

- **SQL_Dynamic** - Ashley database (customer, security data)
- **AFI_Batch** - Datawhse database (EPay transactions)
- **DB2_AFI** - IBM DB2 AS/400 (MC2 batch processing)

### US Bank Integration

**Production URL:** `https://epayment.epymtservice.com/epay.jhtml`

**Parameters:**
- `productCode=OpenInvoices`
- `billerId=INV`
- `billerGroupId=ASH`
- `paymentMethod=ACH`
- `paymentType=Single`
- `amountDue` - Total payment amount
- `billerPayorId` - Customer number
- `ReferenceNumber` - EPay reference number
- Customer address information

---

## Usage

### Customer Workflow

#### 1. Login and Account Selection

Users must be authenticated through Ashley's authentication system and have a customer account selected.

#### 2. View Open Invoices

Navigate to `main.aspx`:

1. **Filter Invoices:**
   - Set date range (From/To dates)
   - Enter PO Number (optional)
   - Enter Invoice Number (optional)
   - Enter Credit Number (optional)

2. **Search:**
   - Click "Search" button
   - Results display in paginated grid

3. **Sort:**
   - Click column headers to sort
   - Toggle ascending/descending

#### 3. Select Invoices for Payment

1. Check individual invoices or use "Select All"
2. Review invoice details:
   - Invoice/Credit number
   - Ship-to location
   - Invoice date
   - Order number
   - PO number
   - Invoice amount
   - Amount paid
   - Balance
   - Aging code and days

3. Click "Make Payment" button

#### 4. Confirm Payment

On `Confirmation.aspx`:

1. Review reference number
2. Review total amount
3. View selected invoices in report
4. Options:
   - **OK** - Proceed to US Bank payment gateway
   - **Cancel** - Cancel payment and delete EPay records

#### 5. Complete Payment at US Bank

1. Redirected to US Bank gateway
2. Enter bank account information
3. Confirm payment
4. Receive confirmation number
5. Redirected back to application

#### 6. View Payment History

Navigate to `History.aspx`:

1. Filter by date range, customer, reference number, confirmation number
2. View payment status:
   - **Sent** - Submitted to US Bank
   - **Verifying** - Payment in process
   - **Confirmed** - Payment completed
3. Access payment receipts

### Analyst Workflow

#### 1. View Analyst Report

Navigate to `AnalystReport.aspx`:

1. **Filter Options:**
   - Customer Number
   - Reference Number
   - Confirmation Number
   - Payment Date
   - Credit Territory

2. **View Results:**
   - Payment details
   - Customer information
   - Status tracking
   - Sortable columns

#### 2. Manage Users

Navigate to `UserList.aspx`:

1. **Filter Users:**
   - Customer Number
   - Customer Name
   - Bill-to State
   - Credit Territory
   - Terms Code

2. **View User List:**
   - Paginated results
   - Export to Excel

#### 3. Administrative Functions

Navigate to `AdminMaintenance.aspx`:

1. **Cancel Payments:**
   - Delete unconfirmed payments
   - Remove EPay records

2. **View Payment Details:**
   - Access detailed payment information
   - Review transaction history

### Export to Excel

Available on invoice grid:

1. Click Excel icon
2. All invoices (not just current page) exported
3. Currency formatting preserved
4. File downloads as `EpaymentCustomerInvoices.xls`

---

## Security

⚠️ **CRITICAL SECURITY ISSUES IDENTIFIED**

This application has **CRITICAL** security vulnerabilities. See [SECURITY_AUDIT.md](./SECURITY_AUDIT.md) for complete details.

### Known Vulnerabilities

1. **SQL Injection** (CRITICAL 🔴)
   - Multiple instances of string concatenation in SQL queries
   - Dynamic SQL in stored procedures
   - **Action Required:** Immediate remediation

2. **Hardcoded Credentials** (CRITICAL 🔴)
   - Passwords in Web.config (commented but exposed in source control)
   - **Action Required:** Remove and rotate credentials

3. **Weak Authentication** (HIGH 🟡)
   - Debug mode bypasses authentication
   - **Action Required:** Never deploy debug builds

4. **Insufficient Input Validation** (MEDIUM 🟡)
   - Minimal validation on user inputs
   - **Action Required:** Implement comprehensive validation

5. **Sensitive Data Exposure** (MEDIUM 🟡)
   - Trace statements log sensitive data
   - Custom errors disabled
   - **Action Required:** Remove trace statements, enable custom errors

### Security Best Practices

**Before deploying to production:**

1. ✅ Review and fix all SQL injection vulnerabilities
2. ✅ Remove hardcoded credentials
3. ✅ Enable custom errors (`mode="RemoteOnly"`)
4. ✅ Set `debug="false"` in Web.config
5. ✅ Implement input validation
6. ✅ Add authorization checks
7. ✅ Remove trace statements
8. ✅ Conduct security testing
9. ✅ Implement logging and monitoring
10. ✅ Document security controls

### Compliance

This application handles sensitive financial data and must comply with:

- **PCI-DSS** - Payment Card Industry Data Security Standard
- **SOX** - Sarbanes-Oxley Act
- **Internal Security Policies** - Ashley Furniture security requirements

---

## Documentation

### Available Documentation

- **[README.md](./README.md)** - This file (overview and usage)
- **[SECURITY_AUDIT.md](./SECURITY_AUDIT.md)** - Security vulnerabilities and remediation
- **[ARCHITECTURE.md](./ARCHITECTURE.md)** - System architecture and design
- **[API_DOCUMENTATION.md](./API_DOCUMENTATION.md)** - Stored procedures and data contracts
- **[DEPLOYMENT.md](./DEPLOYMENT.md)** - Deployment procedures and configuration
- **[USER_GUIDE.md](./USER_GUIDE.md)** - End-user documentation

### Code Documentation

- XML documentation in code files
- Revision history in file headers
- Inline comments for complex logic

### Database Documentation

- Stored procedure scripts in `/SQL` folder
- Database schema documentation (see DBA team)

---

## Support

### Internal Contacts

- **Development Team:** Ashley Furniture IT Development
- **Database Team:** Ashley Furniture DBA Team
- **Security Team:** Ashley Furniture Information Security
- **Business Owner:** Credit Department

### Troubleshooting

#### Common Issues

**Issue:** "Can not call USBank from Test!!"
- **Cause:** Application running in DEV or STAGE environment
- **Solution:** This is expected behavior; US Bank integration disabled in non-production

**Issue:** SQL timeout errors
- **Cause:** Large date ranges or complex queries
- **Solution:** Reduce date range, add indexes, optimize queries

**Issue:** "No Invoices to send!"
- **Cause:** No invoices selected or all selected invoices already in EPay
- **Solution:** Select valid invoices not already in payment status

**Issue:** Authentication errors
- **Cause:** User not authenticated or session expired
- **Solution:** Re-login through Ashley authentication system

### Logging

Application logs errors to:
- `Ashley.dbo.usp_InsertAuditRecord` stored procedure
- IIS logs
- Windows Event Log

### Monitoring

Monitor the following:
- Payment submission success rate
- US Bank integration availability
- Database connection health
- Application errors in audit log

---

## Revision History

| Date | Version | Author | Description |
|------|---------|--------|-------------|
| 2012-03-28 | 1.0 | TKiel | Initial consolidation from OpenInvoicesBLL |
| 2012-06-22 | 1.1 | TKiel | Added payment contact email to US Bank |
| 2012-07-23 | 1.2 | TKiel | Changed disallowLogin parameter |
| 2013-05-20 | 1.3 | TKiel | Added custom SQL paging |
| 2014-05-19 | 1.4 | SeSekar | Modified SQL paging to show all |

---

## License

**Copyright © 2025 Ashley Furniture Industries, Inc. All rights reserved.**

This software is proprietary and confidential. Unauthorized copying, distribution, or use of this software, via any medium, is strictly prohibited.

---

## Additional Resources

- **US Bank EPay Documentation:** Contact US Bank representative
- **Ashley Common Assemblies:** See internal documentation portal
- **ActiveReports Documentation:** https://www.grapecity.com/activereports
- **.NET Framework 3.5:** https://docs.microsoft.com/en-us/dotnet/framework/

---

**⚠️ IMPORTANT:** Before using this application, review the [Security Audit](./SECURITY_AUDIT.md) and address all critical vulnerabilities.


