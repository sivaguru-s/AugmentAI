using Dapper;
using Microsoft.Data.SqlClient;
using Chatbot_Onbase.Models;

namespace Chatbot_Onbase.Data;

/// <summary>
/// "OnBase DB Access Layer" from OnBase_Chatbot_Business_Flow_v7.md (section 4/5).
/// Read-only, least-privilege access to the OnBase database used exclusively by
/// Finance Query Services. No writes. No arbitrary SQL is ever passed in from
/// callers - only these fixed, parameterized queries are executed, mirroring the
/// intent of "approved read-only views/stored procedures" without requiring a
/// schema deployment for this POC.
///
/// Confirmed stage-DB facts used below (see Scripts/ChatbotRequirementQueries.sql
/// header for full detail):
///   - AP (vendor) invoice item types WITH amount data on stage: 743, 914.
///   - Amount lives in hsi.keyitem293.keyvaluecurr.
///   - Vendor code is embedded in hsi.itemdata.itemname as the 3rd " - " segment.
///   - Stage data range for 743/914: 2012-01-25 .. 2023-11-28.
/// </summary>
public interface IFinanceQueryRepository
{
    Task<VendorSpendResult> GetVendorSpendAsync(string vendorCode, DateTime periodStart, DateTime periodEnd);
    Task<InvoiceListResult> GetInvoiceListAsync(string? vendorCode, DateTime periodStart, DateTime periodEnd, int limit);
    Task<VendorTrendResult> GetVendorTrendAsync(string vendorCode, DateTime yearStart, DateTime yearEnd);
    Task<TopInvoicesResult> GetTopInvoicesAsync(DateTime periodStart, DateTime periodEnd, int topN);
}

public class FinanceQueryRepository : IFinanceQueryRepository
{
    private const string ApInvoiceTypeFilter = "i.itemtypenum IN (743, 914)";
    private readonly string _connectionString;
    private readonly ILogger<FinanceQueryRepository> _logger;

    public FinanceQueryRepository(IConfiguration configuration, ILogger<FinanceQueryRepository> logger)
    {
        _connectionString = configuration.GetConnectionString("OnBaseConnection")
            ?? throw new ArgumentNullException("OnBaseConnection not found in configuration");
        _logger = logger;
    }

    private SqlConnection GetConnection() => new SqlConnection(_connectionString);

    public async Task<VendorSpendResult> GetVendorSpendAsync(string vendorCode, DateTime periodStart, DateTime periodEnd)
    {
        using var connection = GetConnection();
        const string sql = $@"
            SELECT
                COUNT(*)                AS InvoiceCount,
                SUM(ki293.keyvaluecurr) AS TotalSpend,
                AVG(ki293.keyvaluecurr) AS AverageInvoiceAmount,
                MIN(i.itemdate)         AS EarliestInvoiceDate,
                MAX(i.itemdate)         AS LatestInvoiceDate
            FROM hsi.itemdata i WITH (NOLOCK)
            INNER JOIN hsi.keyitem293 ki293 WITH (NOLOCK) ON i.itemnum = ki293.itemnum
            WHERE {ApInvoiceTypeFilter}
              AND ki293.keyvaluecurr IS NOT NULL
              AND i.itemname LIKE '% - ' + @VendorCode + ' - %'
              AND i.itemdate BETWEEN @PeriodStart AND @PeriodEnd";

        var row = await connection.QuerySingleAsync(sql,
            new { VendorCode = vendorCode, PeriodStart = periodStart, PeriodEnd = periodEnd },
            commandTimeout: 60);

        return new VendorSpendResult
        {
            VendorCode = vendorCode,
            InvoiceCount = row.InvoiceCount ?? 0,
            TotalSpend = row.TotalSpend ?? 0m,
            AverageInvoiceAmount = row.AverageInvoiceAmount ?? 0m,
            EarliestInvoiceDate = row.EarliestInvoiceDate,
            LatestInvoiceDate = row.LatestInvoiceDate,
            PeriodStart = periodStart,
            PeriodEnd = periodEnd
        };
    }

