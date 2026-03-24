using Dapper;
using DemoWebAPI.model.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
        return new SqlConnection(_connectionString);
    }

    // 🔹 Query (Multiple Rows)
    protected BaseAPIResponse<IEnumerable<T>> Query<T>(string sql, object param = null, CommandType commandType = CommandType.StoredProcedure)
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
    protected BaseAPIResponse<T> QueryFirstOrDefault<T>(string sql, object param = null, CommandType commandType = CommandType.StoredProcedure)
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
    protected BaseAPIResponse<long> Execute(string sql, object param = null, CommandType commandType = CommandType.StoredProcedure)
    {
        try
        {
            using (var connection = CreateConnection())
            {
                // Get the first column of the first row (your SELECT result)
                var result = connection.ExecuteScalar<long>(sql, param, commandType: commandType);

                return new BaseAPIResponse<long>(true, "Execution completed successfully.", result);
            }
        }
        catch (Exception ex)
        {
            return new BaseAPIResponse<long>(false, ex.Message, 0);
        }
    }

    // 🔹 QueryMultiple (Multiple Result Sets) - NEW
    protected async Task<BaseAPIResponse<T>> QueryMultipleAsync<T>(
        string sql,
        object param,
        Func<SqlMapper.GridReader, Task<T>> map,
        CommandType commandType = CommandType.StoredProcedure)
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
}
