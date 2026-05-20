# Stored Procedure: usp_CE_ValidateShortageItems

## Overview
Comprehensive validation stored procedure for credit shortage items. This procedure validates shortage credits before they are submitted to ensure data integrity and prevent duplicate or invalid credits.

## Database
- **Database**: Ashley
- **Schema**: dbo
- **Type**: Stored Procedure
- **Created**: 2026-05-20

## Purpose
Performs comprehensive validation checks on shortage items including:
1. ✅ Item validity
2. ✅ Customer/Serial/Item combination validation
3. ✅ Duplicate credit prevention
4. ✅ Quantity validation against order quantity
5. ✅ Defect code validation (default: XP)
6. ✅ Location code validation (default: WU from config)

---

## Parameters

| Parameter | Type | Required | Default | Description |
|-----------|------|----------|---------|-------------|
| `@ShortageItems` | `typCEShortageItemValidation` | Yes | - | Table of shortage items to validate |
| `@Environment` | `VARCHAR(3)` | No | Auto-detected | Environment code (AFI, WVF, etc.) |
| `@DefaultDefectCode` | `VARCHAR(4)` | No | 'XP' | Default defect code if not provided |
| `@DefaultLocationCode` | `VARCHAR(2)` | No | 'WU' | Default location code if not provided |

---

## Input Table Type: typCEShortageItemValidation

```sql
CREATE TYPE [dbo].[typCEShortageItemValidation] AS TABLE(
    [CustomerNumber] VARCHAR(8) NOT NULL,
    [ShipToNumber] VARCHAR(4) NOT NULL,
    [InvoiceNumber] NUMERIC(6,0) NOT NULL,
    [ItemNumber] VARCHAR(15) NOT NULL,
    [SerialNumber] VARCHAR(10) NOT NULL,
    [ShortageQuantity] NUMERIC(7,0) NOT NULL,
    [DefectCode] VARCHAR(4) NULL,
    [LocationCode] VARCHAR(2) NULL,
    [OrderNumber] NUMERIC(7,0) NULL,
    [OrderItemSeq] INT NULL
)
```

### Input Field Descriptions

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| `CustomerNumber` | VARCHAR(8) | Yes | Customer number from IWS |
| `ShipToNumber` | VARCHAR(4) | Yes | Ship-to location number |
| `InvoiceNumber` | NUMERIC(6,0) | Yes | Invoice number where shortage occurred |
| `ItemNumber` | VARCHAR(15) | Yes | Item/SKU number with shortage |
| `SerialNumber` | VARCHAR(10) | Yes | Serial number obtained from IWS |
| `ShortageQuantity` | NUMERIC(7,0) | Yes | Quantity of items short |
| `DefectCode` | VARCHAR(4) | No | Defect code (defaults to 'XP') |
| `LocationCode` | VARCHAR(2) | No | Warehouse location (defaults to 'WU') |
| `OrderNumber` | NUMERIC(7,0) | No | Original order number |
| `OrderItemSeq` | INT | No | Order item sequence number |

---

## Output Result Set

| Column | Type | Description |
|--------|------|-------------|
| `CustomerNumber` | VARCHAR(8) | Customer number |
| `ShipToNumber` | VARCHAR(4) | Ship-to number |
| `InvoiceNumber` | NUMERIC(6,0) | Invoice number |
| `ItemNumber` | VARCHAR(15) | Item number |
| `SerialNumber` | VARCHAR(10) | Serial number |
| `ShortageQuantity` | NUMERIC(7,0) | Requested shortage quantity |
| `DefectCode` | VARCHAR(4) | Defect code (with defaults applied) |
| `LocationCode` | VARCHAR(2) | Location code (with defaults applied) |
| `OrderNumber` | NUMERIC(7,0) | Order number |
| `OrderItemSeq` | INT | Order item sequence |
| `IsValid` | BIT | **1** = All validations passed<br>**0** = One or more validations failed |
| `ValidationErrors` | VARCHAR(MAX) | Detailed error messages (or SUCCESS message) |
| `OrderedQuantity` | NUMERIC(7,0) | Original ordered quantity |
| `AlreadyCreditedQuantity` | DECIMAL(10,2) | Quantity already credited |
| `RemainingCreditableQuantity` | DECIMAL(10,2) | Quantity remaining that can be credited |
| `ItemExists` | BIT | 1 = Item found in master, 0 = Not found |
| `CustomerSerialItemValid` | BIT | 1 = Valid combination, 0 = Invalid |
| `DefectCodeValid` | BIT | 1 = Valid defect code, 0 = Invalid |
| `LocationCodeValid` | BIT | 1 = Valid location, 0 = Invalid |

---

## Validation Rules

### 1️⃣ Item Validation
- **Check**: Item exists in `tblItemMaster`
- **Tables**: `Ashley.dbo.tblItemMaster`
- **Error**: `"ERROR: Item [{ItemNumber}] does not exist or is invalid."`

### 2️⃣ Customer/Serial/Item Combination
- **Check**: Combination exists in invoice history
- **Tables**: 
  - `Datawhse.dbo.tblInvoiceDetail`
  - `Archive.dbo.tblInvoiceDetail` (fallback)
