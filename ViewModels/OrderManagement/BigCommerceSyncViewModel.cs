using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CbcRoastersErp.Models;
using CbcRoastersErp.Services;
using CbcRoastersErp.Repositories;
using System;
using CbcRoastersErp.Helpers;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using CbcRoastersErp.Repositories.Finance;
using CbcRoastersErp.Services.Finance;

namespace CbcRoastersErp.ViewModels
{
    public class OrderStatus
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class BigCommerceSyncViewModel : BaseViewModel
    {
        private readonly BigCommerceService _service = new();
        private readonly BigCommerceRepository _repository = new();
        private readonly ProductionRepository _productionRepo = new();
        public event PropertyChangedEventHandler PropertyChanged;
        public event Action<string> OnNavigationRequested;

        // Date Time Filter Fields
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public ObservableCollection<Customers> Customers { get; set; } = new();
        public ObservableCollection<BigCommerceOrders> Orders { get; set; } = new();
        public List<OrderStatus> OrderStatuses { get; } = new()
        {
            new OrderStatus { Id = 7, Name = "Awaiting Payment" },
            new OrderStatus { Id = 11, Name = "Awaiting Fulfillment" },
            new OrderStatus { Id = 9, Name = "Awaiting Shipment" },
            new OrderStatus { Id = 2, Name = "Shipped" },
            new OrderStatus { Id = 10, Name = "Completed" },
            new OrderStatus { Id = 8, Name = "Awaiting Pickup" },
            new OrderStatus { Id = 5, Name = "Cancelled" }
        };

        private OrderStatus _selectedOrderStatus;
        public OrderStatus SelectedOrderStatus
        {
            get => _selectedOrderStatus;
            set
            {
                _selectedOrderStatus = value;
                OnPropertyChanged();
            }
        }


        public ICommand SyncCustomersCommand => new RelayCommand(async () => await SyncCustomers());
        public ICommand SyncOrdersCommand => new RelayCommand(async () => await SyncOrders());
        public ICommand NavigateBackCommand => new RelayCommand(_ => OnNavigationRequested?.Invoke("Dashboard"));
        public ICommand SyncOrdersByDateCommand => new RelayCommand(async _ => await SyncOrdersByDateAsync());


        private async Task SyncCustomers()
        {
            try
            {
                IsBusy = true;
                StatusMessage = "Syncing customers...";

                var results = await _service.GetCustomersAsync();
                Customers.Clear();
                foreach (var c in results)
                {
                    Customers.Add(c);
                    _repository.SaveCustomers(new List<Customers> { c });
                }
                ApplicationLogger.LogInfo("Customer sync completed successfully.");
            }
            catch (Exception ex)
            {
                ApplicationLogger.Log(ex, nameof(BigCommerceSyncViewModel), nameof(SyncCustomers), Environment.UserName);
            }
            finally
            {
                IsBusy = false;
                StatusMessage = "Customer Sync completed.";
            }
        }

        private async Task SyncOrders()
        {
            try
            {
                if (SelectedOrderStatus == null) return;

                IsBusy = true;
                StatusMessage = $"Syncing orders ({SelectedOrderStatus.Name})...";

                // Ensure the repository is initialized
                var scheduleRepo = new BatchScheduleRepository();

                var results = await _service.GetOrdersByStatusAsync(SelectedOrderStatus.Id);
                Orders.Clear();
                foreach (var o in results)
                {
                    Orders.Add(o);
                    _repository.SaveOrders(new List<BigCommerceOrders> { o });
                }

                // Save metrics to the database
                var metrics = BigCommerceMetricsMapper.MapToSalesMetrics(results);
                await new DriposSalesMetricsRepository().InsertAsync(metrics);

                foreach (var order in results)
                {
                    if (!_repository.DoesCustomerExist(order.CustomerID))
                    {
                        // This is the missing piece: calling the API
                        var customer = await _service.GetCustomerByIdAsync(order.CustomerID);
                        if (customer != null)
                        {
                            _repository.UpsertCustomer(customer);
                            order.CustomerName = customer.FullName; // Update order with customer name

                            ApplicationLogger.LogInfo($"Fetched and upserted customer {customer.FullName} from BigCommerce.");
                        }
                        else
                        {
                            ApplicationLogger.LogWarning($"Customer with ID {order.CustomerID} not found in BigCommerce.");
                            continue; // Skip syncing this order
                        }
                    }

                    // Check for order status awaiting fulfillment
                    if (order.OrderStatus == "Awaiting Fulfillment")
                    {
                        var orderProducts = await _service.GetOrderProductsAsync(order.BigCommerceID);

                        foreach (var product in orderProducts)
                        {
                            var finishedGood = _repository.GetFinishedGoodByBigCommProdId(product.ProductId);
                            int localOrderId = (int)_repository.GetLocalOrderIdByBigCommerceId(order.BigCommerceID);
                            if (finishedGood != null && localOrderId != null)
                            {
                                var schedule = new BatchSchedule
                                {
                                    OrderID = order.BigCommerceID, //localOrderId,
                                    FinishedGoodID = finishedGood.FinishedGoodID,
                                    Quantity = product.Quantity,
                                    ScheduledDate = DateTime.Now.AddDays(1),
                                    Status = "Scheduled",
                                    Notes = $"Scheduled from BigCommerce Order #{order.BigCommerceID}"
                                };

                                scheduleRepo.AddSchedule(schedule);
                                ApplicationLogger.LogInfo($"Scheduled Order {order.BigCommerceID}, Product {product.ProductId} for {product.Quantity} units.");
                            }
                            else
                            {
                                ApplicationLogger.LogWarning($"Product ID {product.ProductId} from Order {order.BigCommerceID} does not match any FinishedGood.");
                            }
                        }
                    }

                    // Check for Status "Shpped"
                    if (order.OrderStatus == "Shipped")
                    {
                        var orderProducts = await _service.GetOrderProductsAsync(order.BigCommerceID);

                        foreach (var product in orderProducts)
                        {
                            var finishedGood = _repository.GetFinishedGoodByBigCommProdId(product.ProductId);
                            if (finishedGood != null)
                            {
                                _productionRepo.LogFinishedGoodOutboundTransaction(finishedGood.FinishedGoodID, product.Quantity);
                                ApplicationLogger.LogInfo($"Logged shipment of {product.Quantity} units for Product ID {product.ProductId} from Order {order.BigCommerceID}.");
                            }
                            else
                            {
                                ApplicationLogger.LogWarning($"Could not match Product ID {product.ProductId} from shipped Order {order.BigCommerceID} to a FinishedGood.");
                            }
                        }
                    }
                }

                ApplicationLogger.LogInfo($"Order sync for status '{SelectedOrderStatus.Name}' completed successfully.");
            }
            catch (Exception ex)
            {
                ApplicationLogger.Log(ex, nameof(BigCommerceSyncViewModel), nameof(SyncOrders), Environment.UserName);
            }
            finally
            {
                IsBusy = false;
                StatusMessage = "Order Sync completed.";
            }
        }

        private async Task EnsureCustomerExistsAsync(BigCommerceOrders order)
        {
            try
            {
                var customer = await _service.GetCustomerByIdAsync(order.CustomerID);
                if (customer != null)
                {
                    var mappedCustomer = new Customers
                    {
                        BigCommerceID = customer.CustomerID,
                        Company = customer.Company,
                        FullName = $"{customer.FirstName} {customer.LastName}",
                        Email = customer.Email,
                        PhoneNumber = customer.PhoneNumber,
                        Address = customer.Address,
                        CustomerType = "BigCommerce"
                    };

                    _repository.UpsertCustomer(mappedCustomer);

                }
            }
            catch (Exception ex)
            {
                ApplicationLogger.Log(ex, nameof(BigCommerceSyncViewModel), nameof(EnsureCustomerExistsAsync), Environment.UserName);
            }
        }

        private async Task SyncOrdersByDateAsync()
        {
            try
            {
                if (SelectedOrderStatus == null)
                {
                    // Alert user to select a status
                    MessageBox.Show("Please select an order status to sync.", "Warning", MessageBoxButton.OK);
                    return;
                }

                if (StartDate == null || EndDate == null || StartDate > EndDate)
                {
                    // Alert user to select valid date range
                    MessageBox.Show("Please select a valid date range.", "Warning", MessageBoxButton.OK);
                    return;
                }

                IsBusy = true;
                StatusMessage = $"Syncing orders ({SelectedOrderStatus.Name})...";

                // Ensure the repository is initialized
                var scheduleRepo = new BatchScheduleRepository();

                var results = await _service.GetOrdersByStatusAndDateAsync(SelectedOrderStatus.Id, StartDate, EndDate);
                Orders.Clear();

                foreach (var o in results)
                {
                    var oCoustomer = await _service.GetCustomerByIdAsync(o.CustomerID);
                    o.CustomerName = oCoustomer != null ? oCoustomer.FullName : "Unknown Customer";
                    o.OrderNumber = o.BigCommerceID.ToString(); // Ensure OrderNumber is set
                    Orders.Add(o);
                    _repository.SaveOrders(new List<BigCommerceOrders> { o });
                }

                // Save metrics to the database
                var metrics = BigCommerceMetricsMapper.MapToSalesMetrics(results);
                await new DriposSalesMetricsRepository().InsertAsync(metrics);

                foreach (var order in results)
                {
                    if (!_repository.DoesCustomerExist(order.CustomerID))
                    {
                        // This is the missing piece: calling the API
                        var customer = await _service.GetCustomerByIdAsync(order.CustomerID);
                        if (customer != null)
                        {
                            _repository.UpsertCustomer(customer);
                            order.CustomerName = customer.FullName; // Update order with customer name
                            ApplicationLogger.LogInfo($"Fetched and upserted customer {customer.FullName} from BigCommerce.");
                        }
                        else
                        {
                            ApplicationLogger.LogWarning($"Customer with ID {order.CustomerID} not found in BigCommerce.");
                            continue; // Skip syncing this order
                        }
                    }

                    // Check for order status awaiting fulfillment
                    if (order.OrderStatus == "Awaiting Fulfillment")
                    {
                        var orderProducts = await _service.GetOrderProductsAsync(order.BigCommerceID);

                        foreach (var product in orderProducts)
                        {
                            var finishedGood = _repository.GetFinishedGoodByBigCommProdId(product.ProductId);
                            int localOrderId = (int)_repository.GetLocalOrderIdByBigCommerceId(order.BigCommerceID);
                            if (finishedGood != null && localOrderId != null)
                            {
                                var schedule = new BatchSchedule
                                {
                                    OrderID = localOrderId,
                                    FinishedGoodID = finishedGood.FinishedGoodID,
                                    Quantity = product.Quantity,
                                    ScheduledDate = DateTime.Now.AddDays(1),
                                    Status = "Scheduled",
                                    Notes = $"Scheduled from BigCommerce Order #{order.BigCommerceID}"
                                };

                                scheduleRepo.AddSchedule(schedule);
                                ApplicationLogger.LogInfo($"Scheduled Order {order.BigCommerceID}, Product {product.ProductId} for {product.Quantity} units.");
                            }
                            else
                            {
                                ApplicationLogger.LogWarning($"Product ID {product.ProductId} from Order {order.BigCommerceID} does not match any FinishedGood.");
                            }
                        }
                    }

                    // Check for Status "Shpped"
                    if (order.OrderStatus == "Shipped")
                    {
                        var orderProducts = await _service.GetOrderProductsAsync(order.BigCommerceID);

                        foreach (var product in orderProducts)
                        {
                            var finishedGood = _repository.GetFinishedGoodByBigCommProdId(product.ProductId);
                            if (finishedGood != null)
                            {
                                _productionRepo.LogFinishedGoodOutboundTransaction(finishedGood.FinishedGoodID, product.Quantity);
                                ApplicationLogger.LogInfo($"Logged shipment of {product.Quantity} units for Product ID {product.ProductId} from Order {order.BigCommerceID}.");
                            }
                            else
                            {
                                ApplicationLogger.LogWarning($"Could not match Product ID {product.ProductId} from shipped Order {order.BigCommerceID} to a FinishedGood.");
                            }
                        }
                    }
                }

                ApplicationLogger.LogInfo($"Orders synced by status {SelectedOrderStatus.Id} and date range.");
            }
            catch (Exception ex)
            {
                ApplicationLogger.Log(ex, nameof(BigCommerceSyncViewModel), nameof(SyncOrdersByDateAsync), Environment.UserName);
            }
            finally
            {
                IsBusy = false;
                StatusMessage = "Order Sync completed.";
            }
        }


        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
