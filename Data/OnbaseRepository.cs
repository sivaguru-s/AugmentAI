using Dapper;
using Microsoft.Data.SqlClient;
using Chatbot_Onbase.Models;

namespace Chatbot_Onbase.Data;

public interface IOnbaseRepository
{
    Task<List<Invoice>> SearchInvoicesAsync(string searchTerm, string documentType = "all");
    Task<List<Invoice>> GetInvoicesByVendorAsync(string vendorName);
    Task<List<Invoice>> GetInvoicesByNumberAsync(string invoiceNumber, string documentType = "all");
    Task<List<Invoice>> GetInvoicesByDateRangeAsync(DateTime startDate, DateTime endDate, string documentType = "all");
    Task<List<Invoice>> GetInvoicesByStatusAsync(string status);
    Task<List<Invoice>> GetInvoicesByAmountRangeAsync(decimal minAmount, decimal maxAmount, string documentType = "all");
    Task<List<Invoice>> GetAllInvoicesAsync(int limit = 100, string documentType = "all");
}

public class OnbaseRepository : IOnbaseRepository
{
    private readonly string _connectionString;
    private readonly ILogger<OnbaseRepository> _logger;

    public OnbaseRepository(IConfiguration configuration, ILogger<OnbaseRepository> logger)
    {
        _connectionString = configuration.GetConnectionString("OnBaseConnection")
            ?? throw new ArgumentNullException("OnBaseConnection not found in configuration");
        _logger = logger;
    }

    private SqlConnection GetConnection() => new SqlConnection(_connectionString);

