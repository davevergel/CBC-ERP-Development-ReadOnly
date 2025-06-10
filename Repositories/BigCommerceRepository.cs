using System;
using System.Collections.Generic;
using System.Linq;
using CbcRoastersErp.Models;
using Dapper;
using MySqlConnector;
using CbcRoastersErp.Helpers;
using CbcRoastersErp.ViewModels;
using System.Data;

namespace CbcRoastersErp.Repositories
{
    public class BigCommerceRepository
    {
        private readonly IDbConnection _dbConnection;

        public BigCommerceRepository()
        {
            _dbConnection =DatabaseHelper.GetConnection();
        }

        public IEnumerable<Customers> GetAllCustomers()
        {
            try
            {
                const string query = "SELECT * FROM Customers ORDER BY FullName ASC";
                using var connection = DatabaseHelper.GetOpenConnection();
                return connection.Query<Customers>(query);
            }
            catch (Exception ex)
            {
                ApplicationLogger.Log(ex, nameof(BigCommerceRepository), nameof(GetAllCustomers), Environment.UserName);
                return Enumerable.Empty<Customers>();
            }
        }

        public IEnumerable<BigCommerceOrderDisplay> GetOrdersWithCustomerNames()
        {
            try
            {
                var sql = @"
                SELECT o.OrderID, o.BigCommerceID, o.OrderNumber, 
                       c.FullName AS CustomerName, 
                       CAST(o.OrderDate AS CHAR) AS OrderDate,
                       o.TotalAmount, o.OrderStatus
                FROM BigCommerceOrders o
                LEFT JOIN Customers c ON o.CustomerID = c.BigCommerceID
                ORDER BY o.OrderDate DESC";
                
                return _dbConnection.Query<BigCommerceOrderDisplay>(sql);
            } 
            catch (Exception ex)
            {
                ApplicationLogger.Log(ex, "System", "Error");
                return Enumerable.Empty<BigCommerceOrderDisplay>();
            }

        }

        public bool DoesCustomerExist(int customerId)
        {
            try
            {
                using var conn = DatabaseHelper.GetOpenConnection();
                var query = "SELECT COUNT(1) FROM Customers WHERE BigCommerceID = @CustomerId";
                return conn.ExecuteScalar<bool>(query, new { CustomerId = customerId });
            }
            catch (Exception ex)
            {
                ApplicationLogger.Log(ex, nameof(BigCommerceRepository), nameof(DoesCustomerExist), Environment.UserName);
                return false;
            }
        }

        public void UpsertCustomer(Customers customer)
        {
            try
            {
                using var connection = DatabaseHelper.GetOpenConnection();
                const string query = @"
                INSERT INTO Customers (BigCommerceID, Company, FullName, Email, PhoneNumber, Address, CustomerType)
                VALUES (@BigCommerceID, @Company, @FullName, @Email, @PhoneNumber, @Address, @CustomerType)
                ON DUPLICATE KEY UPDATE
                    BigCommerceID = @BigCommerceID,
                    Company = @Company,
                    FullName = @FullName,
                    Email = @Email,
                    PhoneNumber = @PhoneNumber,
                    Address = @Address,
                    CustomerType = @CustomerType";

                connection.Execute(query, customer);

                // Log the operation
                if (customer.CustomerID == 0)
                {
                    ApplicationLogger.LogInfo($"Customer added: {customer.FullName} BigCommerceID {customer.BigCommerceID}");
                }
                else
                {
                    ApplicationLogger.LogInfo($"Customer updated: {customer.FullName}  BigCommerceID {customer.BigCommerceID}");
                }
            }
            catch (Exception ex)
            {
                ApplicationLogger.Log(ex, "System", "Error");
            }
        }

        public void SaveCustomers(List<Customers> customers)
        {
            using var connection = DatabaseHelper.GetOpenConnection();
            using var transaction = connection.BeginTransaction();

            try
            {
                foreach (var customer in customers)
                {
                    var existing = connection.QueryFirstOrDefault<Customers>(
                        "SELECT * FROM Customers WHERE BigCommerceID = @Id",
                        new { Id = customer.BigCommerceID },
                        transaction);

                    if (existing == null)
                    {
                        connection.Execute(@"
                            INSERT INTO Customers (BigCommerceID, FullName, Email, PhoneNumber, Address, CustomerType)
                            VALUES (@BigCommerceID, @FullName, @Email, @PhoneNumber, @Address, @CustomerType)",
                            customer, transaction);

                        ApplicationLogger.LogInfo($"Customer added: {customer.FullName}");
                    }
                    else
                    {
                        connection.Execute(@"
                            UPDATE Customers SET FullName = @FullName, Email = @Email, PhoneNumber = @PhoneNumber,
                            Address = @Address, CustomerType = @CustomerType WHERE BigCommerceID = @BigCommerceID",
                            customer, transaction);

                        ApplicationLogger.LogInfo($"Customer updated: {customer.FullName}");
                    }
                }

                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                ApplicationLogger.Log(ex, "System", "Error");
            }
        }

