using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using CbcRoastersErp.Models;
using CbcRoastersErp.Helpers;
using CbcRoastersErp.Services;
using CbcRoastersErp.Models.Operations.Inventory;

namespace CbcRoastersErp.Repositories
{
    public class InventoryRepository
    {
        private readonly IDbConnection _db;

        public InventoryRepository()
        {
            _db = DatabaseHelper.GetConnection();
        }



        // Green Coffee Inventory CRUD Operations
        public IEnumerable<GreenCoffeeInventory> GetGreenCoffeeInventory()
        {
            try
            {
                const string sql = @"SELECT gc.* 
                                    , sp.Supplier_Name AS SupplierName, sp.Supplier_id AS SupplierId   
                                 FROM GreenCoffeeInventory as gc
                                 LEFT JOIN Suppliers sp on gc.supplierId = sp.Supplier_id";

                var result = _db.Query(sql).Select(row => new GreenCoffeeInventory
                {
                    GreenCoffeeID = row.GreenCoffeeID,
                    CoffeeName = row.CoffeeName,
                    Origin = row.Origin,
                    Variety = row.Variety,
                    ProcessType = row.ProcessType,
                    Certifications = row.Certifications,
                    Quantity = row.Quantity,
                    HarvestYear = row.HarvestYear,
                    BatchNumber = row.BatchNumber,
                    StockLevel = row.StockLevel,
                    SupplierId = row.SupplierId,
                    SupplierName = row.SupplierName, // For display purposes
                    DateReceived = row.DateReceived is MySqlConnector.MySqlDateTime dt && dt.IsValidDateTime ? dt.GetDateTime() : null,
                    Price = row.Price
                });

                return result.ToList();

            }
            catch (System.Exception ex)
            {
                ApplicationLogger.Log(ex, nameof(GetGreenCoffeeInventory), nameof(InventoryRepository), Environment.UserName);
                return new List<GreenCoffeeInventory>(); // fallback: return empty list
            }
        }




        public void AddGreenCoffee(GreenCoffeeInventory item)
        {
            try
            {
                string query = @"
                    INSERT INTO GreenCoffeeInventory (
                        CoffeeName, Origin, Variety, ProcessType, Certifications, Quantity, HarvestYear,
                        BatchNumber, StockLevel, SupplierId, DateReceived, Price
                    ) VALUES (
                        @CoffeeName, @Origin, @Variety, @ProcessType, @Certifications, @Quantity, @HarvestYear,
                        @BatchNumber, @StockLevel, @SupplierId, @DateReceived, @Price
                    )";

                _db.Execute(query, new
                {
                    item.CoffeeName,
                    item.Origin,
                    item.Variety,
                    item.ProcessType,
                    item.Certifications,
                    item.Quantity,
                    HarvestYear = item.HarvestYear ?? (object)DBNull.Value, // Handle nulls
                    item.BatchNumber,
                    item.StockLevel,
                    item.SupplierId,
                    item.DateReceived,
                    item.Price
                });

            }
            catch (System.Exception ex)
            {
                ApplicationLogger.Log(ex, nameof(AddGreenCoffee), nameof(InventoryRepository), Environment.UserName);
                throw; // rethrow to handle it in the calling code if needed
            }
        }



        public void UpdateGreenCoffee(GreenCoffeeInventory item)
        {
            try
            {
                string query = @"
                    UPDATE GreenCoffeeInventory SET
                        CoffeeName = @CoffeeName,
                        Origin = @Origin,
                        Variety = @Variety,
                        ProcessType = @ProcessType,
                        Certifications = @Certifications,
                        Quantity = @Quantity,
                        HarvestYear = @HarvestYear,
                        BatchNumber = @BatchNumber,
                        StockLevel = @StockLevel,
                        SupplierId = @SupplierId,
                        DateReceived = @DateReceived,
                        Price = @Price
                    WHERE GreenCoffeeID = @GreenCoffeeID";

                _db.Execute(query, item);

                //logging the update operation
                ApplicationLogger.LogInfo($"Updated Green Coffee Inventory: {item.GreenCoffeeID} - {item.CoffeeName}", Environment.UserName, "Info", nameof(UpdateGreenCoffee), nameof(InventoryRepository));
            }
            catch (System.Exception ex)
            {
                ApplicationLogger.Log(ex, nameof(UpdateGreenCoffee), nameof(InventoryRepository), Environment.UserName);
                throw; // rethrow to handle it in the calling code if needed
            }
        }
            
