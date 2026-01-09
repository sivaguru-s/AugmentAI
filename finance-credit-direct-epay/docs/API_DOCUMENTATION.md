# API Documentation - Finance Credit Direct EPay

**Version:** 1.0  
**Last Updated:** January 9, 2026  

---

## Table of Contents

- [Overview](#overview)
- [Business Logic API](#business-logic-api)
- [Stored Procedures](#stored-procedures)
- [Data Contracts](#data-contracts)
- [US Bank Integration](#us-bank-integration)

---

## Overview

This document describes the internal APIs, stored procedures, and data contracts used in the Finance Credit Direct EPay system.

**Note:** This is an internal application with no public REST/SOAP APIs. All interactions are through the web interface.

---

## Business Logic API

### Common Class Methods

The `Common` class (`Classes/Common.vb`) provides the core business logic API.

#### GetTotals

**Purpose:** Calculate the total payment amount for a reference number

**Signature:**
```vb
Public Shared Function GetTotals(
    ByVal custNum As String, 
    ByVal referenceNumber As Integer
) As SqlDataReader
```

**Parameters:**
- `custNum` (String) - Customer number (7-8 digits)
- `referenceNumber` (Integer) - EPay reference number

**Returns:** SqlDataReader with total amount

**Result Set:**
| Column | Type | Description |
|--------|------|-------------|
| `Total` | money | Total payment amount |

**Example Usage:**
```vb
Dim reader As SqlDataReader = Common.GetTotals("1011000", 12345)
If reader.Read() Then
    Dim total As Decimal = CDec(reader("Total"))
End If
```

**⚠️ Security Issue:** SQL injection vulnerability (string concatenation)

---

#### UpdateEpayStatus

**Purpose:** Update the status of an EPay payment

**Signature:**
```vb
Public Shared Function UpdateEpayStatus(
    ByVal custNum As String, 
    ByVal referenceNumber As Integer, 
    ByVal status As String, 
    ByVal sUserChanged As String
) As Boolean
```

**Parameters:**
- `custNum` (String) - Customer number
- `referenceNumber` (Integer) - EPay reference number
- `status` (String) - New status ("Sent", "Verifying", "Confirmed")
- `sUserChanged` (String) - User making the change

**Returns:** Boolean - True if successful

**Valid Status Values:**
- `"Sent"` - Payment submitted to US Bank
- `"Verifying"` - Payment being verified
- `"Confirmed"` - Payment confirmed

**Example Usage:**
```vb
Dim success As Boolean = Common.UpdateEpayStatus("1011000", 12345, "Sent", "jdoe")
```

**⚠️ Security Issue:** SQL injection vulnerability

---

#### InsertEpayRecords

**Purpose:** Create EPay payment records for selected invoices

**Signature:**
```vb
Public Shared Function InsertEpayRecords(
    ByVal custNum As String, 
    ByVal shipToNum As String, 
    ByVal invoiceList As String, 
    ByVal sUserAdded As String
) As Integer
```

**Parameters:**
- `custNum` (String) - Customer number
- `shipToNum` (String) - Ship-to number
- `invoiceList` (String) - Comma-separated list of invoice numbers
- `sUserAdded` (String) - User creating the payment

**Returns:** Integer - Reference number for the payment batch

**Example Usage:**
```vb
Dim refNo As Integer = Common.InsertEpayRecords("1011000", "0001", "123456,123457,123458", "jdoe")
```

**Process:**
1. Generates new reference number
2. Splits invoice list
3. Inserts record for each invoice
4. Returns reference number

**⚠️ Security Issue:** SQL injection vulnerability

---

#### DeleteEpayRecords

**Purpose:** Delete/cancel an EPay payment batch

**Signature:**
```vb
Public Shared Function DeleteEpayRecords(
    ByVal custNum As String, 
    ByVal referenceNumber As Integer, 
    ByVal sUserChanged As String
) As Boolean
```

**Parameters:**
- `custNum` (String) - Customer number
- `referenceNumber` (Integer) - EPay reference number
- `sUserChanged` (String) - User deleting the payment

**Returns:** Boolean - True if successful

**Example Usage:**
```vb
Dim success As Boolean = Common.DeleteEpayRecords("1011000", 12345, "jdoe")
```

**⚠️ Security Issue:** SQL injection vulnerability

---

#### DeleteEpayUnconfirmedPayment

**Purpose:** Delete a specific unconfirmed payment invoice

**Signature:**
```vb
Public Shared Function DeleteEpayUnconfirmedPayment(
    ByVal custNum As String, 
    ByVal invoiceNumber As String, 
    ByVal sUserChanged As String
) As Boolean
```

**Parameters:**
- `custNum` (String) - Customer number
- `invoiceNumber` (String) - Invoice number to delete
- `sUserChanged` (String) - User deleting the payment

**Returns:** Boolean - True if successful

**Example Usage:**
```vb
Dim success As Boolean = Common.DeleteEpayUnconfirmedPayment("1011000", "123456", "jdoe")
```

**⚠️ Security Issue:** SQL injection vulnerability

---

#### LoadInvoices

**Purpose:** Retrieve open invoices for a customer with filtering and pagination

**Signature:**
```vb
Public Shared Function LoadInvoices(
    ByVal sCustomerNumber As String,
    ByVal sShipToNumber As String,
    ByVal bAllShipTos As Boolean,
    ByVal sSecurityMHS As String,
    ByVal dtFromDate As DateTime,
    ByVal dtToDate As DateTime,
    ByVal bShowCredits As Boolean,
    ByVal bShowOldCredits As Boolean,
    ByVal sInvoiceNumber As String,
    ByVal sCreditNumber As String,
    ByVal sPONumber As String,
    ByVal sSortColumn As String,
    ByVal bSortAscending As Boolean,
    ByVal iPageNumber As Integer,
    ByVal iPageSize As Integer
) As SqlDataReader
```

**Parameters:**
- `sCustomerNumber` (String) - Customer number
- `sShipToNumber` (String) - Ship-to number (empty if all)
- `bAllShipTos` (Boolean) - Include all ship-tos
- `sSecurityMHS` (String) - Security MHS name
- `dtFromDate` (DateTime) - Start date for invoice date range
- `dtToDate` (DateTime) - End date for invoice date range
- `bShowCredits` (Boolean) - Include credit memos
- `bShowOldCredits` (Boolean) - Include credits older than 1 year
- `sInvoiceNumber` (String) - Filter by invoice number (partial match)
- `sCreditNumber` (String) - Filter by credit number (partial match)
- `sPONumber` (String) - Filter by PO number (partial match)
- `sSortColumn` (String) - Column to sort by
- `bSortAscending` (Boolean) - Sort direction
- `iPageNumber` (Integer) - Page number (1-based)
- `iPageSize` (Integer) - Records per page (0 = all)

**Returns:** SqlDataReader with invoice data

**Result Set:**
| Column | Type | Description |
|--------|------|-------------|
| `opicusno` | varchar(8) | Customer number |
| `opishpno` | varchar(4) | Ship-to number |
| `opiinvno` | varchar(9) | Invoice number |
| `opicrmnr` | varchar(6) | Credit memo number |
| `opiDagedt` | varchar(10) | Invoice date (MM/DD/YYYY) |
| `opiInvam` | money | Invoice amount |
| `opiTtlcr` | money | Total credits |
| `opiOpamt` | money | Open amount (balance) |
| `opicatcd` | varchar(2) | Category code |
| `opiponum` | varchar(22) | PO number |
| `opiOrdno` | varchar(7) | Order number |
| `inhdordda` | varchar(10) | Order date (MM/DD/YYYY) |
| `RPP #` | int | RPP number |
| `Days` | int | Days since invoice date |
| `inhTripNo` | int | Trip number |
| `PageCount` | int | Total pages |

**Example Usage:**
```vb
Dim reader As SqlDataReader = Common.LoadInvoices(
    "1011000", "", True, "MASTERXX",
    DateTime.Parse("01/01/2024"), DateTime.Parse("12/31/2024"),
    True, False, "", "", "",
    "opidagedt", True, 1, 50
)
```

**⚠️ Security Issue:** SQL injection vulnerability in stored procedure

---

#### LoadEpayInvoicesByRefNumber

**Purpose:** Retrieve invoices for a specific EPay reference number

**Signature:**
```vb
Public Shared Function LoadEpayInvoicesByRefNumber(
    ByVal custNum As String,
    ByVal referenceNumber As Integer
) As SqlDataReader
```

**Parameters:**
- `custNum` (String) - Customer number
- `referenceNumber` (Integer) - EPay reference number

**Returns:** SqlDataReader with invoice details

**Result Set:**
| Column | Type | Description |
|--------|------|-------------|
| `epaCusNo` | varchar(8) | Customer number |
| `epaShpNo` | varchar(4) | Ship-to number |
| `epaRefNo` | int | Reference number |
| `epaInvNo` | varchar(9) | Invoice number |
| `epaInvAm` | money | Invoice amount |
| `epaStatus` | varchar(20) | Payment status |
| `epaConfirmationNo` | varchar(50) | US Bank confirmation |
| `epaDateAdded` | datetime | Created date |

**Example Usage:**
```vb
Dim reader As SqlDataReader = Common.LoadEpayInvoicesByRefNumber("1011000", 12345)
```

---

#### LoadEpayAnalystReport

**Purpose:** Retrieve EPay payment data for analyst reporting

**Signature:**
```vb
Public Shared Function LoadEpayAnalystReport(
    ByVal customerNumber As String,
    ByVal referenceNumber As String,
    ByVal ConfirmationNo As String,
    ByVal paymentAdded As String,
    ByVal CreditTerritory As String,
    ByVal sortBy As String,
    ByVal sortAscending As Boolean
) As SqlDataReader
```

**Parameters:**
- `customerNumber` (String) - Filter by customer number (empty = all)
- `referenceNumber` (String) - Filter by reference number (empty = all)
- `ConfirmationNo` (String) - Filter by confirmation number (empty = all)
- `paymentAdded` (String) - Filter by payment date (MM/DD/YYYY)
- `CreditTerritory` (String) - Filter by credit territory (empty = all)
- `sortBy` (String) - Column to sort by
- `sortAscending` (Boolean) - Sort direction

**Returns:** SqlDataReader with payment data

**Result Set:**
| Column | Type | Description |
|--------|------|-------------|
| `epaCusNo` | varchar(8) | Customer number |
| `epaShpNo` | varchar(4) | Ship-to number |
| `epaRefNo` | int | Reference number |
| `epaInvNo` | varchar(9) | Invoice number |
| `epaInvAm` | money | Invoice amount |
| `epaStatus` | varchar(20) | Payment status |
| `epaConfirmationNo` | varchar(50) | Confirmation number |
| `epaDateAdded` | datetime | Payment date |
| `epaUserAdded` | varchar(50) | User who created payment |
| `CustomerName` | varchar(100) | Customer name |
| `CreditTerritory` | varchar(50) | Credit territory |

**Example Usage:**
```vb
Dim reader As SqlDataReader = Common.LoadEpayAnalystReport(
    "", "", "", "01/01/2024", "EAST", "epaDateAdded", True
)
```

---

#### LoadUnconfirmedPayments

**Purpose:** Retrieve unconfirmed EPay payments for administrative review

**Signature:**
```vb
Public Shared Function LoadUnconfirmedPayments(
    ByVal sCustomerNumber As String,
    ByVal sShipToNumber As String,
    ByVal sSecurityMHS As String,
    ByVal sCreditTerritory As String,
    ByVal sSortColumn As String,
    ByVal bSortAscending As Boolean
) As SqlDataReader
```

**Parameters:**
- `sCustomerNumber` (String) - Filter by customer number (empty = all)
- `sShipToNumber` (String) - Filter by ship-to number (empty = all)
- `sSecurityMHS` (String) - Security MHS name
- `sCreditTerritory` (String) - Filter by credit territory (empty = all)
- `sSortColumn` (String) - Column to sort by
- `bSortAscending` (Boolean) - Sort direction

**Returns:** SqlDataReader with unconfirmed payments

**Result Set:**
| Column | Type | Description |
|--------|------|-------------|
| `epaCusNo` | varchar(8) | Customer number |
| `epaShpNo` | varchar(4) | Ship-to number |
| `epaRefNo` | int | Reference number |
| `epaInvNo` | varchar(9) | Invoice number |
| `epaInvAm` | money | Invoice amount |
| `epaStatus` | varchar(20) | Payment status |
| `epaDateAdded` | datetime | Created date |
| `epaUserAdded` | varchar(50) | User who created payment |

---

#### LoadEpayUsers

**Purpose:** Retrieve list of EPay users with filtering

**Signature:**
```vb
Public Shared Function LoadEpayUsers(
    ByVal sCustomerNumber As String,
    ByVal sCustomerName As String,
    ByVal sBillToState As String,
    ByVal sCreditTerritory As String,
    ByVal sTermsCode As String,
    ByVal sSortColumn As String,
    ByVal bSortAscending As Boolean,
    ByVal iPageNumber As Integer,
    ByVal iPageSize As Integer
) As SqlDataReader
```

**Parameters:**
- `sCustomerNumber` (String) - Filter by customer number (partial match)
- `sCustomerName` (String) - Filter by customer name (partial match)
- `sBillToState` (String) - Filter by state (empty = all)
- `sCreditTerritory` (String) - Filter by credit territory (empty = all)
- `sTermsCode` (String) - Filter by terms code (empty = all)
- `sSortColumn` (String) - Column to sort by
- `bSortAscending` (Boolean) - Sort direction
- `iPageNumber` (Integer) - Page number (1-based)
- `iPageSize` (Integer) - Records per page (0 = all)

**Returns:** SqlDataReader with user data

**Result Set:**
| Column | Type | Description |
|--------|------|-------------|
| `CustomerNumber` | varchar(8) | Customer number |
| `CustomerName` | varchar(100) | Customer name |
| `BillToState` | varchar(2) | State code |
| `CreditTerritory` | varchar(50) | Credit territory |
| `TermsCode` | varchar(10) | Payment terms code |
| `ContactEmail` | varchar(100) | Contact email |
| `PageCount` | int | Total pages |

---

#### GetEpayStatus

**Purpose:** Get the current status of an EPay payment

**Signature:**
```vb
Public Shared Function GetEpayStatus(
    ByVal custNum As String,
    ByVal referenceNumber As Integer
) As String
```

**Parameters:**
- `custNum` (String) - Customer number
- `referenceNumber` (Integer) - EPay reference number

**Returns:** String - Payment status ("Sent", "Verifying", "Confirmed", or empty)

**Example Usage:**
```vb
Dim status As String = Common.GetEpayStatus("1011000", 12345)
If status = "Sent" Then
    ' Payment already submitted
End If
```

---

#### GetPaymentContactEmailAddress

**Purpose:** Retrieve the contact email address for payment notifications

**Signature:**
```vb
Private Function GetPaymentContactEmailAddress(
    ByVal sCustomerNumber As String
) As String
```

**Parameters:**
- `sCustomerNumber` (String) - Customer number

**Returns:** String - Contact email address

**Example Usage:**
```vb
Dim email As String = Me.GetPaymentContactEmailAddress("1011000")
```

---

## Stored Procedures

### SQL Server Stored Procedures

#### usp_GetEpayTotal

**Database:** Datawhse

**Purpose:** Calculate total payment amount for a reference number

**Parameters:**
```sql
@CustomerNumber varchar(8)
@ReferenceNumber int
```

**Returns:** Single row with total amount

**Result Set:**
| Column | Type | Description |
|--------|------|-------------|
| `Total` | money | Sum of all invoice amounts |

**Example:**
```sql
EXEC Datawhse.dbo.usp_GetEpayTotal '1011000', 12345
```

---

#### usp_UpdateEpayStatus

**Database:** Datawhse

**Purpose:** Update payment status

**Parameters:**
```sql
@CustomerNumber varchar(8)
@ReferenceNumber int
@Status varchar(20)
@UserChanged varchar(50)
```

**Returns:** No result set (UPDATE statement)

**Example:**
```sql
EXEC Datawhse.dbo.usp_UpdateEpayStatus '1011000', 12345, 'Sent', 'jdoe'
```

---

#### usp_InsertEpay

**Database:** Datawhse

**Purpose:** Insert a new EPay payment record

**Parameters:**
```sql
@CustomerNumber varchar(8)
@ShipToNumber varchar(4)
@ReferenceNumber int
@InvoiceNumber varchar(9)
@InvoiceAmount money
@UserAdded varchar(50)
```

**Returns:** No result set (INSERT statement)

**Example:**
```sql
EXEC Datawhse.dbo.usp_InsertEpay '1011000', '0001', 12345, '123456', 1234.56, 'jdoe'
```

---

#### usp_DeleteEpay

**Database:** Datawhse

**Purpose:** Delete EPay payment records

**Parameters:**
```sql
@CustomerNumber varchar(8)
@ReferenceNumber int
@UserChanged varchar(50)
```

**Returns:** No result set (DELETE statement)

**Example:**
```sql
EXEC Datawhse.dbo.usp_DeleteEpay '1011000', 12345, 'jdoe'
```

---

#### usp_DeleteEpayUnconfirmedPayment

**Database:** Datawhse

**Purpose:** Delete a specific unconfirmed payment

**Parameters:**
```sql
@CustomerNumber varchar(8)
@InvoiceNumber varchar(9)
@UserChanged varchar(50)
```

**Returns:** No result set (DELETE statement)

**Example:**
```sql
EXEC Datawhse.dbo.usp_DeleteEpayUnconfirmedPayment '1011000', '123456', 'jdoe'
```

---

#### usp_GetEpayInvoicesByRefNumber

**Database:** Datawhse

**Purpose:** Retrieve all invoices for a specific reference number

**Parameters:**
```sql
@CustomerNumber varchar(8)
@ReferenceNumber int
```

**Returns:** Multiple rows with invoice details

**Result Set:**
| Column | Type | Description |
|--------|------|-------------|
| `epaCusNo` | varchar(8) | Customer number |
| `epaShpNo` | varchar(4) | Ship-to number |
| `epaRefNo` | int | Reference number |
| `epaInvNo` | varchar(9) | Invoice number |
| `epaInvAm` | money | Invoice amount |
| `epaStatus` | varchar(20) | Payment status |
| `epaConfirmationNo` | varchar(50) | Confirmation number |
| `epaDateAdded` | datetime | Created date |

**Example:**
```sql
EXEC Datawhse.dbo.usp_GetEpayInvoicesByRefNumber '1011000', 12345
```

---

#### usp_GetEpayAnalystReport

**Database:** Datawhse

**Purpose:** Retrieve EPay payment data for analyst reporting

**Parameters:**
```sql
@CustomerNumber varchar(8)
@ReferenceNumber varchar(10)
@ConfirmationNo varchar(50)
@PaymentAdded varchar(10)
@CreditTerritory varchar(50)
@SortBy varchar(50)
@SortAscending bit
```

**Returns:** Multiple rows with payment and customer data

**Result Set:**
| Column | Type | Description |
|--------|------|-------------|
| `epaCusNo` | varchar(8) | Customer number |
| `epaShpNo` | varchar(4) | Ship-to number |
| `epaRefNo` | int | Reference number |
| `epaInvNo` | varchar(9) | Invoice number |
| `epaInvAm` | money | Invoice amount |
| `epaStatus` | varchar(20) | Payment status |
| `epaConfirmationNo` | varchar(50) | Confirmation number |
| `epaDateAdded` | datetime | Payment date |
| `epaUserAdded` | varchar(50) | User who created |
| `CustomerName` | varchar(100) | Customer name |
| `CreditTerritory` | varchar(50) | Credit territory |

**Example:**
```sql
EXEC Datawhse.dbo.usp_GetEpayAnalystReport '', '', '', '01/01/2024', 'EAST', 'epaDateAdded', 1
```

---

#### usp_GetEpayUnconfirmedPayments

**Database:** Datawhse

**Purpose:** Retrieve unconfirmed EPay payments

**Parameters:**
```sql
@CustomerNumber varchar(8)
@ShipToNumber varchar(4)
@SecurityMHS varchar(8)
@CreditTerritory varchar(50)
@SortColumn varchar(50)
@SortAscending bit
```

**Returns:** Multiple rows with unconfirmed payment data

**Result Set:**
| Column | Type | Description |
|--------|------|-------------|
| `epaCusNo` | varchar(8) | Customer number |
| `epaShpNo` | varchar(4) | Ship-to number |
| `epaRefNo` | int | Reference number |
| `epaInvNo` | varchar(9) | Invoice number |
| `epaInvAm` | money | Invoice amount |
| `epaStatus` | varchar(20) | Payment status |
| `epaDateAdded` | datetime | Created date |
| `epaUserAdded` | varchar(50) | User who created |

**Example:**
```sql
EXEC Datawhse.dbo.usp_GetEpayUnconfirmedPayments '', '', 'MASTERXX', '', 'epaDateAdded', 1
```

---

#### usp_GetEpayUsers

**Database:** Datawhse

**Purpose:** Retrieve list of EPay users

**Parameters:**
```sql
@CustomerNumber varchar(8)
@CustomerName varchar(100)
@BillToState varchar(2)
@CreditTerritory varchar(50)
@TermsCode varchar(10)
@SortColumn varchar(50)
@SortAscending bit
@PageNumber int
@PageSize int
```

**Returns:** Multiple rows with user data

**Result Set:**
| Column | Type | Description |
|--------|------|-------------|
| `CustomerNumber` | varchar(8) | Customer number |
| `CustomerName` | varchar(100) | Customer name |
| `BillToState` | varchar(2) | State code |
| `CreditTerritory` | varchar(50) | Credit territory |
| `TermsCode` | varchar(10) | Payment terms |
| `ContactEmail` | varchar(100) | Contact email |
| `PageCount` | int | Total pages |

**Example:**
```sql
EXEC Datawhse.dbo.usp_GetEpayUsers '', '', 'CA', '', '', 'CustomerName', 1, 1, 50
```

---

#### usp_GetEpayStatus

**Database:** Datawhse

**Purpose:** Get payment status for a reference number

**Parameters:**
```sql
@CustomerNumber varchar(8)
@ReferenceNumber int
```

**Returns:** Single row with status

**Result Set:**
| Column | Type | Description |
|--------|------|-------------|
| `epaStatus` | varchar(20) | Payment status |

**Example:**
```sql
EXEC Datawhse.dbo.usp_GetEpayStatus '1011000', 12345
```

---

#### usp_OrderAndInvoiceReportingOpenInvoices2

**Database:** Datawhse

**Purpose:** Retrieve open invoices with filtering, sorting, and pagination

**⚠️ Security Issue:** Uses dynamic SQL with string concatenation (SQL injection vulnerability)

**Parameters:**
```sql
@customerNumber varchar(8)
@shiptoNumber varchar(4)
@allShiptos bit
@securityMHS varchar(8)
@FromDate datetime
@ToDate datetime
@showCredits bit
@showOldCredits bit
@searchInvoice varchar(9)
@searchCredit varchar(6)
@searchPo varchar(22)
@sortBy varchar(20)
@sortAscending varchar(5)
@pageNum int
@pageSize int
```

**Returns:** Multiple rows with invoice data

**Result Set:**
| Column | Type | Description |
|--------|------|-------------|
| `opicusno` | varchar(8) | Customer number |
| `opishpno` | varchar(4) | Ship-to number |
| `opiinvno` | varchar(9) | Invoice number |
| `opicrmnr` | varchar(6) | Credit memo number |
| `opiDagedt` | varchar(10) | Invoice date (MM/DD/YYYY) |
| `opiInvam` | money | Invoice amount |
| `opiTtlcr` | money | Total credits |
| `opiOpamt` | money | Open amount |
| `opicatcd` | varchar(2) | Category code |
| `opiponum` | varchar(22) | PO number |
| `opiOrdno` | varchar(7) | Order number |
| `inhdordda` | varchar(10) | Order date (MM/DD/YYYY) |
| `RPP #` | int | RPP number |
| `Days` | int | Days since invoice |
| `inhTripNo` | int | Trip number |
| `PageCount` | int | Total pages |

**Example:**
```sql
EXEC Datawhse.dbo.usp_OrderAndInvoiceReportingOpenInvoices2
    '1011000', '', 1, 'MASTERXX', '2024-12-31', '2024-01-01',
    1, 0, '', '', '', 'opidagedt', 'ASC', 1, 50
```

**Note:** Date parameters are reversed (FromDate is newer, ToDate is older)

---

#### usp_GetPaymentContactEmailAddress

**Database:** Ashley

**Purpose:** Retrieve contact email address for payment notifications

**Parameters:**
```sql
@CustomerNumber varchar(8)
```

**Returns:** Single row with email address

**Result Set:**
| Column | Type | Description |
|--------|------|-------------|
| `ContactEmail` | varchar(100) | Contact email |

**Example:**
```sql
EXEC Ashley.dbo.usp_GetPaymentContactEmailAddress '1011000'
```

---

## Data Contracts

### EPay Payment Record

**Table:** `Datawhse.dbo.tblEpay`

**Structure:**
```sql
CREATE TABLE tblEpay (
    epaCusNo varchar(8) NOT NULL,
    epaShpNo varchar(4) NOT NULL,
    epaRefNo int NOT NULL PRIMARY KEY,
    epaInvNo varchar(9) NOT NULL,
    epaInvAm money NOT NULL,
    epaStatus varchar(20) NULL,
    epaConfirmationNo varchar(50) NULL,
    epaDateAdded datetime NOT NULL,
    epaUserAdded varchar(50) NOT NULL,
    epaDateChanged datetime NULL,
    epaUserChanged varchar(50) NULL
)
```

**Field Descriptions:**
- `epaCusNo` - Customer number (7-8 digits)
- `epaShpNo` - Ship-to number (4 digits)
- `epaRefNo` - Unique reference number for payment batch
- `epaInvNo` - Invoice or credit memo number
- `epaInvAm` - Invoice amount (positive for invoices, negative for credits)
- `epaStatus` - Payment status: "Sent", "Verifying", "Confirmed"
- `epaConfirmationNo` - US Bank confirmation number
- `epaDateAdded` - Record creation timestamp
- `epaUserAdded` - User who created the record
- `epaDateChanged` - Last modification timestamp
- `epaUserChanged` - User who last modified the record

**Business Rules:**
- Reference number is unique across all customers
- Multiple invoices can share the same reference number (batch payment)
- Status must be one of: "Sent", "Verifying", "Confirmed"
- Invoice amount can be negative for credit memos

---

### Open Invoice Record

**Table:** `Datawhse.dbo.tblOpenInvoices`

**Key Fields:**
- `opiCusNo` - Customer number
- `opiShpNo` - Ship-to number
- `opiInvNo` - Invoice number
- `opiCrmNr` - Credit memo number
- `opiDageDt` - Invoice date
- `opiInvAm` - Invoice amount
- `opiTtlCr` - Total credits applied
- `opiOpAmt` - Open amount (balance due)
- `opiCatCd` - Aging category code
- `opiPoNum` - Customer PO number
- `opiOrdNo` - Order number

**Aging Category Codes:**
- `00` - Current (0-30 days)
- `01` - 31-60 days
- `02` - 61-90 days
- `03` - 91-120 days
- `04` - Over 120 days

---

### Customer Security Record

**Table:** `Ashley.dbo.tblSecurityCustomer`

**Key Fields:**
- `secCusNo` - Customer number
- `secShpNo` - Ship-to number
- `secMhs_name` - Security MHS (group) name

**Purpose:** Controls which customers/ship-tos a user can access

---

## US Bank Integration

### EPay Gateway API

**Endpoint:** `https://epayment.epymtservice.com/epay.jhtml`

**Method:** HTTP GET (redirect)

**Authentication:** None (customer data passed in URL)

**Request Parameters:**

| Parameter | Required | Type | Description | Example |
|-----------|----------|------|-------------|---------|
| `productCode` | Yes | String | Product identifier | "OpenInvoices" |
| `billerId` | Yes | String | Biller ID | "INV" |
| `billerGroupId` | Yes | String | Biller group | "ASH" |
| `disallowLogin` | Yes | String | Disable login | "N" |
| `paymentMethod` | Yes | String | Payment method | "ACH" |
| `paymentType` | Yes | String | Payment type | "Single" |
| `amountDue` | Yes | Decimal | Payment amount | "1234.56" |
| `billerPayorId` | Yes | String | Customer number | "1011000" |
| `ReferenceNumber` | Yes | Integer | EPay reference | "12345" |
| `streetAddress1` | No | String | Address line 1 | URL encoded |
| `streetAddress2` | No | String | Address line 2 | URL encoded |
| `city` | No | String | City | URL encoded |
| `stateRegion` | No | String | State | URL encoded |
| `companyName` | No | String | Company name | URL encoded |
| `zipPostalcode` | No | String | ZIP (5 digits) | "12345" |

**Example Request:**
```
https://epayment.epymtservice.com/epay.jhtml?productCode=OpenInvoices&billerId=INV&billerGroupId=ASH&disallowLogin=N&paymentMethod=ACH&paymentType=Single&amountDue=1234.56&billerPayorId=1011000&ReferenceNumber=12345&streetAddress1=123+Main+St&city=Arcadia&stateRegion=WI&companyName=Ashley+Furniture&zipPostalcode=54612
```

**Response:**
- US Bank displays payment form
- Customer enters bank account information
- US Bank processes ACH payment
- US Bank redirects back to application (callback URL not documented)

**Security Considerations:**
- ⚠️ Customer data exposed in URL
- ⚠️ No authentication token
- ⚠️ No signature/HMAC validation
- ✅ HTTPS encryption

**Status Flow:**
1. Application sets status to "Sent" before redirect
2. Customer completes payment at US Bank
3. US Bank provides confirmation number
4. Status manually updated to "Confirmed" (no automatic callback)

---

## Error Handling

### Common Error Codes

**Database Errors:**
- SQL timeout - Query execution exceeded timeout
- Connection failure - Unable to connect to database
- Constraint violation - Data integrity error

**Business Logic Errors:**
- "No Invoices to send!" - No invoices selected or all already in EPay
- "Can not call USBank from Test!!" - Attempted US Bank call from non-production
- "Are you sure you want to cancel payment?" - Confirmation prompt

### Error Logging

**Method:** `Ashley2.Common.Auditing.usp_InsertAuditRecord`

**Logged Information:**
- Error message
- Stack trace
- User identity
- Timestamp
- Application name

---

## Versioning

**Current Version:** 1.0

**Version History:**
- 1.0 (2012-03-28) - Initial release after consolidation
- 1.1 (2012-06-22) - Added payment contact email
- 1.2 (2012-07-23) - Changed disallowLogin parameter
- 1.3 (2013-05-20) - Added custom SQL paging
- 1.4 (2014-05-19) - Modified SQL paging

**Breaking Changes:** None documented

**Deprecations:** None documented

---

## Best Practices

### Calling Stored Procedures

**❌ Bad (Current Implementation):**
```vb
Dim sql As String = "EXEC usp_GetData '" & custNum & "'"
Dim reader As SqlDataReader = DataAccess.GetDataReader("AFI_Batch", sql)
```

**✅ Good (Recommended):**
```vb
Dim parameters As New Collection
parameters.Add(New SqlParameter("@CustomerNumber", custNum))
Dim reader As SqlDataReader = DataAccess.ExecuteStoredProcedure(
    "AFI_Batch", "usp_GetData",
    DataAccess.StoredProcedureReturnType.DataReader,
    parameters
)
```

### Input Validation

**❌ Bad:**
```vb
Dim custNum As String = Request.Params("CusNo")
' Use directly without validation
```

**✅ Good:**
```vb
Dim custNum As String = Request.Params("CusNo")
If Not Regex.IsMatch(custNum, "^[0-9]{7,8}$") Then
    Throw New ArgumentException("Invalid customer number")
End If
```

### Error Handling

**❌ Bad:**
```vb
Try
    ' Code
Catch ex As Exception
    Throw
End Try
```

**✅ Good:**
```vb
Try
    ' Code
Catch ex As Exception
    LogError("Method failed", ex)
    Throw New ApplicationException("User-friendly message", ex)
End Try
```

---

## Appendix

### Common Query Patterns

**Get all invoices for a customer:**
```sql
EXEC Datawhse.dbo.usp_OrderAndInvoiceReportingOpenInvoices2
    @customerNumber = '1011000',
    @shiptoNumber = '',
    @allShiptos = 1,
    @securityMHS = 'MASTERXX',
    @FromDate = '2024-12-31',
    @ToDate = '2024-01-01',
    @showCredits = 1,
    @showOldCredits = 0,
    @searchInvoice = '',
    @searchCredit = '',
    @searchPo = '',
    @sortBy = 'opidagedt',
    @sortAscending = 'ASC',
    @pageNum = 1,
    @pageSize = 50
```

**Create a payment batch:**
```vb
Dim refNo As Integer = Common.InsertEpayRecords("1011000", "0001", "123456,123457", "jdoe")
```

**Submit payment to US Bank:**
```vb
Common.UpdateEpayStatus("1011000", refNo, "Sent", "jdoe")
Response.Redirect("https://epayment.epymtservice.com/epay.jhtml?...")
```

---

**Copyright © 2025 Ashley Furniture Industries, Inc. All rights reserved.**


