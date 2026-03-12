# Vendor Field Discovery Guide

## 📋 **Overview**

This guide will help you find the vendor field in your Onbase database and add it to the chatbot application.

---

## 🔍 **Step 1: Run the Schema Analysis Script**

### **Option A: Using PowerShell Script**

```powershell
cd C:\Chatbot-Onbase
.\Scripts\AnalyzeOnbaseSchema.ps1 -InvoiceNumber 61304208
```

This script will:
- Find the `itemnum` for invoice 61304208
- Check all `keyitem` tables (100-120) for data related to this invoice
- Check all `keytable` tables for data related to this invoice
- Display all fields and values

### **Option B: Using SQL Queries**

Run these queries in SQL Server Management Studio:

```sql
-- Step 1: Get itemnum for a known invoice
SELECT itemnum, keyvaluesmall as InvoiceNumber 
FROM hsi.keyitem106 
WHERE keyvaluesmall = 61304208;

-- Step 2: Replace @itemnum with the result from Step 1
DECLARE @itemnum INT = 12345; -- Replace with actual itemnum

-- Step 3: Check all keyitem tables
SELECT * FROM hsi.keyitem100 WHERE itemnum = @itemnum;
SELECT * FROM hsi.keyitem101 WHERE itemnum = @itemnum;
SELECT * FROM hsi.keyitem102 WHERE itemnum = @itemnum;
SELECT * FROM hsi.keyitem103 WHERE itemnum = @itemnum;
SELECT * FROM hsi.keyitem104 WHERE itemnum = @itemnum;
SELECT * FROM hsi.keyitem105 WHERE itemnum = @itemnum;
SELECT * FROM hsi.keyitem107 WHERE itemnum = @itemnum;
SELECT * FROM hsi.keyitem108 WHERE itemnum = @itemnum;
SELECT * FROM hsi.keyitem109 WHERE itemnum = @itemnum;
SELECT * FROM hsi.keyitem110 WHERE itemnum = @itemnum;
-- Continue for other tables...

-- Step 4: Check keytable tables (example for keytable105)
SELECT kt.*, kx.itemnum
FROM hsi.keytable105 kt
INNER JOIN hsi.keyxitem105 kx ON kt.keywordnum = kx.keywordnum
WHERE kx.itemnum = @itemnum;
```

---

## 🔎 **Step 2: Identify the Vendor Field**

Look for fields that contain vendor names. Common patterns:

### **In keyitem tables:**
- `keyvaluechar` - Text field (most likely for vendor names)
- `keyvaluesmall` - Numeric field (unlikely for vendor)
- `keyvaluedate` - Date field (unlikely for vendor)

### **In keytable tables:**
- `keyvaluechar` - Text field (most likely for vendor names)

### **Example Output:**
```
Table: keyitem105
  itemnum = 12345
  keyvaluechar = "ABC Corporation"  ← This looks like a vendor!
  keywordnum = 105
```

---

## ✅ **Step 3: Update the Application**

Once you've identified the vendor field, share the information in this format:

**Example:**
```
Vendor field found in: keyitem105.keyvaluechar
Sample value: "ABC Corporation"
```

Then I will update the `OnbaseRepository.cs` file to include the vendor field.

---

## 🛠️ **Step 4: Code Update (I will do this)**

Based on your findings, I will update the SQL query in `OnbaseRepository.cs`:

### **If vendor is in a keyitem table (e.g., keyitem105):**
```csharp
LEFT JOIN hsi.keyitem105 ki105 WITH (NOLOCK) 
    ON i.itemnum = ki105.itemnum
```

And map it:
```csharp
ki105.keyvaluechar as VendorName
```

### **If vendor is in a keytable table (e.g., keytable105):**
```csharp
LEFT JOIN hsi.keyxitem105 kx105 WITH (NOLOCK) 
    ON i.itemnum = kx105.itemnum
LEFT JOIN hsi.keytable105 kt105 WITH (NOLOCK) 
    ON kx105.keywordnum = kt105.keywordnum
```

And map it:
```csharp
kt105.keyvaluechar as VendorName
```

---

## 📊 **Common Onbase Field Patterns**

| Field Type | Typical Location | Example |
|------------|------------------|---------|
| **Vendor Name** | keyitem10X.keyvaluechar or keytable10X.keyvaluechar | "ABC Corp" |
| **Invoice Amount** | keyitem10X.keyvaluesmall or keyvaluebig | 1234.56 |
| **Due Date** | keyitem10X.keyvaluedate | 2024-12-31 |
| **Status** | keyitem10X.keyvaluechar | "Approved" |

---

## 🧪 **Step 5: Test the Changes**

After I update the code:

1. Restart the application
2. Search for invoice 61304208
3. Verify that the vendor name appears correctly

---

## 📝 **Example: Complete Discovery Process**

### **1. Run PowerShell Script:**
```powershell
.\Scripts\AnalyzeOnbaseSchema.ps1 -InvoiceNumber 61304208
```

### **2. Review Output:**
```
Step 2: Checking keyitem tables for itemnum 12345...
  Table: keyitem105
    itemnum = 12345
    keywordnum = 105
    keyvaluechar = ABC Corporation  ← VENDOR FOUND!
```

### **3. Share with Me:**
```
Vendor field: keyitem105.keyvaluechar
Value: "ABC Corporation"
```

### **4. I Update the Code:**
- Add JOIN to keyitem105
- Map keyvaluechar to VendorName
- Rebuild and restart

### **5. Test:**
```
Find invoice 61304208
→ Shows: Vendor: ABC Corporation ✅
```

---

## ❓ **Troubleshooting**

### **Problem: Script fails with "Invalid object name"**
**Solution:** The table doesn't exist. Try the next table number.

### **Problem: No data found for the invoice**
**Solution:** Try a different invoice number that you know exists.

### **Problem: Multiple fields look like vendor names**
**Solution:** Share all of them with me, and we'll determine which is correct.

---

## 🎯 **Ready to Start?**

Run this command now:

```powershell
cd C:\Chatbot-Onbase
.\Scripts\AnalyzeOnbaseSchema.ps1 -InvoiceNumber 61304208
```

Then share the output with me, and I'll update the application! 🚀

