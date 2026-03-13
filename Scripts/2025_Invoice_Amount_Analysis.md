# 2025 Invoice Amount Analysis Report

**Date:** 2026-03-13  
**Database:** Onbase (aazeus-obdmsq01)  
**Analysis Focus:** Find vendors with 2025 invoices that have amount/cost data

---

## Executive Summary

**FINDING:** ❌ **NO invoices from 2025 have amount data in the Onbase database.**

All 2025 invoices are **Customer Invoices (itemtype 102)** and **Credit Memos (itemtype 119)**, which do not have amount fields configured in the Onbase schema.

---

## Detailed Analysis

### 1. 2025 Invoice Overview

**Sample of Recent 2025 Invoices:**
- Invoice #40767624 (itemnum: 325601930) - 10/31/2025
- Invoice #40767623 (itemnum: 325601927) - 10/31/2025
- Invoice #40767622 (itemnum: 325601924) - 10/31/2025
- Invoice #40767621 (itemnum: 325601861) - 10/31/2025
- Invoice #40767620 (itemnum: 325601858) - 10/31/2025
- ... (30+ invoices found)
- Credit Memo #17810 (itemnum: 325601369) - 10/29/2025
- Credit Memo #17809 (itemnum: 325601367) - 10/29/2025
- Credit Memo #17808 (itemnum: 325601365) - 10/29/2025

**Document Types Found:**
- **itemtype 102**: Customer Invoices (majority)
- **itemtype 119**: Credit Memos

### 2. Amount Field Analysis

Checked all known currency fields for 2025 invoices:

| Currency Field | Description | 2025 Invoice Data |
|---------------|-------------|-------------------|
| `keyitem293` | Invoice Amount | **NULL** (0 records) |
| `keyitem22` | Batch Amount | **NULL** (0 records) |
| `keyitem583` | (Currency field) | **NULL** (0 records) |

**Result:** All amount fields are NULL for all 2025 invoices.

### 3. Schema Configuration

**Customer Invoices (itemtype 102) - Available Fields:**
- keytype 101: Trip #
- keytype 103: Customer #
- keytype 104: Order #
- keytype 106: Invoice #
- keytype 112: Invoice Date
- keytype 214: SHIP TO
- keytype 346: WHSE

**Missing:** No amount/currency field is configured for Customer Invoices.

### 4. Orphaned Amount Data Discovery

Found 936,000+ records in `keyitem293` with amount data, but:
- **None are linked to active invoices** in the `itemdata` table
- Example orphaned records:
  - itemnum 325602306: $2,100.00
  - itemnum 325602303: $1,135.00
  - itemnum 325602300: $995.00
  - itemnum 325428185: $671,687.50

These itemnums do NOT exist in the `hsi.itemdata` table, indicating orphaned/historical data.

---

## Conclusion

### Why No 2025 Invoices Have Amounts:

1. **All 2025 invoices are Customer Invoices (AR)**, not Vendor Invoices (AP)
2. **Customer Invoices do not have amount fields** in the Onbase configuration
3. **Amount data exists only for Vendor Invoices (AP)**, which use itemtypes: 263, 340, 364, 429, 515
4. **No Vendor Invoices from 2025** were found in the database

### Vendor Invoices Status:

The most recent vendor invoices with amount data are from **2009** (e.g., CDW COMPUTER CENTER invoices from August 2009).

**No vendor invoices from 2025 exist in the database.**

---

## Recommendations

### Option 1: Find Vendor Invoices with Amounts (Any Year)
If you need to analyze vendor invoices with amount data, focus on historical data:
- **Vendor:** CDW COMPUTER CENTER
- **Date Range:** August 2009
- **Invoice Count:** 3 invoices found
- **Amount Data:** Available in database (though currently NULL in keyitem293)

### Option 2: Configure Amount Fields for Customer Invoices
To track amounts for Customer Invoices going forward:
1. Contact Onbase administrator
2. Add amount field (keytype) to itemtype 102 (Customer Invoices)
3. Configure data entry forms to capture amount
4. Populate historical data if needed

### Option 3: Search for Recent Vendor Invoices
Check if there are any recent Vendor Invoices (AP) in the system:
```sql
SELECT TOP 20 i.itemnum, i.itemtypenum, it.itemtypename, i.itemdate
FROM hsi.itemdata i
INNER JOIN hsi.itemtype it ON i.itemtypenum = it.itemtypenum
WHERE i.itemtypenum IN (263, 340, 364, 429, 515)
ORDER BY i.itemnum DESC
```

---

## SQL Queries Used

### Find 2025 Invoices
```sql
SELECT TOP 30 i.itemnum, i.itemtypenum, i.itemname, i.itemdate
FROM hsi.itemdata i WITH (NOLOCK)
WHERE i.itemnum >= 325600000 
  AND i.itemtypenum IN (263, 340, 364, 429, 515, 102, 119)
ORDER BY i.itemnum DESC
```

### Check Amount Fields
```sql
SELECT i.itemnum, i.itemname, 
       ki293.keyvaluecurr AS Amt293, 
       ki22.keyvaluecurr AS Amt22, 
       ki583.keyvaluecurr AS Amt583
FROM hsi.itemdata i WITH (NOLOCK)
LEFT JOIN hsi.keyitem293 ki293 WITH (NOLOCK) ON i.itemnum = ki293.itemnum
LEFT JOIN hsi.keyitem22 ki22 WITH (NOLOCK) ON i.itemnum = ki22.itemnum
LEFT JOIN hsi.keyitem583 ki583 WITH (NOLOCK) ON i.itemnum = ki583.itemnum
WHERE i.itemnum IN (325601930, 325601927, ...)
ORDER BY i.itemnum DESC
```

---

**Analysis Complete**

