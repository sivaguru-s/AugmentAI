# Onbase Document Type Analysis

**Date:** March 12, 2026  
**Database:** aazeus-obdmsq01 / Onbase  
**Purpose:** Identify Customer vs Vendor Invoice and Credit Memo document types

---

## 📊 **Document Type Classification**

### **Customer Documents (AR - Accounts Receivable)**

| Document Type | itemtypenum | Description | Vendor Field? |
|---------------|-------------|-------------|---------------|
| **Invoices** | 102 | Customer Invoices | ❌ No |
| **Credit Memo** | 119 | Customer Credit Memos | ❌ No |

### **Vendor Documents (AP - Accounts Payable)**

| Document Type | itemtypenum | Description | Vendor Field? | Record Count |
|---------------|-------------|-------------|---------------|--------------|
| **AP Invoices** | 263 | Vendor Invoices (Main) | ✅ Yes | 121,012 |
| **AP Invoices Company 5** | 340 | Vendor Invoices (Co 5) | ✅ Yes | 6,713 |
| **AP Invoices Company 6** | 364 | Vendor Invoices (Co 6) | ✅ Yes | 4,911 |
| **AP Invoices Company 9** | 429 | Vendor Invoices (Co 9) | ✅ Yes | 384 |
| **AP Invoices** | 515 | Vendor Invoices (Alt) | ⚠️ Unknown | - |
| **ApscanCo1** | 492 | Scanned AP Docs (Co 1) | ⚠️ Unknown | - |
| **ApscanCo6** | 493 | Scanned AP Docs (Co 6) | ⚠️ Unknown | - |
| **ApscanCo9** | 494 | Scanned AP Docs (Co 9) | ⚠️ Unknown | - |

### **Credit Documents**

| Document Type | itemtypenum | Description | Type |
|---------------|-------------|-------------|------|
| **Credit Memo** | 119 | Customer Credit Memo | AR |
| **Credit Document** | 200 | Generic Credit Document | Unknown |
| **Credit Document No Go** | 362 | Rejected Credit Document | Unknown |

---

## 🔍 **Key Findings**

### **1. Customer vs Vendor Distinction**

- **Customer Documents (AR):**
  - itemtypenum = 102 (Invoices)
  - itemtypenum = 119 (Credit Memo)
  - **NO vendor field** in keytable105

- **Vendor Documents (AP):**
  - itemtypenum = 263, 340, 364, 429, 515 (AP Invoices)
  - **HAS vendor field** in keytable105
  - Total: 133,020+ vendor invoices with vendor names

### **2. Vendor Credit Memos**

⚠️ **No dedicated "AP Credit Memo" document type found**

Possible scenarios:
- Vendor credit memos might be stored as negative amounts in AP Invoices (263)
- They might use the generic "Credit Document" (200) type
- They might be in a different table or marked differently

---

## 📝 **Modified SQL Queries**

### **Query 1: Customer Invoices (Original - itemtype 102)**

```sql
-- Customer Invoices (AR)
SELECT 
    i.itemnum,
    CAST(ki106.keyvaluesmall AS VARCHAR(50)) as InvoiceNumber,
    ki112.keyvaluedate as InvoiceDate,
    kt104.keyvaluechar as PONumber,
    NULL as VendorName  -- No vendor for customer invoices
FROM hsi.itemdata i WITH (NOLOCK)
LEFT JOIN hsi.keyitem106 ki106 WITH (NOLOCK) 
    ON i.itemnum = ki106.itemnum
LEFT JOIN hsi.keyitem112 ki112 WITH (NOLOCK) 
    ON i.itemnum = ki112.itemnum
LEFT JOIN hsi.keytable104 kt104 WITH (NOLOCK) 
    ON i.itemnum = kt104.itemnum
WHERE i.itemtypenum = 102  -- Customer Invoices
ORDER BY i.itemnum DESC;
```

### **Query 2: Customer Credit Memos (itemtype 119)**

```sql
-- Customer Credit Memos (AR)
SELECT 
    i.itemnum,
    CAST(ki106.keyvaluesmall AS VARCHAR(50)) as CreditMemoNumber,
    ki112.keyvaluedate as CreditMemoDate,
    kt104.keyvaluechar as PONumber,
    NULL as VendorName  -- No vendor for customer credit memos
FROM hsi.itemdata i WITH (NOLOCK)
LEFT JOIN hsi.keyitem106 ki106 WITH (NOLOCK) 
    ON i.itemnum = ki106.itemnum
LEFT JOIN hsi.keyitem112 ki112 WITH (NOLOCK) 
    ON i.itemnum = ki112.itemnum
LEFT JOIN hsi.keytable104 kt104 WITH (NOLOCK) 
    ON i.itemnum = kt104.itemnum
WHERE i.itemtypenum = 119  -- Customer Credit Memos
ORDER BY i.itemnum DESC;
```

### **Query 3: Vendor Invoices (AP - itemtype 263, 340, 364, 429)**

