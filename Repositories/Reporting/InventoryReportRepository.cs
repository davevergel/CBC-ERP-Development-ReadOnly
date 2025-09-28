using CbcRoastersErp.Helpers;
using CbcRoastersErp.Reports;
using Dapper;

namespace CbcRoastersErp.Repositories
{
    public interface IInventoryReportRepository
    {
        Task<IEnumerable<FinishedGoodReportItem>> GetFinishedGoodsReportAsync();
        Task<IEnumerable<GreenCoffeeReportItem>> GetGreenCoffeeReportAsync();
        Task<IEnumerable<TeaReportItem>> GetTeaReportAsync();
        Task<IEnumerable<PackingMaterialReportItem>> GetPackingMaterialsReportAsync();
    }

    public class InventoryReportRepository : IInventoryReportRepository
    {
        public async Task<IEnumerable<FinishedGoodReportItem>> GetFinishedGoodsReportAsync()
        {
            using var connection = DatabaseHelper.GetConnection();
            var sql = @"SELECT BatchNumber, ProductName, RoastLevel, QuantityProduced, DateProduced
                        FROM FinishedGoodInventory"; // Adjust view/table as needed
            return (await connection.QueryAsync<FinishedGoodReportItem>(sql)).ToList();
        }

        public async Task<IEnumerable<GreenCoffeeReportItem>> GetGreenCoffeeReportAsync()
        {
            using var connection = DatabaseHelper.GetConnection();
            var sql = @"SELECT gc.CoffeeName, gc.Origin, gc.Variety, gc.ProcessType, gc.Quantity, gc.StockLevel, sp.Supplier_Name AS SupplierName, 
                         gc.Price
                        FROM GreenCoffeeInventory as gc
                        LEFT JOIN Suppliers sp on gc.supplierId = sp.Supplier_id";

            return (await connection.QueryAsync<GreenCoffeeReportItem>(sql)).ToList();
        }

        public async Task<IEnumerable<TeaReportItem>> GetTeaReportAsync()
        {
            using var connection = DatabaseHelper.GetConnection();
            var sql = @"SELECT TeaName, TeaType, Origin, Certifications, StockLevel, DateReceived, Price
                        FROM TeaInventory";
            return (await connection.QueryAsync<TeaReportItem>(sql)).ToList();
        }
        
        public async Task<IEnumerable<PackingMaterialReportItem>> GetPackingMaterialsReportAsync()
        {
            using var connection = DatabaseHelper.GetConnection();
            var sql = @"SELECT MaterialName, StockLevel, Unit, LastUpdated
                        FROM packing_materials";
            return (await connection.QueryAsync<PackingMaterialReportItem>(sql)).ToList();
        }
    }
}

