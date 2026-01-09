# Architecture Documentation - Finance Credit Direct EPay

**Version:** 1.0  
**Last Updated:** January 9, 2026  
**Framework:** ASP.NET Web Forms (.NET 3.5)  

---

## Table of Contents

- [System Overview](#system-overview)
- [Architecture Patterns](#architecture-patterns)
- [Component Architecture](#component-architecture)
- [Data Architecture](#data-architecture)
- [Integration Architecture](#integration-architecture)
- [Security Architecture](#security-architecture)
- [Deployment Architecture](#deployment-architecture)

---

## System Overview

### Purpose

The Finance Credit Direct EPay system is a web-based payment processing application that enables Ashley Furniture customers to:
1. View open invoices
2. Select invoices for payment
3. Submit payments via US Bank's ACH gateway
4. Track payment status and history

### Context Diagram

```
┌──────────────────────────────────────────────────────────────┐
│                    External Systems                          │
│                                                              │
│  ┌─────────────┐  ┌──────────────┐  ┌──────────────┐       │
│  │   Browser   │  │   US Bank    │  │  IBM DB2     │       │
│  │  (Customer) │  │   Gateway    │  │  (AS/400)    │       │
│  └──────┬──────┘  └──────▲───────┘  └──────▲───────┘       │
│         │                │                  │               │
└─────────┼────────────────┼──────────────────┼───────────────┘
          │                │                  │
          ▼                │                  │
┌──────────────────────────┼──────────────────┼───────────────┐
│         EPay Application │                  │               │
│                          │                  │               │
│  ┌────────────────────┐  │                  │               │
│  │   Presentation     │  │                  │               │
│  │   Layer (ASPX)     │  │                  │               │
│  └─────────┬──────────┘  │                  │               │
│            │             │                  │               │
│  ┌─────────▼──────────┐  │                  │               │
│  │   Business Logic   │──┘                  │               │
│  │   Layer (Classes)  │                     │               │
│  └─────────┬──────────┘                     │               │
│            │                                │               │
│  ┌─────────▼──────────┐  ┌─────────────────▼──────┐        │
│  │   Data Access      │  │   DB2 Data Access      │        │
│  │   Layer (SQL)      │  │   Layer                │        │
│  └─────────┬──────────┘  └─────────┬──────────────┘        │
│            │                       │                        │
└────────────┼───────────────────────┼────────────────────────┘
             │                       │
             ▼                       ▼
    ┌────────────────┐      ┌────────────────┐
    │  SQL Server    │      │   IBM DB2      │
    │  (Ashley,      │      │   (MC2 Batch)  │
    │   Datawhse)    │      │                │
    └────────────────┘      └────────────────┘
```

---

## Architecture Patterns

### 1. Three-Tier Architecture

The application follows a classic three-tier architecture:

#### Presentation Layer
- **Technology:** ASP.NET Web Forms (ASPX pages)
- **Responsibility:** User interface, input validation, display logic
- **Components:** 
  - `main.aspx` - Invoice selection
  - `Confirmation.aspx` - Payment confirmation
  - `History.aspx` - Payment history
  - `AnalystReport.aspx` - Reporting
  - `UserList.aspx` - User management
  - `AdminMaintenance.aspx` - Administration

#### Business Logic Layer
- **Technology:** Visual Basic .NET classes
- **Responsibility:** Business rules, workflow orchestration, data transformation
- **Components:**
  - `Common.vb` - Core business logic
  - `EpayBasePage.vb` - Base page functionality
  - Code-behind files (*.aspx.vb)

#### Data Access Layer
- **Technology:** ADO.NET, Stored Procedures
- **Responsibility:** Database operations, data retrieval, persistence
- **Components:**
  - `DataAccess.vb` - SQL Server data access
  - `Db2DataAccess.vb` - IBM DB2 data access
  - Stored procedures in SQL Server and DB2

### 2. Page Controller Pattern

Each ASPX page acts as a controller:
- Handles user input
- Invokes business logic
- Updates view based on results
- Manages page lifecycle events

### 3. Repository Pattern (Partial)

The `Common.vb` class acts as a repository for EPay data:
- Encapsulates data access logic
- Provides methods for CRUD operations
- Abstracts database implementation details

---

## Component Architecture

### Presentation Components

#### 1. Main Invoice Selection (`main.aspx`)

**Purpose:** Display and filter open invoices, select invoices for payment

**Key Features:**
- GridView with custom paging
- AJAX-enabled filtering
- Excel export functionality
- Multi-select checkboxes
- Dynamic sorting

**Dependencies:**
- `Common.LoadInvoices()` - Retrieve invoices
- `Common.InsertEpayRecords()` - Create payment batch
- `Ashley2.Web.UI.GridViewExportUtil` - Excel export

**User Controls:**
- `ucAccountSelection` - Customer account selector
- `ucNavigation` - Navigation menu

#### 2. Payment Confirmation (`Confirmation.aspx`)

**Purpose:** Review payment details and redirect to US Bank

**Key Features:**
- Display selected invoices
- Calculate total payment amount
- Generate ActiveReports confirmation
- Redirect to US Bank gateway
- Cancel payment option

**Dependencies:**
- `Common.GetTotal()` - Calculate payment total
- `Common.UpdateEpayStatus()` - Update payment status
- `Common.DeleteEpayRecords()` - Cancel payment
- `ReportEpayConfirmation` - Generate PDF report

**Integration:**
- US Bank EPay Gateway (HTTPS redirect)

#### 3. Payment History (`History.aspx`)

**Purpose:** View historical payments and status

**Key Features:**
- Filter by date, customer, reference number
- Display payment status
- Sortable columns
- Pagination

**Dependencies:**
- `Common.LoadEpayAnalystReport()` - Retrieve payment history

#### 4. Analyst Report (`AnalystReport.aspx`)

**Purpose:** Credit analyst reporting and monitoring

**Key Features:**
- Multi-criteria filtering
- Territory-based reporting
- Export capabilities
- Custom sorting

**Dependencies:**
- `Common.LoadEpayAnalystReport()` - Retrieve analyst data
- `Common.LoadTerritoryListbox()` - Load territories

#### 5. User List (`UserList.aspx`)

**Purpose:** View and manage EPay users

**Key Features:**
- Filter by customer, state, territory, terms
- Paginated results
- Excel export

**Dependencies:**
- `Common.LoadEpayUsers()` - Retrieve user list

#### 6. Admin Maintenance (`AdminMaintenance.aspx`)

**Purpose:** Administrative functions for payment management

**Key Features:**
- Delete unconfirmed payments
- Cancel EPay records
- View payment details

**Dependencies:**
- `Common.DeleteEpayUnconfirmedPayment()` - Remove payments
- `Common.DeleteEpayRecords()` - Cancel batches

### Business Logic Components

#### 1. Common Class (`Classes/Common.vb`)

**Purpose:** Central business logic and data access for EPay operations

**Key Methods:**

| Method | Purpose | Parameters | Returns |
|--------|---------|------------|---------|
| `GetTotals()` | Calculate payment total | custNum, referenceNumber | SqlDataReader |
| `UpdateEpayStatus()` | Update payment status | custNum, refNo, status, user | Boolean |
| `InsertEpayRecords()` | Create payment batch | custNum, shipTo, invoices, user | Integer (refNo) |
| `DeleteEpayRecords()` | Cancel payment | custNum, refNo, user | Boolean |
| `DeleteEpayUnconfirmedPayment()` | Remove unconfirmed payment | custNum, invoice, user | Boolean |
| `LoadInvoices()` | Retrieve open invoices | Multiple filters | SqlDataReader |
| `LoadEpayInvoicesByRefNumber()` | Get invoices by reference | custNum, refNo | SqlDataReader |
| `LoadEpayAnalystReport()` | Get analyst report data | Multiple filters | SqlDataReader |
| `LoadUnconfirmedPayments()` | Get unconfirmed payments | Multiple filters | SqlDataReader |
| `LoadEpayUsers()` | Get EPay user list | Multiple filters | SqlDataReader |
| `GetEpayStatus()` | Get payment status | custNum, refNo | String |
| `GetPaymentContactEmailAddress()` | Get contact email | custNum | String |

**Design Issues:**
- ⚠️ SQL injection vulnerabilities (string concatenation)
- ⚠️ Mixed concerns (data access + business logic)
- ⚠️ No separation of interfaces
- ⚠️ Tight coupling to database

#### 2. EpayBasePage Class (`Classes/EpayBasePage.vb`)

**Purpose:** Base page class with common functionality

**Features:**
- Session management
- User authentication (debug mode)
- Common properties and methods
- Error handling

**Inheritance:**
```
Ashley2.Web.UI.BasePage.BasePageClass
    └── EpayBasePage
        ├── main
        ├── Confirmation
        ├── History
        ├── AnalystReport
        ├── UserList
        └── AdminMaintenance
```

**Key Properties:**
- `SessionData()` - Session variable accessor
- `Navigation` - Navigation control
- `ucAccountSelection` - Account selection control

**Design Issues:**
- ⚠️ Debug mode bypasses authentication
- ⚠️ Hardcoded test credentials

#### 3. Report Components (`Reports/ReportEpayConfirmation.vb`)

**Purpose:** Generate payment confirmation PDF reports

**Technology:** GrapeCity ActiveReports 7.1

**Features:**
- Dynamic report generation
- PDF export
- Customer and invoice details
- Payment summary

**Data Source:** `Common.LoadEpayInvoicesByRefNumber()`

### Data Access Components

#### 1. DataAccess Class (`Classes/DataAccess.vb`)

**Purpose:** SQL Server data access abstraction

**Key Methods:**
- `GetDataReader()` - Execute query, return DataReader
- `ExecuteStoredProcedure()` - Execute stored procedure
- `ExecuteNonQuery()` - Execute command without results

**Connection Management:**
- Uses `Ashley.Data.DataAccess` for connection strings
- Supports multiple databases (Ashley, Datawhse, AFI_Batch)

**Design Issues:**
- ⚠️ Accepts raw SQL strings (enables SQL injection)
- ⚠️ No parameterization enforcement
- ⚠️ Limited error handling

#### 2. Db2DataAccess Class (`Classes/Db2DataAccess.vb`)

**Purpose:** IBM DB2 (AS/400) data access

**Key Methods:**
- `ExecuteStoredProcedure()` - Execute DB2 stored procedure
- `GetDataReader()` - Execute query, return DataReader

**Connection Management:**
- Uses IBM.Data.DB2.iSeries provider
- Connects to AS/400 system for MC2 batch processing

**Usage:**
- MC2 batch file generation
- Integration with legacy systems

---

## Data Architecture

### Database Schema

#### SQL Server Databases

**1. Datawhse Database**

**Tables:**
- `tblEpay` - EPay payment records
  - `epaCusNo` (varchar(8)) - Customer number
  - `epaShpNo` (varchar(4)) - Ship-to number
  - `epaRefNo` (int) - Reference number (PK)
  - `epaInvNo` (varchar(9)) - Invoice number
  - `epaInvAm` (money) - Invoice amount
  - `epaStatus` (varchar(20)) - Payment status
  - `epaConfirmationNo` (varchar(50)) - US Bank confirmation
  - `epaDateAdded` (datetime) - Created date
  - `epaUserAdded` (varchar(50)) - Created by
  - `epaDateChanged` (datetime) - Modified date
  - `epaUserChanged` (varchar(50)) - Modified by

- `tblOpenInvoices` - Open invoice data
  - `opiCusNo` (varchar(8)) - Customer number
  - `opiShpNo` (varchar(4)) - Ship-to number
  - `opiInvNo` (varchar(9)) - Invoice number
  - `opiCrmNr` (varchar(6)) - Credit memo number
  - `opiDageDt` (datetime) - Invoice date
  - `opiInvAm` (money) - Invoice amount
  - `opiTtlCr` (money) - Total credits
  - `opiOpAmt` (money) - Open amount
  - `opiDlpDto` (datetime) - Last payment date
  - `opiCatCd` (varchar(2)) - Category code
  - `opiPoNum` (varchar(22)) - PO number
  - `opiOrdNo` (varchar(7)) - Order number

- `tblInvoiceHeader` - Invoice header details
  - `inhInvNo` (varchar(9)) - Invoice number
  - `inhOrdNo` (varchar(7)) - Order number
  - `inhCusNo` (varchar(8)) - Customer number
  - `inhShpNo` (varchar(4)) - Ship-to number
  - `inhdOrdDa` (datetime) - Order date
  - `inhShipInstructions` (varchar(max)) - Shipping instructions
  - `inhTripNo` (int) - Trip number

**2. Ashley Database**

**Tables:**
- `tblSecurityCustomer` - Customer security
  - `secCusNo` (varchar(8)) - Customer number
  - `secShpNo` (varchar(4)) - Ship-to number
  - `secMhs_name` (varchar(8)) - Security MHS name

- `tblCustomerShippingLocations` - Ship-to locations
  - `cslCustomerNumber` (varchar(8)) - Customer number
  - `cslShipToNumber` (varchar(4)) - Ship-to number
  - `acrec` (varchar(1)) - Account receivable status

- `tblCustomer` - Customer master
  - Customer details, address, contact information

**3. AFI_Batch Database**

**Tables:**
- Batch processing tables
- Audit and logging tables

#### Stored Procedures

**Key Stored Procedures:**

| Procedure | Database | Purpose |
|-----------|----------|---------|
| `usp_GetEpayTotal` | Datawhse | Calculate payment total |
| `usp_UpdateEpayStatus` | Datawhse | Update payment status |
| `usp_InsertEpay` | Datawhse | Insert EPay record |
| `usp_DeleteEpay` | Datawhse | Delete EPay record |
| `usp_DeleteEpayUnconfirmedPayment` | Datawhse | Remove unconfirmed payment |
| `usp_GetEpayInvoicesByRefNumber` | Datawhse | Get invoices by reference |
| `usp_GetEpayAnalystReport` | Datawhse | Get analyst report data |
| `usp_GetEpayUnconfirmedPayments` | Datawhse | Get unconfirmed payments |
| `usp_GetEpayUsers` | Datawhse | Get EPay users |
| `usp_GetEpayStatus` | Datawhse | Get payment status |
| `usp_OrderAndInvoiceReportingOpenInvoices2` | Datawhse | Get open invoices |
| `usp_GetPaymentContactEmailAddress` | Ashley | Get contact email |

**⚠️ Security Issue:** Several stored procedures use dynamic SQL with string concatenation (see SECURITY_AUDIT.md)

### Data Flow

#### Invoice Selection Flow

```
User → main.aspx → Common.LoadInvoices() → usp_OrderAndInvoiceReportingOpenInvoices2
                                                    ↓
                                            tblOpenInvoices
                                            tblInvoiceHeader
                                            tblSecurityCustomer
                                            tblCustomerShippingLocations
                                                    ↓
                                            SqlDataReader → GridView
```

#### Payment Submission Flow

```
User selects invoices → main.aspx → Common.InsertEpayRecords()
                                            ↓
                                    usp_InsertEpay (multiple calls)
                                            ↓
                                    tblEpay (insert records)
                                            ↓
                                    Return Reference Number
                                            ↓
                        Redirect to Confirmation.aspx?RefNo=XXX
                                            ↓
                        Common.LoadEpayInvoicesByRefNumber()
                                            ↓
                        Display confirmation report
                                            ↓
                        User clicks OK → Common.UpdateEpayStatus("Sent")
                                            ↓
                        Redirect to US Bank Gateway
                                            ↓
                        User completes payment at US Bank
                                            ↓
                        US Bank redirects back (with confirmation)
                                            ↓
                        Manual status update to "Confirmed"
```

---

## Integration Architecture

### US Bank EPay Gateway Integration

**Integration Type:** HTTP Redirect (Form POST)

**Endpoint:** `https://epayment.epymtservice.com/epay.jhtml`

**Protocol:** HTTPS

**Method:** GET (URL parameters)

**Parameters:**

| Parameter | Type | Description | Example |
|-----------|------|-------------|---------|
| `productCode` | String | Product identifier | "OpenInvoices" |
| `billerId` | String | Biller ID | "INV" |
| `billerGroupId` | String | Biller group | "ASH" |
| `disallowLogin` | String | Disable login | "N" |
| `paymentMethod` | String | Payment type | "ACH" |
| `paymentType` | String | Payment frequency | "Single" |
| `amountDue` | Decimal | Payment amount | "1234.56" |
| `billerPayorId` | String | Customer number | "1011000" |
| `ReferenceNumber` | Integer | EPay reference | "12345" |
| `streetAddress1` | String | Address line 1 | URL encoded |
| `streetAddress2` | String | Address line 2 | URL encoded |
| `city` | String | City | URL encoded |
| `stateRegion` | String | State | URL encoded |
| `companyName` | String | Company name | URL encoded |
| `zipPostalcode` | String | ZIP code (5 digits) | "12345" |

**Return Flow:**
- US Bank processes payment
- User completes ACH setup
- US Bank redirects back to application (URL not specified in code)
- Status updated manually or via callback (not implemented)

**Security:**
- HTTPS encryption
- No authentication token visible in code
- Customer data passed in URL (potential exposure)

**Environment Handling:**
```vb
If Me.SessionData("SITENAME").Equals("DEV.ASHLEYDIRECT.COM") Or
   Me.SessionData("SITENAME").Equals("STAGE.ASHLEYDIRECT.COM") Then
    ' Block US Bank call in test environments
    lblErrorMsg.Text = "Can not call USBank from Test!!"
    Exit Sub
End If
```

### IBM DB2 (AS/400) Integration

**Integration Type:** Direct database connection

**Provider:** IBM.Data.DB2.iSeries (v11.0.11.2)

**Purpose:** MC2 batch file generation

**Connection:** Managed through `Ashley.Data.DataAccess`

**Usage:**
- Generate batch files for legacy systems
- Integration with AS/400 mainframe
- MC2 processing

**Stored Procedures:**
- DB2 stored procedures for batch processing
- Called via `Db2DataAccess.ExecuteStoredProcedure()`

### Ashley Common Assemblies Integration

**Dependencies:**

1. **Ashley.Web.LocalUI (1.0.0.0)**
   - Local UI components
   - Common controls

2. **Ashley.Web.Responsive (1.0.0.22927)**
   - Responsive UI framework
   - Navigation controls
   - Account selection

3. **Ashley2.Common.Auditing (2.0.0.0)**
   - Audit logging
   - Error tracking

4. **Ashley2.Data (2.0.0.0)**
   - Data access utilities
   - Connection management

5. **Ashley2.Web.UI.BasePage (2.0.0.0)**
   - Base page functionality
   - Session management
   - Authentication

6. **Ashley2.Web.UI.GridViewExportUtil (2.0.5669.23981)**
   - Excel export functionality
   - Grid utilities

**Assembly Location:**
```
\\ashleyfurniture.com\afi-dfs\Vaults\Ashley_Common_Assemblies\
```

---

## Security Architecture

⚠️ **See [SECURITY_AUDIT.md](./SECURITY_AUDIT.md) for detailed security analysis**

### Authentication

**Current Implementation:**
- Relies on Ashley's centralized authentication system
- Session-based authentication
- User identity stored in `SessionData("LOGON_USER")`

**Issues:**
- Debug mode bypasses authentication
- No multi-factor authentication
- Session timeout not configured

### Authorization

**Current Implementation:**
- Application-level authorization via `VerifyAppAuthorization("EPAYANLYST")`
- Customer-level security via `tblSecurityCustomer`
- MHS (security group) based access control

**Issues:**
- No role-based access control (RBAC)
- Insufficient authorization checks on reference numbers
- No audit logging for authorization failures

### Data Protection

**Current Implementation:**
- HTTPS for US Bank communication
- Database encryption (if configured at DB level)

**Issues:**
- Sensitive data in trace logs
- Customer data in URL parameters
- No data masking in logs
- Hardcoded credentials in source control

### Input Validation

**Current Implementation:**
- Minimal validation (single quote removal)
- Date validation
- Numeric validation

**Issues:**
- No comprehensive input validation
- No length checks
- No format validation
- SQL injection vulnerabilities

### Error Handling

**Current Implementation:**
- Try-catch blocks in most methods
- Redirect to error page
- Audit logging via `usp_InsertAuditRecord`

**Issues:**
- Custom errors disabled (exposes stack traces)
- Sensitive data in error messages
- Insufficient error logging

---

## Deployment Architecture

### Physical Architecture

```
┌─────────────────────────────────────────────────────────┐
│                    Load Balancer                        │
│                  (Optional/Not shown)                   │
└────────────────────┬────────────────────────────────────┘
                     │
         ┌───────────┴───────────┐
         │                       │
┌────────▼────────┐    ┌────────▼────────┐
│   Web Server 1  │    │   Web Server 2  │
│   IIS 7.0+      │    │   IIS 7.0+      │
│   .NET 3.5      │    │   .NET 3.5      │
│   EPay App      │    │   EPay App      │
└────────┬────────┘    └────────┬────────┘
         │                       │
         └───────────┬───────────┘
                     │
         ┌───────────┴───────────┐
         │                       │
┌────────▼────────┐    ┌────────▼────────┐
│  SQL Server 1   │    │  SQL Server 2   │
│  (Primary)      │    │  (Replica)      │
│  Ashley DB      │    │  Read-only      │
│  Datawhse DB    │    │                 │
└─────────────────┘    └─────────────────┘
         │
         │
┌────────▼────────┐
│   IBM AS/400    │
│   DB2 Database  │
│   MC2 System    │
└─────────────────┘
```

### Deployment Environments

#### Development (DEV.ASHLEYDIRECT.COM)
- Development and testing
- US Bank integration disabled
- Debug mode enabled
- Test data

#### Staging (STAGE.ASHLEYDIRECT.COM)
- Pre-production testing
- US Bank integration disabled
- Production-like configuration
- Sanitized production data

#### Production
- Live environment
- US Bank integration enabled
- Debug mode disabled
- Production data
- High availability configuration

### Deployment Process

1. **Build:**
   - Compile in Release mode
   - Set `debug="false"` in Web.config
   - Remove test credentials

2. **Package:**
   - Create deployment package
   - Include all assemblies
   - Include Web.config transformations

3. **Deploy:**
   - Stop IIS application pool
   - Backup current version
   - Copy files to web server
   - Update Web.config
   - Start IIS application pool

4. **Verify:**
   - Smoke test critical paths
   - Verify database connectivity
   - Check error logs
   - Monitor application health

### Configuration Management

**Environment-Specific Settings:**
- Connection strings (managed in Ashley.Data.DataAccess)
- US Bank URLs
- Debug flags
- Custom error modes
- Trace settings

**Configuration Files:**
- `Web.config` - Application configuration
- `Web.Debug.config` - Debug transformations
- `Web.Release.config` - Release transformations

---

## Performance Considerations

### Current Performance Characteristics

**Strengths:**
- Stored procedures for data access
- Pagination on large datasets
- AJAX for partial page updates
- Caching in session state

**Weaknesses:**
- No output caching
- No data caching
- Large datasets loaded into memory
- Inefficient dynamic SQL in stored procedures
- No connection pooling configuration

### Optimization Recommendations

1. **Database:**
   - Add indexes on frequently queried columns
   - Optimize stored procedures (remove dynamic SQL)
   - Implement query result caching
   - Use read-only replicas for reporting

2. **Application:**
   - Enable output caching for static content
   - Implement data caching for reference data
   - Use async/await for I/O operations (requires .NET upgrade)
   - Optimize ViewState usage

3. **Infrastructure:**
   - Configure connection pooling
   - Enable IIS compression
   - Implement CDN for static resources
   - Load balancing for high availability

---

## Scalability Considerations

### Current Scalability Limitations

- Session state stored in-process (not web farm compatible)
- No distributed caching
- Stateful session management
- Single database server

### Scalability Recommendations

1. **Horizontal Scaling:**
   - Move session state to SQL Server or Redis
   - Implement sticky sessions on load balancer
   - Ensure all servers have identical configuration

2. **Vertical Scaling:**
   - Increase server resources (CPU, RAM)
   - Optimize database server
   - Add read replicas

3. **Database Scaling:**
   - Implement database sharding (by customer)
   - Use read replicas for reporting
   - Archive old payment data

---

## Monitoring and Observability

### Current Monitoring

**Application Logging:**
- Trace statements (⚠️ should be removed)
- Error logging to database
- IIS logs

**Database Monitoring:**
- SQL Server logs
- Query performance monitoring

### Recommended Monitoring

1. **Application Performance Monitoring (APM):**
   - Implement Application Insights or similar
   - Track response times
   - Monitor error rates
   - Track user sessions

2. **Infrastructure Monitoring:**
   - Server health (CPU, memory, disk)
   - IIS metrics
   - Network latency

3. **Business Metrics:**
   - Payment success rate
   - Average payment amount
   - Payment processing time
   - US Bank integration availability

4. **Security Monitoring:**
   - Failed authentication attempts
   - SQL injection attempts
   - Unusual access patterns
   - Data access auditing

---

## Disaster Recovery

### Current Backup Strategy

- Database backups (managed by DBA team)
- Source code in version control
- No documented recovery procedures

### Recommended DR Strategy

1. **Backup:**
   - Daily full database backups
   - Transaction log backups every 15 minutes
   - Application file backups
   - Configuration backups

2. **Recovery:**
   - Document recovery procedures
   - Test recovery process quarterly
   - Define RTO (Recovery Time Objective): 4 hours
   - Define RPO (Recovery Point Objective): 15 minutes

3. **High Availability:**
   - SQL Server Always On Availability Groups
   - Multiple web servers
   - Load balancer with health checks
   - Automated failover

---

## Future Architecture Recommendations

### Short-term (3-6 months)

1. **Security Hardening:**
   - Fix SQL injection vulnerabilities
   - Implement input validation
   - Remove hardcoded credentials
   - Enable custom errors

2. **Code Quality:**
   - Add unit tests
   - Implement code reviews
   - Add static code analysis
   - Document APIs

### Medium-term (6-12 months)

1. **Framework Upgrade:**
   - Migrate to .NET Framework 4.8
   - Update third-party dependencies
   - Modernize UI components

2. **Architecture Improvements:**
   - Implement repository pattern
   - Add dependency injection
   - Separate concerns (business logic from data access)
   - Implement CQRS for reporting

### Long-term (12-24 months)

1. **Platform Modernization:**
   - Migrate to .NET 6/8
   - Migrate to ASP.NET Core
   - Implement microservices architecture
   - Move to cloud (Azure/AWS)

2. **Technology Upgrades:**
   - Replace Web Forms with modern SPA (React/Angular)
   - Implement RESTful APIs
   - Add real-time notifications (SignalR)
   - Implement event-driven architecture

---

## Conclusion

The Finance Credit Direct EPay system follows a traditional three-tier architecture appropriate for its time (.NET 3.5 era). However, it has significant technical debt and security vulnerabilities that need to be addressed.

**Critical Actions:**
1. Address security vulnerabilities (see SECURITY_AUDIT.md)
2. Implement proper input validation
3. Remove hardcoded credentials
4. Plan framework modernization

**Long-term Vision:**
- Modern cloud-native architecture
- Microservices-based design
- API-first approach
- Enhanced security and compliance

---

**Copyright © 2025 Ashley Furniture Industries, Inc. All rights reserved.**


