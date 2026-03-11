# Onbase Schema Update - Complete Summary

## ✅ **What Was Fixed**

Based on your actual Onbase query, I've updated the entire repository to match your **real database schema**.

---

## 📊 **Your Actual Onbase Schema**

From your example query, your Onbase system uses:

### **Tables:**
- `hsi.itemdata` - Main document/item table
- `hsi.keyitem106` - Invoice Number keyword (numeric)
- `hsi.keyxitem104` - Cross-reference table for Order Number
- `hsi.keytable104` - Order Number keyword (character)
- `hsi.keyitem112` - Invoice Date keyword (date)

### **Key Fields:**
- **Invoice Number**: `keyitem106.keyvaluesmall` (INT)
- **Order Number**: `keytable104.keyvaluechar` (VARCHAR) - Used as "Vendor/PO Number"
- **Invoice Date**: `keyitem112.keyvaluedate` (DATETIME)
- **Item Type**: `itemdata.itemtypenum = 102` (Invoice document type)

---

## 🔄 **Changes Made to Repository**

### **File: `Data/OnbaseRepository.cs`**

All SQL queries have been rewritten to match your exact schema:

#### **Before (Generic Onbase Schema):**
```sql
FROM hsi.rmitem i
LEFT JOIN hsi.rmkeyitem kt1 ON i.itemnum = kt1.itemnum 
    AND kt1.keywordnum = (SELECT keywordnum FROM hsi.rmkeyword WHERE keywordname = 'Invoice Number')
```

#### **After (Your Actual Schema):**
```sql
FROM hsi.itemdata i
LEFT OUTER JOIN hsi.keyitem106 ki106 ON i.itemnum = ki106.itemnum
LEFT OUTER JOIN hsi.keyxitem104 kx104 ON i.itemnum = kx104.itemnum
LEFT OUTER JOIN hsi.keytable104 kt104 ON kx104.keywordnum = kt104.keywordnum
LEFT OUTER JOIN hsi.keyitem112 ki112 ON i.itemnum = ki112.itemnum
WHERE i.itemtypenum = 102
```

### **Updated Methods:**

1. **SearchInvoicesAsync** - General search across invoice number, order number, and description
2. **GetInvoicesByVendorAsync** - Search by Order Number (keytable104)
3. **GetInvoicesByNumberAsync** - Search by Invoice Number (keyitem106)
4. **GetInvoicesByDateRangeAsync** - Search by Invoice Date (keyitem112)
5. **GetInvoicesByAmountRangeAsync** - Search by Invoice Number range (assuming it represents amount)
6. **GetInvoicesByStatusAsync** - Returns all invoices (status field not available)
7. **GetAllInvoicesAsync** - Get all invoices with limit

---

## 🎯 **Field Mappings**

| Chatbot Field | Onbase Source | Data Type |
|--------------|---------------|-----------|
| InvoiceId | `itemdata.itemnum` | INT |
| InvoiceNumber | `keyitem106.keyvaluesmall` | INT → VARCHAR |
| VendorName / PONumber | `keytable104.keyvaluechar` | VARCHAR |
| Amount | `keyitem106.keyvaluesmall` | INT → DECIMAL |
| InvoiceDate | `keyitem112.keyvaluedate` | DATETIME |
| Description | `itemdata.itemname` | VARCHAR |
| DocumentType | 'Invoice' (hardcoded) | VARCHAR |
| CreatedDate | `itemdata.itemdate` | DATETIME |
| DueDate | NULL (not available) | - |
| Status | NULL (not available) | - |

---

## 🧪 **Testing the Chatbot**

The application is now running on **http://localhost:5060**

### **Try These Queries:**

1. **Search by Invoice Number:**
   - "Find invoice 61304208"
   - "Show me invoice number 61304208"

2. **Search by Order Number:**
   - "Find order D264904"
   - "Show invoices for order D264904"

3. **Search by Date:**
   - "Show invoices from November 2024"
   - "Find invoices between 2024-11-01 and 2024-11-30"

4. **General Search:**
   - "Show all invoices"
   - "Find recent invoices"

---

## ⚠️ **Important Notes**

### **1. Invoice Number as Amount**
Since your schema doesn't have a separate amount field, I'm using `keyitem106.keyvaluesmall` for both:
- **InvoiceNumber** (displayed as text)
- **Amount** (displayed as decimal)

If you have a different keyword table for amount, please let me know the table name (e.g., `keyitem###`).

### **2. Missing Fields**
The following fields are not available in your current schema:
- **Status** - Returns NULL
- **DueDate** - Returns NULL

If these exist in other keyword tables, please share the table names.

### **3. Vendor Name**
Currently using **Order Number** (`keytable104.keyvaluechar`) as the vendor/customer identifier. If you have a separate vendor name field, please provide the keyword table number.

---

## 📝 **Example Query from Your System**

Your original query:
```sql
DECLARE @InvoiceNumber INT = 61304208
DECLARE @OrderNumber VARCHAR(20) = 'D264904'
DECLARE @InvoiceDate DATETIME = '2024-11-20 00:00:00.000'

SELECT itemdata.itemnum 
FROM hsi.itemdata
LEFT OUTER JOIN hsi.keyitem106 ON (itemdata.itemnum = keyitem106.itemnum)
LEFT OUTER JOIN hsi.keyxitem104 ON (itemdata.itemnum = keyxitem104.itemnum)
LEFT OUTER JOIN hsi.keytable104 ON (keyxitem104.keywordnum = keytable104.keywordnum)
LEFT OUTER JOIN hsi.keyitem112 ON (itemdata.itemnum = keyitem112.itemnum)
WHERE keyitem106.keyvaluesmall = @InvoiceNumber
  AND keytable104.keyvaluechar = @OrderNumber
  AND keyitem112.keyvaluedate = @InvoiceDate
  AND itemdata.itemtypenum = 102
```

This exact pattern is now used in all repository queries!

---

## 🚀 **Next Steps**

1. **Test the chatbot** with real data from your database
2. **Verify the results** match what you expect
3. **Let me know if you need additional keyword fields** (provide table names like `keyitem###`)
4. **Report any issues** and I'll fix them immediately

---

## 📞 **Need More Fields?**

If you have additional keyword tables for:
- **Vendor Name** (separate from Order Number)
- **Invoice Amount** (separate from Invoice Number)
- **Status**
- **Due Date**
- **Any other fields**

Just provide the keyword table names (e.g., `keyitem120`, `keytable105`, etc.) and I'll add them!

