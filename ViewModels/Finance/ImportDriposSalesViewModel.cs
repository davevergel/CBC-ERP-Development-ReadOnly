using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using CbcRoastersErp.Helpers;
using CbcRoastersErp.Repositories.Finance;
using CbcRoastersErp.Services.Finance;
using CbcRoastersErp.Services;

namespace CbcRoastersErp.ViewModels.Finance
{
    public class ImportDriposSalesViewModel : INotifyPropertyChanged
    {
        private readonly DriposSalesImporterService _importService;
        private string _csvFilePath;
        private DriposSaleRow _lastDeleted;
        public bool IsLoading { get; set; }

        public ObservableCollection<DriposSaleRow> PreviewEntries { get; set; } = new();
        public ObservableCollection<DriposSaleRow> FilteredEntries { get; set; } = new();

        public ImportDriposSalesViewModel()
        {
            var journalRepo = new JournalEntryRepository();
            var accountRepo = new AccountRepository();
            _importService = new DriposSalesImporterService(journalRepo, accountRepo);

            LoadPreviewCommand = new RelayCommand(async _ => await LoadCsvPreview());
            ConfirmImportCommand = new RelayCommand(async _ => await ConfirmAndImport());
            ApplyFilterCommand = new RelayCommand(_ => ApplyFilter());
            DeleteSelectedCommand = new RelayCommand(_ => DeleteSelected());
            UndoDeleteCommand = new RelayCommand(_ => UndoDelete());
            NavigateBackCommand = new RelayCommand(_ => OnNavigationRequested?.Invoke("Dashboard"));
        }

        public ICommand LoadPreviewCommand { get; }
        public ICommand ConfirmImportCommand { get; }
        public ICommand ApplyFilterCommand { get; }
        public ICommand DeleteSelectedCommand { get; }
        public ICommand UndoDeleteCommand { get; }
        public ICommand NavigateBackCommand { get; }

        public string FilterText { get; set; }
        public DriposSaleRow SelectedEntry { get; set; }

        private async Task LoadCsvPreview()
        {
            try
            {
                IsLoading = true;
                OnPropertyChanged(nameof(IsLoading));

                var dialog = new OpenFileDialog
                {
                    Filter = "CSV files (*.csv)|*.csv",
                    Title = "Select Dripos Daily Sales CSV"
                };

                if (dialog.ShowDialog() == true)
                {
                    _csvFilePath = dialog.FileName;
                    var lines = File.ReadAllLines(_csvFilePath);
                    if (lines.Length < 6) return;

                    // Parse header year from line 1, column 1
                    var headerRange = lines[1].Split(',').ElementAtOrDefault(1)?.Trim();
                    var headerYear = DateTime.Today.Year;
                    if (headerRange != null && headerRange.Contains("-") &&
                        DateTime.TryParse(headerRange.Split('-')[0].Trim(), out var parsedStart))
                    {
                        headerYear = parsedStart.Year;
                    }

                    var dateColumns = lines[3].Split(',').Skip(1).ToArray();    // row 3 = index 3
                    var salesColumns = lines[5].Split(',').Skip(1).ToArray();  // row 5 = index 5

                    PreviewEntries.Clear();
                    for (int i = 0; i < dateColumns.Length && i < salesColumns.Length; i++)
                    {
                        var rawDate = dateColumns[i].Trim('"').Trim();
                        var rawAmount = salesColumns[i].Trim('"').Trim();

                        if (!DateTime.TryParseExact(rawDate, "M/d", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                        {
                            ApplicationLogger.LogInfo($"Invalid date format: '{rawDate}'", "DriposSalesImport");
                            continue;
                        }

                        if (!decimal.TryParse(rawAmount.Replace("$", "").Replace(",", ""), out var amount))
                        {
                            ApplicationLogger.LogInfo($"Invalid amount format: '{rawAmount}'", "DriposSalesImport");
                            continue;
                        }

                        if (amount == 0) continue;

                        PreviewEntries.Add(new DriposSaleRow
                        {
                            Date = new DateTime(headerYear, date.Month, date.Day),
                            Amount = amount
                        });
                    }

                    ApplyFilter();
                }
            }
            catch (Exception ex)
            {
                ApplicationLogger.Log(ex, "System", "Error");
                MessageBox.Show("Failed to load the CSV. See logs for details.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
                OnPropertyChanged(nameof(IsLoading));
            }
        }

        private async Task ConfirmAndImport()
        {
            if (string.IsNullOrEmpty(_csvFilePath) || FilteredEntries.Count == 0)
            {
                MessageBox.Show("No file selected or nothing to import.");
                return;
            }

            var result = MessageBox.Show("Are you sure you want to import these sales entries?",
                                         "Confirm Import", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    int count = await _importService.ImportDailySalesCsvAsync(_csvFilePath);
                    MessageBox.Show($"Successfully imported {count} sales entries.", "Import Complete", MessageBoxButton.OK, MessageBoxImage.Information);
                    OnNavigationRequested?.Invoke("Dashboard");
                }
                catch (Exception ex)
                {
                    ApplicationLogger.Log(ex, "System", "Error");
                    MessageBox.Show("Import failed. See logs for more information.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ApplyFilter()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(FilterText))
                {
                    FilteredEntries = new ObservableCollection<DriposSaleRow>(PreviewEntries);
                }
                else
                {
                    FilteredEntries = new ObservableCollection<DriposSaleRow>(
                        PreviewEntries.Where(e =>
                            e.Date.ToShortDateString().Contains(FilterText, StringComparison.OrdinalIgnoreCase) ||
                            e.Amount.ToString("C").Contains(FilterText, StringComparison.OrdinalIgnoreCase))
                    );
                }
                OnPropertyChanged(nameof(FilteredEntries));
            }
            catch (Exception ex)
            {
                ApplicationLogger.Log(ex, "System", "Error");
            }
        }

        private void DeleteSelected()
        {
            if (SelectedEntry != null)
            {
                _lastDeleted = SelectedEntry;
                PreviewEntries.Remove(SelectedEntry);
                ApplyFilter();
            }
        }

        private void UndoDelete()
        {
            if (_lastDeleted != null)
            {
                PreviewEntries.Add(_lastDeleted);
                ApplyFilter();
                _lastDeleted = null;
            }
        }

        public class DriposSaleRow
        {
            public DateTime Date { get; set; }
            public decimal Amount { get; set; }
        }

        public Action<string> OnNavigationRequested { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
