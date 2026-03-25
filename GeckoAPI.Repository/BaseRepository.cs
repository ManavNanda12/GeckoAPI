using Dapper;
using DemoWebAPI.model.Models;
using Microsoft.Extensions.Options;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

public abstract class BaseRepository
{
    private readonly string _connectionString;

    protected BaseRepository(IOptions<DbConfig> config)
    {
        _connectionString = config.Value.DefaultConnection;
    }

    protected IDbConnection CreateConnection()
    {
        return new NpgsqlConnection(_connectionString);
    }

    // 🔹 Query (Multiple Rows)
    protected BaseAPIResponse<IEnumerable<T>> Query<T>(
        string sql,
        object param = null,
        CommandType commandType = CommandType.Text) // ✅ Changed
    {
        try
        {
            using (var connection = CreateConnection())
            {
                var result = connection.Query<T>(sql, param, commandType: commandType);
                return new BaseAPIResponse<IEnumerable<T>>(true, "Query executed successfully.", result);
            }
        }
        catch (Exception ex)
        {
            return new BaseAPIResponse<IEnumerable<T>>(false, ex.Message, default);
        }
    }

    // 🔹 Query Single (First Row)
    protected BaseAPIResponse<T> QueryFirstOrDefault<T>(
        string sql,
        object param = null,
        CommandType commandType = CommandType.Text) // ✅ Changed
    {
        try
        {
            using (var connection = CreateConnection())
            {
                var result = connection.QueryFirstOrDefault<T>(sql, param, commandType: commandType);
                return new BaseAPIResponse<T>(true, "Query executed successfully.", result);
            }
        }
        catch (Exception ex)
        {
            return new BaseAPIResponse<T>(false, ex.Message, default);
        }
    }

    // 🔹 Execute (Insert/Update/Delete)
    protected BaseAPIResponse<long> Execute(
        string sql,
        object param = null,
        CommandType commandType = CommandType.Text) // ✅ Changed
    {
        try
        {
            using (var connection = CreateConnection())
            {
                var result = connection.ExecuteScalar<long>(sql, param, commandType: commandType);
                return new BaseAPIResponse<long>(true, "Execution completed successfully.", result);
            }
        }
        catch (Exception ex)
        {
            return new BaseAPIResponse<long>(false, ex.Message, 0);
        }
    }

    // 🔹 QueryMultiple (Multiple Result Sets)
    protected async Task<BaseAPIResponse<T>> QueryMultipleAsync<T>(
        string sql,
        object param,
        Func<SqlMapper.GridReader, Task<T>> map,
        CommandType commandType = CommandType.Text) // ✅ Changed
    {
        try
        {
            using (var connection = CreateConnection())
            {
                using (var multi = await connection.QueryMultipleAsync(sql, param, commandType: commandType))
                {
                    var result = await map(multi);
                    return new BaseAPIResponse<T>(true, "Query executed successfully.", result);
                }
            }
        }
        catch (Exception ex)
        {
            return new BaseAPIResponse<T>(false, ex.Message, default);
        }
    }

    #region Helper (IMPORTANT)
    protected string GetPgFunctionQuery(string functionName, bool isTable = true, string parameters = "")
    {
        var fn = functionName.ToLower();
        return isTable
       ? $"SELECT * FROM {fn}({parameters})"
       : $"SELECT {fn}({parameters})";
    }
    #endregion
}