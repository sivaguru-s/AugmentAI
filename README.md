# Credit Shortage Validation System

## 📋 Overview

This repository contains the **Credit Shortage Validation Microservice** and associated database objects for validating shortage items before credit entry. The system integrates with IWS (Inventory Warehouse System) to obtain serial numbers and performs comprehensive validation checks to ensure data integrity and prevent duplicate or invalid credits.

## 🎯 Purpose

The Credit Shortage Validation System validates shortage items against multiple criteria:

1. ✅ **Item Validity** - Verify item exists in the system
2. ✅ **Customer/Serial/Item Combination** - Validate the combination is legitimate
3. ✅ **Duplicate Credit Prevention** - Check if shortage already credited
4. ✅ **Quantity Validation** - Ensure shortage doesn't exceed order quantity
5. ✅ **Defect Code Validation** - Validate defect code (default: XP)
6. ✅ **Location Code Validation** - Validate warehouse location (default: WU)

---

## 🏗️ Architecture

```
Credit Shortage Validation System
│
├── Database Layer
│   ├── Stored Procedure: usp_CE_ValidateShortageItems
│   ├── Table Type: typCEShortageItemValidation
│   └── Tables: tblRetAllowHeader, tblRetAllowDetail, etc.
│
├── Microservice API (Planned)
│   ├── REST API Endpoints
│   ├── IWS Integration Service
│   └── Validation Service
│
└── Integration Points
    ├── IWS - Serial Number Retrieval
    └── Credit Direct Entry System
```

---

## 📁 Repository Structure

```
Credit_Shortage/
├── Database/
│   ├── StoredProcedures/
│   │   ├── usp_CE_ValidateShortageItems.sql
│   │   └── usp_CE_ValidateShortageItems_TestScript.sql
│   └── Documentation/
│       └── usp_CE_ValidateShortageItems_Documentation.md
│
├── Microservice/ (To be created)
│   ├── API/
│   ├── Services/
│   ├── Models/
│   └── Tests/
│
└── README.md
```

---

## 🚀 Getting Started

### Prerequisites

- SQL Server 2016 or higher
- Access to Ashley database
- Access to Datawhse database
- Access to Archive database
- Access to Environment database

### Database Setup

1. **Create the User-Defined Table Type**
   ```sql
   -- This is included in the stored procedure script
   -- Run the usp_CE_ValidateShortageItems.sql script
   ```

2. **Deploy the Stored Procedure**
   ```sql
   -- Execute the script in your database
   USE [Ashley]
   GO
   
   -- Run: Database/StoredProcedures/usp_CE_ValidateShortageItems.sql
   ```

3. **Verify Installation**
   ```sql
   -- Check if type exists
   SELECT * FROM sys.types 
   WHERE is_table_type = 1 AND name = 'typCEShortageItemValidation'
   
   -- Check if procedure exists
   SELECT * FROM sys.procedures 
   WHERE name = 'usp_CE_ValidateShortageItems'
   ```

4. **Run Test Scripts**
   ```sql
   -- Execute: Database/StoredProcedures/usp_CE_ValidateShortageItems_TestScript.sql
   ```

---

## 💻 Usage

### Basic Usage Example

```sql
-- Declare table variable
DECLARE @ShortageItems AS dbo.typCEShortageItemValidation;

-- Insert shortage items to validate
INSERT INTO @ShortageItems 
VALUES (
    '8888300',      -- CustomerNumber
    '0001',         -- ShipToNumber
    110433,         -- InvoiceNumber
    'C310325',      -- ItemNumber
    '7320325',      -- SerialNumber (from IWS)
    1,              -- ShortageQuantity
    'XP',           -- DefectCode (or NULL for default)
    'WU',           -- LocationCode (or NULL for default)
    1234567,        -- OrderNumber
    100             -- OrderItemSeq
);

-- Execute validation
EXEC usp_CE_ValidateShortageItems @ShortageItems, 'AFI';
```

### Integration with IWS

```
1. Call IWS API to get order/shipment details
2. Extract Serial# from IWS response
3. Build ShortageItem object with serial#
4. Call usp_CE_ValidateShortageItems
5. Process validation results
6. If valid → Submit credit
   If invalid → Return errors to user
```

