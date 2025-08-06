using System.Data;
using CbcRoastersErp.Services;
using MySqlConnector;

namespace CbcRoastersErp.Helpers
{
    public static class DatabaseHelper
    {
        private static readonly string _connectionString = AppConfig.GetConnectionString();

        public static IDbConnection GetConnection()
        {
            return new MySqlConnection(_connectionString);
        }


        public static IDbConnection GetOpenConnection()
        {
            var connection = new MySqlConnection(_connectionString);
            connection.Open();
            return connection;
        }

        public static async Task<MySqlConnection> GetOpenConnectionAsync()
        {
            var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            return connection;
        }
    }
}