    public async Task<List<Invoice>> SearchInvoicesAsync(string searchTerm, string documentType = "all")
    {
        using var connection = GetConnection();

        // Try to parse as numeric for exact invoice number match
        int numericSearch = 0;
        bool isNumeric = int.TryParse(searchTerm, out numericSearch);
        bool includeVendor = ShouldIncludeVendorField(documentType);

        string query;
        object parameters;

        if (isNumeric)
        {
            // If numeric, search by invoice number with all joins
            // No TOP limit - return all matching invoices
            if (includeVendor)
            {
                query = $@"
                    SELECT
                        i.itemnum as InvoiceId,
                        CAST(ki106.keyvaluesmall AS VARCHAR(50)) as InvoiceNumber,
                        RTRIM(kt105.keyvaluechar) as VendorName,
                        NULL as Amount,
                        COALESCE(ki112.keyvaluedate, i.itemdate) as InvoiceDate,
                        NULL as DueDate,
                        NULL as Status,
                        kt104.keyvaluechar as PONumber,
                        i.itemname as Description,
                        it.itemtypename as DocumentType,
                        i.itemdate as CreatedDate
                    FROM hsi.keyitem106 ki106 WITH (NOLOCK)
                    INNER JOIN hsi.itemdata i WITH (NOLOCK) ON i.itemnum = ki106.itemnum
                    INNER JOIN hsi.itemtype it WITH (NOLOCK) ON i.itemtypenum = it.itemtypenum
                    LEFT OUTER JOIN hsi.keyxitem104 kx104 WITH (NOLOCK) ON i.itemnum = kx104.itemnum
                    LEFT OUTER JOIN hsi.keytable104 kt104 WITH (NOLOCK) ON kx104.keywordnum = kt104.keywordnum
                    LEFT OUTER JOIN hsi.keyitem112 ki112 WITH (NOLOCK) ON i.itemnum = ki112.itemnum
                    LEFT OUTER JOIN hsi.keyxitem105 kx105 WITH (NOLOCK) ON i.itemnum = kx105.itemnum
                    LEFT OUTER JOIN hsi.keytable105 kt105 WITH (NOLOCK) ON kx105.keywordnum = kt105.keywordnum
                    WHERE ki106.keyvaluesmall = @SearchTermNumeric
                        AND {GetDocumentTypeFilter(documentType)}
                    ORDER BY i.itemdate DESC";
            }
            else
            {
                query = $@"
                    SELECT
                        i.itemnum as InvoiceId,
                        CAST(ki106.keyvaluesmall AS VARCHAR(50)) as InvoiceNumber,
                        NULL as VendorName,
                        NULL as Amount,
                        ki112.keyvaluedate as InvoiceDate,
                        NULL as DueDate,
                        NULL as Status,
                        kt104.keyvaluechar as PONumber,
                        i.itemname as Description,
                        it.itemtypename as DocumentType,
                        i.itemdate as CreatedDate
                    FROM hsi.keyitem106 ki106 WITH (NOLOCK)
                    INNER JOIN hsi.itemdata i WITH (NOLOCK) ON i.itemnum = ki106.itemnum
                    INNER JOIN hsi.itemtype it WITH (NOLOCK) ON i.itemtypenum = it.itemtypenum
                    LEFT OUTER JOIN hsi.keyxitem104 kx104 WITH (NOLOCK) ON i.itemnum = kx104.itemnum
                    LEFT OUTER JOIN hsi.keytable104 kt104 WITH (NOLOCK) ON kx104.keywordnum = kt104.keywordnum
                    LEFT OUTER JOIN hsi.keyitem112 ki112 WITH (NOLOCK) ON i.itemnum = ki112.itemnum
                    WHERE ki106.keyvaluesmall = @SearchTermNumeric
                        AND {GetDocumentTypeFilter(documentType)}
                    ORDER BY i.itemdate DESC";
            }

            parameters = new { SearchTermNumeric = numericSearch };
        }
        else
        {
            // If text, return recent invoices with all data
            // Limit to 100 for general searches to avoid overwhelming results
            if (includeVendor)
            {
                query = $@"
                    SELECT TOP 100
                        i.itemnum as InvoiceId,
                        CAST(ki106.keyvaluesmall AS VARCHAR(50)) as InvoiceNumber,
                        RTRIM(kt105.keyvaluechar) as VendorName,
                        NULL as Amount,
                        ki112.keyvaluedate as InvoiceDate,
                        NULL as DueDate,
                        NULL as Status,
                        kt104.keyvaluechar as PONumber,
                        i.itemname as Description,
                        it.itemtypename as DocumentType,
                        i.itemdate as CreatedDate
                    FROM hsi.itemdata i WITH (NOLOCK)
                    INNER JOIN hsi.itemtype it WITH (NOLOCK) ON i.itemtypenum = it.itemtypenum
                    LEFT OUTER JOIN hsi.keyitem106 ki106 WITH (NOLOCK) ON i.itemnum = ki106.itemnum
                    LEFT OUTER JOIN hsi.keyxitem104 kx104 WITH (NOLOCK) ON i.itemnum = kx104.itemnum
                    LEFT OUTER JOIN hsi.keytable104 kt104 WITH (NOLOCK) ON kx104.keywordnum = kt104.keywordnum
                    LEFT OUTER JOIN hsi.keyitem112 ki112 WITH (NOLOCK) ON i.itemnum = ki112.itemnum
                    LEFT OUTER JOIN hsi.keyxitem105 kx105 WITH (NOLOCK) ON i.itemnum = kx105.itemnum
                    LEFT OUTER JOIN hsi.keytable105 kt105 WITH (NOLOCK) ON kx105.keywordnum = kt105.keywordnum
                    WHERE {GetDocumentTypeFilter(documentType)}
                    ORDER BY i.itemdate DESC";
            }
            else
            {
                query = $@"
                    SELECT TOP 100
                        i.itemnum as InvoiceId,
                        CAST(ki106.keyvaluesmall AS VARCHAR(50)) as InvoiceNumber,
                        NULL as VendorName,
                        NULL as Amount,
                        ki112.keyvaluedate as InvoiceDate,
                        NULL as DueDate,
                        NULL as Status,
                        kt104.keyvaluechar as PONumber,
                        i.itemname as Description,
                        it.itemtypename as DocumentType,
                        i.itemdate as CreatedDate
                    FROM hsi.itemdata i WITH (NOLOCK)
                    INNER JOIN hsi.itemtype it WITH (NOLOCK) ON i.itemtypenum = it.itemtypenum
                    LEFT OUTER JOIN hsi.keyitem106 ki106 WITH (NOLOCK) ON i.itemnum = ki106.itemnum
                    LEFT OUTER JOIN hsi.keyxitem104 kx104 WITH (NOLOCK) ON i.itemnum = kx104.itemnum
                    LEFT OUTER JOIN hsi.keytable104 kt104 WITH (NOLOCK) ON kx104.keywordnum = kt104.keywordnum
                    LEFT OUTER JOIN hsi.keyitem112 ki112 WITH (NOLOCK) ON i.itemnum = ki112.itemnum
                    WHERE {GetDocumentTypeFilter(documentType)}
                    ORDER BY i.itemdate DESC";
            }

            parameters = new { };
        }

        var result = await connection.QueryAsync<Invoice>(query, parameters, commandTimeout: 60);
        return result.ToList();
    }