- **Error**: `"ERROR: Customer/Serial/Item combination is invalid. Item not found on serial {SerialNumber} for customer {CustomerNumber}."`

### 3️⃣ Order Quantity Validation
- **Check**: Retrieves original ordered quantity
- **Tables**: 
  - `Datawhse.dbo.tblInvoiceDetail`
  - `Archive.dbo.tblInvoiceDetail` (fallback)
- **Error**: `"ERROR: Cannot determine original order quantity for item."`

### 4️⃣ Duplicate Credit Check
- **Check**: Calculates already credited quantity
- **Tables**: 
  - `Ashley.dbo.tblRetAllowHeader`
  - `Ashley.dbo.tblRetAllowDetail`
- **Logic**: Only counts approved credits (`rahAprvDny = 'A'`)
- **Calculation**: Uses allowance percentage: `SUM(radOrdQty * (radPctAllow / 100.0))`
- **Error**: `"ERROR: Shortage quantity ({qty}) exceeds remaining creditable quantity ({remaining}). Already credited: {credited}."`

### 5️⃣ Shortage Quantity vs Creditable Quantity
- **Check**: `ShortageQuantity <= RemainingCreditableQuantity`
- **Formula**: `RemainingCreditableQuantity = OrderedQuantity - AlreadyCreditedQuantity`
- **Error**: Details provided in validation message

### 6️⃣ Defect Code Validation
- **Check**: Defect code exists and is active
- **Tables**: `Ashley.dbo.tblDefectCodes`
- **Default**: 'XP' (if NULL)
- **Error**: `"ERROR: Defect code [{code}] is invalid or inactive."`

### 7️⃣ Location Code Validation
- **Check**: Warehouse location exists and is active
- **Tables**: `Ashley.dbo.tblWarehouse`
- **Default**: 'WU' (if NULL or from web.config)
- **Error**: `"ERROR: Location code [{code}] is invalid or inactive."`

---

## Usage Examples

### Example 1: Single Item Validation
```sql
DECLARE @ShortageItems AS dbo.typCEShortageItemValidation;

INSERT INTO @ShortageItems 
SELECT '8888300', '0001', 110433, 'C310325', '7320325', 1, 'XP', 'WU', 1234567, 100;

EXEC usp_CE_ValidateShortageItems @ShortageItems, 'AFI';
```

### Example 2: Batch Validation (Multiple Items)
```sql
DECLARE @ShortageItems AS dbo.typCEShortageItemValidation;

INSERT INTO @ShortageItems
VALUES
    ('8888300', '0001', 110433, 'C310325', '7320325', 1, NULL, NULL, 1234567, 100),
    ('8888300', '0001', 110434, 'D582-01', '7320326', 2, NULL, NULL, 1234568, 200),
    ('8888400', '0002', 110435, 'A123456', '7320327', 1, 'DA', 'RC', 1234569, 300);

EXEC usp_CE_ValidateShortageItems @ShortageItems, 'AFI';
```

### Example 3: Using Custom Default Codes
```sql
DECLARE @ShortageItems AS dbo.typCEShortageItemValidation;

INSERT INTO @ShortageItems
SELECT '8888300', '0001', 110433, 'C310325', '7320325', 1, NULL, NULL, 1234567, 100;

-- Override defaults: Defect='DA', Location='RC'
EXEC usp_CE_ValidateShortageItems @ShortageItems, 'AFI', 'DA', 'RC';
```

### Example 4: Auto-detect Environment
```sql
DECLARE @ShortageItems AS dbo.typCEShortageItemValidation;

INSERT INTO @ShortageItems
SELECT '8888300', '0001', 110433, 'C310325', '7320325', 1, 'XP', 'WU', 1234567, 100;

-- Environment will be auto-detected from Environment.dbo.tblEnvironmentControl
EXEC usp_CE_ValidateShortageItems @ShortageItems, NULL;
```

---

## Sample Output

### ✅ Success Case
```
CustomerNumber: 8888300
InvoiceNumber: 110433
ItemNumber: C310325
SerialNumber: 7320325
ShortageQuantity: 1
DefectCode: XP
LocationCode: WU
IsValid: 1
ValidationErrors: SUCCESS: All validations passed.
OrderedQuantity: 5
AlreadyCreditedQuantity: 0.00
RemainingCreditableQuantity: 5.00
ItemExists: 1
CustomerSerialItemValid: 1
DefectCodeValid: 1
LocationCodeValid: 1
```

### ❌ Failure Case (Multiple Errors)
```
CustomerNumber: 8888300
InvoiceNumber: 110433
ItemNumber: INVALIDITEM
SerialNumber: BADSERIAL
ShortageQuantity: 999
DefectCode: BADCODE
LocationCode: ZZ
IsValid: 0
ValidationErrors: ERROR: Item [INVALIDITEM] does not exist or is invalid.
                  ERROR: Customer/Serial/Item combination is invalid.
                  ERROR: Shortage quantity (999) exceeds remaining creditable quantity (0).
                  ERROR: Defect code [BADCODE] is invalid or inactive.
                  ERROR: Location code [ZZ] is invalid or inactive.
OrderedQuantity: 0
AlreadyCreditedQuantity: 0.00
RemainingCreditableQuantity: 0.00
ItemExists: 0
CustomerSerialItemValid: 0
DefectCodeValid: 0
LocationCodeValid: 0
```

