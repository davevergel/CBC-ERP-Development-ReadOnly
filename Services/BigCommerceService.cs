using System.Net.Http;
using Newtonsoft.Json.Linq;
using CbcRoastersErp.Models;
using CbcRoastersErp.Helpers;
using Dapper;
using Newtonsoft.Json;
using System.Windows;

namespace CbcRoastersErp.Services
{
    public class BigCommerceService
    {
        private readonly HttpClient _httpClient;

        public BigCommerceService()
        {
            var config = ConfigHelper.Configuration.GetSection("BigCommerce");
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(config["ApiBaseUrl"])
            };
            _httpClient.DefaultRequestHeaders.Add("X-Auth-Token", config["AccessToken"]);
            _httpClient.DefaultRequestHeaders.Add("X-Auth-Client", config["ClientId"]);
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        public async Task<List<Customers>> GetCustomersAsync()
        {
            var customers = new List<Customers>();
            try
            {
                int page = 1;
                bool morePages = true;

                while (morePages)
                {
                    var response = await _httpClient.GetStringAsync($"customers?limit=250&page={page}");
                    var jsonArray = JArray.Parse(response);

                    if (jsonArray.Count == 0)
                    {
                        morePages = false;
                        break;
                    }

                    foreach (var item in jsonArray)
                    {
                        var company = item["company"]?.ToString();
                        var fullName = string.IsNullOrWhiteSpace(company)
                            ? $"{item["first_name"]} {item["last_name"]}".Trim()
                            : company;

                        customers.Add(new Customers
                        {
                            BigCommerceID = (int)item["id"],
                            FullName = fullName,
                            Email = item["email"]?.ToString(),
                            PhoneNumber = item["phone"]?.ToString(),
                            Address = item["address1"]?.ToString(),
                            CustomerType = "Retail"
                        });
                    }

                    page++;
                }

                ApplicationLogger.LogInfo($"Retrieved {customers.Count} customers from BigCommerce.");
            }
            catch (Exception ex)
            {
                ApplicationLogger.Log(ex, nameof(BigCommerceService), nameof(GetCustomersAsync), Environment.UserName);
            }

            return customers;
        }

        public async Task<List<BigCommerceOrders>> GetOrdersByStatusAsync(int status)
        {
            var orders = new List<BigCommerceOrders>();
            try
            {
                int page = 1;
                bool morePages = true;

                while (morePages)
                {
                    var response = await _httpClient.GetStringAsync($"orders?status_id={status}&limit=250&page={page}");
                    var jsonArray = JArray.Parse(response);

                    if (jsonArray.Count == 0)
                    {
                        morePages = false;
                        break;
                    }

                    foreach (var item in jsonArray)
                    {
                        orders.Add(new BigCommerceOrders
                        {
                            BigCommerceID = (int)item["id"],
                            OrderNumber = item["order_number"]?.ToString(),
                            CustomerID = item["customer_id"]?.Value<int>() ?? 0,
                            OrderDate = item["date_created"]?.Value<DateTime>(),
                            TotalAmount = item["total_inc_tax"]?.Value<decimal>() ?? 0,
                            OrderStatus = item["status"]?.ToString()
                        });
                    }

                    page++;
                }

                ApplicationLogger.LogInfo($"Retrieved {orders.Count} orders with status ID {status} from BigCommerce.");
            }
            catch (Exception ex)
            {
                ApplicationLogger.Log(ex, nameof(BigCommerceService), nameof(GetOrdersByStatusAsync), Environment.UserName);
            }

            return orders;
        }