```sql
-- Vendor Invoices (AP) - ALL COMPANIES
SELECT 
    i.itemnum,
    i.itemtypenum,
    it.itemtypename as DocumentType,
    CAST(ki106.keyvaluesmall AS VARCHAR(50)) as InvoiceNumber,
    ki112.keyvaluedate as InvoiceDate,
    kt104.keyvaluechar as PONumber,
    RTRIM(kt105.keyvaluechar) as VendorName  -- ✅ Vendor field available
FROM hsi.itemdata i WITH (NOLOCK)
LEFT JOIN hsi.itemtype it WITH (NOLOCK)
    ON i.itemtypenum = it.itemtypenum
LEFT JOIN hsi.keyitem106 ki106 WITH (NOLOCK) 
    ON i.itemnum = ki106.itemnum
LEFT JOIN hsi.keyitem112 ki112 WITH (NOLOCK) 
    ON i.itemnum = ki112.itemnum
LEFT JOIN hsi.keytable104 kt104 WITH (NOLOCK) 
    ON i.itemnum = kt104.itemnum
LEFT JOIN hsi.keyxitem105 kx105 WITH (NOLOCK) 
    ON i.itemnum = kx105.itemnum
LEFT JOIN hsi.keytable105 kt105 WITH (NOLOCK) 
    ON kx105.keywordnum = kt105.keywordnum
WHERE i.itemtypenum IN (263, 340, 364, 429, 515)  -- All AP Invoice types
ORDER BY i.itemnum DESC;
```

### **Query 4: Vendor Invoices - Main Company Only (itemtype 263)**

```sql
-- Vendor Invoices (AP) - MAIN COMPANY ONLY
SELECT 
    i.itemnum,
    CAST(ki106.keyvaluesmall AS VARCHAR(50)) as InvoiceNumber,
    ki112.keyvaluedate as InvoiceDate,
    kt104.keyvaluechar as PONumber,
    RTRIM(kt105.keyvaluechar) as VendorName
FROM hsi.itemdata i WITH (NOLOCK)
LEFT JOIN hsi.keyitem106 ki106 WITH (NOLOCK) 
    ON i.itemnum = ki106.itemnum
LEFT JOIN hsi.keyitem112 ki112 WITH (NOLOCK) 
    ON i.itemnum = ki112.itemnum
LEFT JOIN hsi.keytable104 kt104 WITH (NOLOCK) 
    ON i.itemnum = kt104.itemnum
LEFT JOIN hsi.keyxitem105 kx105 WITH (NOLOCK) 
    ON i.itemnum = kx105.itemnum
LEFT JOIN hsi.keytable105 kt105 WITH (NOLOCK) 
    ON kx105.keywordnum = kt105.keywordnum
WHERE i.itemtypenum = 263  -- AP Invoices (Main)
ORDER BY i.itemnum DESC;
```

---

## 🧪 **Test Queries**

### **Test 1: Find CDW Vendor Invoices**

```sql
SELECT TOP 10
    i.itemnum,
    it.itemtypename,
    CAST(ki106.keyvaluesmall AS VARCHAR(50)) as InvoiceNumber,
    RTRIM(kt105.keyvaluechar) as VendorName
FROM hsi.itemdata i WITH (NOLOCK)
INNER JOIN hsi.itemtype it WITH (NOLOCK)
    ON i.itemtypenum = it.itemtypenum
INNER JOIN hsi.keyxitem105 kx105 WITH (NOLOCK) 
    ON i.itemnum = kx105.itemnum
INNER JOIN hsi.keytable105 kt105 WITH (NOLOCK) 
    ON kx105.keywordnum = kt105.keywordnum
LEFT JOIN hsi.keyitem106 ki106 WITH (NOLOCK) 
    ON i.itemnum = ki106.itemnum
WHERE i.itemtypenum IN (263, 340, 364, 429)
  AND kt105.keyvaluechar LIKE '%CDW COMPUTER CENTERS%'
ORDER BY i.itemnum DESC;
```

### **Test 2: Count Documents by Type**

```sql
SELECT 
    i.itemtypenum,
    it.itemtypename,
    COUNT(*) as TotalDocuments,
    COUNT(DISTINCT kx105.itemnum) as DocumentsWithVendor,
    (COUNT(DISTINCT kx105.itemnum) * 100.0 / COUNT(*)) as VendorCoveragePercent
FROM hsi.itemdata i WITH (NOLOCK)
INNER JOIN hsi.itemtype it WITH (NOLOCK)
    ON i.itemtypenum = it.itemtypenum
LEFT JOIN hsi.keyxitem105 kx105 WITH (NOLOCK) 
    ON i.itemnum = kx105.itemnum
WHERE i.itemtypenum IN (102, 119, 263, 340, 364, 429)
GROUP BY i.itemtypenum, it.itemtypename
ORDER BY i.itemtypenum;
```

---

## ✅ **Summary**

| Category | Document Type | itemtypenum | Vendor Field | Query Modified |
|----------|---------------|-------------|--------------|----------------|
| **Customer** | Invoices | 102 | ❌ No | Original query |
| **Customer** | Credit Memo | 119 | ❌ No | ✅ Created |
| **Vendor** | AP Invoices | 263 | ✅ Yes | ✅ Created |
| **Vendor** | AP Invoices Co 5 | 340 | ✅ Yes | ✅ Included |
| **Vendor** | AP Invoices Co 6 | 364 | ✅ Yes | ✅ Included |
| **Vendor** | AP Invoices Co 9 | 429 | ✅ Yes | ✅ Included |
| **Vendor** | Credit Memo | ??? | ⚠️ Unknown | ⚠️ Not found |