---

## 📊 Validation Details

### Input Parameters

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| CustomerNumber | VARCHAR(8) | Yes | Customer number |
| ShipToNumber | VARCHAR(4) | Yes | Ship-to location |
| InvoiceNumber | NUMERIC(6,0) | Yes | Invoice number |
| ItemNumber | VARCHAR(15) | Yes | Item/SKU number |
| SerialNumber | VARCHAR(10) | Yes | Serial# from IWS |
| ShortageQuantity | NUMERIC(7,0) | Yes | Shortage quantity |
| DefectCode | VARCHAR(4) | No | Default: 'XP' |
| LocationCode | VARCHAR(2) | No | Default: 'WU' |
| OrderNumber | NUMERIC(7,0) | No | Order number |
| OrderItemSeq | INT | No | Item sequence |

### Output Fields

| Field | Description |
|-------|-------------|
| IsValid | 1 = Pass, 0 = Fail |
| ValidationErrors | Detailed error messages |
| OrderedQuantity | Original order qty |
| AlreadyCreditedQuantity | Previously credited qty |
| RemainingCreditableQuantity | Remaining qty available for credit |
| ItemExists | Item validation flag |
| CustomerSerialItemValid | Combination validation flag |
| DefectCodeValid | Defect code validation flag |
| LocationCodeValid | Location validation flag |

---

## 🔍 Validation Logic

### 1. Item Validation
- Checks `Ashley.dbo.tblItemMaster`
- Ensures item exists and is active

### 2. Customer/Serial/Item Combination
- Checks `Datawhse.dbo.tblInvoiceDetail`
- Falls back to `Archive.dbo.tblInvoiceDetail`
- Validates the item was on the specified serial for the customer

### 3. Duplicate Credit Check
- Queries `Ashley.dbo.tblRetAllowHeader/Detail`
- Calculates already credited quantities
- Only counts approved credits (`rahAprvDny = 'A'`)

### 4. Quantity Validation
- Compares shortage qty vs remaining creditable qty
- Formula: `Remaining = Ordered - AlreadyCredited`
- Prevents over-crediting

### 5. Defect Code Validation
- Checks `Ashley.dbo.tblDefectCodes`
- Ensures code exists and is active
- Applies default 'XP' if not provided

### 6. Location Code Validation
- Checks `Ashley.dbo.tblWarehouse`
- Ensures warehouse exists and is active
- Applies default 'WU' if not provided

---

## 📖 Documentation

Detailed documentation available in:
- [Stored Procedure Documentation](Database/Documentation/usp_CE_ValidateShortageItems_Documentation.md)
- [Test Scripts](Database/StoredProcedures/usp_CE_ValidateShortageItems_TestScript.sql)

---

## 🧪 Testing

Run the comprehensive test suite:

```sql
-- Execute all test cases
-- See: Database/StoredProcedures/usp_CE_ValidateShortageItems_TestScript.sql

-- Tests include:
-- ✅ Valid shortage item
-- ❌ Invalid item number
-- ❌ Invalid customer/serial/item combination
-- ❌ Quantity exceeds order
-- ❌ Invalid defect code
-- ❌ Invalid location code
-- ✅ Default code application
-- ✅ Batch validation
-- ✅ Custom defaults
```

---

## 🔗 Related Systems

### Reference Repository
The validation logic is based on the existing **finance-credit-direct-entry** system:
- Repository: `https://github.com/afi-internal/finance-credit-direct-entry`
- Language: VB.NET
- Architecture: 3-tier (Data Layer, Business Logic, Service Layer)

### Key Stored Procedures Referenced
- `usp_CEImportSubmittedCredits` - Credit submission
- `usp_CEGetCreditItemQtys` - Existing credit check
- `usp_CEGetItemOrdQtys` - Order quantity lookup
- `usp_CEGetAddDefects` - Valid defect codes
- `usp_CEGetAddLocations` - Valid location codes

---

## 📈 Performance Considerations

### Recommended Indexes

