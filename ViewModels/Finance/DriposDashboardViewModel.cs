using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using CbcRoastersErp.Models.Finance;
using CbcRoastersErp.Repositories.Finance;
using CbcRoastersErp.Services;

namespace CbcRoastersErp.ViewModels.Finance
{
    public class DriposDashboardViewModel : INotifyPropertyChanged
    {
        private readonly DriposSalesMetricsRepository _metricsRepo = new();

        public SeriesCollection SeriesCollection { get; set; } = new SeriesCollection();
        public SeriesCollection PieSeriesCollection { get; set; } = new SeriesCollection();
        public string[] Labels { get; set; } = Array.Empty<string>();

        public Func<double, string> YFormatter { get; set; } = val => val.ToString("C");

        public DateTime StartDate { get; set; } = DateTime.Today.AddDays(-30);
        public DateTime EndDate { get; set; } = DateTime.Today;

        public int SelectedTabIndex { get; set; } = 0;
        private bool _showPieChart;
        public bool ShowPieChart
        {
            get => _showPieChart;
            set
            {
                if (_showPieChart != value)
                {
                    _showPieChart = value;
                    OnPropertyChanged();
                }
            }
        }

        public decimal CurrentTotal { get; set; }
        public decimal PreviousTotal { get; set; }
        public decimal GrowthPercent { get; set; }

        public ICommand RefreshCommand => new RelayCommand(async _ => await LoadDataAsync());
        public ICommand NavigateBackCommand => new RelayCommand(_ => OnNavigationRequested?.Invoke("Dashboard"));

        public event PropertyChangedEventHandler PropertyChanged;
        public event Action<string> OnNavigationRequested;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public async Task LoadDataAsync()
        {
            var all = (await _metricsRepo.GetByDateRangeAsync(StartDate.AddDays(-30), EndDate)).ToList();
            all = SelectedTabIndex switch
            {
                1 => all.Where(x => x.Source == "Dripos").ToList(),
                2 => all.Where(x => x.Source == "BigCommerce").ToList(),
                _ => all
            };

            var previous = all.Where(x => x.MetricDate < StartDate).ToList();
            var current = all.Where(x => x.MetricDate >= StartDate && x.MetricDate <= EndDate).ToList();

            var categories = all.Select(x => x.MetricName).Distinct().ToArray();
            Labels = categories;

            var currentSums = categories.Select(cat => current.Where(x => x.MetricName == cat).Sum(x => x.Amount)).ToList();
            var previousSums = categories.Select(cat => previous.Where(x => x.MetricName == cat).Sum(x => x.Amount)).ToList();

            SeriesCollection = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Current",
                    Values = new ChartValues<decimal>(currentSums ?? new List<decimal>())
                },
                new ColumnSeries
                {
                    Title = "Previous",
                    Values = new ChartValues<decimal>(previousSums ?? new List<decimal>())
                }
            };

            PieSeriesCollection = new SeriesCollection();

            if (categories != null && categories.Length > 0)
            {
                for (int i = 0; i < categories.Length; i++)
                {
                    var value = (i < currentSums.Count && currentSums[i] != null) ? currentSums[i] : 0;
                    PieSeriesCollection.Add(new PieSeries
                    {
                        Title = categories[i] ?? $"Metric {i}",
                        Values = new ChartValues<decimal> { value },
                        DataLabels = true
                    });
                }
            }
            else
            {
                PieSeriesCollection.Add(new PieSeries
                {
                    Title = "No Data",
                    Values = new ChartValues<decimal> { 0 },
                    DataLabels = true
                });
            }

            CurrentTotal = current.Sum(x => x.Amount);
            PreviousTotal = previous.Sum(x => x.Amount);
            GrowthPercent = PreviousTotal == 0 ? 0 : ((CurrentTotal - PreviousTotal) / PreviousTotal) * 100;

            OnPropertyChanged(nameof(SeriesCollection));
            OnPropertyChanged(nameof(PieSeriesCollection));
            OnPropertyChanged(nameof(Labels));
            OnPropertyChanged(nameof(CurrentTotal));
            OnPropertyChanged(nameof(PreviousTotal));
            OnPropertyChanged(nameof(GrowthPercent));
        }
    }
}
