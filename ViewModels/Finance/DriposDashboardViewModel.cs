using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using LiveCharts;
using LiveCharts.Wpf;
using CbcRoastersErp.Repositories.Finance;
using CbcRoastersErp.Helpers;
using CbcRoastersErp.Services;

namespace CbcRoastersErp.ViewModels.Finance
{
    public class DriposDashboardViewModel : INotifyPropertyChanged
    {
        private readonly DriposSalesMetricsRepository _repo = new();

        public SeriesCollection SeriesCollection { get; set; } = new();
        public SeriesCollection PieSeriesCollection { get; set; } = new();
        public string[] Labels { get; set; } = Array.Empty<string>();

        public DateTime StartDate { get; set; } = new(DateTime.Now.Year, DateTime.Now.Month, 1);
        public DateTime EndDate { get; set; } = DateTime.Now;
        public bool ShowPieChart { get; set; }

        public ICommand RefreshCommand { get; }
        public ICommand NavigateBackCommand { get; }

        public DriposDashboardViewModel()
        {
            RefreshCommand = new RelayCommand(async _ => await LoadDataAsync());
            NavigateBackCommand = new RelayCommand(_ => OnNavigationRequested?.Invoke("Dashboard"));
            _ = LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            try
            {
                var all = (await _repo.GetByDateRangeAsync(StartDate.AddMonths(-1), EndDate)).ToList();

                var current = all.Where(m => m.MetricDate >= StartDate && m.MetricDate <= EndDate).ToList();
                var previous = all.Where(m => m.MetricDate < StartDate).ToList();

                var dates = current.Select(m => m.MetricDate.Date)
                                    .Union(previous.Select(m => m.MetricDate.Date))
                                    .Distinct()
                                    .OrderBy(d => d)
                                    .ToList();

                Labels = dates.Select(d => d.ToString("MM/dd")).ToArray();

                var currentDict = current.GroupBy(m => m.MetricDate.Date).ToDictionary(g => g.Key, g => g.Sum(x => x.Amount));
                var previousDict = previous.GroupBy(m => m.MetricDate.Date).ToDictionary(g => g.Key, g => g.Sum(x => x.Amount));

                var currentValues = new ChartValues<decimal>(dates.Select(d => currentDict.ContainsKey(d) ? currentDict[d] : 0));
                var previousValues = new ChartValues<decimal>(dates.Select(d => previousDict.ContainsKey(d) ? previousDict[d] : 0));

                SeriesCollection = new SeriesCollection
                {
                    new ColumnSeries
                    {
                        Title = "Current",
                        Values = currentValues
                    },
                    new ColumnSeries
                    {
                        Title = "Previous",
                        Values = previousValues
                    }
                };

                PieSeriesCollection = new SeriesCollection
                {
                    new PieSeries
                    {
                        Title = "Current",
                        Values = new ChartValues<decimal> { current.Sum(m => m.Amount) },
                        DataLabels = true
                    },
                    new PieSeries
                    {
                        Title = "Previous",
                        Values = new ChartValues<decimal> { previous.Sum(m => m.Amount) },
                        DataLabels = true
                    }
                };

                OnPropertyChanged(nameof(SeriesCollection));
                OnPropertyChanged(nameof(PieSeriesCollection));
                OnPropertyChanged(nameof(Labels));
            }
            catch (Exception ex)
            {
                ApplicationLogger.Log(ex, "DriposDashboardViewModel");
            }
        }

        public event Action<string>? OnNavigationRequested;
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}