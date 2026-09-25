-- =====================================================================
-- Chatbot-Onbase-POC : SQL queries mapped to Questions.md (10 prompts)
-- Verified against STAGE DB: aazeus-obdmsq01 / Onbase   (2026-09-24)
--
-- Confirmed schema facts used below:
--   hsi.itemdata      -> itemnum, itemtypenum, itemname, itemdate
--   hsi.keyitem106    -> Invoice Number   (keyvaluesmall)
--   hsi.keyitem112    -> Invoice Date     (keyvaluedate)
--   hsi.keyitem293    -> Amount           (keyvaluecurr)
--   hsi.keyxitem104/keytable104 -> PO Number
--   hsi.keyxitem105/keytable105 -> Vendor Name (old AP types only)
--   AP (vendor) invoice item types: 263,340,364,429,515 (legacy, no amount
--     data on stage) and 743,914 (current, HAVE amount data on stage:
--     750,337 rows / $44.25B, dated 2012-01-25 .. 2023-11-28).
--   Vendor code for 743/914 is embedded in itemname, 3rd " - " segment,
--     e.g. "01 - AP01 - API853670 - V24277 - ...".
--   NOTE: hsi.itemtype does NOT have rows for 743/914, so do not
--     INNER JOIN itemtype when querying these types.
--   NOT AVAILABLE on this schema: tax amount/rate, invoice line detail,
--     reconciliation tables, payment/due date, status.
-- =====================================================================

DECLARE @VendorTypes VARCHAR(50) = '743,914';   -- AP invoices with amounts

-- ---------------------------------------------------------------------
-- Sample DECLARE block -- run once per SSMS session, adjust values as
-- needed, then run the query block(s) below in the SAME query window.
-- Replace @VendorCode / @VendorCodeA / @VendorCodeB with a real vendor
-- code found via:
--   SELECT TOP 5 LTRIM(RTRIM(PARSENAME(REPLACE(itemname,' - ','.'), 2)))
--          AS VendorCode, COUNT(*) AS Cnt
--   FROM hsi.itemdata WITH (NOLOCK)
--   WHERE itemtypenum IN (743,914)
--   GROUP BY LTRIM(RTRIM(PARSENAME(REPLACE(itemname,' - ','.'), 2)))
--   HAVING COUNT(*) > 5
--   ORDER BY Cnt DESC;
-- ---------------------------------------------------------------------
DECLARE @VendorCode        VARCHAR(50) = 'V24277';        -- Prompt 1, 6
DECLARE @VendorCodeA       VARCHAR(50) = 'V24277';        -- Prompt 3
DECLARE @VendorCodeB       VARCHAR(50) = 'V207';          -- Prompt 3
DECLARE @StartDate         DATE        = '2023-01-01';    -- Prompt 1, 2, 7, 10
DECLARE @EndDate           DATE        = '2023-11-28';    -- Prompt 1, 2, 7, 10
DECLARE @MaxRows           INT         = 100;             -- Prompt 2
DECLARE @CurrentYearStart  DATE        = '2023-01-01';    -- Prompt 6

-- ---------------------------------------------------------------------
-- Prompt 1: Vendor spend summary
-- "total spend" = sum of invoiced amount (keyitem293) for the vendor/period
-- ---------------------------------------------------------------------
SELECT
    @VendorCode                          AS VendorCode,
    COUNT(*)                             AS InvoiceCount,
    SUM(ki293.keyvaluecurr)              AS TotalSpend,
    AVG(ki293.keyvaluecurr)              AS AvgInvoiceAmount,
    MIN(i.itemdate)                      AS EarliestInvoice,
    MAX(i.itemdate)                      AS LatestInvoice
FROM hsi.itemdata i WITH (NOLOCK)
INNER JOIN hsi.keyitem293 ki293 WITH (NOLOCK) ON i.itemnum = ki293.itemnum
WHERE i.itemtypenum IN (743, 914)
  AND ki293.keyvaluecurr IS NOT NULL
  AND i.itemname LIKE '% - ' + @VendorCode + ' - %'
  AND i.itemdate BETWEEN @StartDate AND @EndDate;