        public void DeleteGreenCoffee(int id)
        {
            try
            {
                _db.Execute("DELETE FROM GreenCoffeeInventory WHERE GreenCoffeeID = @GreenCoffeeID", new { GreenCoffeeID = id });

                // logging the delete operation
                ApplicationLogger.LogInfo($"Deleted Green Coffee Inventory: {id}", Environment.UserName, "Info", nameof(DeleteGreenCoffee), nameof(InventoryRepository));
            }
            catch (System.Exception ex)
            {
                ApplicationLogger.Log(ex, nameof(DeleteGreenCoffee), nameof(InventoryRepository), Environment.UserName);
                throw; // rethrow to handle it in the calling code if needed
            }
        }

        // Tea Inventory CRUD Operations
        public IEnumerable<TeaInventory> GetTeaInventory()
        {
            try
            {
                const string sql = "SELECT * FROM TeaInventory";

                return _db.Query(sql).Select(row => new TeaInventory
                {
                    TeaID = row.TeaID,
                    TeaName = row.TeaName,
                    TeaType = row.TeaType,
                    Origin = row.Origin,
                    Certifications = row.Certifications,
                    Quantity = row.Quantity,
                    BatchNumber = row.BatchNumber,
                    StockLevel = row.StockLevel,
                    SupplierId = row.SupplierId,
                    DateReceived = row.DateReceived is MySqlConnector.MySqlDateTime dt && dt.IsValidDateTime ? dt.GetDateTime() : null,
                    HarvestYear = row.HarvestYear,
                    Price = row.Price
                }).ToList();
            }
            catch (System.Exception ex)
            {
                ApplicationLogger.Log(ex, nameof(GetTeaInventory), nameof(InventoryRepository), Environment.UserName);
                return new List<TeaInventory>(); // fallback: return empty list
            }
        }

        public void AddTea(TeaInventory item)
        {
            try
            {
                string query = "INSERT INTO TeaInventory (TeaName, TeaType, Origin, Certifications, StockLevel, BatchNumber, SupplierId, DateReceived, HarvestYear, Price) " +
                               "VALUES (@TeaName, @TeaType, @Origin, @Certifications, @StockLevel, @BatchNumber, @SupplierId, @DateReceived, @HarvestYear, @Price)";
                _db.Execute(query, item);

                // logging the add operation
                ApplicationLogger.LogInfo($"Added Tea Inventory: {item.TeaName} - {item.TeaID}", Environment.UserName, "Info", nameof(AddTea), nameof(InventoryRepository));
            }
            catch (System.Exception ex)
            {
                ApplicationLogger.Log(ex, nameof(AddTea), nameof(InventoryRepository), Environment.UserName);
                throw; // rethrow to handle it in the calling code if needed
            }
        }

        public void UpdateTea(TeaInventory item)
        {
            try
            {
                string query = "UPDATE TeaInventory SET TeaName = @TeaName, TeaType = @TeaType, Origin = @Origin, Certifications = @Certifications, " +
                               "StockLevel = @StockLevel, BatchNumber = @BatchNumber, SupplierId = @SupplierId, DateReceived = @DateReceived, " +
                               "HarvestYear = @HarvestYear, Price = @Price WHERE TeaID = @TeaID";
                _db.Execute(query, item);
            }
            catch (System.Exception ex)
            {
                ApplicationLogger.Log(ex, nameof(UpdateTea), nameof(InventoryRepository), Environment.UserName);
                throw; // rethrow to handle it in the calling code if needed
            }
            finally
            {
                // logging the update operation
                ApplicationLogger.LogInfo($"Updated Tea Inventory: {item.TeaID} - {item.TeaName}", Environment.UserName, "Info", nameof(UpdateTea), nameof(InventoryRepository));
            }
        }