---

## Integration with Microservice

### Recommended C# Integration Pattern

```csharp
public class ShortageValidationService
{
    private readonly string _connectionString;

    public async Task<List<ShortageValidationResult>> ValidateShortageItems(
        List<ShortageItem> items,
        string environment = "AFI",
        string defaultDefectCode = "XP",
        string defaultLocationCode = "WU")
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            var table = new DataTable();
            table.Columns.Add("CustomerNumber", typeof(string));
            table.Columns.Add("ShipToNumber", typeof(string));
            table.Columns.Add("InvoiceNumber", typeof(int));
            table.Columns.Add("ItemNumber", typeof(string));
            table.Columns.Add("SerialNumber", typeof(string));
            table.Columns.Add("ShortageQuantity", typeof(int));
            table.Columns.Add("DefectCode", typeof(string));
            table.Columns.Add("LocationCode", typeof(string));
            table.Columns.Add("OrderNumber", typeof(int));
            table.Columns.Add("OrderItemSeq", typeof(int));

            foreach (var item in items)
            {
                table.Rows.Add(
                    item.CustomerNumber,
                    item.ShipToNumber,
                    item.InvoiceNumber,
                    item.ItemNumber,
                    item.SerialNumber,
                    item.ShortageQuantity,
                    item.DefectCode,
                    item.LocationCode,
                    item.OrderNumber,
                    item.OrderItemSeq
                );
            }

            var cmd = new SqlCommand("usp_CE_ValidateShortageItems", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ShortageItems", table);
            cmd.Parameters.AddWithValue("@Environment", environment);
            cmd.Parameters.AddWithValue("@DefaultDefectCode", defaultDefectCode);
            cmd.Parameters.AddWithValue("@DefaultLocationCode", defaultLocationCode);

            await connection.OpenAsync();

            var results = new List<ShortageValidationResult>();
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    results.Add(new ShortageValidationResult
                    {
                        CustomerNumber = reader["CustomerNumber"].ToString(),
                        InvoiceNumber = Convert.ToInt32(reader["InvoiceNumber"]),
                        ItemNumber = reader["ItemNumber"].ToString(),
                        SerialNumber = reader["SerialNumber"].ToString(),
                        ShortageQuantity = Convert.ToInt32(reader["ShortageQuantity"]),
                        IsValid = Convert.ToBoolean(reader["IsValid"]),
                        ValidationErrors = reader["ValidationErrors"].ToString(),
                        OrderedQuantity = Convert.ToInt32(reader["OrderedQuantity"]),
                        AlreadyCreditedQuantity = Convert.ToDecimal(reader["AlreadyCreditedQuantity"]),
                        RemainingCreditableQuantity = Convert.ToDecimal(reader["RemainingCreditableQuantity"])
                    });
                }
            }

            return results;
        }
    }
}
```

---

## Performance Considerations

1. **Indexes Required**:
   - `tblInvoiceDetail`: (indCusno, indInvno, indSerno, indItnbr)
   - `tblRetAllowHeader`: (rahCustNo, rahEnterDate, rahEnterTime)
   - `tblRetAllowDetail`: (radEnterDate, radEnterTime, radInvoiceNo, radItemNo)

2. **NOLOCK Hints**: Used for read-only operations to prevent blocking

3. **Archive Fallback**: Only queries archive table if not found in primary

4. **Batch Processing**: Designed to validate multiple items in a single call

---

## Error Handling

- All SQL errors are propagated to the caller
- Validation errors are returned in the `ValidationErrors` column
- Failed items have `IsValid = 0`
- Results are ordered with failed validations first

---

## Dependencies

### Tables
- `Ashley.dbo.tblItemMaster`
- `Ashley.dbo.tblDefectCodes`
- `Ashley.dbo.tblWarehouse`
- `Ashley.dbo.tblRetAllowHeader`
- `Ashley.dbo.tblRetAllowDetail`
- `Ashley.dbo.tblCustomerShippingLocations`
- `Datawhse.dbo.tblInvoiceDetail`
- `Archive.dbo.tblInvoiceDetail`
- `Environment.dbo.tblEnvironmentControl`

### User-Defined Types
- `dbo.typCEShortageItemValidation`

---

## Change History

| Date | Author | Version | Description |
|------|--------|---------|-------------|
| 2026-05-20 | Credit Team | 1.0 | Initial creation |

---

## Related Procedures

- `usp_CEImportSubmittedCredits` - Submits validated credits
- `usp_CEGetCreditItemQtys` - Gets existing credit quantities
- `usp_CEGetItemOrdQtys` - Gets order quantities
- `usp_CEGetAddDefects` - Gets valid defect codes
- `usp_CEGetAddLocations` - Gets valid location codes

