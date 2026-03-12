-- ============================================================================
-- Onbase Document Type Query Reference
-- ============================================================================
-- Purpose: Query templates for Customer (AR) and Vendor (AP) documents
-- Date: March 12, 2026
-- ============================================================================

USE Onbase;
GO

PRINT '============================================================================';
PRINT 'ONBASE DOCUMENT TYPE QUERIES';
PRINT '============================================================================';
PRINT '';

-- ============================================================================
-- SECTION 1: CUSTOMER DOCUMENTS (AR - Accounts Receivable)
-- ============================================================================

PRINT '--- SECTION 1: CUSTOMER INVOICES (itemtype 102) ---';
PRINT '';

-- Query 1A: Customer Invoices (NO vendor field)
SELECT TOP 10
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

PRINT '';
PRINT '--- SECTION 2: CUSTOMER CREDIT MEMOS (itemtype 119) ---';
PRINT '';

-- Query 1B: Customer Credit Memos (NO vendor field)
SELECT TOP 10
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

PRINT '';
PRINT '============================================================================';
PRINT 'SECTION 2: VENDOR DOCUMENTS (AP - Accounts Payable)';
PRINT '============================================================================';
PRINT '';

PRINT '--- SECTION 3: VENDOR INVOICES - ALL COMPANIES (itemtype 263, 340, 364, 429) ---';
PRINT '';

-- Query 2A: Vendor Invoices - ALL COMPANIES (HAS vendor field)
SELECT TOP 10
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

PRINT '';
PRINT '--- SECTION 4: VENDOR INVOICES - MAIN COMPANY ONLY (itemtype 263) ---';
PRINT '';

-- Query 2B: Vendor Invoices - MAIN COMPANY ONLY
SELECT TOP 10
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

PRINT '';
PRINT '--- SECTION 5: SEARCH VENDOR INVOICES BY VENDOR NAME ---';
PRINT '';

-- Query 2C: Find CDW Vendor Invoices
SELECT TOP 10
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

PRINT '';
PRINT '============================================================================';
PRINT 'SECTION 3: DOCUMENT TYPE SUMMARY';
PRINT '============================================================================';
PRINT '';

-- Query 3: Count Documents by Type
SELECT
    i.itemtypenum,
    it.itemtypename,
    COUNT(*) as TotalDocuments,
    COUNT(DISTINCT kx105.itemnum) as DocumentsWithVendor,
    CASE
        WHEN COUNT(*) > 0
        THEN CAST((COUNT(DISTINCT kx105.itemnum) * 100.0 / COUNT(*)) AS DECIMAL(5,2))
        ELSE 0
    END as VendorCoveragePercent
FROM hsi.itemdata i WITH (NOLOCK)
INNER JOIN hsi.itemtype it WITH (NOLOCK)
    ON i.itemtypenum = it.itemtypenum
LEFT JOIN hsi.keyxitem105 kx105 WITH (NOLOCK)
    ON i.itemnum = kx105.itemnum
WHERE i.itemtypenum IN (102, 119, 263, 340, 364, 429)
GROUP BY i.itemtypenum, it.itemtypename
ORDER BY i.itemtypenum;

PRINT '';
PRINT '============================================================================';
PRINT 'QUERY EXECUTION COMPLETE';
PRINT '============================================================================';
PRINT '';
PRINT 'DOCUMENT TYPE REFERENCE:';
PRINT '  102 = Customer Invoices (AR) - NO vendor field';
PRINT '  119 = Customer Credit Memos (AR) - NO vendor field';
PRINT '  263 = Vendor Invoices (AP) - HAS vendor field in keytable105';
PRINT '  340 = Vendor Invoices Company 5 (AP) - HAS vendor field';
PRINT '  364 = Vendor Invoices Company 6 (AP) - HAS vendor field';
PRINT '  429 = Vendor Invoices Company 9 (AP) - HAS vendor field';
PRINT '';
PRINT 'VENDOR FIELD LOCATION:';
PRINT '  Table: hsi.keytable105';
PRINT '  Column: keyvaluechar';
PRINT '  Link: hsi.keyxitem105 (itemnum -> keywordnum)';
PRINT '';
PRINT '============================================================================';

