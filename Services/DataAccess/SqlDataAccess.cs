using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Randstad.RSM.PollingService.PwP.Services.DataAccess
{
    public class SqlDataAccess : IDataAccess
    {
        private readonly string _connectionString;

        public SqlDataAccess(string connectionString)
        {
            _connectionString = connectionString;
        }
        public async Task<int> ExecuteAsync(string sqlCommand, Guid correlationId)
        {
            using var connection = EnsureConnection(GetConnection);
            return await connection.ExecuteAsync(sqlCommand);
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string sqlSelect, Guid correlationId)
        {
            using var connection = EnsureConnection(GetConnection);
            return await connection.QueryAsync<T>(sqlSelect);
        }

        private IDbConnection GetConnection => new SqlConnection(_connectionString);

        private IDbConnection EnsureConnection(IDbConnection connection)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            return connection;
        }
    }
}