        public void DeleteTea(int id)
        {
            try
            {
                _db.Execute("DELETE FROM TeaInventory WHERE TeaID = @TeaID", new { TeaID = id });
            }
            catch (System.Exception ex)
            {
                ApplicationLogger.Log(ex, nameof(DeleteTea), nameof(InventoryRepository), Environment.UserName);
                throw; // rethrow to handle it in the calling code if needed
            }
            finally
            {
                // logging the delete operation
                ApplicationLogger.LogInfo($"Deleted Tea Inventory: {id}", Environment.UserName, "Info", nameof(DeleteTea), nameof(InventoryRepository));
            }
        }

        // Packing Materials CRUD Operations
        public IEnumerable<PackingMaterials> GetPackingMaterials()
        {
            try
            {
                const string sql = "SELECT * FROM PackingMaterials";

                return _db.Query(sql).Select(row => new PackingMaterials
                {
                    MaterialID = row.MaterialID,
                    MaterialName = row.MaterialName,
                    StockLevel = row.StockLevel,
                    Unit = row.Unit,
                    LastUpdated = row.LastUpdated is MySqlConnector.MySqlDateTime dt && dt.IsValidDateTime ? dt.GetDateTime() : null
                }).ToList();
            }
            catch (System.Exception ex)
            {
                ApplicationLogger.Log(ex, nameof(GetPackingMaterials), nameof(InventoryRepository), Environment.UserName);
                return new List<PackingMaterials>(); // fallback: return empty list
            }
        }

        public void AddPackingMaterial(PackingMaterials item)
        {
            try
            {
                string query = "INSERT INTO PackingMaterials (MaterialName, StockLevel, Unit, LastUpdated) " +
                               "VALUES (@MaterialName, @StockLevel, @Unit, @LastUpdated)";

                // Ensure LastUpdated is formatted correctly
                item.LastUpdated ??= DateTime.Now; // Ensure it's a valid DateTime

                _ = _db.Execute(query, new
                {
                    item.MaterialName,
                    item.StockLevel,
                    item.Unit,
                    LastUpdated = item.LastUpdated?.ToString("yyyy-MM-dd HH:mm:ss") // Proper date format
                });
            }
            catch (System.Exception ex)
            {
                ApplicationLogger.Log(ex, nameof(AddPackingMaterial), nameof(InventoryRepository), Environment.UserName);
                throw; // rethrow to handle it in the calling code if needed
            }
            finally
            {
                // logging the add operation
                ApplicationLogger.LogInfo($"Added Packing Material: {item.MaterialName} - {item.MaterialID}", Environment.UserName, "Info", nameof(AddPackingMaterial), nameof(InventoryRepository));
            }
        }

        public void UpdatePackingMaterial(PackingMaterials item)
        {
            try
            {
                string query = "UPDATE PackingMaterials SET MaterialName = @MaterialName, StockLevel = @StockLevel, " +
                               "Unit = @Unit, LastUpdated = NOW() WHERE MaterialID = @MaterialID";
                _db.Execute(query, item);
            }
            catch (System.Exception ex)
            {
                ApplicationLogger.Log(ex, nameof(UpdatePackingMaterial), nameof(InventoryRepository), Environment.UserName);
                throw; // rethrow to handle it in the calling code if needed
            }
            finally
            {
                // logging the update operation
                ApplicationLogger.LogInfo($"Updated Packing Material: {item.MaterialID} - {item.MaterialName}", Environment.UserName, "Info", nameof(UpdatePackingMaterial), nameof(InventoryRepository));
            }
        }

        public void DeletePackingMaterial(int id)
        {
            try
            {
                _db.Execute("DELETE FROM PackingMaterials WHERE MaterialID = @MaterialID", new { MaterialID = id });
            }
            catch (System.Exception ex)
            {
                ApplicationLogger.Log(ex, nameof(DeletePackingMaterial), nameof(InventoryRepository), Environment.UserName);
                throw; // rethrow to handle it in the calling code if needed
            }
            finally
            {
                // logging the delete operation
                ApplicationLogger.LogInfo($"Deleted Packing Material: {id}", Environment.UserName, "Info", nameof(DeletePackingMaterial), nameof(InventoryRepository));
            }
        }

