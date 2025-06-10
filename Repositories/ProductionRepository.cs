using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using CbcRoastersErp.Models;
using CbcRoastersErp.Helpers;

namespace CbcRoastersErp.Repositories
{
    public class ProductionRepository
    {
        private readonly IDbConnection _db;

        public ProductionRepository()
        {
            _db = DatabaseHelper.GetConnection();
        }

        public IEnumerable<BatchRoasting> GetRoastBatchesPaged(int page, int pageSize)
        {
            try
            {
                string sql = @"
                            SELECT 
                                b.BatchID, b.RoastDate, b.ProductionDate, b.BatchNumber, b.BatchSize,
                                b.ProfileID, b.FinishedGoodID, b.RoastLevel,
                                rp.ProfileName,
                                fg.ProductName AS FinishedGoodName
                            FROM BatchRoasting b
                            LEFT JOIN RoastingProfiles rp ON b.ProfileID = rp.ProfileID
                            LEFT JOIN FinishedGoods fg ON b.FinishedGoodID = fg.FinishedGoodID
                            ORDER BY b.RoastDate DESC
                            LIMIT @Limit OFFSET @Offset";

                return _db.Query(sql, new { Limit = pageSize, Offset = (page - 1) * pageSize }).Select(row => new BatchRoasting
                {
                    BatchID = row.BatchID,
                    RoastDate = row.RoastDate is MySqlConnector.MySqlDateTime dt && dt.IsValidDateTime ? dt.GetDateTime() : null,
                    ProductionDate = row.ProductionDate is MySqlConnector.MySqlDateTime pd && pd.IsValidDateTime ? pd.GetDateTime() : null,
                    BatchNumber = row.BatchNumber,
                    BatchSize = row.BatchSize,
                    ProfileID = row.ProfileID,
                    FinishedGoodID = row.FinishedGoodID,
                    RoastLevel = row.RoastLevel,
                    ProfileName = row.ProfileName,
                    FinishedGoodName = row.FinishedGoodName
                });
            }
            catch (Exception ex)
            {
                ApplicationLogger.Log(ex, "System", "Error");
                return Enumerable.Empty<BatchRoasting>();
            }
        }

        public int GetRoastBatchCount()
        {
            try
            {
                const string sql = "SELECT COUNT(*) FROM BatchRoasting";
                return _db.ExecuteScalar<int>(sql);
            }
            catch (Exception ex)
            {
                ApplicationLogger.Log(ex, "System", "Error");
                return 0;
            }
        }

        public IEnumerable<BatchRoasting> GetAllRoastBatches()
        {
            const string sql = @"
        SELECT 
            b.BatchID, b.RoastDate, b.ProductionDate, b.BatchNumber, b.BatchSize,
            b.ProfileID, b.FinishedGoodID,
            b.RoastLevel,
            rp.ProfileName,
            fg.ProductName AS FinishedGoodName
        FROM BatchRoasting b
        LEFT JOIN RoastingProfiles rp ON b.ProfileID = rp.ProfileID
        LEFT JOIN FinishedGoods fg ON b.FinishedGoodID = fg.FinishedGoodID";

            return _db.Query(sql).Select(row => new BatchRoasting
            {
                BatchID = row.BatchID,
                RoastDate = row.RoastDate is MySqlConnector.MySqlDateTime dt && dt.IsValidDateTime ? dt.GetDateTime() : null,
                ProductionDate = row.ProductionDate is MySqlConnector.MySqlDateTime pd && pd.IsValidDateTime ? pd.GetDateTime() : null,
                BatchNumber = row.BatchNumber,
                BatchSize = row.BatchSize,
                ProfileID = row.ProfileID,
                FinishedGoodID = row.FinishedGoodID,
                ProfileName = row.ProfileName,
                FinishedGoodName = row.FinishedGoodName
            });
        }

        public BatchRoasting? GetRoastBatchById(int batchId)
        {
            const string sql = @"
        SELECT 
            b.BatchID, b.RoastDate, b.ProductionDate, b.BatchNumber, b.BatchSize,
            b.ProfileID, b.FinishedGoodID, b.RoastLevel,
            rp.ProfileName,
            fg.ProductName AS FinishedGoodName
        FROM BatchRoasting b
        LEFT JOIN RoastingProfiles rp ON b.ProfileID = rp.ProfileID
        LEFT JOIN FinishedGoods fg ON b.FinishedGoodID = fg.FinishedGoodID
        WHERE b.BatchID = @BatchID";

            var row = _db.QuerySingleOrDefault(sql, new { BatchID = batchId });

            return row == null ? null : new BatchRoasting
            {
                BatchID = row.BatchID,
                RoastDate = row.RoastDate is MySqlConnector.MySqlDateTime dt && dt.IsValidDateTime ? dt.GetDateTime() : null,
                ProductionDate = row.ProductionDate is MySqlConnector.MySqlDateTime pd && pd.IsValidDateTime ? pd.GetDateTime() : null,
                BatchNumber = row.BatchNumber,
                BatchSize = row.BatchSize,
                ProfileID = row.ProfileID,
                FinishedGoodID = row.FinishedGoodID,
                RoastLevel = row.RoastLevel,
                ProfileName = row.ProfileName,
                FinishedGoodName = row.FinishedGoodName
            };
        }

