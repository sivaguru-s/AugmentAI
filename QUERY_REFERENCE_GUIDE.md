# Onbase Query Reference Guide

**Quick reference for Customer (AR) vs Vendor (AP) document queries**

---

## 📋 **Document Type Quick Reference**

```
┌─────────────────────────────────────────────────────────────────┐
│                    CUSTOMER DOCUMENTS (AR)                      │
├──────────────┬─────────────┬──────────────┬────────────────────┤
│ Type         │ itemtypenum │ Vendor Field │ Description        │
├──────────────┼─────────────┼──────────────┼────────────────────┤
│ Invoices     │ 102         │ ❌ NO        │ Customer Invoices  │
│ Credit Memo  │ 119         │ ❌ NO        │ Customer Credits   │
└──────────────┴─────────────┴──────────────┴────────────────────┘

┌─────────────────────────────────────────────────────────────────┐
│                     VENDOR DOCUMENTS (AP)                       │
├──────────────┬─────────────┬──────────────┬────────────────────┤
│ Type         │ itemtypenum │ Vendor Field │ Description        │
├──────────────┼─────────────┼──────────────┼────────────────────┤
│ AP Invoices  │ 263         │ ✅ YES       │ Vendor Invoices    │
│ AP Inv Co 5  │ 340         │ ✅ YES       │ Vendor Inv (Co 5)  │
│ AP Inv Co 6  │ 364         │ ✅ YES       │ Vendor Inv (Co 6)  │
│ AP Inv Co 9  │ 429         │ ✅ YES       │ Vendor Inv (Co 9)  │
└──────────────┴─────────────┴──────────────┴────────────────────┘
```

---

## 🔍 **Query Templates**

### **1. Customer Invoices (itemtype 102) - NO Vendor**

```sql
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
WHERE i.itemtypenum = 102
ORDER BY i.itemnum DESC;
```

---

### **2. Customer Credit Memos (itemtype 119) - NO Vendor**

```sql
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
WHERE i.itemtypenum = 119
ORDER BY i.itemnum DESC;
```

---

### **3. Vendor Invoices - ALL COMPANIES (itemtype 263, 340, 364, 429) - HAS Vendor**

```sql
SELECT 
    i.itemnum,
    i.itemtypenum,
    it.itemtypename as DocumentType,
    CAST(ki106.keyvaluesmall AS VARCHAR(50)) as InvoiceNumber,
    ki112.keyvaluedate as InvoiceDate,
    kt104.keyvaluechar as PONumber,
    RTRIM(kt105.keyvaluechar) as VendorName  -- ✅ Vendor field
FROM hsi.itemdata i WITH (NOLOCK)
LEFT JOIN hsi.itemtype it WITH (NOLOCK)
    ON i.itemtypenum = it.itemtypenum
LEFT JOIN hsi.keyitem106 ki106 WITH (NOLOCK) 
    ON i.itemnum = ki106.itemnum
LEFT JOIN hsi.keyitem112 ki112 WITH (NOLOCK) 
    ON i.itemnum = ki112.itemnum
LEFT JOIN hsi.keytable104 kt104 WITH (NOLOCK) 
    ON i.itemnum = kt104.itemnum
LEFT JOIN hsi.keyxitem105 kx105 WITH (NOLOCK)   -- ← Link to vendor
    ON i.itemnum = kx105.itemnum
LEFT JOIN hsi.keytable105 kt105 WITH (NOLOCK)   -- ← Vendor table
    ON kx105.keywordnum = kt105.keywordnum
WHERE i.itemtypenum IN (263, 340, 364, 429, 515)
ORDER BY i.itemnum DESC;
```

---

### **4. Vendor Invoices - MAIN COMPANY ONLY (itemtype 263)**

```sql
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
WHERE i.itemtypenum = 263
ORDER BY i.itemnum DESC;
```

---

### **5. Search Vendor Invoices by Vendor Name**

```sql
SELECT 
    i.itemnum,
    it.itemtypename,
    CAST(ki106.keyvaluesmall AS VARCHAR(50)) as InvoiceNumber,
    ki112.keyvaluedate as InvoiceDate,
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
LEFT JOIN hsi.keyitem112 ki112 WITH (NOLOCK) 
    ON i.itemnum = ki112.itemnum
WHERE i.itemtypenum IN (263, 340, 364, 429)
  AND kt105.keyvaluechar LIKE '%CDW COMPUTER CENTERS%'
ORDER BY i.itemnum DESC;
```

---

## 🔑 **Key Differences**

| Aspect | Customer (AR) | Vendor (AP) |
|--------|---------------|-------------|
| **Document Types** | 102, 119 | 263, 340, 364, 429 |
| **Vendor Field** | ❌ Not available | ✅ Available in keytable105 |
| **Vendor JOIN** | Not needed | Required (keyxitem105 + keytable105) |
| **Use Case** | Sales invoices to customers | Purchase invoices from vendors |

---

## 📊 **Table Relationships**

### **Customer Documents (Simple)**
```
hsi.itemdata (itemtypenum = 102 or 119)
    ├─ hsi.keyitem106 (Invoice Number)
    ├─ hsi.keyitem112 (Invoice Date)
    └─ hsi.keytable104 (PO Number)
```

### **Vendor Documents (With Vendor)**
```
hsi.itemdata (itemtypenum = 263, 340, 364, 429)
    ├─ hsi.keyitem106 (Invoice Number)
    ├─ hsi.keyitem112 (Invoice Date)
    ├─ hsi.keytable104 (PO Number)
    └─ hsi.keyxitem105 ──→ hsi.keytable105 (Vendor Name) ✅
```

---

## ✅ **Summary**

- **Customer Invoices (102)** and **Credit Memos (119)** = NO vendor field
- **Vendor Invoices (263, 340, 364, 429)** = HAS vendor field in `keytable105`
- Use the appropriate query template based on document type
- All queries are available in `Scripts/FindVendorInTables.sql`


