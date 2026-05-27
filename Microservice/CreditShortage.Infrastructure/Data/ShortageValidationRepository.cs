using CreditShortage.Application.Interfaces;
using CreditShortage.Domain.Entities;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data;

namespace CreditShortage.Infrastructure.Data;

/// <summary>
/// Repository implementation for shortage validation data access
/// </summary>
public class ShortageValidationRepository : IShortageValidationRepository
{
    private readonly string _connectionString;
    private readonly ILogger<ShortageValidationRepository> _logger;
    private readonly int _commandTimeout;

    public ShortageValidationRepository(
        IConfiguration configuration,
        ILogger<ShortageValidationRepository> logger)
    {
        _connectionString = configuration.GetConnectionString("DatawhseDatabase")
            ?? throw new InvalidOperationException("DatawhseDatabase connection string not configured");
        _logger = logger;
        _commandTimeout = int.Parse(configuration["ShortageValidationSettings:CommandTimeout"] ?? "60");
    }

    public async Task<List<ShortageValidationResult>> ValidateShortageItemsAsync(
        List<ShortageValidationInput> items,
        string environment,
        string defaultDefectCode,
        string defaultLocationCode)
    {
        _logger.LogInformation("Validating {Count} shortage items in database", items.Count);

        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        // Create a DataTable for the Table-Valued Parameter
        var dataTable = CreateShortageItemDataTable(items);

        var parameters = new DynamicParameters();
        parameters.Add("@ShortageItems", dataTable.AsTableValuedParameter("typCEShortageItemValidation"));
        parameters.Add("@Environment", environment);
        parameters.Add("@DefaultDefectCode", defaultDefectCode);
        parameters.Add("@DefaultLocationCode", defaultLocationCode);

        try
        {
            var results = await connection.QueryAsync<ShortageValidationResult>(
                "usp_CE_ValidateShortageItems",
                parameters,
                commandType: CommandType.StoredProcedure,
                commandTimeout: _commandTimeout);

            _logger.LogInformation("Successfully validated {Count} shortage items", results.Count());

            return results.ToList();
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "SQL error validating shortage items");
            throw new InvalidOperationException("Database error during validation. " + ex.Message, ex);
        }
    }

    public async Task<List<DefectCode>> GetActiveDefectCodesAsync()
    {
        _logger.LogInformation("Fetching active defect codes from database");

        using var connection = new SqlConnection(_connectionString);

        // Adjust table name based on actual database schema
        var sql = @"
            SELECT 
                DefectCode as Code,
                DefectDescription as Description,
                CAST(1 AS BIT) as IsActive
            FROM tblDefectCodes
            WHERE IsActive = 1
            ORDER BY DefectCode";

        try
        {
            var results = await connection.QueryAsync<DefectCode>(sql, commandTimeout: _commandTimeout);
            return results.ToList();
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "Error fetching defect codes");
            throw new InvalidOperationException("Database error fetching defect codes. " + ex.Message, ex);
        }
    }

    public async Task<List<LocationCode>> GetActiveLocationCodesAsync()
    {
        _logger.LogInformation("Fetching active location codes from database");

        using var connection = new SqlConnection(_connectionString);

        // Adjust table name based on actual database schema
        var sql = @"
            SELECT 
                WarehouseCode as Code,
                WarehouseName as Description,
                CAST(1 AS BIT) as IsActive
            FROM tblWarehouse
            WHERE IsActive = 1
            ORDER BY WarehouseCode";

        try
        {
            var results = await connection.QueryAsync<LocationCode>(sql, commandTimeout: _commandTimeout);
            return results.ToList();
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "Error fetching location codes");
            throw new InvalidOperationException("Database error fetching location codes. " + ex.Message, ex);
        }
    }

    /// <summary>
    /// Creates a DataTable for the Table-Valued Parameter
    /// </summary>
    private DataTable CreateShortageItemDataTable(List<ShortageValidationInput> items)
    {
        var table = new DataTable();
        table.Columns.Add("CustomerNumber", typeof(string));
        table.Columns.Add("ShipToNumber", typeof(string));
        table.Columns.Add("InvoiceNumber", typeof(int));
        table.Columns.Add("ItemNumber", typeof(string));
        table.Columns.Add("SerialNumber", typeof(string));
        table.Columns.Add("ShortageQuantity", typeof(int));
        table.Columns.Add("Amount", typeof(decimal));  // Added Amount column
        table.Columns.Add("DefectCode", typeof(string));
        table.Columns.Add("LocationCode", typeof(string));
        table.Columns.Add("OrderNumber", typeof(string));
        table.Columns.Add("OrderItemSeq", typeof(int));

        foreach (var item in items)
        {
            table.Rows.Add(
                item.CustomerNumber,
                item.ShipToNumber,
                item.InvoiceNumber,
                item.ItemNumber,
                item.SerialNumber,
                item.ShortageQuantity,
                item.Amount ?? (object)DBNull.Value,  // Added Amount parameter
                item.DefectCode ?? (object)DBNull.Value,
                item.LocationCode ?? (object)DBNull.Value,
                item.OrderNumber ?? (object)DBNull.Value,
                item.OrderItemSeq ?? (object)DBNull.Value
            );
        }

        return table;
    }
}