    public async Task<List<Invoice>> GetInvoicesByVendorAsync(string vendorName)
    {
        _logger.LogInformation("Searching invoices for vendor: {VendorName}", vendorName);
        using var connection = GetConnection();

        // Check if this looks like a vendor code (starts with V followed by numbers)
        bool isVendorCode = System.Text.RegularExpressions.Regex.IsMatch(vendorName, @"^V\d+$");
        _logger.LogDebug("Vendor search type: {SearchType}", isVendorCode ? "VendorCode" : "VendorName");

        string query;
        if (isVendorCode)
        {
            // For vendor codes (e.g., V207), search in itemname for new invoice types only
            // This is much faster than searching all types
            // Extract invoice number from itemname (4th segment between dashes)
            // Try to get vendor name from old records in keytable105
            query = @"
                SELECT TOP 500
                    i.itemnum as InvoiceId,
                    LTRIM(RTRIM(
                        SUBSTRING(
                            i.itemname,
                            CHARINDEX(' - ', i.itemname, CHARINDEX(' - ', i.itemname, CHARINDEX(' - ', i.itemname) + 3) + 3) + 3,
                            CHARINDEX(' - ', i.itemname, CHARINDEX(' - ', i.itemname, CHARINDEX(' - ', i.itemname, CHARINDEX(' - ', i.itemname) + 3) + 3) + 3) -
                            CHARINDEX(' - ', i.itemname, CHARINDEX(' - ', i.itemname, CHARINDEX(' - ', i.itemname) + 3) + 3) - 3
                        )
                    )) as InvoiceNumber,
                    COALESCE(
                        (SELECT TOP 1 RTRIM(kt105.keyvaluechar)
                         FROM hsi.itemdata i2 WITH (NOLOCK)
                         INNER JOIN hsi.keyxitem105 kx105 WITH (NOLOCK) ON i2.itemnum = kx105.itemnum
                         INNER JOIN hsi.keytable105 kt105 WITH (NOLOCK) ON kx105.keywordnum = kt105.keywordnum
                         WHERE i2.itemname LIKE @VendorName + ' - %'
                         AND kt105.keyvaluechar IS NOT NULL
                         ORDER BY i2.itemdate DESC),
                        @VendorName
                    ) as VendorName,
                    ki293.keyvaluecurr as Amount,
                    i.itemdate as InvoiceDate,
                    NULL as DueDate,
                    NULL as Status,
                    NULL as PONumber,
                    i.itemname as Description,
                    'AP Invoice' as DocumentType,
                    i.itemdate as CreatedDate
                FROM hsi.itemdata i WITH (NOLOCK)
                LEFT OUTER JOIN hsi.keyitem293 ki293 WITH (NOLOCK) ON i.itemnum = ki293.itemnum
                WHERE i.itemtypenum IN (743, 914)
                    AND i.itemnum > 380000000
                    AND i.itemname LIKE '% - ' + @VendorName + ' - %'
                ORDER BY
                    CASE WHEN ki293.keyvaluecurr IS NOT NULL THEN 0 ELSE 1 END,
                    i.itemdate DESC";
        }
        else
        {
            // For vendor names (e.g., SUPREME GRAPHICS), search BOTH old and new invoice types
            // First find the vendor code from old records, then search new invoices by that code
            // Use UNION to combine results from both sources
            query = @"
                SELECT TOP 500 * FROM (
                    -- New invoice types (743, 914) - search by vendor code found in old records
                    SELECT
                        i.itemnum as InvoiceId,
                        LTRIM(RTRIM(
                            SUBSTRING(
                                i.itemname,
                                CHARINDEX(' - ', i.itemname, CHARINDEX(' - ', i.itemname, CHARINDEX(' - ', i.itemname) + 3) + 3) + 3,
                                CHARINDEX(' - ', i.itemname, CHARINDEX(' - ', i.itemname, CHARINDEX(' - ', i.itemname, CHARINDEX(' - ', i.itemname) + 3) + 3) + 3) -
                                CHARINDEX(' - ', i.itemname, CHARINDEX(' - ', i.itemname, CHARINDEX(' - ', i.itemname) + 3) + 3) - 3
                            )
                        )) as InvoiceNumber,
                        @VendorName as VendorName,
                        ki293.keyvaluecurr as Amount,
                        i.itemdate as InvoiceDate,
                        NULL as DueDate,
                        NULL as Status,
                        NULL as PONumber,
                        i.itemname as Description,
                        'AP Invoice' as DocumentType,
                        i.itemdate as CreatedDate,
                        CASE WHEN ki293.keyvaluecurr IS NOT NULL THEN 0 ELSE 1 END as AmountSort
                    FROM hsi.itemdata i WITH (NOLOCK)
                    LEFT OUTER JOIN hsi.keyitem293 ki293 WITH (NOLOCK) ON i.itemnum = ki293.itemnum
                    WHERE i.itemtypenum IN (743, 914)
                        AND i.itemnum > 380000000
                        AND i.itemname LIKE '% - ' +
                            COALESCE(
                                (SELECT TOP 1 SUBSTRING(i2.itemname, 1, CHARINDEX(' - ', i2.itemname) - 1)
                                 FROM hsi.itemdata i2 WITH (NOLOCK)
                                 INNER JOIN hsi.keyxitem105 kx105 WITH (NOLOCK) ON i2.itemnum = kx105.itemnum
                                 INNER JOIN hsi.keytable105 kt105 WITH (NOLOCK) ON kx105.keywordnum = kt105.keywordnum
                                 WHERE kt105.keyvaluechar LIKE '%' + @VendorName + '%'
                                 AND i2.itemname LIKE 'V%'
                                 ORDER BY i2.itemdate DESC),
                                @VendorName
                            ) + ' - %'

                    UNION ALL

                    -- Old invoice types (263, 340, 364, 429, 515) - search in keytable105
                    SELECT
                        i.itemnum as InvoiceId,
                        CAST(ki106.keyvaluesmall AS VARCHAR(50)) as InvoiceNumber,
                        RTRIM(kt105.keyvaluechar) as VendorName,
                        ki293.keyvaluecurr as Amount,
                        COALESCE(ki112.keyvaluedate, i.itemdate) as InvoiceDate,
                        NULL as DueDate,
                        NULL as Status,
                        kt104.keyvaluechar as PONumber,
                        i.itemname as Description,
                        'AP Invoice' as DocumentType,
                        i.itemdate as CreatedDate,
                        CASE WHEN ki293.keyvaluecurr IS NOT NULL THEN 0 ELSE 1 END as AmountSort
                    FROM hsi.itemdata i WITH (NOLOCK)
                    INNER JOIN hsi.keyxitem105 kx105 WITH (NOLOCK) ON i.itemnum = kx105.itemnum
                    INNER JOIN hsi.keytable105 kt105 WITH (NOLOCK) ON kx105.keywordnum = kt105.keywordnum
                    LEFT OUTER JOIN hsi.keyitem106 ki106 WITH (NOLOCK) ON i.itemnum = ki106.itemnum
                    LEFT OUTER JOIN hsi.keyitem293 ki293 WITH (NOLOCK) ON i.itemnum = ki293.itemnum
                    LEFT OUTER JOIN hsi.keyxitem104 kx104 WITH (NOLOCK) ON i.itemnum = kx104.itemnum
                    LEFT OUTER JOIN hsi.keytable104 kt104 WITH (NOLOCK) ON kx104.keywordnum = kt104.keywordnum
                    LEFT OUTER JOIN hsi.keyitem112 ki112 WITH (NOLOCK) ON i.itemnum = ki112.itemnum
                    WHERE i.itemtypenum IN (263, 340, 364, 429, 515)
                        AND kt105.keyvaluechar LIKE '%' + @VendorName + '%'
                ) AS CombinedResults
                ORDER BY AmountSort, InvoiceDate DESC";
        }

        var startTime = DateTime.UtcNow;
        var results = (await connection.QueryAsync<Invoice>(query, new { VendorName = vendorName }, commandTimeout: 120)).ToList();
        var duration = (DateTime.UtcNow - startTime).TotalMilliseconds;

        _logger.LogInformation("Found {Count} invoices for vendor {VendorName} in {Duration}ms",
            results.Count, vendorName, duration);

        return results;
    }

