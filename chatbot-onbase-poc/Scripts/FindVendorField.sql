-- ========================================
-- FIND VENDOR FIELD IN ONBASE DATABASE
-- ========================================
-- Run this script to identify which table contains vendor information
-- Replace 61304208 with any known invoice number from your system

-- ========================================
-- STEP 1: List all available keyitem tables
-- ========================================
SELECT
    TABLE_NAME
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_SCHEMA = 'hsi'
  AND TABLE_NAME LIKE 'keyitem%'
ORDER BY TABLE_NAME;

-- ========================================
-- STEP 2: List all available keytable tables
-- ========================================
SELECT
    TABLE_NAME
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_SCHEMA = 'hsi'
  AND TABLE_NAME LIKE 'keytable%'
ORDER BY TABLE_NAME;

-- ========================================
-- STEP 3: Check what data exists for a known invoice
-- ========================================
-- First, get the itemnum for invoice 61304208
SELECT 
    itemnum,
    keyvaluesmall as InvoiceNumber
FROM hsi.keyitem106
WHERE keyvaluesmall = 61304208;

-- ========================================
-- STEP 4: Check common keyitem tables for this invoice
-- ========================================
-- Replace @itemnum with the actual itemnum from STEP 3

-- Example: If itemnum = 12345, then run these queries:

-- Check keyitem100 (might be vendor)
-- SELECT * FROM hsi.keyitem100 WHERE itemnum = 12345;

-- Check keyitem101
-- SELECT * FROM hsi.keyitem101 WHERE itemnum = 12345;

-- Check keyitem102
-- SELECT * FROM hsi.keyitem102 WHERE itemnum = 12345;

-- Check keyitem103
-- SELECT * FROM hsi.keyitem103 WHERE itemnum = 12345;

-- Check keyitem104
-- SELECT * FROM hsi.keyitem104 WHERE itemnum = 12345;

-- Check keyitem105
-- SELECT * FROM hsi.keyitem105 WHERE itemnum = 12345;

-- Check keyitem107
-- SELECT * FROM hsi.keyitem107 WHERE itemnum = 12345;

-- Check keyitem108
-- SELECT * FROM hsi.keyitem108 WHERE itemnum = 12345;

-- Check keyitem109
-- SELECT * FROM hsi.keyitem109 WHERE itemnum = 12345;

-- Check keyitem110
-- SELECT * FROM hsi.keyitem110 WHERE itemnum = 12345;

-- ========================================
-- STEP 5: Check keytable entries
-- ========================================
-- Check if there are other keytable entries besides 104

-- SELECT kt.*, kx.itemnum
-- FROM hsi.keytable105 kt
-- INNER JOIN hsi.keyxitem105 kx ON kt.keywordnum = kx.keywordnum
-- WHERE kx.itemnum = 12345;

-- ========================================
-- STEP 6: QUICK TEST - Check all data for invoice
-- ========================================
-- This shows all currently mapped data
SELECT 
    i.itemnum,
    i.itemname as Description,
    i.itemdate as CreatedDate,
    CAST(ki106.keyvaluesmall AS VARCHAR(50)) as InvoiceNumber,
    ki112.keyvaluedate as InvoiceDate,
    kt104.keyvaluechar as OrderNumber
FROM hsi.itemdata i WITH (NOLOCK)
LEFT JOIN hsi.keyitem106 ki106 WITH (NOLOCK) ON i.itemnum = ki106.itemnum
LEFT JOIN hsi.keyitem112 ki112 WITH (NOLOCK) ON i.itemnum = ki112.itemnum
LEFT JOIN hsi.keyxitem104 kx104 WITH (NOLOCK) ON i.itemnum = kx104.itemnum
LEFT JOIN hsi.keytable104 kt104 WITH (NOLOCK) ON kx104.keywordnum = kt104.keywordnum
WHERE i.itemtypenum = 102
  AND ki106.keyvaluesmall = 61304208;

-- ========================================
-- INSTRUCTIONS:
-- ========================================
-- 1. Run STEP 1 and STEP 2 to see what tables exist
-- 2. Run STEP 3 to get the itemnum for invoice 61304208
-- 3. Uncomment and run the queries in STEP 4, replacing 12345 with the actual itemnum
-- 4. Look for fields that contain vendor names (usually text/varchar fields)
-- 5. Once you find the vendor field, share the table name and I'll update the code
--
-- Common patterns:
-- - keyitem tables usually have: keyvaluesmall (numeric), keyvaluechar (text), keyvaluedate (date)
-- - keytable tables usually have: keyvaluechar (text)
-- - Vendor names are typically in keyvaluechar columns

