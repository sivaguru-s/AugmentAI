using Dapper;
using Microsoft.Data.SqlClient;
using Chatbot_Onbase.Models;

namespace Chatbot_Onbase.Data;

public interface IOnbaseRepository
{
    Task<List<Invoice>> SearchInvoicesAsync(string searchTerm);
    Task<List<Invoice>> GetInvoicesByVendorAsync(string vendorName);
    Task<List<Invoice>> GetInvoicesByNumberAsync(string invoiceNumber);
    Task<List<Invoice>> GetInvoicesByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<List<Invoice>> GetInvoicesByStatusAsync(string status);
    Task<List<Invoice>> GetInvoicesByAmountRangeAsync(decimal minAmount, decimal maxAmount);
    Task<List<Invoice>> GetAllInvoicesAsync(int limit = 100);
}

public class OnbaseRepository : IOnbaseRepository
{
    private readonly string _connectionString;

    public OnbaseRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("OnBaseConnection") 
            ?? throw new ArgumentNullException("OnBaseConnection not found in configuration");
    }

    private SqlConnection GetConnection() => new SqlConnection(_connectionString);

    public async Task<List<Invoice>> SearchInvoicesAsync(string searchTerm)
    {
        using var connection = GetConnection();

        // Try to parse as numeric for exact invoice number match
        int numericSearch = 0;
        bool isNumeric = int.TryParse(searchTerm, out numericSearch);

        string query;
        object parameters;

        if (isNumeric)
        {
            // If numeric, search by invoice number with all joins
            query = @"
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
                FROM hsi.keyitem106 ki106 WITH (NOLOCK)
                INNER JOIN hsi.itemdata i WITH (NOLOCK) ON i.itemnum = ki106.itemnum AND i.itemtypenum = 102
                LEFT OUTER JOIN hsi.keyxitem104 kx104 WITH (NOLOCK) ON i.itemnum = kx104.itemnum
                LEFT OUTER JOIN hsi.keytable104 kt104 WITH (NOLOCK) ON kx104.keywordnum = kt104.keywordnum
                LEFT OUTER JOIN hsi.keyitem112 ki112 WITH (NOLOCK) ON i.itemnum = ki112.itemnum
                WHERE ki106.keyvaluesmall = @SearchTermNumeric
                ORDER BY i.itemdate DESC";

            parameters = new { SearchTermNumeric = numericSearch };
        }
        else
        {
            // If text, return recent invoices with all data
            query = @"
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

            parameters = new { };
        }

        var result = await connection.QueryAsync<Invoice>(query, parameters, commandTimeout: 60);
        return result.ToList();
    }

    public async Task<List<Invoice>> GetInvoicesByVendorAsync(string vendorName)
    {
        using var connection = GetConnection();

        // Note: Vendor name is not available in current schema
        // keytable104 contains Order Number, not Vendor Name
        // Returning empty list until vendor table is identified
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
                AND i.itemname LIKE @VendorName + '%'
            ORDER BY i.itemdate DESC";

        return (await connection.QueryAsync<Invoice>(query, new { VendorName = vendorName }, commandTimeout: 30)).ToList();
    }

    public async Task<List<Invoice>> GetInvoicesByNumberAsync(string invoiceNumber)
    {
        using var connection = GetConnection();

        // Parse invoice number as integer
        int invoiceNumberInt = 0;
        if (!int.TryParse(invoiceNumber, out invoiceNumberInt) || invoiceNumberInt == 0)
        {
            // If not a valid number, return empty list
            return new List<Invoice>();
        }

        // Search by Invoice Number in keyitem106 - use exact match
        // Start from keyitem106 for better performance (smaller table, indexed)
        var query = @"
            SELECT TOP 1
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
            FROM hsi.keyitem106 ki106 WITH (NOLOCK)
            INNER JOIN hsi.itemdata i WITH (NOLOCK) ON i.itemnum = ki106.itemnum AND i.itemtypenum = 102
            LEFT OUTER JOIN hsi.keyxitem104 kx104 WITH (NOLOCK) ON i.itemnum = kx104.itemnum
            LEFT OUTER JOIN hsi.keytable104 kt104 WITH (NOLOCK) ON kx104.keywordnum = kt104.keywordnum
            LEFT OUTER JOIN hsi.keyitem112 ki112 WITH (NOLOCK) ON i.itemnum = ki112.itemnum
            WHERE ki106.keyvaluesmall = @InvoiceNumberInt
            ORDER BY i.itemdate DESC";

        return (await connection.QueryAsync<Invoice>(query, new { InvoiceNumberInt = invoiceNumberInt }, commandTimeout: 60)).ToList();
    }

    public async Task<List<Invoice>> GetInvoicesByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        using var connection = GetConnection();

        // Search by Invoice Date in keyitem112
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
                AND ki112.keyvaluedate BETWEEN @StartDate AND @EndDate
            ORDER BY ki112.keyvaluedate DESC";

        return (await connection.QueryAsync<Invoice>(query, new { StartDate = startDate, EndDate = endDate }, commandTimeout: 30)).ToList();
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

    public async Task<List<Invoice>> GetInvoicesByAmountRangeAsync(decimal minAmount, decimal maxAmount)
    {
        using var connection = GetConnection();

        // Search by amount - using invoice number as proxy (if it represents amount)
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
                AND ki106.keyvaluesmall BETWEEN @MinAmount AND @MaxAmount
            ORDER BY ki106.keyvaluesmall DESC";

        return (await connection.QueryAsync<Invoice>(query, new { MinAmount = minAmount, MaxAmount = maxAmount }, commandTimeout: 30)).ToList();
    }

    public async Task<List<Invoice>> GetAllInvoicesAsync(int limit = 100)
    {
        using var connection = GetConnection();

        // Limit to 50 for performance
        if (limit > 50) limit = 50;

        // Query with all joins - start from itemdata and filter first
        var query = @"
            SELECT TOP (@Limit)
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

        return (await connection.QueryAsync<Invoice>(query, new { Limit = limit }, commandTimeout: 60)).ToList();
    }
}