```sql
-- tblInvoiceDetail (Datawhse & Archive)
CREATE INDEX IX_InvoiceDetail_Validation
ON dbo.tblInvoiceDetail (indCusno, indInvno, indSerno, indItnbr)
INCLUDE (indQtysh, indOrdno);

-- tblRetAllowDetail
CREATE INDEX IX_RetAllowDetail_CreditCheck
ON Ashley.dbo.tblRetAllowDetail (radInvoiceNo, radOrderNumber, radItemNo, radEnterDate, radEnterTime)
INCLUDE (radOrdQty, radPctAllow);

-- tblRetAllowHeader
CREATE INDEX IX_RetAllowHeader_Approval
ON Ashley.dbo.tblRetAllowHeader (rahCustNo, rahAprvDny, rahEnterDate, rahEnterTime);
```

### Optimization Tips
- ✅ Uses `WITH (NOLOCK)` for read-only operations
- ✅ Falls back to Archive only when needed
- ✅ Batch validation supported for efficiency
- ✅ Returns early on validation failures

---

## 🛠️ Troubleshooting

### Common Issues

#### Issue: "Type does not exist"
**Solution**: Run the CREATE TYPE statement first (included in stored procedure script)

#### Issue: "Invalid object name tblItemMaster"
**Solution**: Ensure you have access to the Ashley database and all referenced tables

#### Issue: "Cannot determine original order quantity"
**Solution**:
- Verify the order number and invoice number are correct
- Check if data exists in Archive database
- Ensure invoice was shipped (exists in tblInvoiceDetail)

#### Issue: "Defect code invalid"
**Solution**:
- Check `Ashley.dbo.tblDefectCodes` for valid codes
- Ensure code is marked as active (`defActive = 'Y'`)
- Use default 'XP' if unsure

#### Issue: "Already credited" warnings
**Solution**:
- Review existing credits in `tblRetAllowHeader/Detail`
- Check if credit was previously approved
- Verify remaining creditable quantity

---

## 🚦 Next Steps

### Phase 1: Database Layer ✅ COMPLETED
- [x] Create stored procedure `usp_CE_ValidateShortageItems`
- [x] Create user-defined table type
- [x] Write comprehensive documentation
- [x] Create test scripts

### Phase 2: Microservice Development (Planned)
- [ ] Create .NET Core Web API project
- [ ] Implement IWS integration service
- [ ] Create validation service wrapper
- [ ] Build REST API endpoints
- [ ] Add authentication/authorization
- [ ] Implement logging and monitoring

### Phase 3: Testing & Deployment (Planned)
- [ ] Unit tests
- [ ] Integration tests
- [ ] Performance testing
- [ ] User acceptance testing
- [ ] Production deployment

---

## 📞 Support & Contact

For questions or issues:
- Create an issue in this repository
- Contact the Credit Development Team
- Email: SSampanthamoorthy@ashleyfurnitureindia.com

---

## 📝 License

Internal use only - Ashley Furniture Industries, Inc.

---

## 🙏 Acknowledgments

- Based on the existing `finance-credit-direct-entry` system
- Credit Direct Entry team for domain knowledge
- IWS team for serial number integration support

---

## 📚 Additional Resources

### Documentation
- [Stored Procedure Full Documentation](Database/Documentation/usp_CE_ValidateShortageItems_Documentation.md)
- [Test Script Examples](Database/StoredProcedures/usp_CE_ValidateShortageItems_TestScript.sql)

### Related Documentation
- IWS API Documentation
- Credit Entry User Guide
- Database Schema Documentation

---

## 🔄 Version History

| Version | Date | Author | Description |
|---------|------|--------|-------------|
| 1.0.0 | 2026-05-20 | Credit Team | Initial release with stored procedure |

---

## ⚡ Quick Start Checklist

- [ ] Clone repository
- [ ] Run `usp_CE_ValidateShortageItems.sql` script
- [ ] Verify installation (check sys.types and sys.procedures)
- [ ] Run test script to validate setup
- [ ] Review documentation
- [ ] Update default codes if needed (XP, WU)
- [ ] Test with real data
- [ ] Ready for integration!