    public async Task<List<Invoice>> GetInvoicesByNumberAsync(string invoiceNumber, string documentType = "all")
    {
        using var connection = GetConnection();

        // Parse invoice number as integer
        int invoiceNumberInt = 0;
        if (!int.TryParse(invoiceNumber, out invoiceNumberInt) || invoiceNumberInt == 0)
        {
            // If not a valid number, return empty list
            return new List<Invoice>();
        }

        bool includeVendor = ShouldIncludeVendorField(documentType);

        // Search by Invoice Number in keyitem106 - use exact match
        // Start from keyitem106 for better performance (smaller table, indexed)
        string query;

        if (includeVendor)
        {
            query = $@"
                SELECT TOP 1
                    i.itemnum as InvoiceId,
                    CAST(ki106.keyvaluesmall AS VARCHAR(50)) as InvoiceNumber,
                    RTRIM(kt105.keyvaluechar) as VendorName,
                    NULL as Amount,
                    ki112.keyvaluedate as InvoiceDate,
                    NULL as DueDate,
                    NULL as Status,
                    kt104.keyvaluechar as PONumber,
                    i.itemname as Description,
                    it.itemtypename as DocumentType,
                    i.itemdate as CreatedDate
                FROM hsi.keyitem106 ki106 WITH (NOLOCK)
                INNER JOIN hsi.itemdata i WITH (NOLOCK) ON i.itemnum = ki106.itemnum
                INNER JOIN hsi.itemtype it WITH (NOLOCK) ON i.itemtypenum = it.itemtypenum
                LEFT OUTER JOIN hsi.keyxitem104 kx104 WITH (NOLOCK) ON i.itemnum = kx104.itemnum
                LEFT OUTER JOIN hsi.keytable104 kt104 WITH (NOLOCK) ON kx104.keywordnum = kt104.keywordnum
                LEFT OUTER JOIN hsi.keyitem112 ki112 WITH (NOLOCK) ON i.itemnum = ki112.itemnum
                LEFT OUTER JOIN hsi.keyxitem105 kx105 WITH (NOLOCK) ON i.itemnum = kx105.itemnum
                LEFT OUTER JOIN hsi.keytable105 kt105 WITH (NOLOCK) ON kx105.keywordnum = kt105.keywordnum
                WHERE ki106.keyvaluesmall = @InvoiceNumberInt
                    AND {GetDocumentTypeFilter(documentType)}
                ORDER BY i.itemdate DESC";
        }
        else
        {
            query = $@"
                SELECT TOP 1
                    i.itemnum as InvoiceId,
                    CAST(ki106.keyvaluesmall AS VARCHAR(50)) as InvoiceNumber,
                    NULL as VendorName,
                    NULL as Amount,
                    ki112.keyvaluedate as InvoiceDate,
                    NULL as DueDate,
                    NULL as Status,
                    kt104.keyvaluechar as PONumber,
                    i.itemname as Description,
                    it.itemtypename as DocumentType,
                    i.itemdate as CreatedDate
                FROM hsi.keyitem106 ki106 WITH (NOLOCK)
                INNER JOIN hsi.itemdata i WITH (NOLOCK) ON i.itemnum = ki106.itemnum
                INNER JOIN hsi.itemtype it WITH (NOLOCK) ON i.itemtypenum = it.itemtypenum
                LEFT OUTER JOIN hsi.keyxitem104 kx104 WITH (NOLOCK) ON i.itemnum = kx104.itemnum
                LEFT OUTER JOIN hsi.keytable104 kt104 WITH (NOLOCK) ON kx104.keywordnum = kt104.keywordnum
                LEFT OUTER JOIN hsi.keyitem112 ki112 WITH (NOLOCK) ON i.itemnum = ki112.itemnum
                WHERE ki106.keyvaluesmall = @InvoiceNumberInt
                    AND {GetDocumentTypeFilter(documentType)}
                ORDER BY i.itemdate DESC";
        }

        return (await connection.QueryAsync<Invoice>(query, new { InvoiceNumberInt = invoiceNumberInt }, commandTimeout: 60)).ToList();
    }

