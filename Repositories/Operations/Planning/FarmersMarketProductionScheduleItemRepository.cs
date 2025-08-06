using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using CbcRoastersErp.Helpers;
using CbcRoastersErp.Models.Planning;
using MySqlConnector;

namespace CbcRoastersErp.Repositories.Operations.Planning
{
    public class FarmersMarketProductionScheduleItemRepository
    {
        public async Task<int> GetItemsCountByScheduleIdAsync(int scheduleId)
        {
            try
            {
                using (var connection = await DatabaseHelper.GetOpenConnectionAsync())
                {
                    var command = new MySqlCommand(@"
                SELECT COUNT(*) FROM farmers_market_production_schedule_item
                WHERE schedule_id = @schedule_id", (MySqlConnection)connection);

                    command.Parameters.AddWithValue("@schedule_id", scheduleId);

                    var count = Convert.ToInt32(await command.ExecuteScalarAsync());
                    return count;
                }
            }
            catch (Exception ex)
            {
                ApplicationLogger.Log(ex, nameof(GetItemsCountByScheduleIdAsync), nameof(FarmersMarketProductionScheduleItemRepository), Environment.UserName);
                return 0;
            }
        }

        public async Task<List<FarmersMarketProductionScheduleItem>> GetItemsPagedByScheduleIdAsync(int scheduleId, int page, int pageSize)
        {
            var items = new List<FarmersMarketProductionScheduleItem>();
            try
            {
                using (var connection = await DatabaseHelper.GetOpenConnectionAsync())
                {
                    var command = new MySqlCommand(@"
                SELECT i.*, f.ProductName
                FROM farmers_market_production_schedule_item i
                JOIN FinishedGoods f ON i.product_id = f.FinishedGoodID
                WHERE i.schedule_id = @schedule_id
                ORDER BY i.roast_date DESC
                LIMIT @offset, @pageSize", (MySqlConnection)connection);

                    command.Parameters.AddWithValue("@schedule_id", scheduleId);
                    command.Parameters.AddWithValue("@offset", (page - 1) * pageSize);
                    command.Parameters.AddWithValue("@pageSize", pageSize);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            items.Add(new FarmersMarketProductionScheduleItem
                            {
                                Id = reader.GetInt32("id"),
                                ScheduleId = reader.GetInt32("schedule_id"),
                                ProductId = reader.GetInt32("product_id"),
                                ProductName = reader.GetString("ProductName"),
                                UsageType = reader.GetString("usage_type"),
                                PlannedQuantity = reader.GetDecimal("planned_quantity"),
                                ActualProducedQuantity = reader.IsDBNull("actual_produced_quantity") ? (decimal?)null : reader.GetDecimal("actual_produced_quantity"),
                                RoastDate = reader.IsDBNull("roast_date") ? (DateTime?)null : reader.GetDateTime("roast_date"),
                                Notes = reader.IsDBNull("notes") ? null : reader.GetString("notes")
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ApplicationLogger.Log(ex, nameof(GetItemsPagedByScheduleIdAsync), nameof(FarmersMarketProductionScheduleItemRepository), Environment.UserName);
            }
            return items;
        }


        public async Task<List<FarmersMarketProductionScheduleItem>> GetItemsByScheduleIdAsync(int scheduleId)
        {
            var items = new List<FarmersMarketProductionScheduleItem>();
            try
            {
                using (var connection = await DatabaseHelper.GetOpenConnectionAsync())
                {
                    var command = new MySqlCommand(@"
                        SELECT i.*, f.ProductName
                        FROM farmers_market_production_schedule_item i
                        JOIN FinishedGoods f ON i.product_id = f.FinishedGoodID
                        WHERE i.schedule_id = @schedule_id", (MySqlConnection)connection);

                    command.Parameters.AddWithValue("@schedule_id", scheduleId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            items.Add(new FarmersMarketProductionScheduleItem
                            {
                                Id = reader.GetInt32("id"),
                                ScheduleId = reader.GetInt32("schedule_id"),
                                ProductId = reader.GetInt32("product_id"),
                                ProductName = reader.GetString("ProductName"),
                                UsageType = reader.GetString("usage_type"),
                                PlannedQuantity = reader.GetDecimal("planned_quantity"),
                                ActualProducedQuantity = reader.IsDBNull("actual_produced_quantity") ? (decimal?)null : reader.GetDecimal("actual_produced_quantity"),
                                RoastDate = reader.IsDBNull("roast_date") ? (DateTime?)null : reader.GetDateTime("roast_date"),
                                Notes = reader.IsDBNull("notes") ? null : reader.GetString("notes")
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ApplicationLogger.Log(ex, nameof(GetItemsByScheduleIdAsync), nameof(FarmersMarketProductionScheduleItemRepository), Environment.UserName);
            }
            return items;
        }

        public async Task<FarmersMarketProductionScheduleItem> GetItemByIdAsync(int id)
        {
            FarmersMarketProductionScheduleItem item = null;
            try
            {
                using (var connection = await DatabaseHelper.GetOpenConnectionAsync())
                {
                    var command = new MySqlCommand(@"
                        SELECT i.*, f.ProductName
                        FROM farmers_market_production_schedule_item i
                        JOIN FinishedGoods f ON i.product_id = f.FinishedGoodID
                        WHERE i.id = @id", (MySqlConnection)connection);

                    command.Parameters.AddWithValue("@id", id);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            item = new FarmersMarketProductionScheduleItem
                            {
                                Id = reader.GetInt32("id"),
                                ScheduleId = reader.GetInt32("schedule_id"),
                                ProductId = reader.GetInt32("product_id"),
                                ProductName = reader.GetString("ProductName"),
                                UsageType = reader.GetString("usage_type"),
                                PlannedQuantity = reader.GetDecimal("planned_quantity"),
                                ActualProducedQuantity = reader.IsDBNull("actual_produced_quantity") ? (decimal?)null : reader.GetDecimal("actual_produced_quantity"),
                                RoastDate = reader.IsDBNull("roast_date") ? (DateTime?)null : reader.GetDateTime("roast_date"),
                                Notes = reader.IsDBNull("notes") ? null : reader.GetString("notes")
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ApplicationLogger.Log(ex, nameof(GetItemByIdAsync), nameof(FarmersMarketProductionScheduleItemRepository), Environment.UserName);
            }
            return item;
        }

        public async Task<int> AddItemAsync(FarmersMarketProductionScheduleItem item)
        {
            try
            {
                using (var connection = await DatabaseHelper.GetOpenConnectionAsync())
                {
                    var command = new MySqlCommand(@"
                        INSERT INTO farmers_market_production_schedule_item
                        (schedule_id, product_id, usage_type, planned_quantity, actual_produced_quantity, roast_date, notes)
                        VALUES
                        (@schedule_id, @product_id, @usage_type, @planned_quantity, @actual_produced_quantity, @roast_date, @notes);
                        SELECT LAST_INSERT_ID();", (MySqlConnection)connection);

                    command.Parameters.AddWithValue("@schedule_id", item.ScheduleId);
                    command.Parameters.AddWithValue("@product_id", item.ProductId);
                    command.Parameters.AddWithValue("@usage_type", item.UsageType);
                    command.Parameters.AddWithValue("@planned_quantity", item.PlannedQuantity);
                    command.Parameters.AddWithValue("@actual_produced_quantity", item.ActualProducedQuantity ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@roast_date", item.RoastDate ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@notes", item.Notes ?? (object)DBNull.Value);

                    var insertedId = Convert.ToInt32(await command.ExecuteScalarAsync());
                    return insertedId;
                }
            }
            catch (Exception ex)
            {
                ApplicationLogger.Log(ex, nameof(AddItemAsync), nameof(FarmersMarketProductionScheduleItemRepository), Environment.UserName);
                return -1;
            }
        }

        public async Task<bool> UpdateItemAsync(FarmersMarketProductionScheduleItem item)
        {
            try
            {
                using (var connection = await DatabaseHelper.GetOpenConnectionAsync())
                {
                    var command = new MySqlCommand(@"
                        UPDATE farmers_market_production_schedule_item
                        SET
                            product_id = @product_id,
                            usage_type = @usage_type,
                            planned_quantity = @planned_quantity,
                            actual_produced_quantity = @actual_produced_quantity,
                            roast_date = @roast_date,
                            notes = @notes
                        WHERE id = @id", (MySqlConnection)connection);

                    command.Parameters.AddWithValue("@id", item.Id);
                    command.Parameters.AddWithValue("@product_id", item.ProductId);
                    command.Parameters.AddWithValue("@usage_type", item.UsageType);
                    command.Parameters.AddWithValue("@planned_quantity", item.PlannedQuantity);
                    command.Parameters.AddWithValue("@actual_produced_quantity", item.ActualProducedQuantity ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@roast_date", item.RoastDate ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@notes", item.Notes ?? (object)DBNull.Value);

                    var rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                ApplicationLogger.Log(ex, nameof(UpdateItemAsync), nameof(FarmersMarketProductionScheduleItemRepository), Environment.UserName);
                return false;
            }
        }

        public async Task<bool> DeleteItemAsync(int id)
        {
            try
            {
                using (var connection = await DatabaseHelper.GetOpenConnectionAsync())
                {
                    var command = new MySqlCommand(@"
                        DELETE FROM farmers_market_production_schedule_item
                        WHERE id = @id", (MySqlConnection)connection);

                    command.Parameters.AddWithValue("@id", id);

                    var rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                ApplicationLogger.Log(ex, nameof(DeleteItemAsync), nameof(FarmersMarketProductionScheduleItemRepository), Environment.UserName);
                return false;
            }
        }
    }
}
