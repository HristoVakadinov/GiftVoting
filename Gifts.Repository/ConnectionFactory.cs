using System;
using System.Data;
using System.Data.SqlClient;

namespace Gifts.Repository
{
    public static class ConnectionFactory
    {
        private static string _connectionString;

        public static void Initialize(string connectionString)
        {
            _connectionString = connectionString;
        }

        public static async Task<SqlConnection> Connect()
        {
            var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            return connection;
        }

    }
}
