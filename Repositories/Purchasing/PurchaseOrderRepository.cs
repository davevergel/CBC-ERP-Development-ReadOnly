using Dapper;
using CbcRoastersErp.Helpers;
using CbcRoastersErp.Models.Purchasing;
using System.Collections.ObjectModel;
using CbcRoastersErp.Models.Finance;
using CbcRoastersErp.Repositories.Finance;
using System.Data;

namespace CbcRoastersErp.Repositories
{
    public class PurchaseOrderRepository
    {
        public async Task<List<PurchaseOrder>> GetAllAsync()
        {
            try
            {
                using var connection = DatabaseHelper.GetConnection();
                string query = @"SELECT po.*, s.Supplier_Name AS SupplierName
                                 FROM purchase_orders po
                                 LEFT JOIN Suppliers s ON po.Supplier_id = s.Supplier_id";

                var results = await connection.QueryAsync<PurchaseOrder>(query);
                return results.AsList();
            }
            catch (Exception ex)
            {
                ApplicationLogger.Log(ex, nameof(GetAllAsync), nameof(PurchaseOrderRepository), Environment.UserName);
                return new List<PurchaseOrder>();
            }
        }

        public async Task<PurchaseOrder> GetByIdAsync(int id)
        {
            try
            {
                using var connection = DatabaseHelper.GetConnection();
                string query = @"SELECT * FROM purchase_orders WHERE PurchaseOrderId = @Id;
                                 SELECT * FROM purchase_order_items WHERE PurchaseOrderId = @Id";
                using var multi = await connection.QueryMultipleAsync(query, new { Id = id });
                var order = await multi.ReadFirstOrDefaultAsync<PurchaseOrder>();
                if (order != null)
                    order.Items = new ObservableCollection<PurchaseOrderItem>(await multi.ReadAsync<PurchaseOrderItem>());
                return order;
            }
            catch (Exception ex)
            {
                ApplicationLogger.Log(ex, nameof(GetByIdAsync), nameof(PurchaseOrderRepository), Environment.UserName);
                return null;
            }
        }

        public async Task<int> AddOrUpdateAsync(PurchaseOrder order)
        {
            try
            {
                using var connection = DatabaseHelper.GetOpenConnection();
                using var transaction = connection.BeginTransaction();

                try
                {
                    // Recalculate totals
                    order.Subtotal = order.Items.Sum(i => i.Quantity * i.UnitPrice);
                    order.TotalAmount = order.Subtotal + order.TaxAmount + order.ShippingCost;

                    if (order.PurchaseOrderId == 0)
                    {
                        // Generate PO Number if not already provided
                        if (string.IsNullOrWhiteSpace(order.po_number))
                        {
                            string prefix = $"PO-{DateTime.Now:yyyyMM}";
                            string countQuery = "SELECT COUNT(*) FROM purchase_orders WHERE po_number LIKE @PrefixPattern";
                            int count = await connection.ExecuteScalarAsync<int>(
                                countQuery, new { PrefixPattern = $"{prefix}-%" }, transaction);

                            order.po_number = $"{prefix}-{(count + 1):D4}";
                        }

                        string insert = @"INSERT INTO purchase_orders 
                    (Supplier_id, OrderDate, Status, TotalAmount, Subtotal, TaxAmount, ShippingCost, Created_at, po_number, InvoicePdfPath)
                    VALUES 
                    (@Supplier_id, @OrderDate, @Status, @TotalAmount, @Subtotal, @TaxAmount, @ShippingCost, NOW(), @po_number, @InvoicePdfPath);
                    SELECT LAST_INSERT_ID();";

                        order.PurchaseOrderId = await connection.ExecuteScalarAsync<int>(insert, order, transaction);
                    }
                    else
                    {
                        string update = @"UPDATE purchase_orders 
                    SET Supplier_id = @Supplier_id, 
                        OrderDate = @OrderDate, 
                        Status = @Status, 
                        TotalAmount = @TotalAmount,
                        Subtotal = @Subtotal,
                        TaxAmount = @TaxAmount,
                        ShippingCost = @ShippingCost,
                        InvoicePdfPath = @InvoicePdfPath
                    WHERE PurchaseOrderId = @PurchaseOrderId;";

                        await connection.ExecuteAsync(update, order, transaction);

                        string deleteItems = "DELETE FROM purchase_order_items WHERE PurchaseOrderId = @PurchaseOrderId";
                        await connection.ExecuteAsync(deleteItems, new { order.PurchaseOrderId }, transaction);
                    }

                    foreach (var item in order.Items)
                    {
                        item.PurchaseOrderId = order.PurchaseOrderId;
                        string insertItem = @"INSERT INTO purchase_order_items 
                    (PurchaseOrderId, ProductName, Quantity, UnitPrice) 
                    VALUES (@PurchaseOrderId, @ProductName, @Quantity, @UnitPrice)";
                        await connection.ExecuteAsync(insertItem, item, transaction);
                    }

                    // Add Finance Journal Entry
                    if (order.PurchaseOrderId > 0)
                        await AddFinanceEntryAsync(order, transaction);

                    transaction.Commit();
                    return order.PurchaseOrderId;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ApplicationLogger.Log(ex, "Transaction Failure", nameof(PurchaseOrderRepository), Environment.UserName);
                    return -1;
                }
            }
            catch (Exception ex)
            {
                ApplicationLogger.Log(ex, nameof(AddOrUpdateAsync), nameof(PurchaseOrderRepository), Environment.UserName);
                return -1;
            }
        }

        private async Task AddFinanceEntryAsync(PurchaseOrder order, IDbTransaction transaction)
        {
            var entry = new JournalEntry
            {
                EntryDate = order.OrderDate,
                Description = $"PO #{order.po_number} created for Supplier #{order.Supplier_id}",
                CreatedAt = DateTime.Now
            };

            var lines = new List<JournalEntryLine>
    {
        new JournalEntryLine
        {
            AccountID = 7, // Inventory
            Amount = order.TotalAmount,
            IsDebit = true
        },
        new JournalEntryLine
        {
            AccountID = 22, // Accounts Payable
            Amount = order.TotalAmount,
            IsDebit = false
        }
    };

            var journalRepo = new JournalEntryRepository();
            await journalRepo.AddAsync(entry, lines);
        }



        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                using var connection = DatabaseHelper.GetOpenConnection();
                using var transaction = connection.BeginTransaction();

                try
                {
                    // First delete the items
                    string deleteItems = "DELETE FROM purchase_order_items WHERE PurchaseOrderId = @Id";
                    await connection.ExecuteAsync(deleteItems, new { Id = id }, transaction);

                    // Then delete the order
                    string deleteOrder = "DELETE FROM purchase_orders WHERE PurchaseOrderId = @Id";
                    await connection.ExecuteAsync(deleteOrder, new { Id = id }, transaction);

                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ApplicationLogger.Log(ex, "TransactionFailure", nameof(PurchaseOrderRepository), Environment.UserName);
                    return false;
                }
            }
            catch (Exception ex)
            {
                ApplicationLogger.Log(ex, nameof(DeleteAsync), nameof(PurchaseOrderRepository), Environment.UserName);
                return false;
            }
        }

    }
}
