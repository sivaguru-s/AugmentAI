using FinanceBudget.Models;
using Microsoft.Data.SqlClient;
using Dapper;

namespace FinanceBudget.Services;

public class UnitBudgetService : IUnitBudgetService
{
    private readonly string _connectionString;
    private readonly ILogger<UnitBudgetService> _logger;

    public UnitBudgetService(IConfiguration configuration, ILogger<UnitBudgetService> logger)
    {
        _connectionString = configuration.GetConnectionString("AS400Database") 
            ?? throw new ArgumentNullException("AS400Database connection string is not configured");
        _logger = logger;
    }

    public async Task<List<UnitBudget>> GetUnitBudgetDataAsync(string? unitCode = null, string? natureId = null)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                EXEC ('SELECT
                    UNIT.ASALCD as Unit,
                    UNIT.ASAQNA as UnitDescription,
                    NATURE.AHAFCD as Nature,
                    Nature.AHADNA as NatureDescription,
                    GLFile.A7AKNB as TransactionId,
                    TRANSACTION.ARALNB as TransactionNumber,
                    GLFile.A7ADZZ as Amount
                FROM AMFLIBA.YAASREP as UNIT
                INNER JOIN AMFLIBA.YAC4REP as UNITNATURE ON UNIT.asalcd = UNITNATURE.C4ALCD
                INNER JOIN AMFLIBA.YAAHREP as NATURE ON NATURE.AHAFCD = UNITNATURE.C4AFCD
                INNER JOIN AMFLIBA.YAA7REP as GLFile ON GLFile.A7KMCD = UNIT.asalcd AND GLFile.A7KNCD = NATURE.AHAFCD
                INNER JOIN AMFLIBA.YAARREP as TRANSACTION ON TRANSACTION.ARAKNB = GLFile.A7AKNB
                WHERE TRANSACTION.AREDST = ''5''";

            // Add unit filter if provided
            if (!string.IsNullOrEmpty(unitCode))
            {
                query += " AND UNIT.asalcd = ''" + unitCode + "''";
            }

            // Add nature filter if provided
            if (!string.IsNullOrEmpty(natureId))
            {
                query += " AND NATURE.AHAFCD = ''" + natureId + "''";
            }

            query += "') at DB2";

            var result = await connection.QueryAsync<UnitBudget>(query);
            return result.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching unit budget data from AS400");
            throw;
        }
    }

    public async Task<List<Unit>> GetAllUnitsAsync()
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"EXEC ('SELECT ASALCD as UnitCode, ASAQNA as UnitDescription FROM AMFLIBA.YAASREP') at DB2";

            var result = await connection.QueryAsync<Unit>(query);
            return result.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching units from AS400");
            throw;
        }
    }

    public async Task<List<Nature>> GetNaturesByUnitAsync(string unitCode)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                EXEC ('SELECT AHAFCD as NatureId, AHADNA as NatureDescription
                FROM AMFLIBA.YAAHREP as NATURE
                INNER JOIN AMFLIBA.YAC4REP as UNITNATURE ON NATURE.AHAFCD = UNITNATURE.C4AFCD
                WHERE C4ALCD = ''" + unitCode + "''') at DB2";

            var result = await connection.QueryAsync<Nature>(query);
            return result.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching natures from AS400");
            throw;
        }
    }

    public async Task<List<UnitBudget>> GetUnitBudgetByTransactionStatusAsync(string unitCode, string transactionStatus)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                SELECT 
                    UNIT.ASALCD as Unit,
                    UNIT.ASAQNA as UnitDescription,
                    NATURE.AHAFCD as Nature,
                    Nature.AHADNA as NatureDescription,
                    GLFile.A7AKNB as TransactionId, 
                    TRANSACTION.ARALNB as TransactionNumber,
                    GLFile.A7ADZZ as Amount
                FROM AMFLIBA.YAASREP as UNIT 
                INNER JOIN AMFLIBA.YAC4REP as UNITNATURE ON UNIT.asalcd = UNITNATURE.C4ALCD 
                INNER JOIN AMFLIBA.YAAHREP as NATURE ON NATURE.AHAFCD = UNITNATURE.C4AFCD
                INNER JOIN AMFLIBA.YAA7REP as GLFile ON GLFile.A7KMCD = UNIT.asalcd AND GLFile.A7KNCD = NATURE.AHAFCD
                INNER JOIN AMFLIBA.YAARREP as TRANSACTION ON TRANSACTION.ARAKNB = GLFile.A7AKNB
                WHERE UNIT.asalcd = @UnitCode AND TRANSACTION.AREDST = @TransactionStatus";

            var result = await connection.QueryAsync<UnitBudget>(query, new { UnitCode = unitCode, TransactionStatus = transactionStatus });
            return result.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching unit budget data by transaction status from AS400");
            throw;
        }
    }
}

