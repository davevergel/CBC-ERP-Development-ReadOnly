using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Threading;
using LiveCharts;
using LiveCharts.Wpf;
using MaterialDesignThemes.Wpf;
using CbcRoastersErp.Services;

namespace CbcRoastersErp.ViewModels
{
    public class SalesDashboardViewModel : INotifyPropertyChanged
    {
        private readonly BigCommerceService _bigCommerceService;
        private readonly DispatcherTimer _refreshTimer;
        private readonly DispatcherTimer _snackbarTimer;
        public SeriesCollection SeriesCollection { get; set; } = new();
        public SeriesCollection PieSeriesCollection { get; set; } = new();
        public string[] Labels { get; set; } = [];

        public bool IsLoading { get; set; }
        public bool ShowPieChart { get; set; }
        public DateTime StartDate { get; set; } = new(DateTime.Now.Year, DateTime.Now.Month, 1);
        public DateTime EndDate { get; set; } = DateTime.Now;
        public string DateRangeText => $"Sales Dashboard for {StartDate:MMM dd, yyyy} - {EndDate:MMM dd, yyyy}";

        // KPI Metrics
        public int TotalOrders { get; set; }
        public int TotalShipped { get; set; }
        public int TotalAwaitingPayment { get; set; }
        public int TotalAwaitingFulfillment { get; set; }
        public int TotalAwaitingShipment { get; set; }
        public decimal TotalSalesAmount { get; set; }

        public string SnackbarMessage { get; set; }
        public bool IsSnackbarActive { get; set; }

        // Commands
        public ICommand RefreshCommand { get; }
        public ICommand ToggleChartCommand { get; }
        public ICommand ToggleThemeCommand { get; }
        public ICommand NavigateBackCommand { get; }
        public ICommand DateChangedCommand { get; }
        public ICommand SetLast30DaysCommand { get; }
        public ICommand SetThisYearCommand { get; }
        public ICommand SetLastYearCommand { get; }
        public ICommand SetThisMonthCommand { get; }
        public ICommand SetLastMonthCommand { get; }

        public event Action<string> OnNavigationRequested;

        public SalesDashboardViewModel()
        {
            _bigCommerceService = new BigCommerceService();

            // Intialize the snackbar timer to auto-dismiss after 3 seconds
            _snackbarTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(3) // Snackbar will auto-dismiss after 3 seconds
            };
            _snackbarTimer.Tick += (s, e) =>
            {
                IsSnackbarActive = false;
                OnPropertyChanged(nameof(IsSnackbarActive));
                _snackbarTimer.Stop();
            };

            RefreshCommand = new RelayCommand(async _ => await LoadDataAsync());
            ToggleChartCommand = new RelayCommand(_ => ToggleChart());
            ToggleThemeCommand = new RelayCommand(_ => ToggleTheme());
            NavigateBackCommand = new RelayCommand(_ => OnNavigationRequested?.Invoke("Dashboard"));
            DateChangedCommand = new RelayCommand(async _ => await LoadDataAsync());
            SetLast30DaysCommand = new RelayCommand(async _ => { SetLast30Days(); await LoadDataAsync(); });
            SetThisYearCommand = new RelayCommand(async _ => { SetThisYear(); await LoadDataAsync(); });
            SetLastYearCommand = new RelayCommand(async _ => { SetLastYear(); await LoadDataAsync(); });
            SetThisMonthCommand = new RelayCommand(async _ => { SetThisMonth(); await LoadDataAsync(); });
            SetLastMonthCommand = new RelayCommand(async _ => { SetLastMonth(); await LoadDataAsync(); });

            _refreshTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMinutes(5) // Auto-refresh every 5 minutes
            };
            _refreshTimer.Tick += async (s, e) => await LoadDataAsync();
            _refreshTimer.Start();

