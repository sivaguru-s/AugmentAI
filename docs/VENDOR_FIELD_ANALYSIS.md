# Finding Vendor Field in Onbase Database

## 🎯 **Goal**
Identify which Onbase table contains the **Vendor Name** field so we can add it to the chatbot results.

---

## ⚠️ **Problem**
The standard Onbase metadata table `hsi.rmkeyword` doesn't exist in your database. Your Onbase installation uses a **numbered table schema** (e.g., `keyitem106`, `keytable104`).

---

## 🔍 **Current Known Mappings**

| Field | Onbase Table | Column | Example Value |
|-------|--------------|--------|---------------|
| Invoice Number | `hsi.keyitem106` | `keyvaluesmall` | 61304208 |
| Invoice Date | `hsi.keyitem112` | `keyvaluedate` | 2024-11-20 |
| Order Number (PO) | `hsi.keytable104` | `keyvaluechar` | 'D264904' |
| Description | `hsi.itemdata` | `itemname` | (document name) |

---

## 📋 **Option 1: Run PowerShell Script (Easiest)**

I've created a PowerShell script that will automatically check all keyitem tables for your invoice.

### **Steps:**

1. Open PowerShell in the project directory
2. Run the script:
   ```powershell
   .\Scripts\AnalyzeOnbaseSchema.ps1 -InvoiceNumber 61304208
   ```

3. The script will output all fields found for that invoice
4. Look for any field that contains vendor/supplier name
5. Share the results with me

---

## 📋 **Option 2: Run SQL Queries Manually**

If you prefer to run SQL queries directly, use the file `Scripts/FindVendorField.sql`.

### **Steps:**

1. Open SQL Server Management Studio (SSMS)
2. Connect to: `aazeus-obdmsq01` → `Onbase` database
3. Open the file: `Scripts/FindVendorField.sql`
4. Run **STEP 3** first to get the `itemnum` for invoice 61304208
5. Then uncomment and run the queries in **STEP 4**, replacing `12345` with the actual `itemnum`
6. Look for any `keyitem` table that has vendor name data

---

## 🔎 **What to Look For**

When reviewing the results, look for:

### **Vendor Name Indicators:**
- A `keyvaluechar` or `keyvaluesmall` column containing text like:
  - Company names
  - Supplier names
  - Vendor codes
  - Any text that looks like a vendor identifier

### **Amount Indicators:**
- A `keyvaluesmall` or `keyvaluenumeric` column containing:
  - Decimal numbers (e.g., 1234.56)
  - Large numbers that could be invoice amounts
  - Numbers different from the invoice number

### **Common Table Numbers:**
Based on typical Onbase configurations, vendor info is often in:
- `keyitem100` - `keyitem105`
- `keyitem107` - `keyitem111`
- `keytable105` - `keytable110`

---

## 📊 **Example Output**

When you run the script or queries, you might see something like:

```
keyitem103:
  itemnum: 123456
  keyvaluesmall: 0
  keyvaluechar: "ABC Corporation"  ← This could be vendor name!

keyitem108:
  itemnum: 123456
  keyvaluesmall: 15234.50  ← This could be amount!
  keyvaluechar: NULL
```

---

## ✅ **Once You Find the Vendor Field**

Share with me:
1. **Table name** (e.g., `keyitem103`)
2. **Column name** (e.g., `keyvaluechar`)
3. **Sample value** (e.g., "ABC Corporation")

I will then:
1. Add the appropriate JOIN to `OnbaseRepository.cs`
2. Map it to the `VendorName` field in the Invoice model
3. Rebuild and restart the application
4. Test to confirm vendor names appear in search results

---

## 🚀 **Quick Start**

**Fastest way to find vendor field:**

```powershell
# Run this in PowerShell from the project directory:
.\Scripts\AnalyzeOnbaseSchema.ps1 -InvoiceNumber 61304208
```

Then share the output with me! 📤

---

## 📝 **Alternative: Check Your Original Query**

If you have access to the original query or report that shows vendor names in Onbase, check:
- What table is being joined?
- What column contains the vendor name?
- What is the keyitem or keytable number?

This is often the fastest way to identify the correct field!

---

## 🆘 **Need Help?**

If you're unsure about the results, just share:
- The output from the PowerShell script, OR
- Screenshots of the SQL query results

I'll help you identify which field is the vendor name! 👍

