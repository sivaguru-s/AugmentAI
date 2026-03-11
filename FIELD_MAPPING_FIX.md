# Field Mapping Correction

## ✅ **Issue Fixed**

The `keytable104.keyvaluechar` field was incorrectly mapped to **VendorName** when it actually contains the **Order Number** (PO Number).

---

## 🔧 **Correct Field Mapping**

Based on your original query example:

```sql
SET @InvoiceNumber = 61304208
SET @OrderNumber = 'D264904'
SET @InvoiceDate = '2024-11-20 00:00:00.000'

WHERE keyitem106.keyvaluesmall = @InvoiceNumber
  AND keytable104.keyvaluechar = @OrderNumber
  AND keyitem112.keyvaluedate = @InvoiceDate
```

### **Correct Mapping:**

| Onbase Table/Column | Invoice Model Field | Example Value |
|---------------------|---------------------|---------------|
| `keyitem106.keyvaluesmall` | InvoiceNumber | 61304208 |
| `keytable104.keyvaluechar` | PONumber | 'D264904' |
| `keyitem112.keyvaluedate` | InvoiceDate | 2024-11-20 |
| `itemdata.itemnum` | InvoiceId | (internal ID) |
| `itemdata.itemname` | Description | (document name) |
| `itemdata.itemdate` | CreatedDate | (creation date) |

### **Fields Not Yet Mapped:**

| Invoice Model Field | Status | Notes |
|---------------------|--------|-------|
| VendorName | ❌ Not available | Need to identify vendor table |
| Amount | ⚠️ Using InvoiceNumber | Need actual amount field |
| DueDate | ❌ Not available | Need to identify due date table |
| Status | ❌ Not available | Need to identify status table |

---

## 🛠️ **Changes Made**

### **Before (Incorrect):**
```csharp
CAST(ki106.keyvaluesmall AS VARCHAR(50)) as InvoiceNumber,
kt104.keyvaluechar as VendorName,  // ❌ WRONG
CAST(ki106.keyvaluesmall AS DECIMAL(18,2)) as Amount,
ki112.keyvaluedate as InvoiceDate,
NULL as DueDate,
NULL as Status,
kt104.keyvaluechar as PONumber,  // Same field mapped twice!
```

### **After (Correct):**
```csharp
CAST(ki106.keyvaluesmall AS VARCHAR(50)) as InvoiceNumber,
NULL as VendorName,  // ✅ Correctly set to NULL (not available)
CAST(ki106.keyvaluesmall AS DECIMAL(18,2)) as Amount,
ki112.keyvaluedate as InvoiceDate,
NULL as DueDate,
NULL as Status,
kt104.keyvaluechar as PONumber,  // ✅ Correctly mapped to Order Number
```

---

## 📊 **Current Data Availability**

When you search for an invoice, you'll now see:

| Field | Status | Value |
|-------|--------|-------|
| Invoice Number | ✅ Available | From `keyitem106` |
| PO Number | ✅ Available | From `keytable104` (Order Number) |
| Invoice Date | ✅ Available | From `keyitem112` |
| Description | ✅ Available | From `itemdata.itemname` |
| Created Date | ✅ Available | From `itemdata.itemdate` |
| Vendor Name | ❌ N/A | Not yet identified |
| Amount | ⚠️ Placeholder | Using invoice number as proxy |
| Due Date | ❌ N/A | Not yet identified |
| Status | ❌ N/A | Not yet identified |

---

## 🔍 **Next Steps to Complete Mapping**

To get the missing fields, we need to identify the correct Onbase tables:

### **1. Find Vendor Name Table**
Run this query to find vendor-related keywords:
```sql
SELECT keywordnum, keywordname 
FROM hsi.rmkeyword 
WHERE keywordname LIKE '%vendor%' 
   OR keywordname LIKE '%supplier%'
   OR keywordname LIKE '%company%'
```

### **2. Find Amount Table**
Run this query to find amount-related keywords:
```sql
SELECT keywordnum, keywordname 
FROM hsi.rmkeyword 
WHERE keywordname LIKE '%amount%' 
   OR keywordname LIKE '%total%'
   OR keywordname LIKE '%value%'
```

### **3. Find Due Date Table**
Run this query to find due date keywords:
```sql
SELECT keywordnum, keywordname 
FROM hsi.rmkeyword 
WHERE keywordname LIKE '%due%' 
   OR keywordname LIKE '%payment%'
```

### **4. Find Status Table**
Run this query to find status keywords:
```sql
SELECT keywordnum, keywordname 
FROM hsi.rmkeyword 
WHERE keywordname LIKE '%status%' 
   OR keywordname LIKE '%state%'
   OR keywordname LIKE '%approved%'
```

Once you provide the keyword numbers, I can add the appropriate joins to populate these fields.

---

## 🌐 **Application Status**

✅ **Running on:** http://localhost:5001  
✅ **Build:** Successful  
✅ **Field Mapping:** Corrected  

---

## 🧪 **Test the Fix**

Try searching for an invoice:

**Query:** "find invoice 61304208"

**Expected Result:**
- ✅ Invoice Number: 61304208
- ✅ PO Number: D264904 (Order Number)
- ✅ Invoice Date: 2024-11-20
- ❌ Vendor: N/A (not yet mapped)
- ⚠️ Amount: 61304208 (placeholder - using invoice number)

The PO Number field should now correctly show the Order Number instead of incorrectly showing it as Vendor Name! 🎉


