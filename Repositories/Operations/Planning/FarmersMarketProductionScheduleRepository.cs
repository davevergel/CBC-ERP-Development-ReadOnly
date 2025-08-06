using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CbcRoastersErp.Helpers;
using CbcRoastersErp.Models.Planning;
using MySqlConnector;

namespace CbcRoastersErp.Repositories.Operations.Planning
{
    public class FarmersMarketProductionScheduleRepository
    {
        public async Task<List<FarmersMarketProductionSchedule>> GetSchedulesPagedAsync(int page, int pageSize)
        {
            var schedules = new List<FarmersMarketProductionSchedule>();
            try
            {
                using (var connection = await DatabaseHelper.GetOpenConnectionAsync())
                {
                    var command = new MySqlCommand(@"
                        SELECT * FROM farmers_market_production_schedule
                        ORDER BY market_date DESC
                        LIMIT @offset, @pageSize", (MySqlConnection)connection);

                    command.Parameters.AddWithValue("@offset", (page - 1) * pageSize);
                    command.Parameters.AddWithValue("@pageSize", pageSize);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            schedules.Add(new FarmersMarketProductionSchedule
                            {
                                Id = reader.GetInt32("id"),
                                MarketDate = reader.GetDateTime("market_date"),
                                CreatedBy = reader["created_by"]?.ToString(),
                                CreatedAt = reader.GetDateTime("created_at"),
                                Notes = reader["notes"]?.ToString()
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ApplicationLogger.Log(ex, nameof(GetSchedulesPagedAsync), nameof(FarmersMarketProductionScheduleRepository), Environment.UserName);
            }
            return schedules;
        }

        public async Task<int> GetSchedulesCountAsync()
        {
            try
            {
                using (var connection = await DatabaseHelper.GetOpenConnectionAsync())
                {
                    var command = new MySqlCommand("SELECT COUNT(*) FROM farmers_market_production_schedule", (MySqlConnection)connection);
                    var count = Convert.ToInt32(await command.ExecuteScalarAsync());
                    return count;
                }
            }
            catch (Exception ex)
            {
                ApplicationLogger.Log(ex, nameof(GetSchedulesCountAsync), nameof(FarmersMarketProductionScheduleRepository), Environment.UserName);
                return 0;
            }
        }

        public async Task<FarmersMarketProductionSchedule> GetScheduleByIdAsync(int id)
        {
            FarmersMarketProductionSchedule schedule = null;
            try
            {
                using (var connection = await DatabaseHelper.GetOpenConnectionAsync())
                {
                    var command = new MySqlCommand(@"
                        SELECT * FROM farmers_market_production_schedule
                        WHERE id = @id", (MySqlConnection)connection);

                    command.Parameters.AddWithValue("@id", id);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            schedule = new FarmersMarketProductionSchedule
                            {
                                Id = reader.GetInt32("id"),
                                MarketDate = reader.GetDateTime("market_date"),
                                CreatedBy = reader["created_by"]?.ToString(),
                                CreatedAt = reader.GetDateTime("created_at"),
                                Notes = reader["notes"]?.ToString()
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ApplicationLogger.Log(ex, nameof(GetScheduleByIdAsync), nameof(FarmersMarketProductionScheduleRepository), Environment.UserName);
            }
            return schedule;
        }

        public async Task<int> AddScheduleAsync(FarmersMarketProductionSchedule schedule)
        {
            try
            {
                using (var connection = await DatabaseHelper.GetOpenConnectionAsync())
                {
                    var command = new MySqlCommand(@"
                        INSERT INTO farmers_market_production_schedule
                        (market_date, created_by, created_at, notes)
                        VALUES
                        (@market_date, @created_by, @created_at, @notes);
                        SELECT LAST_INSERT_ID();", (MySqlConnection)connection);

                    command.Parameters.AddWithValue("@market_date", schedule.MarketDate);
                    command.Parameters.AddWithValue("@created_by", schedule.CreatedBy ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@created_at", schedule.CreatedAt);
                    command.Parameters.AddWithValue("@notes", schedule.Notes ?? (object)DBNull.Value);

                    var insertedId = Convert.ToInt32(await command.ExecuteScalarAsync());
                    return insertedId;
                }
            }
            catch (Exception ex)
            {
                ApplicationLogger.Log(ex, nameof(AddScheduleAsync), nameof(FarmersMarketProductionScheduleRepository), Environment.UserName);
                return -1;
            }
        }

        public async Task<bool> UpdateScheduleAsync(FarmersMarketProductionSchedule schedule)
        {
            try
            {
                using (var connection = await DatabaseHelper.GetOpenConnectionAsync())
                {
                    var command = new MySqlCommand(@"
                        UPDATE farmers_market_production_schedule
                        SET
                            market_date = @market_date,
                            created_by = @created_by,
                            created_at = @created_at,
                            notes = @notes
                        WHERE id = @id", (MySqlConnection)connection);

                    command.Parameters.AddWithValue("@id", schedule.Id);
                    command.Parameters.AddWithValue("@market_date", schedule.MarketDate);
                    command.Parameters.AddWithValue("@created_by", schedule.CreatedBy ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@created_at", schedule.CreatedAt);
                    command.Parameters.AddWithValue("@notes", schedule.Notes ?? (object)DBNull.Value);

                    var rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                ApplicationLogger.Log(ex, nameof(UpdateScheduleAsync), nameof(FarmersMarketProductionScheduleRepository), Environment.UserName);
                return false;
            }
        }

        public async Task<bool> DeleteScheduleAsync(int id)
        {
            try
            {
                using (var connection = await DatabaseHelper.GetOpenConnectionAsync())
                {
                    var command = new MySqlCommand(@"
                        DELETE FROM farmers_market_production_schedule
                        WHERE id = @id", (MySqlConnection)connection);

                    command.Parameters.AddWithValue("@id", id);

                    var rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                ApplicationLogger.Log(ex, nameof(DeleteScheduleAsync), nameof(FarmersMarketProductionScheduleRepository), Environment.UserName);
                return false;
            }
        }
    }
}
