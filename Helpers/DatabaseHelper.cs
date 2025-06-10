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


        // New method for returning an open connection
        public static IDbConnection GetOpenConnection()
        {
            var connection = new MySqlConnection(_connectionString);
            connection.Open();
            return connection;
        }
    }
}