    public async Task<List<Invoice>> GetInvoicesByDateRangeAsync(DateTime startDate, DateTime endDate, string documentType = "all")
    {
        using var connection = GetConnection();
        bool includeVendor = ShouldIncludeVendorField(documentType);

        // Search by Invoice Date in keyitem112
        // No TOP limit - return all invoices in the date range
        string query;

        if (includeVendor)
        {
            query = $@"
                SELECT
                    i.itemnum as InvoiceId,
                    CAST(ki106.keyvaluesmall AS VARCHAR(50)) as InvoiceNumber,
                    RTRIM(kt105.keyvaluechar) as VendorName,
                    NULL as Amount,
                    COALESCE(ki112.keyvaluedate, i.itemdate) as InvoiceDate,
                    NULL as DueDate,
                    NULL as Status,
                    kt104.keyvaluechar as PONumber,
                    i.itemname as Description,
                    it.itemtypename as DocumentType,
                    i.itemdate as CreatedDate
                FROM hsi.itemdata i WITH (NOLOCK)
                INNER JOIN hsi.itemtype it WITH (NOLOCK) ON i.itemtypenum = it.itemtypenum
                LEFT OUTER JOIN hsi.keyitem106 ki106 WITH (NOLOCK) ON i.itemnum = ki106.itemnum
                LEFT OUTER JOIN hsi.keyxitem104 kx104 WITH (NOLOCK) ON i.itemnum = kx104.itemnum
                LEFT OUTER JOIN hsi.keytable104 kt104 WITH (NOLOCK) ON kx104.keywordnum = kt104.keywordnum
                LEFT OUTER JOIN hsi.keyitem112 ki112 WITH (NOLOCK) ON i.itemnum = ki112.itemnum
                LEFT OUTER JOIN hsi.keyxitem105 kx105 WITH (NOLOCK) ON i.itemnum = kx105.itemnum
                LEFT OUTER JOIN hsi.keytable105 kt105 WITH (NOLOCK) ON kx105.keywordnum = kt105.keywordnum
                WHERE {GetDocumentTypeFilter(documentType)}
                    AND COALESCE(ki112.keyvaluedate, i.itemdate) BETWEEN @StartDate AND @EndDate
                ORDER BY COALESCE(ki112.keyvaluedate, i.itemdate) DESC";
        }
        else
        {
            query = $@"
                SELECT
                    i.itemnum as InvoiceId,
                    CAST(ki106.keyvaluesmall AS VARCHAR(50)) as InvoiceNumber,
                    NULL as VendorName,
                    NULL as Amount,
                    COALESCE(ki112.keyvaluedate, i.itemdate) as InvoiceDate,
                    NULL as DueDate,
                    NULL as Status,
                    kt104.keyvaluechar as PONumber,
                    i.itemname as Description,
                    it.itemtypename as DocumentType,
                    i.itemdate as CreatedDate
                FROM hsi.itemdata i WITH (NOLOCK)
                INNER JOIN hsi.itemtype it WITH (NOLOCK) ON i.itemtypenum = it.itemtypenum
                LEFT OUTER JOIN hsi.keyitem106 ki106 WITH (NOLOCK) ON i.itemnum = ki106.itemnum
                LEFT OUTER JOIN hsi.keyxitem104 kx104 WITH (NOLOCK) ON i.itemnum = kx104.itemnum
                LEFT OUTER JOIN hsi.keytable104 kt104 WITH (NOLOCK) ON kx104.keywordnum = kt104.keywordnum
                LEFT OUTER JOIN hsi.keyitem112 ki112 WITH (NOLOCK) ON i.itemnum = ki112.itemnum
                WHERE {GetDocumentTypeFilter(documentType)}
                    AND COALESCE(ki112.keyvaluedate, i.itemdate) BETWEEN @StartDate AND @EndDate
                ORDER BY COALESCE(ki112.keyvaluedate, i.itemdate) DESC";
        }

        return (await connection.QueryAsync<Invoice>(query, new { StartDate = startDate, EndDate = endDate }, commandTimeout: 60)).ToList();
    }