        public void SaveOrders(List<BigCommerceOrders> orders)
        {
            using var connection = DatabaseHelper.GetOpenConnection();
            using var transaction = connection.BeginTransaction();

            try
            {
                foreach (var order in orders)
                {
                    var existing = connection.QueryFirstOrDefault<BigCommerceOrders>(
                        "SELECT * FROM BigCommerceOrders WHERE BigCommerceID = @Id",
                        new { Id = order.BigCommerceID },
                        transaction);

                    if (existing == null)
                    {
                        connection.Execute(@"
                            INSERT INTO BigCommerceOrders (BigCommerceID, OrderNumber, CustomerID, OrderDate, TotalAmount, OrderStatus)
                            VALUES (@BigCommerceID, @OrderNumber, @CustomerID, @OrderDate, @TotalAmount, @OrderStatus)",
                            order, transaction);

                        ApplicationLogger.LogInfo($"Order added: {order.OrderNumber}");
                    }
                    else
                    {
                        connection.Execute(@"
                            UPDATE BigCommerceOrders SET OrderNumber = @OrderNumber, CustomerID = @CustomerID,
                            OrderDate = @OrderDate, TotalAmount = @TotalAmount, OrderStatus = @OrderStatus
                            WHERE BigCommerceID = @BigCommerceID",
                            order, transaction);

                        ApplicationLogger.LogInfo($"Order updated: {order.OrderNumber}");
                    }
                }

                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                ApplicationLogger.Log(ex, "System", "Error");
            }
        }

        // Insert Batch Schedule
        public FinishedGoods GetFinishedGoodByBigCommProdId(int bigCommProdId)
        {
            try
            {
                using var connection = DatabaseHelper.GetOpenConnection();
                string sql = "SELECT * FROM FinishedGoods WHERE BigCommProdID = @BigCommProdID LIMIT 1";
                return connection.QueryFirstOrDefault<FinishedGoods>(sql, new { BigCommProdID = bigCommProdId });
            }
            catch (Exception ex)
            {
                ApplicationLogger.Log(ex, nameof(BigCommerceRepository), nameof(GetFinishedGoodByBigCommProdId), Environment.UserName);
                return null;
            }
        }

        public int? GetLocalOrderIdByBigCommerceId(int bigCommerceId)
        {
            const string query = "SELECT OrderID FROM BigCommerceOrders WHERE BigCommerceID = @BigCommerceID";
            return _dbConnection.QueryFirstOrDefault<int?>(query, new { BigCommerceID = bigCommerceId });
        }

        // This method retrieves orders with customer names, paginated and filtered by various criteria.
        public (IEnumerable<BigCommerceOrderDisplay> Orders, int TotalCount) GetOrdersWithCustomerNamesPaginated(
               string customerName, string status, DateTime? startDate, DateTime? endDate, int page, int pageSize)
        {
            try
            {
                using var conn = DatabaseHelper.GetOpenConnection();
                var whereClauses = new List<string>();
                var parameters = new DynamicParameters();

                if (!string.IsNullOrEmpty(customerName))
                {
                    whereClauses.Add("c.FullName LIKE @CustomerName");
                    parameters.Add("@CustomerName", $"%{customerName}%");
                }

                if (!string.IsNullOrEmpty(status))
                {
                    whereClauses.Add("o.OrderStatus = @Status");
                    parameters.Add("@Status", status);
                }

                if (startDate.HasValue)
                {
                    whereClauses.Add("o.OrderDate >= @StartDate");
                    parameters.Add("@StartDate", startDate.Value);
                }

                if (endDate.HasValue)
                {
                    whereClauses.Add("o.OrderDate <= @EndDate");
                    parameters.Add("@EndDate", endDate.Value);
                }

                var whereSql = whereClauses.Any() ? "WHERE " + string.Join(" AND ", whereClauses) : "";

                var sql = $@"
                            SELECT SQL_CALC_FOUND_ROWS
                                   o.OrderID, o.BigCommerceID, o.OrderNumber, 
                                   c.FullName AS CustomerName,
                                   CAST(o.OrderDate AS CHAR) AS OrderDate,
                                   o.TotalAmount, o.OrderStatus
                            FROM BigCommerceOrders o
                            LEFT JOIN Customers c ON o.CustomerID = c.BigCommerceID
                            {whereSql}
                            ORDER BY o.OrderDate DESC
                            LIMIT @Offset, @PageSize;
            
                            SELECT FOUND_ROWS();";

                parameters.Add("@Offset", (page - 1) * pageSize);
                parameters.Add("@PageSize", pageSize);

                using var multi = conn.QueryMultiple(sql, parameters);
                var orders = multi.Read<BigCommerceOrderDisplay>().ToList();
                var totalCount = multi.ReadFirst<int>();

                return (orders, totalCount);
            }
            catch (Exception ex)
            {
                ApplicationLogger.Log(ex, nameof(BigCommerceRepository), nameof(GetOrdersWithCustomerNamesPaginated), Environment.UserName);
                return (Enumerable.Empty<BigCommerceOrderDisplay>(), 0);
            }
        }

    }
}