-- ---------------------------------------------------------------------
-- Prompt 2: Invoice count and list
-- Unique invoice = distinct itemnum. Default sort = InvoiceDate DESC.
-- ---------------------------------------------------------------------
SELECT TOP (@MaxRows)
    i.itemnum                                          AS InvoiceId,
    LTRIM(RTRIM(PARSENAME(REPLACE(i.itemname,' - ','.'), 2))) AS VendorCode,
    ki293.keyvaluecurr                                 AS Amount,
    i.itemdate                                         AS InvoiceDate,
    i.itemname                                         AS Description
FROM hsi.itemdata i WITH (NOLOCK)
LEFT OUTER JOIN hsi.keyitem293 ki293 WITH (NOLOCK) ON i.itemnum = ki293.itemnum
WHERE i.itemtypenum IN (743, 914)
  AND i.itemdate BETWEEN @StartDate AND @EndDate
ORDER BY i.itemdate DESC;

-- ---------------------------------------------------------------------
-- Prompt 3: Vendor comparison (two vendors, same period)
-- Compares amount, invoice count, average invoice value.
-- ---------------------------------------------------------------------
SELECT
    CASE WHEN i.itemname LIKE '% - ' + @VendorCodeA + ' - %' THEN @VendorCodeA
         ELSE @VendorCodeB END             AS VendorCode,
    COUNT(*)                               AS InvoiceCount,
    SUM(ki293.keyvaluecurr)                AS TotalSpend,
    AVG(ki293.keyvaluecurr)                AS AvgInvoiceAmount
FROM hsi.itemdata i WITH (NOLOCK)
INNER JOIN hsi.keyitem293 ki293 WITH (NOLOCK) ON i.itemnum = ki293.itemnum
WHERE i.itemtypenum IN (743, 914)
  AND ki293.keyvaluecurr IS NOT NULL
  AND (i.itemname LIKE '% - ' + @VendorCodeA + ' - %'
       OR i.itemname LIKE '% - ' + @VendorCodeB + ' - %')
  AND i.itemdate BETWEEN @StartDate AND @EndDate
GROUP BY CASE WHEN i.itemname LIKE '% - ' + @VendorCodeA + ' - %' THEN @VendorCodeA
              ELSE @VendorCodeB END;

-- ---------------------------------------------------------------------
-- Prompt 4: Invoice-line extraction -- NOT SUPPORTED
-- No line-level table (qty/rate/tax) exists in this schema; only a
-- single header amount (keyitem293) per itemnum. Would require PDF/OCR
-- extraction or a source ERP feed. Left out of MVP scope.
-- ---------------------------------------------------------------------

-- ---------------------------------------------------------------------
-- Prompt 5: Tax anomalies -- NOT SUPPORTED
-- No tax amount/rate keyword exists on AP invoice item types (743,914,
-- 263,340,364,429,515). Requires AS400/ERP as authoritative tax source.
-- ---------------------------------------------------------------------

-- ---------------------------------------------------------------------
-- Prompt 6: Vendor movement / trend (current year vs previous year, by month)
-- ---------------------------------------------------------------------
SELECT
    YEAR(i.itemdate)                       AS InvoiceYear,
    MONTH(i.itemdate)                      AS InvoiceMonth,
    COUNT(*)                               AS InvoiceCount,
    SUM(ki293.keyvaluecurr)                AS TotalSpend
FROM hsi.itemdata i WITH (NOLOCK)
INNER JOIN hsi.keyitem293 ki293 WITH (NOLOCK) ON i.itemnum = ki293.itemnum
WHERE i.itemtypenum IN (743, 914)
  AND ki293.keyvaluecurr IS NOT NULL
  AND i.itemname LIKE '% - ' + @VendorCode + ' - %'
  AND i.itemdate >= DATEADD(YEAR, -1, @CurrentYearStart)