    public async Task<List<Invoice>> GetInvoicesByStatusAsync(string status)
    {
        using var connection = GetConnection();

        // Status field not available in current schema, return recent invoices
        var query = @"
            SELECT TOP 50
                i.itemnum as InvoiceId,
                CAST(ki106.keyvaluesmall AS VARCHAR(50)) as InvoiceNumber,
                NULL as VendorName,
                CAST(ki106.keyvaluesmall AS DECIMAL(18,2)) as Amount,
                ki112.keyvaluedate as InvoiceDate,
                NULL as DueDate,
                NULL as Status,
                kt104.keyvaluechar as PONumber,
                i.itemname as Description,
                'Invoice' as DocumentType,
                i.itemdate as CreatedDate
            FROM hsi.itemdata i WITH (NOLOCK)
            LEFT OUTER JOIN hsi.keyitem106 ki106 WITH (NOLOCK) ON i.itemnum = ki106.itemnum
            LEFT OUTER JOIN hsi.keyxitem104 kx104 WITH (NOLOCK) ON i.itemnum = kx104.itemnum
            LEFT OUTER JOIN hsi.keytable104 kt104 WITH (NOLOCK) ON kx104.keywordnum = kt104.keywordnum
            LEFT OUTER JOIN hsi.keyitem112 ki112 WITH (NOLOCK) ON i.itemnum = ki112.itemnum
            WHERE i.itemtypenum = 102
            ORDER BY i.itemdate DESC";

        return (await connection.QueryAsync<Invoice>(query, commandTimeout: 30)).ToList();
    }