        public async Task<List<BigCommerceOrders>> GetOrdersByStatusAndDateAsync(int statusId, DateTime? startDate, DateTime? endDate)
        {
            var orders = new List<BigCommerceOrders>();
            try
            {
                int page = 1;
                bool morePages = true;

                var dateParams = string.Empty;

                if (startDate.HasValue)
                    dateParams += $"&min_date_created={startDate.Value:yyyy-MM-dd}";

                if (endDate.HasValue)
                    dateParams += $"&max_date_created={endDate.Value:yyyy-MM-dd}";

                while (morePages)
                {
                    string url = $"orders?status_id={statusId}&limit=250&page={page}{dateParams}";
                    var response = await _httpClient.GetStringAsync(url);

                    if (string.IsNullOrWhiteSpace(response))
                    {
                        morePages = false;
                        ApplicationLogger.LogInfo($"No response received for orders with status {statusId} and date range from BigCommerce.");
                        MessageBox.Show("No response received from BigCommerce for the specified orders. Please check your connection or the API status.", "Error", MessageBoxButton.OK);
                        break;
                    }

                    var jsonArray = JArray.Parse(response);
                    if (jsonArray.Count == 0)
                    {
                        morePages = false;
                        break;
                    }

                    foreach (var item in jsonArray)
                    {
                        orders.Add(new BigCommerceOrders
                        {
                            BigCommerceID = (int)item["id"],
                            OrderNumber = item["order_number"]?.ToString(),
                            CustomerID = item["customer_id"]?.Value<int>() ?? 0,
                            OrderDate = item["date_created"]?.Value<DateTime>(),
                            TotalAmount = item["total_inc_tax"]?.Value<decimal>() ?? 0,
                            OrderStatus = item["status"]?.ToString()
                        });
                    }

                    if (jsonArray.Count < 250)
                    {
                        morePages = false;
                    }
                    else
                    {
                        page++;
                    }
                }

                ApplicationLogger.LogInfo($"Retrieved {orders.Count} orders with status {statusId} and date range from BigCommerce.");
            }
            catch (Exception ex)
            {
                ApplicationLogger.Log(ex, nameof(BigCommerceService), nameof(GetOrdersByStatusAndDateAsync), Environment.UserName);
                MessageBox.Show("An error occurred while retrieving orders from BigCommerce. Please check the logs for more details.", "Error", MessageBoxButton.OK);
            }

            return orders;
        }



        public void SaveCustomer(Customers customer)
        {
            try
            {
                using var conn = DatabaseHelper.GetOpenConnection();
                var query = @"INSERT INTO Customers (BigCommerceID, FullName, Email, PhoneNumber, Address, CustomerType)
                  VALUES (@BigCommerceID, @FullName, @Email, @PhoneNumber, @Address, @CustomerType)
                  ON DUPLICATE KEY UPDATE
                    FullName = VALUES(FullName),
                    Email = VALUES(Email),
                    PhoneNumber = VALUES(PhoneNumber),
                    Address = VALUES(Address),
                    CustomerType = VALUES(CustomerType);";
                conn.Execute(query, customer);

                ApplicationLogger.LogInfo($"Customer {customer.FullName} saved successfully.");
            }
            catch (Exception ex)
            {
                ApplicationLogger.Log(ex, nameof(BigCommerceService), nameof(SaveCustomer), Environment.UserName);
            }
        }

        public void SaveOrder(BigCommerceOrders order)
        {
            try
            {
                using var conn = DatabaseHelper.GetOpenConnection();
                var query = @"INSERT INTO BigCommerceOrders (BigCommerceID, OrderNumber, CustomerID, OrderDate, TotalAmount, OrderStatus)
                  VALUES (@BigCommerceID, @OrderNumber, @CustomerID, @OrderDate, @TotalAmount, @OrderStatus)
                  ON DUPLICATE KEY UPDATE
                    OrderNumber = VALUES(OrderNumber),
                    CustomerID = VALUES(CustomerID),
                    OrderDate = VALUES(OrderDate),
                    TotalAmount = VALUES(TotalAmount),
                    OrderStatus = VALUES(OrderStatus);";
                conn.Execute(query, order);

                ApplicationLogger.LogInfo($"Order {order.OrderNumber} saved successfully.");
            }
            catch (Exception ex)
            {
                ApplicationLogger.Log(ex, nameof(BigCommerceService), nameof(SaveOrder), Environment.UserName);
            }
        }

