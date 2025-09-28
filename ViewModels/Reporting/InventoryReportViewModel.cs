using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using CbcRoastersErp.Reports;
using CbcRoastersErp.Repositories;
using CbcRoastersErp.Services;
using CbcRoastersErp.Services.Reporting;

namespace CbcRoastersErp.ViewModels.Reporting
{
    public class InventoryReportViewModel : INotifyPropertyChanged
    {
        private readonly InventoryReportRepository _repository;

        public ObservableCollection<string> AvailableReports { get; set; } = new()
        {
            "Green Coffee"
        };

        private string _selectedReport;
        public string SelectedReport
        {
            get => _selectedReport;
            set { _selectedReport = value; OnPropertyChanged(); }
        }

        private string _reportPath;
        public string ReportPath
        {
            get => _reportPath;
            set { _reportPath = value; OnPropertyChanged(); }
        }

        public ICommand LoadReportCommand { get; }
        public ICommand NavigateBackCommand { get; }

        public event Action<string> OnNavigationRequested;
        public event PropertyChangedEventHandler PropertyChanged;

        public InventoryReportViewModel()
        {
            _repository = new InventoryReportRepository();
            LoadReportCommand = new RelayCommand(async _ => await LoadReportAsync());
            NavigateBackCommand = new RelayCommand(_ => OnNavigationRequested?.Invoke("Dashboard"));
        }

        private async Task LoadReportAsync()
        {
            if (SelectedReport == "Green Coffee")
            {
                var data = await _repository.GetGreenCoffeeReportAsync();
                string pdfPath = GreenCoffeeInventoryReportPdfService.Generate(data.ToList());
                if (File.Exists(pdfPath))
                    ReportPath = pdfPath;
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}