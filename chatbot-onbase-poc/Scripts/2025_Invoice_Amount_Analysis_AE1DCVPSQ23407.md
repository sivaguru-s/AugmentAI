# 2025 Invoice Amount Analysis Report - Server AE1DCVPSQ23407

**Date:** 2026-03-13  
**Database:** Onbase (AE1DCVPSQ23407)  
**Analysis Focus:** Find vendors with 2025 invoices that have amount/cost data

---

## Executive Summary

**FINDING:** ✅ **SUCCESS! Found 285,810+ invoices from 2025 WITH amount data totaling $9.28 BILLION!**

This is a **MAJOR DIFFERENCE** from the previous server (aazeus-obdmsq01) which had NO 2025 invoices with amounts.

---

## Key Findings

### 📊 2025 Invoice Statistics

| Metric | Value |
|--------|-------|
| **Total Invoices with Amounts** | 285,810+ |
| **Total Amount** | $9,284,922,464.18 (over $9.28 billion) |
| **Date Range** | January 1, 2025 - December 31, 2025 |
| **Document Types** | AP Invoices (itemtype 743, 914) |
| **Amount Field** | keyitem293 (populated and active) |

### 📅 Date Range
- **Earliest Invoice:** January 1, 2025 00:56:13
- **Latest Invoice:** December 31, 2025 16:18:14
- **Full Year Coverage:** ✅ Complete 2025 data available

---

## Sample 2025 Invoices with Amounts

### Recent Invoices (December 2025)

| Item Number | Invoice Description | Date | Amount |
|-------------|---------------------|------|--------|
| 390026504 | 94 - AP94 - API853670 - 061225YTH - **V24277** | 06/12/2025 | **$57,000.00** |
| 387023070 | 01 - AP01 - API818390 - 122925AFI - **V12668** | 12/29/2025 | **$10,167.38** |
| 385832376 | 47 - AP47 - API807076 - 1QFM-1NRY-C1JK - 624328 | 12/31/2025 | **$102.35** |
| 385832331 | 11 - AP11 - API807075 - 123125-6019.00 - 633422 | 12/31/2025 | **$6,019.00** |
| 385829982 | 65 - AP65 - API807074 - 111825-400.00 - 659611 | 12/31/2025 | **$400.00** |
| 385829955 | 01 - AP01 - API807062 - 198789 - **V207** | 12/31/2025 | **$900.40** |

### Early 2025 Invoices (January 2025)

| Item Number | Invoice Description | Date | Amount |
|-------------|---------------------|------|--------|
| 357517853 | 01 - AP01 - API544458 - 827776 - **V21645** | 01/01/2025 | **$8,004.80** |
| 357517827 | 04 - AP04 - API300667 - 122408882 - 604589 | 01/01/2025 | **$138.33** |
| 357517824 | 01 - AP01 - API569656 - 116832194 - **V1176** | 01/01/2025 | **$2,513.36** |
| 357517821 | 01 - AP01 - API544457 - 3430738 - **V5048** | 01/01/2025 | **$11,520.00** |
| 357517818 | 01 - AP01 - API544456 - 3430737 - **V5048** | 01/01/2025 | **$5,760.00** |
| 357516683 | 47 - AP47 - API544357 - 25954360 - 618277 | 01/01/2025 | **$582,093.27** |

---

## Vendor Information

### How Vendors are Stored

On this server, vendor information is **embedded in the `itemname` field** rather than in a separate vendor table.

**Format:** `[AP Code] - [Invoice Number] - [Vendor Code] - [Amount] - Ap Invoices - [Date]`

### Vendor Code Analysis Results

**⚠️ IMPORTANT FINDING:** The vendor codes (e.g., V24277, V12668) embedded in the `itemname` field are **NOT directly mapped** to vendor names in the `hsi.keytable105` table on this Onbase server.

**What was found:**
1. ✅ **Vendor codes exist** in the `itemname` field (e.g., "V24277", "V12668", "V21645")
2. ✅ **Amount data is available** in `keyitem293` for 285,810+ invoices
3. ❌ **Vendor names are NOT stored** in Onbase for these vendor codes
4. ✅ **One vendor name found**: V207 = **SUPREME GRAPHICS** (found in itemnum 15435073)

### Vendor Codes Analyzed (2025 Invoices with Amounts)