    public async Task<InvoiceListResult> GetInvoiceListAsync(string? vendorCode, DateTime periodStart, DateTime periodEnd, int limit)
    {
        if (limit <= 0 || limit > 500) limit = 100;
        using var connection = GetConnection();

        var vendorFilter = string.IsNullOrWhiteSpace(vendorCode)
            ? string.Empty
            : "AND i.itemname LIKE '% - ' + @VendorCode + ' - %'";

        var sql = $@"
            SELECT TOP (@Limit)
                i.itemnum                                                    AS InvoiceId,
                LTRIM(RTRIM(PARSENAME(REPLACE(i.itemname,' - ','.'), 2)))    AS VendorName,
                ki293.keyvaluecurr                                           AS Amount,
                i.itemdate                                                   AS InvoiceDate,
                i.itemname                                                   AS Description,
                'AP Invoice'                                                 AS DocumentType
            FROM hsi.itemdata i WITH (NOLOCK)
            LEFT OUTER JOIN hsi.keyitem293 ki293 WITH (NOLOCK) ON i.itemnum = ki293.itemnum
            WHERE {ApInvoiceTypeFilter}
              AND i.itemdate BETWEEN @PeriodStart AND @PeriodEnd
              {vendorFilter}
            ORDER BY i.itemdate DESC";

        var invoices = (await connection.QueryAsync<Invoice>(sql,
            new { VendorCode = vendorCode, PeriodStart = periodStart, PeriodEnd = periodEnd, Limit = limit },
            commandTimeout: 60)).ToList();

        return new InvoiceListResult
        {
            TotalCount = invoices.Count,
            Invoices = invoices,
            AppliedFilters = new Dictionary<string, object?>
            {
                ["vendorCode"] = vendorCode,
                ["periodStart"] = periodStart,
                ["periodEnd"] = periodEnd,
                ["limit"] = limit
            }
        };
    }

    public async Task<VendorTrendResult> GetVendorTrendAsync(string vendorCode, DateTime yearStart, DateTime yearEnd)
    {
        using var connection = GetConnection();
        const string sql = $@"
            SELECT
                YEAR(i.itemdate)         AS Year,
                MONTH(i.itemdate)        AS Month,
                COUNT(*)                 AS InvoiceCount,
                SUM(ki293.keyvaluecurr)  AS TotalSpend
            FROM hsi.itemdata i WITH (NOLOCK)
            INNER JOIN hsi.keyitem293 ki293 WITH (NOLOCK) ON i.itemnum = ki293.itemnum
            WHERE {ApInvoiceTypeFilter}
              AND ki293.keyvaluecurr IS NOT NULL
              AND i.itemname LIKE '% - ' + @VendorCode + ' - %'
              AND i.itemdate BETWEEN @YearStart AND @YearEnd
            GROUP BY YEAR(i.itemdate), MONTH(i.itemdate)
            ORDER BY Year, Month";

        var points = (await connection.QueryAsync<VendorTrendPoint>(sql,
            new { VendorCode = vendorCode, YearStart = yearStart, YearEnd = yearEnd },
            commandTimeout: 60)).ToList();

        return new VendorTrendResult { VendorCode = vendorCode, TrendPoints = points };
    }

    public async Task<TopInvoicesResult> GetTopInvoicesAsync(DateTime periodStart, DateTime periodEnd, int topN)
    {
        if (topN <= 0 || topN > 100) topN = 10;
        using var connection = GetConnection();
        string sql = $@"
            SELECT TOP (@TopN)
                i.itemnum                                                 AS InvoiceItemNum,
                LTRIM(RTRIM(PARSENAME(REPLACE(i.itemname,' - ','.'), 2))) AS VendorCode,
                ki293.keyvaluecurr                                        AS Amount,
                i.itemdate                                                AS InvoiceDate,
                CASE WHEN EXISTS (
                    SELECT 1 FROM hsi.itemdata i2 WITH (NOLOCK)
                    INNER JOIN hsi.keyitem293 ki2 WITH (NOLOCK) ON i2.itemnum = ki2.itemnum
                    WHERE i2.itemnum <> i.itemnum
                      AND {ApInvoiceTypeFilter.Replace("i.", "i2.")}
                      AND ki2.keyvaluecurr = ki293.keyvaluecurr
                      AND LTRIM(RTRIM(PARSENAME(REPLACE(i2.itemname,' - ','.'), 2)))
                          = LTRIM(RTRIM(PARSENAME(REPLACE(i.itemname,' - ','.'), 2)))
                      AND ABS(DATEDIFF(DAY, i2.itemdate, i.itemdate)) <= 7
                ) THEN 1 ELSE 0 END                                       AS PossibleDuplicateFlag
            FROM hsi.itemdata i WITH (NOLOCK)
            INNER JOIN hsi.keyitem293 ki293 WITH (NOLOCK) ON i.itemnum = ki293.itemnum
            WHERE {ApInvoiceTypeFilter}
              AND ki293.keyvaluecurr IS NOT NULL
              AND i.itemdate BETWEEN @PeriodStart AND @PeriodEnd
            ORDER BY ki293.keyvaluecurr DESC";

        var items = (await connection.QueryAsync<TopInvoiceItem>(sql,
            new { PeriodStart = periodStart, PeriodEnd = periodEnd, TopN = topN },
            commandTimeout: 90)).ToList();

        return new TopInvoicesResult { Invoices = items };
    }
}
