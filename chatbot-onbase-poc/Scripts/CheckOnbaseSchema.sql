-- Script to check Onbase database schema and find vendor fields
-- This script works with the numbered table schema (keyitem106, keytable104, etc.)

-- ========================================
-- PART 1: FIND ALL HSI TABLES
-- ========================================

-- List all tables in the hsi schema to see what's available
SELECT
    TABLE_NAME,
    TABLE_TYPE
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_SCHEMA = 'hsi'
  AND (TABLE_NAME LIKE 'keyitem%'
    OR TABLE_NAME LIKE 'keytable%'
    OR TABLE_NAME LIKE 'keyxitem%')
ORDER BY TABLE_NAME;

-- ========================================
-- PART 2: CHECK COLUMNS IN KEYITEM TABLES
-- ========================================

-- See what columns exist in keyitem tables (to find vendor, amount, etc.)
SELECT
    TABLE_NAME,
    COLUMN_NAME,
    DATA_TYPE,
    CHARACTER_MAXIMUM_LENGTH
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_SCHEMA = 'hsi'
  AND TABLE_NAME LIKE 'keyitem%'
ORDER BY TABLE_NAME, ORDINAL_POSITION;

-- ========================================
-- PART 3: SAMPLE DATA FROM KNOWN INVOICE
-- ========================================

-- Get the itemnum for invoice 61304208
DECLARE @InvoiceItemNum INT;
SELECT @InvoiceItemNum = itemnum FROM hsi.keyitem106 WHERE keyvaluesmall = 61304208;

SELECT 'Invoice ItemNum: ' + CAST(@InvoiceItemNum AS VARCHAR(20)) as Info;

-- ========================================
-- PART 4: CHECK ALL KEYITEM TABLES (100-120)
-- ========================================

-- Check each keyitem table to see if it has data for our invoice
-- This will help us find vendor, amount, and other fields

-- keyitem100
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'hsi' AND TABLE_NAME = 'keyitem100')
    SELECT 'keyitem100' as TableName, * FROM hsi.keyitem100 WHERE itemnum = @InvoiceItemNum;

-- keyitem101
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'hsi' AND TABLE_NAME = 'keyitem101')
    SELECT 'keyitem101' as TableName, * FROM hsi.keyitem101 WHERE itemnum = @InvoiceItemNum;

-- keyitem102
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'hsi' AND TABLE_NAME = 'keyitem102')
    SELECT 'keyitem102' as TableName, * FROM hsi.keyitem102 WHERE itemnum = @InvoiceItemNum;

-- keyitem103
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'hsi' AND TABLE_NAME = 'keyitem103')
    SELECT 'keyitem103' as TableName, * FROM hsi.keyitem103 WHERE itemnum = @InvoiceItemNum;

-- keyitem104
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'hsi' AND TABLE_NAME = 'keyitem104')
    SELECT 'keyitem104' as TableName, * FROM hsi.keyitem104 WHERE itemnum = @InvoiceItemNum;

-- keyitem105
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'hsi' AND TABLE_NAME = 'keyitem105')
    SELECT 'keyitem105' as TableName, * FROM hsi.keyitem105 WHERE itemnum = @InvoiceItemNum;

-- keyitem107
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'hsi' AND TABLE_NAME = 'keyitem107')
    SELECT 'keyitem107' as TableName, * FROM hsi.keyitem107 WHERE itemnum = @InvoiceItemNum;

-- keyitem108
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'hsi' AND TABLE_NAME = 'keyitem108')
    SELECT 'keyitem108' as TableName, * FROM hsi.keyitem108 WHERE itemnum = @InvoiceItemNum;

-- keyitem109
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'hsi' AND TABLE_NAME = 'keyitem109')
    SELECT 'keyitem109' as TableName, * FROM hsi.keyitem109 WHERE itemnum = @InvoiceItemNum;

-- keyitem110
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'hsi' AND TABLE_NAME = 'keyitem110')
    SELECT 'keyitem110' as TableName, * FROM hsi.keyitem110 WHERE itemnum = @InvoiceItemNum;

-- keyitem111
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'hsi' AND TABLE_NAME = 'keyitem111')
    SELECT 'keyitem111' as TableName, * FROM hsi.keyitem111 WHERE itemnum = @InvoiceItemNum;

-- keyitem113
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'hsi' AND TABLE_NAME = 'keyitem113')
    SELECT 'keyitem113' as TableName, * FROM hsi.keyitem113 WHERE itemnum = @InvoiceItemNum;

-- keyitem114
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'hsi' AND TABLE_NAME = 'keyitem114')
    SELECT 'keyitem114' as TableName, * FROM hsi.keyitem114 WHERE itemnum = @InvoiceItemNum;

-- keyitem115
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'hsi' AND TABLE_NAME = 'keyitem115')
    SELECT 'keyitem115' as TableName, * FROM hsi.keyitem115 WHERE itemnum = @InvoiceItemNum;

-- ========================================
-- PART 5: CHECK KEYTABLE TABLES
-- ========================================

-- Check keytable entries linked to our invoice via keyxitem tables
SELECT 'keytable104' as TableName, kt.*, kx.itemnum
FROM hsi.keytable104 kt
INNER JOIN hsi.keyxitem104 kx ON kt.keywordnum = kx.keywordnum
WHERE kx.itemnum = @InvoiceItemNum;

-- ========================================
-- PART 6: SIMPLE QUERY - ALL DATA FOR INVOICE
-- ========================================

-- This query shows ALL available data for invoice 61304208
SELECT
    i.itemnum,
    i.itemname,
    i.itemdate,
    ki106.keyvaluesmall as InvoiceNumber,
    ki112.keyvaluedate as InvoiceDate,
    kt104.keyvaluechar as OrderNumber
FROM hsi.itemdata i WITH (NOLOCK)
LEFT JOIN hsi.keyitem106 ki106 WITH (NOLOCK) ON i.itemnum = ki106.itemnum
LEFT JOIN hsi.keyitem112 ki112 WITH (NOLOCK) ON i.itemnum = ki112.itemnum
LEFT JOIN hsi.keyxitem104 kx104 WITH (NOLOCK) ON i.itemnum = kx104.itemnum
LEFT JOIN hsi.keytable104 kt104 WITH (NOLOCK) ON kx104.keywordnum = kt104.keywordnum
WHERE i.itemtypenum = 102
  AND ki106.keyvaluesmall = 61304208;