| Vendor Code | Sample Invoice Amount | Vendor Name | Status |
|-------------|----------------------|-------------|--------|
| **V24277** | $57,000.00 | *Not found in Onbase* | ❌ |
| **V12668** | $10,167.38 | *Not found in Onbase* | ❌ |
| **V21645** | $8,004.80 | *Not found in Onbase* | ❌ |
| **V207** | $900.40, $480.55, $371.82, $725.00, $232.14 | **SUPREME GRAPHICS** | ✅ |
| **V5048** | $11,520.00, $5,760.00 | *Not found in Onbase* | ❌ |
| **V1176** | $2,513.36 | *Not found in Onbase* | ❌ |
| **V22917** | $9,542.40, $7,411.71 (multiple) | *Not found in Onbase* | ❌ |
| **V11623** | Numerous invoices | *Not found in Onbase* | ❌ |
| **V30074** | Multiple invoices | *Not found in Onbase* | ❌ |
| **V70080** | $139.24, $16.01 (multiple) | *Not found in Onbase* | ❌ |
| **V57** | Multiple invoices | *Not found in Onbase* | ❌ |
| **V1629** | Multiple invoices | *Not found in Onbase* | ❌ |
| **V27201** | Multiple large invoices | *Not found in Onbase* | ❌ |
| **V31340** | $79,958.27, $79,958.26, $79,958.27 | *Not found in Onbase* | ❌ |

### Vendor Codes with Numeric IDs

Many invoices use numeric vendor IDs instead of "V" prefix:
- **604589**, **606757**, **631414**, **624514**, **636957**, **626020**, **618277**, **647257**, **647420**, **645067**, **624328**, **649639**, **801789**, **803133**, etc.
- These numeric codes are also **NOT mapped** to vendor names in Onbase

### Where to Find Vendor Names

The vendor codes (V24277, V12668, etc.) are likely **master data from the source ERP/accounting system** (e.g., SAP, Oracle, JD Edwards, etc.) that feeds data into Onbase. To get the actual vendor names, you would need to:

1. **Query the source ERP system** using these vendor codes
2. **Contact the AP/Finance team** who manage vendor master data
3. **Check the ERP vendor master table** (e.g., `VENDOR`, `AP_VENDORS`, `SUPPLIERS`, etc.)

---

## Document Types with 2025 Amount Data

| Item Type | Description | Sample Count |
|-----------|-------------|--------------|
| **743** | Ap Invoices | Majority of invoices |
| **914** | ADS-Ap Invoices new | Significant volume |

---

## Comparison with Previous Server

| Server | 2025 Invoices with Amounts | Total Amount | Status |
|--------|---------------------------|--------------|--------|
| **aazeus-obdmsq01** | 0 | $0.00 | ❌ No data |
| **AE1DCVPSQ23407** | 285,810+ | $9.28 billion | ✅ Full data |

---

## Next Steps

### 1. Update Application Configuration
The application's `appsettings.json` has already been updated to point to **AE1DCVPSQ23407**.

### 2. Update Vendor Extraction Logic
The current application expects vendor names in `keytable105`, but this server stores vendor codes in the `itemname` field.

**Recommendation:** Update `OnbaseRepository.cs` to extract vendor codes from the `itemname` field using pattern matching.

### 3. Test Analytics Feature
With this server, the Advanced Invoice Analytics feature will now show:
- ✅ Total amounts
- ✅ Average invoice amounts
- ✅ Trend analysis
- ✅ Cost summaries

---

## SQL Queries Used

### Find 2025 Invoices with Amounts
```sql
SELECT i.itemnum, i.itemname, i.itemdate, ki293.keyvaluecurr AS Amount
FROM hsi.itemdata i WITH (NOLOCK)
INNER JOIN hsi.keyitem293 ki293 WITH (NOLOCK) ON i.itemnum = ki293.itemnum
WHERE i.itemtypenum IN (263, 340, 364, 429, 515, 743, 914)
  AND ki293.keyvaluecurr IS NOT NULL
  AND ki293.keyvaluecurr > 0
  AND i.itemdate >= '2025-01-01'
  AND i.itemdate < '2026-01-01'
ORDER BY i.itemnum DESC
```

### Count and Sum 2025 Invoices
```sql
SELECT COUNT(DISTINCT i.itemnum) AS InvoiceCount,
       SUM(ki293.keyvaluecurr) AS TotalAmount,
       MIN(i.itemdate) AS EarliestInvoice,
       MAX(i.itemdate) AS LatestInvoice
FROM hsi.itemdata i WITH (NOLOCK)
INNER JOIN hsi.keyitem293 ki293 WITH (NOLOCK) ON i.itemnum = ki293.itemnum
WHERE i.itemtypenum IN (743, 914)
  AND ki293.keyvaluecurr IS NOT NULL
  AND ki293.keyvaluecurr > 0
  AND i.itemdate >= '2025-01-01'
  AND i.itemdate < '2026-01-01'
```

---

**Analysis Complete - SUCCESS!** ✅

The new server **AE1DCVPSQ23407** has complete 2025 invoice data with amounts, totaling over $9.28 billion across 285,810+ invoices.

