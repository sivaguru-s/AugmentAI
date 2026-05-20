# Quick Reference Card: Credit Shortage Validation

## 🚀 Quick Start (5 Minutes)

### Step 1: Deploy
```sql
USE [Ashley]
GO

-- Run this file:
-- Database/StoredProcedures/usp_CE_ValidateShortageItems.sql
```

### Step 2: Test
```sql
DECLARE @Items AS dbo.typCEShortageItemValidation;
INSERT INTO @Items VALUES ('8888300', '0001', 110433, 'C310325', '7320325', 1, NULL, NULL, 1234567, 100);
EXEC usp_CE_ValidateShortageItems @Items, 'AFI';
```

### Step 3: Integrate
```csharp
// Call from your microservice
var results = await ValidateShortageItems(items, "AFI", "XP", "WU");
if (results.All(r => r.IsValid))
{
    // Proceed with credit submission
}
```

---

## 📋 Input Fields

| Field | Type | Required | Example | Notes |
|-------|------|----------|---------|-------|
| CustomerNumber | VARCHAR(8) | ✅ | '8888300' | From IWS |
| ShipToNumber | VARCHAR(4) | ✅ | '0001' | Ship-to location |
| InvoiceNumber | NUMERIC(6,0) | ✅ | 110433 | Invoice with shortage |
| ItemNumber | VARCHAR(15) | ✅ | 'C310325' | Item/SKU |
| SerialNumber | VARCHAR(10) | ✅ | '7320325' | **From IWS API** |
| ShortageQuantity | NUMERIC(7,0) | ✅ | 1 | Shortage amount |
| DefectCode | VARCHAR(4) | ⚪ | 'XP' | Default: 'XP' |
| LocationCode | VARCHAR(2) | ⚪ | 'WU' | Default: 'WU' |
| OrderNumber | NUMERIC(7,0) | ⚪ | 1234567 | Optional |
| OrderItemSeq | INT | ⚪ | 100 | Optional |

---

## ✅ Validation Checklist

- [ ] **Item Exists** → `tblItemMaster`
- [ ] **Customer/Serial/Item Valid** → `tblInvoiceDetail`
- [ ] **No Duplicate Credit** → `tblRetAllowHeader/Detail`
- [ ] **Shortage Qty ≤ Available** → Calculated
- [ ] **Defect Code Active** → `tblDefectCodes` (default: XP)
- [ ] **Location Code Active** → `tblWarehouse` (default: WU)

---

## 📊 Output Interpretation

### Success ✅
```sql
IsValid = 1
ValidationErrors = 'SUCCESS: All validations passed.'
RemainingCreditableQuantity > 0
```
**Action**: Proceed with credit submission

### Failure ❌
```sql
IsValid = 0
ValidationErrors = 'ERROR: Item [X] does not exist...'
```
**Action**: Display errors to user, do NOT submit credit

---

## 🔍 Common Scenarios

### Scenario 1: First-Time Shortage
```
OrderedQuantity = 10
AlreadyCreditedQuantity = 0
RemainingCreditableQuantity = 10
ShortageQuantity = 2
Result: ✅ VALID (2 <= 10)
```

### Scenario 2: Partial Credit Exists
```
OrderedQuantity = 10
AlreadyCreditedQuantity = 7
RemainingCreditableQuantity = 3
ShortageQuantity = 2
Result: ✅ VALID (2 <= 3)
```

### Scenario 3: Already Fully Credited
```
OrderedQuantity = 10
AlreadyCreditedQuantity = 10
RemainingCreditableQuantity = 0
ShortageQuantity = 1
Result: ❌ INVALID (1 > 0)
Error: "Already credited: 10"
```

### Scenario 4: Over-Credit Attempt
```
OrderedQuantity = 10
AlreadyCreditedQuantity = 3
RemainingCreditableQuantity = 7
ShortageQuantity = 15
Result: ❌ INVALID (15 > 7)
Error: "Shortage quantity (15) exceeds remaining creditable quantity (7)"
```

---

## 🎯 Default Values

