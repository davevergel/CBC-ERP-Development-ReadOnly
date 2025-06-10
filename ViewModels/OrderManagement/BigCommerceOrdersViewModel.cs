using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CbcRoastersErp.Models;
using CbcRoastersErp.Repositories;
using CbcRoastersErp.Services;

namespace CbcRoastersErp.ViewModels
{
    public class BigCommerceOrdersViewModel : INotifyPropertyChanged
    {
        private readonly BigCommerceRepository _repository = new();

        public event Action<string> OnNavigationRequested;
        public ICommand NavigateBackCommand => new RelayCommand(_ => OnNavigationRequested?.Invoke("Dashboard"));

        private ObservableCollection<BigCommerceOrderDisplay> _orders;
        public ObservableCollection<BigCommerceOrderDisplay> Orders
        {
            get => _orders;
            set { _orders = value; OnPropertyChanged(); }
        }

        private int _currentPage = 1;
        private int _totalPages;
        private int _pageSize = 25;
        private int _totalRecords;

        public string SearchCustomer { get; set; }
        public string SearchStatus { get; set; }
        public string SearchOrderNumber { get; set; }
        public DateTime SearchStartDate { get; set; } // Assuming this is a string for simplicity, could be DateTime if needed
        public DateTime SearchEndDate { get; set; } // Assuming this is a string for simplicity, could be DateTime if needed  


        public int CurrentPage
        {
            get => _currentPage;
            set { _currentPage = value; OnPropertyChanged(); }
        }

        public int TotalPages
        {
            get => _totalPages;
            set { _totalPages = value; OnPropertyChanged(); }
        }

        private const int PageSize = 25;

        public ICommand SearchCommand => new RelayCommand(_ => { CurrentPage = 1; LoadOrders(); });
        public ICommand NextPageCommand => new RelayCommand(_ =>
        {
            if (CurrentPage < TotalPages)
            {
                CurrentPage++;
                LoadOrders();
            }
        });

        public ICommand PreviousPageCommand => new RelayCommand(_ =>
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                LoadOrders();
            }
        });

        public BigCommerceOrdersViewModel()
        {
            SearchStartDate = DateTime.Now.AddMonths(-1); // Default to last month
            SearchEndDate = DateTime.Now; // Default to today
            LoadOrders();
        }

        private void LoadOrders()
        {
            try 
            {
            var result = _repository.GetOrdersWithCustomerNamesPaginated(
                customerName: SearchCustomer,
                status: SearchStatus,
                startDate: SearchStartDate,
                endDate: SearchEndDate,
                page: CurrentPage,
                pageSize: PageSize
            );

            Orders = new ObservableCollection<BigCommerceOrderDisplay>(result.Orders);
                
            _totalRecords = result.TotalCount;
            TotalPages = (_totalRecords + PageSize - 1) / PageSize;
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., log them, show a message to the user, etc.)
                Console.WriteLine($"Error loading orders: {ex.Message}");
            }
            finally
            {
                OnPropertyChanged(nameof(Orders));
                OnPropertyChanged(nameof(TotalPages));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

