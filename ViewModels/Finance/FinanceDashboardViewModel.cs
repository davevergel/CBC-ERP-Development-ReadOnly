using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using CbcRoastersErp.Factories;
using CbcRoastersErp.Repositories.Finance;
using CbcRoastersErp.Services;
using LiveCharts;
using LiveCharts.Wpf;
using System.Collections.ObjectModel;

namespace CbcRoastersErp.ViewModels.Finance
{
    public class FinanceDashboardViewModel : INotifyPropertyChanged
    {
        private decimal _revenue;
        private decimal _cogs;
        private decimal _expenses;
        private decimal _openPoLiabilities;

        public SeriesCollection IncomeSeries { get; set; } = new();
        public string[] MonthLabels { get; set; } = new string[0];
        public Func<double, string> YFormatter => value => value.ToString("C");

        public decimal Revenue
        {
            get => _revenue;
            set { _revenue = value; OnPropertyChanged(); OnPropertyChanged(nameof(NetIncome)); }
        }

        public decimal COGS
        {
            get => _cogs;
            set { _cogs = value; OnPropertyChanged(); OnPropertyChanged(nameof(NetIncome)); }
        }

        public decimal OperatingExpenses
        {
            get => _expenses;
            set { _expenses = value; OnPropertyChanged(); OnPropertyChanged(nameof(NetIncome)); }
        }

        public decimal OpenPoLiabilities
        {
            get => _openPoLiabilities;
            set { _openPoLiabilities = value; OnPropertyChanged(); }
        }

        public decimal NetIncome => Revenue - COGS - OperatingExpenses;

        public ICommand NavigateBackCommand { get; }

        public FinanceDashboardViewModel()
        {
            _ = LoadDataAsync();
            _ = LoadChartDataAsync();

            NavigateBackCommand = new RelayCommand(_ => OnNavigationRequested?.Invoke("Dashboard"));
        }

        public async Task LoadDataAsync()
        {
            var financeRepo = new FinanceReportingRepository();

            DateTime start = new DateTime(DateTime.Now.Year, 1, 1);
            DateTime end = DateTime.Today;

            Revenue = await financeRepo.GetTotalAsync("Revenue", start, end);
            COGS = await financeRepo.GetTotalAsync("Expense", start, end);
            OperatingExpenses = await financeRepo.GetTotalAsync("Operating", start, end);
            OpenPoLiabilities = await financeRepo.GetTotalOpenPOLiabilitiesAsync();
        }

        public async Task LoadChartDataAsync()
        {
            var repo = new FinanceReportingRepository();
            var labels = new List<string>();
            var incomeValues = new ChartValues<decimal>();

            for (int month = 1; month <= 12; month++)
            {
                var start = new DateTime(DateTime.Now.Year, month, 1);
                var end = start.AddMonths(1).AddDays(-1);

                decimal revenue = await repo.GetTotalAsync("Revenue", start, end);
                decimal cogs = await repo.GetTotalAsync("Expense", start, end);
                decimal opx = await repo.GetTotalAsync("Operating", start, end);
                decimal netIncome = revenue - cogs - opx;

                incomeValues.Add(netIncome);
                labels.Add(start.ToString("MMM"));
            }

            MonthLabels = labels.ToArray();

            IncomeSeries.Clear();
            IncomeSeries.Add(new ColumnSeries
            {
                Title = "Net Income",
                Values = incomeValues,
                Fill = System.Windows.Media.Brushes.SteelBlue
            });

            OnPropertyChanged(nameof(MonthLabels));
            OnPropertyChanged(nameof(IncomeSeries));
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public Action<string> OnNavigationRequested { get; set; }

        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