        public async Task<Customers> GetCustomerByIdAsync(int customerId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"customers/{customerId}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var customerData = JObject.Parse(json);

                    var company = customerData["company"]?.ToString();
                    var fullName = string.IsNullOrWhiteSpace(company)
                        ? $"{customerData["first_name"]} {customerData["last_name"]}".Trim()
                        : company;

                    var customer = new Customers
                    {
                        BigCommerceID = customerData["id"]?.Value<int>() ?? 0,
                        FullName = fullName,
                        Email = customerData["email"]?.ToString(),
                        PhoneNumber = customerData["phone"]?.ToString(),
                        Address = customerData["address1"]?.ToString(),
                        CustomerType = "Retail"
                    };

                    return customer;
                }
                return null;
            }
            catch (Exception ex)
            {
                ApplicationLogger.Log(ex, nameof(BigCommerceService), nameof(GetCustomerByIdAsync), Environment.UserName);
                return null;
            }
        }

        public async Task<List<OrderProduct>> GetOrderProductsAsync(int orderId)
        {
            try
            {
                var requestUrl = $"orders/{orderId}/products";
                var response = await _httpClient.GetAsync(requestUrl);
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<OrderProduct>>(content);
            }
            catch (Exception ex)
            {
                ApplicationLogger.Log(ex, nameof(BigCommerceService), nameof(GetOrderProductsAsync), Environment.UserName);
                return new List<OrderProduct>();
            }
        }

        // Sales Dashboard Methods
        public async Task<OrderSummary> GetOrderSummaryByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            // BigCommerce API sometimes treats max_date_created as exclusive — fix it
            var inclusiveEndDate = endDate.AddDays(1);

            var orders = await GetOrdersByDateRangeAsync(startDate, inclusiveEndDate);

            var summary = new OrderSummary();
            foreach (var order in orders)
            {
                switch (order.OrderStatus?.ToLowerInvariant())
                {
                    case "awaiting shipment":
                        summary.AwaitingShipment++;
                        break;
                    case "awaiting fulfillment":
                        summary.AwaitingFulfillment++;
                        break;
                    case "awaiting payment":
                        summary.AwaitingPayment++;
                        break;
                    case "shipped":
                        summary.Shipped++;
                        break;
                    case "completed":
                        summary.Placed++;
                        break;
                }

                summary.TotalSalesAmount += order.TotalAmount;
            }
            return summary;
        }

        public async Task<List<BigCommerceOrders>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var orders = new List<BigCommerceOrders>();
            int page = 1;
            bool morePages = true;

            while (morePages)
            {
                var response = await _httpClient.GetStringAsync($"orders?limit=250&page={page}&min_date_created={startDate:yyyy-MM-dd}&max_date_created={endDate:yyyy-MM-dd}");
                if (string.IsNullOrWhiteSpace(response))
                    break;

                var jsonArray = JArray.Parse(response);
                if (jsonArray.Count == 0)
                    break;

                foreach (var item in jsonArray)
                {
                    orders.Add(new BigCommerceOrders
                    {
                        BigCommerceID = (int)item["id"],
                        OrderNumber = item["order_number"]?.ToString(),
                        CustomerID = item["customer_id"]?.Value<int>() ?? 0,
                        OrderDate = item["date_created"]?.Value<DateTime>(),
                        TotalAmount = item["total_inc_tax"]?.Value<decimal>() ?? 0,
                        OrderStatus = item["status"]?.ToString()
                    });
                }

                if (jsonArray.Count < 250)
                    morePages = false;
                else
                    page++;
            }

            return orders;
        }

        public async Task<List<BigCommerceOrders>> GetOrdersForCurrentMonthAsync()
        {
            var orders = new List<BigCommerceOrders>();
            try
            {
                int page = 1;
                bool morePages = true;

                var startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                var endDate = DateTime.Now;

                while (morePages)
                {
                    string url = $"orders?limit=250&page={page}&min_date_created={startDate:yyyy-MM-dd}&max_date_created={endDate:yyyy-MM-dd}";
                    var response = await _httpClient.GetStringAsync(url);

                    if (string.IsNullOrWhiteSpace(response))
                    {
                        morePages = false;
                        ApplicationLogger.LogInfo($"No orders found for current month.");
                        break;
                    }

                    var jsonArray = JArray.Parse(response);
                    if (jsonArray.Count == 0)
                    {
                        morePages = false;
                        break;
                    }

                    foreach (var item in jsonArray)
                    {
                        orders.Add(new BigCommerceOrders
                        {
                            BigCommerceID = (int)item["id"],
                            OrderNumber = item["order_number"]?.ToString(),
                            CustomerID = item["customer_id"]?.Value<int>() ?? 0,
                            OrderDate = item["date_created"]?.Value<DateTime>(),
                            TotalAmount = item["total_inc_tax"]?.Value<decimal>() ?? 0,
                            OrderStatus = item["status"]?.ToString()
                        });
                    }

                    if (jsonArray.Count < 250)
                    {
                        morePages = false;
                    }
                    else
                    {
                        page++;
                    }
                }

                ApplicationLogger.LogInfo($"Retrieved {orders.Count} orders for the current month from BigCommerce.");
            }
            catch (Exception ex)
            {
                ApplicationLogger.Log(ex, nameof(BigCommerceService), nameof(GetOrdersForCurrentMonthAsync), Environment.UserName);
            }

            return orders;
        }



        public async Task<OrderSummary> GetCurrentMonthOrderSummaryAsync()
        {
            var orders = await GetOrdersForCurrentMonthAsync(); // Reuse your existing API call

            var summary = new OrderSummary();
            foreach (var order in orders)
            {
                switch (order.OrderStatus?.ToLowerInvariant())
                {
                    case "awaiting_shipment":
                        summary.AwaitingShipment++;
                        break;
                    case "awaiting_fulfillment":
                        summary.AwaitingFulfillment++;
                        break;
                    case "shipped":
                        summary.Shipped++;
                        break;
                    case "completed":
                        summary.Placed++;
                        break;
                }
            }
            return summary;
        }

    }
}