    public async Task<List<Invoice>> GetInvoicesByAmountRangeAsync(decimal minAmount, decimal maxAmount, string documentType = "all")
    {
        using var connection = GetConnection();
        bool includeVendor = ShouldIncludeVendorField(documentType);

        // Search by amount - using invoice number as proxy (if it represents amount)
        // No TOP limit - return all invoices in the amount range
        string query;

        if (includeVendor)
        {
            query = $@"
                SELECT
                    i.itemnum as InvoiceId,
                    CAST(ki106.keyvaluesmall AS VARCHAR(50)) as InvoiceNumber,
                    RTRIM(kt105.keyvaluechar) as VendorName,
                    NULL as Amount,
                    ki112.keyvaluedate as InvoiceDate,
                    NULL as DueDate,
                    NULL as Status,
                    kt104.keyvaluechar as PONumber,
                    i.itemname as Description,
                    it.itemtypename as DocumentType,
                    i.itemdate as CreatedDate
                FROM hsi.itemdata i WITH (NOLOCK)
                INNER JOIN hsi.itemtype it WITH (NOLOCK) ON i.itemtypenum = it.itemtypenum
                LEFT OUTER JOIN hsi.keyitem106 ki106 WITH (NOLOCK) ON i.itemnum = ki106.itemnum
                LEFT OUTER JOIN hsi.keyxitem104 kx104 WITH (NOLOCK) ON i.itemnum = kx104.itemnum
                LEFT OUTER JOIN hsi.keytable104 kt104 WITH (NOLOCK) ON kx104.keywordnum = kt104.keywordnum
                LEFT OUTER JOIN hsi.keyitem112 ki112 WITH (NOLOCK) ON i.itemnum = ki112.itemnum
                LEFT OUTER JOIN hsi.keyxitem105 kx105 WITH (NOLOCK) ON i.itemnum = kx105.itemnum
                LEFT OUTER JOIN hsi.keytable105 kt105 WITH (NOLOCK) ON kx105.keywordnum = kt105.keywordnum
                WHERE {GetDocumentTypeFilter(documentType)}
                    AND ki106.keyvaluesmall BETWEEN @MinAmount AND @MaxAmount
                ORDER BY ki106.keyvaluesmall DESC";
        }
        else
        {
            query = $@"
                SELECT
                    i.itemnum as InvoiceId,
                    CAST(ki106.keyvaluesmall AS VARCHAR(50)) as InvoiceNumber,
                    NULL as VendorName,
                    NULL as Amount,
                    ki112.keyvaluedate as InvoiceDate,
                    NULL as DueDate,
                    NULL as Status,
                    kt104.keyvaluechar as PONumber,
                    i.itemname as Description,
                    it.itemtypename as DocumentType,
                    i.itemdate as CreatedDate
                FROM hsi.itemdata i WITH (NOLOCK)
                INNER JOIN hsi.itemtype it WITH (NOLOCK) ON i.itemtypenum = it.itemtypenum
                LEFT OUTER JOIN hsi.keyitem106 ki106 WITH (NOLOCK) ON i.itemnum = ki106.itemnum
                LEFT OUTER JOIN hsi.keyxitem104 kx104 WITH (NOLOCK) ON i.itemnum = kx104.itemnum
                LEFT OUTER JOIN hsi.keytable104 kt104 WITH (NOLOCK) ON kx104.keywordnum = kt104.keywordnum
                LEFT OUTER JOIN hsi.keyitem112 ki112 WITH (NOLOCK) ON i.itemnum = ki112.itemnum
                WHERE {GetDocumentTypeFilter(documentType)}
                    AND ki106.keyvaluesmall BETWEEN @MinAmount AND @MaxAmount
                ORDER BY ki106.keyvaluesmall DESC";
        }

        return (await connection.QueryAsync<Invoice>(query, new { MinAmount = minAmount, MaxAmount = maxAmount }, commandTimeout: 60)).ToList();
    }