        public void AddRoastBatch(BatchRoasting batch)
        {
            string query = @"INSERT INTO BatchRoasting (RoastDate, ProductionDate, BatchNumber, BatchSize, ProfileID, FinishedGoodID) 
                 VALUES (@RoastDate, @ProductionDate, @BatchNumber, @BatchSize, @ProfileID, @FinishedGoodID);
                 SELECT LAST_INSERT_ID();"; // fetch last inserted BatchID

            batch.RoastDate ??= DateTime.Now;
            batch.ProductionDate ??= DateTime.Now;
            batch.BatchNumber ??= $"B-{DateTime.Now:yyyyMMddHHmmss}";

            // Capture the new BatchID
            batch.BatchID = _db.ExecuteScalar<int>(query, new
            {
                RoastDate = batch.RoastDate?.ToString("yyyy-MM-dd HH:mm:ss"),
                ProductionDate = batch.ProductionDate?.ToString("yyyy-MM-dd HH:mm:ss"),
                batch.BatchNumber,
                batch.BatchSize,
                batch.ProfileID,
                batch.FinishedGoodID
            });

            // Update FinishedGoods stock
            string updateStock = "UPDATE FinishedGoods SET StockLevel = StockLevel + @Qty WHERE FinishedGoodID = @ID";
            _db.Execute(updateStock, new { Qty = batch.BatchSize, ID = batch.FinishedGoodID });

            // Log transaction
            string logQuery = @"INSERT INTO WarehouseTransactions (TransactionType, ProductID, Quantity, TransactionDate)
                        VALUES ('IN', @ProductID, @Qty, @TransactionDate)";
            _db.Execute(logQuery, new
            {
                ProductID = batch.FinishedGoodID,
                Qty = batch.BatchSize,
                TransactionDate = batch.ProductionDate
            });

            // 🔗 Insert into FinishedGoodInventory
            AddFinishedGoodInventory(batch);
        }


        public void UpdateRoastBatch(BatchRoasting batch)
        {
            string query = "UPDATE BatchRoasting SET RoastDate = @RoastDate, BatchSize = @BatchSize, " +
                           "ProfileID = @ProfileID, FinishedGoodID = @FinishedGoodID WHERE BatchID = @BatchID";
            _db.Execute(query, batch);
        }

        public void DeleteRoastBatch(int batchID)
        {
            _db.Execute("DELETE FROM BatchRoasting WHERE BatchID = @BatchID", new { BatchID = batchID });
        }

        public IEnumerable<FinishedGoods> GetFinishedGoods()
        {
            return _db.Query<FinishedGoods>("SELECT * FROM FinishedGoods");
        }

        public void AddFinishedGoodInventory(BatchRoasting batch)
        {
            //  using var connection = DatabaseHelper.GetConnection();
            const string sql = @"
        INSERT INTO FinishedGoodInventory (FinishedGoodID, QuantityProduced, BatchID, DateProduced)
        VALUES (@FinishedGoodID, @QuantityProduced, @BatchID, @DateProduced);";

            _db.Execute(sql, new
            {
                batch.FinishedGoodID,
                QuantityProduced = batch.BatchSize, // or adjust if quantity is calculated differently
                batch.BatchID,
                DateProduced = batch.ProductionDate ?? DateTime.Now // Ensure a date is set
            });
        }

