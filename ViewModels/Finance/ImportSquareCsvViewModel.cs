
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CbcRoastersErp.Helpers;
using CbcRoastersErp.Repositories.Finance;
using CbcRoastersErp.Services.Finance;
using CbcRoastersErp.Services;

namespace CbcRoastersErp.ViewModels.Finance
{
    public class ImportSquareCsvViewModel : INotifyPropertyChanged
    {
        private readonly SquareImportService _importService;
        private string _csvFilePath;
        private SquareCsvRow _lastDeleted;

        public ObservableCollection<SquareCsvRow> PreviewEntries { get; set; } = new();
        public ObservableCollection<SquareCsvRow> FilteredEntries { get; set; } = new();

        public ImportSquareCsvViewModel()
        {
            var accountRepo = new AccountRepository();
            var journalRepo = new JournalEntryRepository();
            _importService = new SquareImportService(accountRepo, journalRepo);

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
        public SquareCsvRow SelectedEntry { get; set; }

        private async Task LoadCsvPreview()
        {
            try
            {
                var dialog = new OpenFileDialog
                {
                    Filter = "CSV files (*.csv)|*.csv",
                    Title = "Select Square Banking CSV"
                };

                if (dialog.ShowDialog() == true)
                {
                    _csvFilePath = dialog.FileName;
                    var lines = File.ReadAllLines(_csvFilePath).Skip(1);
                    PreviewEntries.Clear();

                    foreach (var line in lines)
                    {
                        var cols = line.Split(',');
                        if (cols.Length < 8 || !cols[6].Equals("Business", StringComparison.OrdinalIgnoreCase))
                            continue;

                        if (DateTime.TryParse(cols[0], out var date))
                        {
                            PreviewEntries.Add(new SquareCsvRow
                            {
                                Date = date,
                                Details = cols[1],
                                Amount = ParseAmount(cols[2]),
                                Category = cols[7]
                            });
                        }
                    }

                    ApplyFilter();
                }
            }
            catch (Exception ex)
            {
                ApplicationLogger.Log(ex, "System", "Error");
                MessageBox.Show("Failed to load the CSV. See logs for details.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyFilter()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(FilterText))
                {
                    FilteredEntries = new ObservableCollection<SquareCsvRow>(PreviewEntries);
                }
                else
                {
                    FilteredEntries = new ObservableCollection<SquareCsvRow>(
                        PreviewEntries.Where(e => e.Details.Contains(FilterText, StringComparison.OrdinalIgnoreCase)));
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

        private async Task ConfirmAndImport()
        {
            if (string.IsNullOrEmpty(_csvFilePath) || FilteredEntries.Count == 0)
            {
                MessageBox.Show("No file selected or nothing to import.");
                return;
            }

            var result = MessageBox.Show("Are you sure you want to import these transactions?",
                                         "Confirm Import", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var count = await _importService.ImportFromCsvAsync(_csvFilePath);
                    MessageBox.Show($"Successfully imported {count} transactions.", "Import Complete", MessageBoxButton.OK, MessageBoxImage.Information);
                    OnNavigationRequested?.Invoke("Dashboard");
                }
                catch (Exception ex)
                {
                    ApplicationLogger.Log(ex, "System", "Error");
                    MessageBox.Show("Import failed. See logs for more information.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private decimal ParseAmount(string raw)
        {
            raw = raw.Replace("(", "-").Replace(")", "").Replace("$", "").Trim();
            return decimal.Parse(raw, CultureInfo.InvariantCulture);
        }

        // Support Model
        public class SquareCsvRow
        {
            public DateTime Date { get; set; }
            public string Details { get; set; }
            public decimal Amount { get; set; }
            public string Category { get; set; }
        }

        public Action<string> OnNavigationRequested { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