    public async Task<List<Invoice>> GetAllInvoicesAsync(int limit = 100, string documentType = "all")
    {
        using var connection = GetConnection();

        // Limit to 50 for performance
        if (limit > 50) limit = 50;

        bool includeVendor = ShouldIncludeVendorField(documentType);

        // Query with all joins - start from itemdata and filter first
        string query;

        if (includeVendor)
        {
            query = $@"
                SELECT TOP (@Limit)
                    i.itemnum as InvoiceId,
                    CAST(ki106.keyvaluesmall AS VARCHAR(50)) as InvoiceNumber,
                    RTRIM(kt105.keyvaluechar) as VendorName,
                    NULL as Amount,
                    ki112.keyvaluedate as InvoiceDate,
                    NULL as DueDate,
                    NULL as Status,
                    kt104.keyvaluechar as PONumber,
                    i.itemname as Description,
                    it.itemtypename as DocumentType,
                    i.itemdate as CreatedDate
                FROM hsi.itemdata i WITH (NOLOCK)
                INNER JOIN hsi.itemtype it WITH (NOLOCK) ON i.itemtypenum = it.itemtypenum
                LEFT OUTER JOIN hsi.keyitem106 ki106 WITH (NOLOCK) ON i.itemnum = ki106.itemnum
                LEFT OUTER JOIN hsi.keyxitem104 kx104 WITH (NOLOCK) ON i.itemnum = kx104.itemnum
                LEFT OUTER JOIN hsi.keytable104 kt104 WITH (NOLOCK) ON kx104.keywordnum = kt104.keywordnum
                LEFT OUTER JOIN hsi.keyitem112 ki112 WITH (NOLOCK) ON i.itemnum = ki112.itemnum
                LEFT OUTER JOIN hsi.keyxitem105 kx105 WITH (NOLOCK) ON i.itemnum = kx105.itemnum
                LEFT OUTER JOIN hsi.keytable105 kt105 WITH (NOLOCK) ON kx105.keywordnum = kt105.keywordnum
                WHERE {GetDocumentTypeFilter(documentType)}
                ORDER BY i.itemdate DESC";
        }
        else
        {
            query = $@"
                SELECT TOP (@Limit)
                    i.itemnum as InvoiceId,
                    CAST(ki106.keyvaluesmall AS VARCHAR(50)) as InvoiceNumber,
                    NULL as VendorName,
                    NULL as Amount,
                    ki112.keyvaluedate as InvoiceDate,
                    NULL as DueDate,
                    NULL as Status,
                    kt104.keyvaluechar as PONumber,
                    i.itemname as Description,
                    it.itemtypename as DocumentType,
                    i.itemdate as CreatedDate
                FROM hsi.itemdata i WITH (NOLOCK)
                INNER JOIN hsi.itemtype it WITH (NOLOCK) ON i.itemtypenum = it.itemtypenum
                LEFT OUTER JOIN hsi.keyitem106 ki106 WITH (NOLOCK) ON i.itemnum = ki106.itemnum
                LEFT OUTER JOIN hsi.keyxitem104 kx104 WITH (NOLOCK) ON i.itemnum = kx104.itemnum
                LEFT OUTER JOIN hsi.keytable104 kt104 WITH (NOLOCK) ON kx104.keywordnum = kt104.keywordnum
                LEFT OUTER JOIN hsi.keyitem112 ki112 WITH (NOLOCK) ON i.itemnum = ki112.itemnum
                WHERE {GetDocumentTypeFilter(documentType)}
                ORDER BY i.itemdate DESC";
        }

        return (await connection.QueryAsync<Invoice>(query, new { Limit = limit }, commandTimeout: 60)).ToList();
    }

    /// <summary>
    /// Helper method to get document type filter for WHERE clause
    /// </summary>
    private string GetDocumentTypeFilter(string documentType)
    {
        return documentType.ToLower() switch
        {
            "customer" => "i.itemtypenum = 102", // Customer Invoices
            "customer_credit" => "i.itemtypenum = 119", // Customer Credit Memos
            "vendor" => "i.itemtypenum IN (263, 340, 364, 429, 515)", // Vendor Invoices (all companies)
            "vendor_credit" => "i.itemtypenum = 0", // Vendor credit memos (not found yet)
            "credit" => "i.itemtypenum IN (119)", // All credit memos
            "all" => "i.itemtypenum IN (102, 119, 263, 340, 364, 429, 515)", // All document types
            _ => "i.itemtypenum IN (102, 119, 263, 340, 364, 429, 515)" // Default to all
        };
    }

    /// <summary>
    /// Helper method to determine if vendor field should be included
    /// </summary>
    private bool ShouldIncludeVendorField(string documentType)
    {
        return documentType.ToLower() switch
        {
            "vendor" => true,
            "vendor_credit" => true,
            "all" => true,
            _ => false
        };
    }
}
