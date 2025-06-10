using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Microsoft.WindowsAPICodePack.Dialogs;
using CbcRoastersErp.Models.Finance;
using CbcRoastersErp.Repositories.Finance;
using CbcRoastersErp.Services.Finance;
using CbcRoastersErp.Services;

namespace CbcRoastersErp.ViewModels.Finance
{
    public class ImportDriposCombinedViewModel : INotifyPropertyChanged
    {
        private readonly DriposSalesImporterService _summaryImporter;
        private readonly DriposSalesMetricsImporterService _metricsImporter;
        public bool IsLoading { get; set; }
        private DriposSaleRow _lastDeleted;
        private string _csvFilePath;

        public ObservableCollection<DriposSaleRow> JournalPreview { get; set; } = new();
        public ObservableCollection<DriposSaleRow> FilteredJournalPreview { get; set; } = new();
        public ObservableCollection<SalesMetricRow> MetricsPreview { get; set; } = new();

        public ImportDriposCombinedViewModel()
        {
            var journalRepo = new JournalEntryRepository();
            var accountRepo = new AccountRepository();

            _summaryImporter = new DriposSalesImporterService(journalRepo, accountRepo);
            _metricsImporter = new DriposSalesMetricsImporterService();

            LoadCsvCommand = new RelayCommand(async _ => await LoadCsvFile());
            ImportCommand = new RelayCommand(async _ => await ImportJournalEntries());
            ExportJournalCommand = new RelayCommand(_ => ExportJournal());
            ExportMetricsCommand = new RelayCommand(_ => ExportMetrics());
            DeleteSelectedCommand = new RelayCommand(_ => DeleteSelected());
            UndoDeleteCommand = new RelayCommand(_ => UndoDelete());
            ApplyFilterCommand = new RelayCommand(_ => ApplyFilter());
            NavigateBackCommand = new RelayCommand(_ => OnNavigationRequested?.Invoke("Dashboard"));
            SaveJournalToDbCommand = new RelayCommand(async _ => await SaveJournalToDb());
            SaveMetricsToDbCommand = new RelayCommand(async _ => await SaveMetricsToDb());
        }

        public ICommand LoadCsvCommand { get; }
        public ICommand ImportCommand { get; }
        public ICommand ExportJournalCommand { get; }
        public ICommand ExportMetricsCommand { get; }
        public ICommand DeleteSelectedCommand { get; }
        public ICommand UndoDeleteCommand { get; }
        public ICommand ApplyFilterCommand { get; }
        public ICommand NavigateBackCommand { get; }
        public ICommand SaveJournalToDbCommand { get; }
        public ICommand SaveMetricsToDbCommand { get; }


        public DriposSaleRow SelectedJournalRow { get; set; }
        public string FilterText { get; set; }

        private async Task LoadCsvFile()
        {
            try 
            {
            IsLoading = true;
            OnPropertyChanged(nameof(IsLoading));

            var dialog = new OpenFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv",
                Title = "Select Dripos CSV"
            };

            if (dialog.ShowDialog() == true)
            {
                _csvFilePath = dialog.FileName;
                var journalRows = await _summaryImporter.PreviewSalesRowsAsync(_csvFilePath);
                var metricRows = await _metricsImporter.LoadDetailedMetricsAsync(_csvFilePath);

                JournalPreview = new ObservableCollection<DriposSaleRow>(journalRows);
                FilteredJournalPreview = new ObservableCollection<DriposSaleRow>(journalRows);
                MetricsPreview = new ObservableCollection<SalesMetricRow>(metricRows);

                OnPropertyChanged(nameof(JournalPreview));
                OnPropertyChanged(nameof(FilteredJournalPreview));
                OnPropertyChanged(nameof(MetricsPreview));
            }
                IsLoading = false;
                OnPropertyChanged(nameof(IsLoading));
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error loading file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                IsLoading = false;
            }
            finally
            {
                IsLoading = false;
                OnPropertyChanged(nameof(IsLoading));
            }
        }

        private async Task ImportJournalEntries()
        {
            if (string.IsNullOrWhiteSpace(_csvFilePath))
            {
                System.Windows.MessageBox.Show("No file loaded.");
                return;
            }

            var result = System.Windows.MessageBox.Show("Are you sure you want to import journal entries?",
                "Confirm Import", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                var count = await _summaryImporter.ImportDailySalesCsvAsync(_csvFilePath);
                System.Windows.MessageBox.Show($"Successfully imported {count} journal entries.", "Import Complete");
            }
        }

        private void ExportJournal()
        {
            var dialog = new CommonOpenFileDialog { IsFolderPicker = true };
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                var path = dialog.FileName;
                ExportService.ExportToCsv(FilteredJournalPreview, path, "Dripos_Journal_Entries");
                ExportService.ExportToExcel(FilteredJournalPreview, path, "Dripos_Journal_Entries");
                MessageBox.Show("Journal entries exported.");
            }
        }

        private void ExportMetrics()
        {
            var dialog = new CommonOpenFileDialog { IsFolderPicker = true };
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                var path = dialog.FileName;
                ExportService.ExportToCsv(MetricsPreview, path, "Dripos_Sales_Metrics");
                ExportService.ExportToExcel(MetricsPreview, path, "Dripos_Sales_Metrics");
                MessageBox.Show("Metrics exported.");
            }
        }

        private void ApplyFilter()
        {
            if (string.IsNullOrWhiteSpace(FilterText))
            {
                FilteredJournalPreview = new ObservableCollection<DriposSaleRow>(JournalPreview);
            }
            else
            {
                FilteredJournalPreview = new ObservableCollection<DriposSaleRow>(
                    JournalPreview.Where(j =>
                        j.Date.ToShortDateString().Contains(FilterText, StringComparison.OrdinalIgnoreCase) ||
                        j.Amount.ToString("C").Contains(FilterText, StringComparison.OrdinalIgnoreCase)));
            }
            OnPropertyChanged(nameof(FilteredJournalPreview));
        }

        private void DeleteSelected()
        {
            if (SelectedJournalRow != null)
            {
                _lastDeleted = SelectedJournalRow;
                JournalPreview.Remove(SelectedJournalRow);
                ApplyFilter();
            }
        }

        private void UndoDelete()
        {
            if (_lastDeleted != null)
            {
                JournalPreview.Add(_lastDeleted);
                ApplyFilter();
                _lastDeleted = null;
            }
        }

        private async Task SaveJournalToDb()
        {
            await _summaryImporter.SaveToDatabaseAsync(FilteredJournalPreview);
            System.Windows.MessageBox.Show("Journal entries saved to database.");
        }

        private async Task SaveMetricsToDb()
        {
            await _metricsImporter.SaveToDatabaseAsync(MetricsPreview);
            System.Windows.MessageBox.Show("Metrics saved to database.");
        }

        public Action<string> OnNavigationRequested { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
