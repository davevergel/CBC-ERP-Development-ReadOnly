using CbcRoastersErp.Helpers;
using CbcRoastersErp.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CbcRoastersErp.Repositories
{
    public class ApplicationLogRepository
    {
        public List<ApplicationErrorLogs> GetLogs(string searchTerm, int page, int pageSize, out int totalCount)
        {
            try
            {
                using var connection = DatabaseHelper.GetOpenConnection();

                string query = @"SELECT * FROM ApplicationErrorLogs
                             WHERE Message LIKE @Search OR Source LIKE @Search OR Method LIKE @Search OR UserName LIKE @Search
                             ORDER BY Timestamp DESC
                             LIMIT @Offset, @PageSize";

                string countQuery = @"SELECT COUNT(*) FROM ApplicationErrorLogs
                                  WHERE Message LIKE @Search OR Source LIKE @Search OR Method LIKE @Search OR UserName LIKE @Search";

                var parameters = new
                {
                    Search = $"%{searchTerm}%",
                    Offset = (page - 1) * pageSize,
                    PageSize = pageSize
                };

                totalCount = connection.ExecuteScalar<int>(countQuery, parameters);

                // Log successful query execution
                ApplicationLogger.LogInfo($"Executed query: {query} with parameters: {parameters}", "System", "Info");

                return connection.Query<ApplicationErrorLogs>(query, parameters).ToList();
            }
            catch (Exception ex)
            {
                // Log the exception
                       ApplicationLogger.Log(ex, "System", "Error");
                totalCount = 0;
                return new List<ApplicationErrorLogs>();
            }
        }
    }
}

