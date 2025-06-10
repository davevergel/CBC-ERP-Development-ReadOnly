using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using CbcRoastersErp.Helpers;
using CbcRoastersErp.Services;

namespace CbcRoastersErp.ViewModels.Finance
{
    public class ProfitAndLossViewModel : INotifyPropertyChanged
    {
        public DateTime StartDate { get; set; } = new DateTime(DateTime.Today.Year, 1, 1);
        public DateTime EndDate { get; set; } = DateTime.Today;

        public decimal TotalRevenue { get; set; }
        public decimal TotalCOGS { get; set; }
        public decimal TotalOperatingExpenses { get; set; }
        public decimal NetIncome => TotalRevenue - TotalCOGS - TotalOperatingExpenses;

        public ICommand GenerateCommand { get; }
        public ICommand NavigateBackCommand { get; }

        public ProfitAndLossViewModel()
        {
            GenerateCommand = new RelayCommand(async _ => await GenerateReport());
            NavigateBackCommand = new RelayCommand(_ => OnNavigationRequested?.Invoke("Dashboard"));
        }

        private async Task GenerateReport()
        {
            // TODO: Replace with real queries from your finance journal
            await Task.Delay(500); // Simulated load

            TotalRevenue = 125000.00m;
            TotalCOGS = 43000.00m;
            TotalOperatingExpenses = 28000.00m;

            OnPropertyChanged(nameof(TotalRevenue));
            OnPropertyChanged(nameof(TotalCOGS));
            OnPropertyChanged(nameof(TotalOperatingExpenses));
            OnPropertyChanged(nameof(NetIncome));
        }

        // Events
        public Action<string> OnNavigationRequested { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