| Item | Default | Override? |
|------|---------|-----------|
| DefectCode | 'XP' | ✅ Yes (parameter) |
| LocationCode | 'WU' | ✅ Yes (parameter) |
| Environment | Auto-detect | ✅ Yes (parameter) |

### Override Example
```sql
-- Use custom defaults
EXEC usp_CE_ValidateShortageItems @Items, 'AFI', 'DA', 'RC';
```

---

## 🔧 Troubleshooting

| Error | Cause | Solution |
|-------|-------|----------|
| "Type does not exist" | Type not created | Run CREATE TYPE (in SP script) |
| "Item does not exist" | Invalid item number | Verify item in `tblItemMaster` |
| "Customer/Serial/Item invalid" | Wrong combination | Check IWS serial# correct |
| "Cannot determine order qty" | Invoice not found | Check invoice exists in system |
| "Defect code invalid" | Bad/inactive code | Use 'XP' or check `tblDefectCodes` |
| "Location code invalid" | Bad/inactive location | Use 'WU' or check `tblWarehouse` |

---

## 📞 Quick Help

### Files to Reference
- **API Docs**: `Database/Documentation/usp_CE_ValidateShortageItems_Documentation.md`
- **Flow Diagram**: `Database/Documentation/ValidationFlow.md`
- **Examples**: `Database/StoredProcedures/usp_CE_ValidateShortageItems_TestScript.sql`
- **Overview**: `README.md`

### Key Tables
```sql
-- Check valid defect codes
SELECT * FROM Ashley.dbo.tblDefectCodes WHERE defActive = 'Y'

-- Check valid locations
SELECT * FROM Ashley.dbo.tblWarehouse WHERE whsActive = 'Y'

-- Check existing credits for item
SELECT * FROM Ashley.dbo.tblRetAllowDetail 
WHERE radInvoiceNo = {invoice} AND radItemNo = '{item}'
```

---

## 💡 Pro Tips

1. **Always validate before submitting** - Prevents data integrity issues
2. **Use NULL for defaults** - Let SP apply XP and WU automatically
3. **Batch validate** - More efficient than one-at-a-time
4. **Check `IsValid` flag** - Don't parse error messages for logic
5. **Log validation failures** - Helps identify data quality issues

---

## 📈 Performance Tips

- **Index exists?** Check `tblInvoiceDetail` has composite index
- **Archive queries slow?** Only fallback when needed (automatic)
- **Batch size?** 100-500 items per call is optimal
- **NOLOCK concerns?** Acceptable for validation (eventual consistency)

---

## 🎓 Integration Pattern

```
┌─────────────┐
│   IWS API   │ ← 1. Call to get serial#
└──────┬──────┘
       │
       ↓ 2. Extract serial#
┌─────────────┐
│  Build Item │
└──────┬──────┘
       │
       ↓ 3. Validate
┌─────────────┐
│  Validation │
│      SP     │
└──────┬──────┘
       │
       ↓ 4. Check IsValid
┌─────────────┐
│   Submit?   │
└──────┬──────┘
       │
   ┌───┴───┐
   │       │
   ↓       ↓
 ✅YES   ❌NO
Submit   Error
```

---

## 📌 Remember

- ✅ **Serial# comes from IWS** - Always call IWS first
- ✅ **XP = Default defect code** - For shortage scenarios
- ✅ **WU = Default location** - Configurable in web.config
- ✅ **Batch validation supported** - More efficient
- ✅ **All validations run** - See all errors at once
- ✅ **Archive fallback automatic** - Don't worry about it

---

## 🎯 Success Criteria

Before deploying to production, verify:
- [ ] Stored procedure deployed successfully
- [ ] Test script runs without errors
- [ ] Sample data validates correctly
- [ ] Default codes configured (XP, WU)
- [ ] Indexes exist on key tables
- [ ] Service account has execute permission
- [ ] Logging/monitoring configured
- [ ] Error handling tested

---

**Need More Help?** See `README.md` or full documentation in `Database/Documentation/`

