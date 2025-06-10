using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using CbcRoastersErp.Models;
using CbcRoastersErp.Helpers;

namespace CbcRoastersErp.Repositories
{
    public class BatchScheduleRepository
    {
        private readonly IDbConnection _dbConnection;

        public BatchScheduleRepository()
        {
            _dbConnection = DatabaseHelper.GetConnection();
        }

        public IEnumerable<BatchSchedule> GetAllSchedules()
        {
            try
            {
                const string query = @"
                SELECT bs.*, fg.ProductName AS FinishedGoodName
                FROM BatchSchedule bs
                LEFT JOIN FinishedGoods fg ON bs.FinishedGoodID = fg.FinishedGoodID
                ORDER BY bs.ScheduledDate DESC";

                return _dbConnection.Query<BatchSchedule>(query);
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                ApplicationLogger.Log(ex, "System", "Error");
                return Enumerable.Empty<BatchSchedule>();
            }
        }
        public void AddSchedule(BatchSchedule schedule)
        {
            try
            {
                const string query = @"
                INSERT INTO BatchSchedule (OrderID, FinishedGoodID, Quantity, ScheduledDate, Status, Notes)
                VALUES (@OrderID, @FinishedGoodID, @Quantity, @ScheduledDate, @Status, @Notes)";

                _dbConnection.Execute(query, schedule);

                // Log successful insertion
                ApplicationLogger.LogInfo($"Inserted new schedule: {schedule}", "System", "Info");
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                ApplicationLogger.Log(ex, "System", "Error");
            }
        }

        public void UpdateScheduleStatus(int scheduleId, string status)
        {
            try
            {
                const string query = @"
                UPDATE BatchSchedule
                SET Status = @Status
                WHERE ScheduleID = @ScheduleID";

                _dbConnection.Execute(query, new { ScheduleID = scheduleId, Status = status });
                // Log successful update
                ApplicationLogger.LogInfo($"Updated schedule status: ScheduleID={scheduleId}, Status={status}", "System", "Info");
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                ApplicationLogger.Log(ex, "System", "Error");
            }
        }

        public void DeleteSchedule(int scheduleId)
        {
            try
            {
                const string query = "DELETE FROM BatchSchedule WHERE ScheduleID = @ScheduleID";
                _dbConnection.Execute(query, new { ScheduleID = scheduleId });
                // Log successful deletion
                ApplicationLogger.LogInfo($"Deleted schedule: ScheduleID={scheduleId}", "System", "Info");
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                ApplicationLogger.Log(ex, "System", "Error");
            }
        }

        public BatchSchedule? GetScheduleById(int scheduleId)
        {
            try
            {
                const string query = @"
                SELECT bs.*, fg.ProductName AS FinishedGoodName
                FROM BatchSchedule bs
                LEFT JOIN FinishedGoods fg ON bs.FinishedGoodID = fg.FinishedGoodID   
                WHERE bs.ScheduleID = @ScheduleID
                ORDER BY bs.ScheduledDate DESC";
                var row = _dbConnection.QuerySingleOrDefault<BatchSchedule>(query, new { ScheduleID = scheduleId });

                return row == null ? null : new BatchSchedule
                {
                    ScheduleID = row.ScheduleID,
                    OrderID = row.OrderID,
                    FinishedGoodID = row.FinishedGoodID,
                    Quantity = row.Quantity,
                    ScheduledDate = row.ScheduledDate,
                    Status = row.Status,
                    Notes = row.Notes,
                    FinishedGoodName = row.FinishedGoodName
                };
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                ApplicationLogger.Log(ex, "System", "Error");
                return null;
            }

        }

        public IEnumerable<FinishedGoods> GetAllFinishedGoods()
        {
            try
            {
                const string query = "SELECT FinishedGoodID, ProductName FROM FinishedGoods ORDER BY ProductName";
                return _dbConnection.Query<FinishedGoods>(query).ToList();
            }
            catch (Exception ex)
            {
                ApplicationLogger.Log(ex, "System", "Error");
                return Enumerable.Empty<FinishedGoods>();
            }
        }
    }
}