        // Finished Goods CRUD Operations
        public IEnumerable<FinishedGoods> GetFinishedGoods()
        {
            try {
            const string sql = @"
        SELECT 
            fg.FinishedGoodID,
            fg.ProductName,
            fg.BatchID,
            fg.StockLevel,
            fg.ProfileID,
            br.RoastLevel,
            br.BatchNumber,
            br.ProductionDate,
            rp.ProfileName
        FROM FinishedGoods fg
        LEFT JOIN BatchRoasting br ON fg.BatchID = br.BatchID
        LEFT JOIN RoastingProfiles rp ON fg.ProfileID = rp.ProfileID";

           return _db.Query(sql).Select(row => new FinishedGoods
           {
               FinishedGoodID = row.FinishedGoodID,
               ProductName = row.ProductName,
               BatchID = row.BatchID,
               StockLevel = row.StockLevel,
               ProfileID = row.ProfileID,
               BatchNumber = row.BatchNumber,
               ProductionDate = row.ProductionDate is MySqlConnector.MySqlDateTime dt && dt.IsValidDateTime ? dt.GetDateTime() : null,
               ProfileName = row.ProfileName,
               RoastLevel = row.RoastLevel
           }).ToList();
            }
            catch (System.Exception ex)
            {
                ApplicationLogger.Log(ex, nameof(GetFinishedGoods), nameof(InventoryRepository), Environment.UserName);
                return new List<FinishedGoods>(); // fallback: return empty list
            }
        }

        public IEnumerable<FinishedGoodInventory> GetFinishedGoodInventory()
        {
            try
            {
                const string query = @"
            SELECT fgi.*, fg.ProductName, br.BatchNumber, br.ProductionDate
            FROM FinishedGoodInventory fgi
            JOIN FinishedGoods fg ON fgi.FinishedGoodID = fg.FinishedGoodID
            LEFT JOIN BatchRoasting br ON fgi.BatchID = br.BatchID";

                return _db.Query(query).Select(row => new FinishedGoodInventory
                {
                    InventoryID = row.InventoryID,
                    FinishedGoodID = row.FinishedGoodID,
                    ProductName = row.ProductName,
                    QuantityProduced = row.QuantityProduced,
                    BatchNumber = row.BatchNumber,
                    DateProduced = row.DateProduced is MySqlConnector.MySqlDateTime pd && pd.IsValidDateTime ? pd.GetDateTime() : null
                }).ToList();
            }
            catch (System.Exception ex)
            {
                ApplicationLogger.Log(ex, nameof(GetFinishedGoodInventory), nameof(InventoryRepository), Environment.UserName);
                return new List<FinishedGoodInventory>(); // fallback: return empty list
            }
        }


        public void AddFinishedGood(FinishedGoods item)
        {
            try
            {
                string query = "INSERT INTO FinishedGoods (ProductName, BatchNumber, StockLevel, ProductionDate) " +
                               "VALUES (@ProductName, @BatchNumber, @StockLevel, @ProductionDate)";
                _db.Execute(query, item);
            }
            catch (System.Exception ex)
            {
                ApplicationLogger.Log(ex, nameof(AddFinishedGood), nameof(InventoryRepository), Environment.UserName);
                throw; // rethrow to handle it in the calling code if needed
            }
            finally
            {
                // logging the add operation
                ApplicationLogger.LogInfo($"Added Finished Good: {item.ProductName} - {item.FinishedGoodID}", Environment.UserName, "Info", nameof(AddFinishedGood), nameof(InventoryRepository));
            }
        }