GROUP BY YEAR(i.itemdate), MONTH(i.itemdate)
ORDER BY InvoiceYear, InvoiceMonth;

-- ---------------------------------------------------------------------
-- Prompt 7: Top 10 invoices by amount + simple duplicate anomaly flag
-- Anomaly = another invoice with same vendor + same amount within 7 days.
-- ---------------------------------------------------------------------
SELECT TOP 10
    i.itemnum                              AS InvoiceId,
    LTRIM(RTRIM(PARSENAME(REPLACE(i.itemname,' - ','.'), 2))) AS VendorCode,
    ki293.keyvaluecurr                     AS Amount,
    i.itemdate                             AS InvoiceDate,
    CASE WHEN EXISTS (
        SELECT 1 FROM hsi.itemdata i2 WITH (NOLOCK)
        INNER JOIN hsi.keyitem293 ki2 WITH (NOLOCK) ON i2.itemnum = ki2.itemnum
        WHERE i2.itemnum <> i.itemnum
          AND i2.itemtypenum IN (743, 914)
          AND ki2.keyvaluecurr = ki293.keyvaluecurr
          AND LTRIM(RTRIM(PARSENAME(REPLACE(i2.itemname,' - ','.'), 2)))
              = LTRIM(RTRIM(PARSENAME(REPLACE(i.itemname,' - ','.'), 2)))
          AND ABS(DATEDIFF(DAY, i2.itemdate, i.itemdate)) <= 7
    ) THEN 1 ELSE 0 END                    AS PossibleDuplicateFlag
FROM hsi.itemdata i WITH (NOLOCK)
INNER JOIN hsi.keyitem293 ki293 WITH (NOLOCK) ON i.itemnum = ki293.itemnum
WHERE i.itemtypenum IN (743, 914)
  AND ki293.keyvaluecurr IS NOT NULL
  AND i.itemdate BETWEEN @StartDate AND @EndDate
ORDER BY ki293.keyvaluecurr DESC;

-- ---------------------------------------------------------------------
-- Prompt 8 & 9: Reconciliation summary -- NOT SUPPORTED
-- No reconciliation table/status keyword found among the 162 tables
-- scanned in Onbase_DB_Tables.md. AS400/ERP reconciliation module is
-- the likely authoritative source; needs SME confirmation.
-- ---------------------------------------------------------------------

-- ---------------------------------------------------------------------
-- Prompt 10: Month-over-month change (using invoice date; posting/payment
-- date keywords not present in schema)
-- ---------------------------------------------------------------------
;WITH Monthly AS (
    SELECT
        YEAR(i.itemdate) AS Yr, MONTH(i.itemdate) AS Mo,
        SUM(ki293.keyvaluecurr) AS TotalSpend
    FROM hsi.itemdata i WITH (NOLOCK)
    INNER JOIN hsi.keyitem293 ki293 WITH (NOLOCK) ON i.itemnum = ki293.itemnum
    WHERE i.itemtypenum IN (743, 914)
      AND ki293.keyvaluecurr IS NOT NULL
      AND i.itemdate BETWEEN @StartDate AND @EndDate
    GROUP BY YEAR(i.itemdate), MONTH(i.itemdate)
)
SELECT
    Yr, Mo, TotalSpend,
    LAG(TotalSpend) OVER (ORDER BY Yr, Mo)              AS PriorMonthSpend,
    CASE WHEN LAG(TotalSpend) OVER (ORDER BY Yr, Mo) IS NULL
              OR LAG(TotalSpend) OVER (ORDER BY Yr, Mo) = 0 THEN NULL
         ELSE (TotalSpend - LAG(TotalSpend) OVER (ORDER BY Yr, Mo))
              / LAG(TotalSpend) OVER (ORDER BY Yr, Mo) * 100
    END                                                  AS PctChange
FROM Monthly
ORDER BY Yr, Mo;