            _ = LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            try
            {
                IsLoading = true;
                OnPropertyChanged(nameof(IsLoading));

                var summary = await _bigCommerceService.GetOrderSummaryByDateRangeAsync(StartDate, EndDate);

                Labels = new[] { "Placed", "Shipped","Awaiting Payment", "Awaiting Fulfillment", "Awaiting Shipment" };

                TotalOrders = summary.Placed;
                TotalShipped = summary.Shipped;
                TotalAwaitingPayment = summary.AwaitingPayment;
                TotalAwaitingFulfillment = summary.AwaitingFulfillment;
                TotalAwaitingShipment = summary.AwaitingShipment;
                TotalSalesAmount = summary.TotalSalesAmount;

                SeriesCollection = new SeriesCollection
                {
                    new ColumnSeries
                    {
                        Title = "Orders",
                        Values = new ChartValues<int> { summary.Placed, summary.Shipped, summary.AwaitingPayment, summary.AwaitingFulfillment, summary.AwaitingShipment }
                    }
                };

                PieSeriesCollection = new SeriesCollection
                {
                    new PieSeries { Title = "Placed", Values = new ChartValues<int> { summary.Placed } },
                    new PieSeries { Title = "Shipped", Values = new ChartValues<int> { summary.Shipped } },
                    new PieSeries { Title = "Awaiting Payment", Values = new ChartValues<int> { summary.AwaitingPayment } },
                    new PieSeries { Title = "Awaiting Fulfillment", Values = new ChartValues<int> { summary.AwaitingFulfillment } },
                    new PieSeries { Title = "Awaiting Shipment", Values = new ChartValues<int> { summary.AwaitingShipment } }
                };

                SnackbarMessage = "Data Refreshed!";
                IsSnackbarActive = true;
                _snackbarTimer.Start();

                OnPropertyChanged(nameof(SeriesCollection));
                OnPropertyChanged(nameof(PieSeriesCollection));
                OnPropertyChanged(nameof(Labels));
                OnPropertyChanged(nameof(TotalOrders));
                OnPropertyChanged(nameof(TotalShipped));
                OnPropertyChanged(nameof(TotalAwaitingPayment));
                OnPropertyChanged(nameof(TotalAwaitingFulfillment));
                OnPropertyChanged(nameof(TotalAwaitingShipment));
                OnPropertyChanged(nameof(TotalSalesAmount));
                OnPropertyChanged(nameof(DateRangeText));
                OnPropertyChanged(nameof(SnackbarMessage));
                OnPropertyChanged(nameof(IsSnackbarActive));
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error loading sales dashboard: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
                OnPropertyChanged(nameof(IsLoading));
            }
        }

        private void ToggleChart()
        {
            ShowPieChart = !ShowPieChart;
            OnPropertyChanged(nameof(ShowPieChart));
        }

        private void ToggleTheme()
        {
            var paletteHelper = new PaletteHelper();
            ITheme theme = paletteHelper.GetTheme();

            theme.SetBaseTheme(theme.GetBaseTheme() == BaseTheme.Dark
                ? new MaterialDesignLightTheme()
                : new MaterialDesignDarkTheme());

            paletteHelper.SetTheme(theme);
        }

        private void SetLast30Days()
        {
            EndDate = DateTime.Now;
            StartDate = EndDate.AddDays(-30);
            OnPropertyChanged(nameof(StartDate));
            OnPropertyChanged(nameof(EndDate));
        }

        private void SetThisYear()
        {
            StartDate = new DateTime(DateTime.Now.Year, 1, 1);
            EndDate = DateTime.Now;
            OnPropertyChanged(nameof(StartDate));
            OnPropertyChanged(nameof(EndDate));
        }

        private void SetLastYear()
        {
            StartDate = new DateTime(DateTime.Now.Year - 1, 1, 1);
            EndDate = new DateTime(DateTime.Now.Year - 1, 12, 31);
            OnPropertyChanged(nameof(StartDate));
            OnPropertyChanged(nameof(EndDate));
        }

        private void SetThisMonth()
        {
            StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            EndDate = DateTime.Now;
            OnPropertyChanged(nameof(StartDate));
            OnPropertyChanged(nameof(EndDate));
        }

        private void SetLastMonth()
        {
            var firstDayLastMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-1);
            StartDate = firstDayLastMonth;
            EndDate = firstDayLastMonth.AddMonths(1).AddDays(-1);
            OnPropertyChanged(nameof(StartDate));
            OnPropertyChanged(nameof(EndDate));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