        public void UpdateFinishedGood(FinishedGoods item)
        {
            try
            {
                string query = "UPDATE FinishedGoods SET ProductName = @ProductName, BatchNumber = @BatchNumber, " +
                               "StockLevel = @StockLevel, ProductionDate = @ProductionDate WHERE FinishedGoodID = @FinishedGoodID";
                _db.Execute(query, item);
            }
            catch (System.Exception ex)
            {
                ApplicationLogger.Log(ex, nameof(UpdateFinishedGood), nameof(InventoryRepository), Environment.UserName);
                throw; // rethrow to handle it in the calling code if needed
            }
            finally
            {
                // logging the update operation
                ApplicationLogger.LogInfo($"Updated Finished Good: {item.FinishedGoodID} - {item.ProductName}", Environment.UserName, "Info", nameof(UpdateFinishedGood), nameof(InventoryRepository));
            }
        }

        public void DeleteFinishedGood(int id)
        {
            try
            {
                _db.Execute("DELETE FROM FinishedGoods WHERE FinishedGoodID = @FinishedGoodID", new { FinishedGoodID = id });
            }
            catch (System.Exception ex)
            {
                ApplicationLogger.Log(ex, nameof(DeleteFinishedGood), nameof(InventoryRepository), Environment.UserName);
                throw; // rethrow to handle it in the calling code if needed
            }
            finally
            {
                // logging the delete operation
                ApplicationLogger.LogInfo($"Deleted Finished Good: {id}", Environment.UserName, "Info", nameof(DeleteFinishedGood), nameof(InventoryRepository));
            }
        }

        public IEnumerable<BatchRoasting> GetBatchRoasts()
        {
            try
            {
                const string sql = "SELECT * FROM BatchRoasting";

                return _db.Query(sql).Select(row => new BatchRoasting
                {
                    BatchID = row.BatchID,
                    BatchNumber = row.BatchNumber,
                    RoastDate = row.RoastDate is MySqlConnector.MySqlDateTime dt && dt.IsValidDateTime ? dt.GetDateTime() : null,
                    ProductionDate = row.ProductionDate is MySqlConnector.MySqlDateTime dtPD && dtPD.IsValidDateTime ? dtPD.GetDateTime() : null,
                    BatchSize = row.BatchSize,
                    ProfileID = row.ProfileID,
                    RoastLevel = row.RoastLevel,
                    FinishedGoodID = row.FinishedGoodID,
                    ProfileName = row.ProfileName,
                    FinishedGoodName = row.FinishedGoodName
                }).ToList();
            }
            catch (System.Exception ex)
            {
                ApplicationLogger.Log(ex, nameof(GetBatchRoasts), nameof(InventoryRepository), Environment.UserName);
                return new List<BatchRoasting>(); // fallback: return empty list
            }
        }

        // Get Suppliers
        public IEnumerable<Suppliers> GetSuppliers()
        {
            try
            {
                return _db.Query<Suppliers>("SELECT * FROM Suppliers");
            }
            catch (System.Exception ex)
            {
                ApplicationLogger.Log(ex, nameof(GetSuppliers), nameof(InventoryRepository), Environment.UserName);
                return new List<Suppliers>(); // fallback: return empty list
            }
        }

        // KPIs
        public InventoryKpiSummary GetInventorySummary()
        {
            try
            {
                using var connection = DatabaseHelper.GetOpenConnection();

                var query = @"
            SELECT 
                (SELECT IFNULL(SUM(Quantity), 0) FROM GreenCoffeeInventory) AS TotalGreenCoffee,
                (SELECT IFNULL(SUM(Quantity), 0) FROM TeaInventory) AS TotalTea,
                (SELECT IFNULL(SUM(StockLevel), 0) FROM PackingMaterials) AS TotalPackingStock,
                (SELECT IFNULL(SUM(StockLevel), 0) FROM FinishedGoods) AS TotalFinishedGoods;
        ";

                return connection.QueryFirstOrDefault<InventoryKpiSummary>(query);
            }
            catch (Exception ex)
            {
                ApplicationLogger.Log(ex, nameof(GetInventorySummary), nameof(InventoryRepository), Environment.UserName);
                return new InventoryKpiSummary(); // fallback
            }
        }

    }
}