        public void LogFinishedGoodOutboundTransaction(int finishedGoodId, int quantity)
        {
            try
            {
                string updateStock = "UPDATE FinishedGoods SET StockLevel = StockLevel - @Qty WHERE FinishedGoodID = @ID";
                _db.Execute(updateStock, new { Qty = quantity, ID = finishedGoodId });

                string logQuery = @"INSERT INTO WarehouseTransactions 
                            (TransactionType, ProductID, Quantity, TransactionDate) 
                            VALUES ('OUT', @ProductID, @Qty, @DateProduced)";

                _db.Execute(logQuery, new
                {
                    ProductID = finishedGoodId,
                    Qty = quantity,
                    DateProduced = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                ApplicationLogger.Log(ex, nameof(ProductionRepository), nameof(LogFinishedGoodOutboundTransaction), Environment.UserName);
            }
        }

        // Finished Goods CRUD Operations
        public int GetTotalFinishedGoodsCount()
        {
            try
            {
                const string sql = "SELECT COUNT(*) FROM FinishedGoods";
                return _db.ExecuteScalar<int>(sql);
            }
            catch (Exception ex)
            {
                ApplicationLogger.Log(ex, "System", "Error");
                return 0;
            }
        }

        public IEnumerable<FinishedGoods> GetAllFinishedGoods()
        {
            try
            {
                string sql = @"
            SELECT 
                fg.FinishedGoodID, fg.BigCommProdID, fg.ProductDescription, fg.ProductName,
                fg.BatchID, fg.StockLevel, fg.ProfileID, fg.SKU,
                rp.ProfileName
            FROM FinishedGoods fg
            LEFT JOIN RoastingProfiles rp ON fg.ProfileID = rp.ProfileID
            ORDER BY fg.FinishedGoodID DESC";
                return _db.Query<FinishedGoods>(sql);
            }
            catch (Exception ex)
            {
                ApplicationLogger.Log(ex, "System", "Error");
                return Enumerable.Empty<FinishedGoods>();
            }
        }

        public FinishedGoods? GetFinishedGoodById(int id)
        {
            try
            {
                string sql = @"
            SELECT 
                fg.FinishedGoodID, fg.BigCommProdID, fg.ProductDescription, fg.ProductName,
                fg.BatchID, fg.StockLevel, fg.ProfileID, fg.SKU,
                rp.ProfileName
            FROM FinishedGoods fg
            LEFT JOIN RoastingProfiles rp ON fg.ProfileID = rp.ProfileID
            WHERE fg.FinishedGoodID = @FinishedGoodID";
                return _db.QuerySingleOrDefault<FinishedGoods>(sql, new { FinishedGoodID = id });
            }
            catch (Exception ex)
            {
                ApplicationLogger.Log(ex, "System", "Error");
                return null;
            }
        }
        public void AddFinishedGood(FinishedGoods finishedGood)
        {
            try
            {
                string sql = @"
                INSERT INTO FinishedGoods (BigCommProdID, ProductDescription, ProductName, BatchID, StockLevel, ProfileID, SKU)
                VALUES (@BigCommProdID, @ProductDescription, @ProductName, @BatchID, @StockLevel, @ProfileID, @SKU);
                SELECT LAST_INSERT_ID();";
                finishedGood.FinishedGoodID = _db.ExecuteScalar<int>(sql, finishedGood);
            }
            catch (Exception ex)
            {
                ApplicationLogger.Log(ex, "System", "Error");
            }
        }
        public void UpdateFinishedGood(FinishedGoods finishedGood)
        {
            try
            {
                string sql = @"
                UPDATE FinishedGoods 
                SET BigCommProdID = @BigCommProdID, ProductDescription = @ProductDescription, 
                    ProductName = @ProductName, BatchID = @BatchID, StockLevel = @StockLevel, 
                    ProfileID = @ProfileID, SKU = @SKU
                WHERE FinishedGoodID = @FinishedGoodID";
                _db.Execute(sql, finishedGood);
            }
            catch (Exception ex)
            {
                ApplicationLogger.Log(ex, "System", "Error");
            }
        }

        public IEnumerable<FinishedGoods> GetFinishedGoodsPaged(int page, int pageSize)
        {
            try
            {
                string sql = @"
            SELECT 
                fg.FinishedGoodID, fg.BigCommProdID, fg.ProductDescription, fg.ProductName,
                fg.BatchID, fg.StockLevel, fg.ProfileID, fg.SKU,
                rp.ProfileName, rp.RoastLevel
            FROM FinishedGoods fg
            LEFT JOIN RoastingProfiles rp ON fg.ProfileID = rp.ProfileID
            ORDER BY fg.FinishedGoodID DESC
            LIMIT @Limit OFFSET @Offset";

                return _db.Query<FinishedGoods>(sql, new { Limit = pageSize, Offset = (page - 1) * pageSize });
            }
            catch (Exception ex)
            {
                ApplicationLogger.Log(ex, "System", "Error");
                return Enumerable.Empty<FinishedGoods>();
            }
        }



        /// <summary>
        /// Deletes a finished good by its ID.
        /// </summary>
        /// <param name="id"></param>
        public void DeleteFinishedGoods(int id)
        {
            try
            {
                _db.Execute("DELETE FROM FinishedGoods WHERE FinishedGoodID = @FinishedGoodID", new { FinishedGoodID = id });
            }
            catch (System.Exception ex)
            {
                ApplicationLogger.Log(ex, nameof(DeleteFinishedGoods), nameof(InventoryRepository), Environment.UserName);
                throw; // rethrow to handle it in the calling code if needed
            }
            finally
            {
                // logging the delete operation
                ApplicationLogger.LogInfo($"Deleted Finished Good: {id}", Environment.UserName, "Info", nameof(DeleteFinishedGoods), nameof(InventoryRepository));
            }
        }

        //Roasting Profiles CRUD Operations
        public IEnumerable<RoastingProfiles> GetRoastingProfilesPaged(int page, int pageSize)
        {
            try
            {
                string sql = @"
                SELECT rp.*, 
                       gc.GreenCoffeeID, gc.CoffeeName AS GreenCoffeeName
                       FROM RoastingProfiles rp
                LEFT JOIN GreenCoffeeInventory gc ON rp.GreenCoffeeID = gc.GreenCoffeeID    
                ORDER BY ProfileID DESC
                LIMIT @Limit OFFSET @Offset";
                return _db.Query<RoastingProfiles>(sql, new { Limit = pageSize, Offset = (page - 1) * pageSize });
            }
            catch (Exception ex)
            {
                ApplicationLogger.Log(ex, "System", "Error");
                return Enumerable.Empty<RoastingProfiles>();
            }
        }


        public IEnumerable<RoastingProfiles> GetRoastingProfiles()
        {
            // return _db.Query<RoastingProfiles>("SELECT * FROM RoastingProfiles");

            const string sql = "SELECT * FROM RoastingProfiles";

            return _db.Query(sql).Select(row => new RoastingProfiles
            {
                ProfileID = row.ProfileID,
                ProfileName = row.ProfileName,
                Description = row.Description,
                RoastLevel = row.RoastLevel,
                GreenCoffeeID = row.GreenCoffeeID,
                RoastTime = row.RoastTime,
                Temperature = row.Temperature,
                Notes = row.Notes
            });
        }

        public RoastingProfiles? GetRoastingProfileById(int profileId)
        {
            try
            {
                string sql = "SELECT * FROM RoastingProfiles WHERE ProfileID = @ProfileID";
                return _db.QuerySingleOrDefault<RoastingProfiles>(sql, new { ProfileID = profileId });
            }
            catch (Exception ex)
            {
                ApplicationLogger.Log(ex, "System", "Error");
                return null;
            }
        }

        public int GetTotalRoastingProfilesCount()
        {
            try
            {
                const string sql = "SELECT COUNT(*) FROM RoastingProfiles";
                return _db.ExecuteScalar<int>(sql);
            }
            catch (Exception ex)
            {
                ApplicationLogger.Log(ex, "System", "Error");
                return 0;
            }
        }

        public void AddRoastingProfile(RoastingProfiles profile)
        {
            try
            {
                string sql = @"
                INSERT INTO RoastingProfiles (ProfileName, Description, RoastLevel, GreenCoffeeID, RoastTime, Temperature, Notes)
                VALUES (@ProfileName, @Description, @RoastLevel, @GreenCoffeeID, @RoastTime, @Temperature, @Notes);
                SELECT LAST_INSERT_ID();";
                profile.ProfileID = _db.ExecuteScalar<int>(sql, profile);
            }
            catch (Exception ex)
            {
                ApplicationLogger.Log(ex, "System", "Error");
            }
        }
        public void UpdateRoastingProfile(RoastingProfiles profile)
        {
            try
            {
                string sql = @"
                UPDATE RoastingProfiles 
                SET ProfileName = @ProfileName, RoastLevel = @RoastLevel, 
                    GreenCoffeeID = @GreenCoffeeID, RoastTime = @RoastTime, 
                    Temperature = @Temperature, Notes = @Notes,
                    Description = @Description
                WHERE ProfileID = @ProfileID";
                _db.Execute(sql, profile);
            }
            catch (Exception ex)
            {
                ApplicationLogger.Log(ex, "System", "Error");
            }
        }

        public async Task DeleteRoastingProfileAsync(int profileId)
        {
            try
            {
                await _db.ExecuteAsync("DELETE FROM RoastingProfiles WHERE ProfileID = @ProfileID", new { ProfileID = profileId });
            }
            catch (Exception ex)
            {
                ApplicationLogger.Log(ex, "System", "Error");
            }
        }


        // Green Coffee CRUD Operations
        public IEnumerable<GreenCoffeeInventory> GetGreenCoffees()
        {
            try
            {
                const string sql = @"
                SELECT 
                GreenCoffeeID,
                CoffeeName
                FROM GreenCoffeeInventory";
                return _db.Query<GreenCoffeeInventory>(sql);
            }
            catch (Exception ex)
            {
                ApplicationLogger.Log(ex, "System", "Error");
                return Enumerable.Empty<GreenCoffeeInventory>();
            }
        }
    }

 }

