using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using CbcRoastersErp.Factories;
using CbcRoastersErp.Services;

namespace CbcRoastersErp.ViewModels.Finance
{
    public class FinanceDashboardViewModel : INotifyPropertyChanged
    {
        private decimal _revenue;
        private decimal _cogs;
        private decimal _expenses;

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

        public decimal NetIncome => Revenue - COGS - OperatingExpenses;

        public ICommand NavigateBackCommand { get; }

        public FinanceDashboardViewModel()
        {
            NavigateBackCommand = new RelayCommand(_ => OnNavigationRequested?.Invoke("Dashboard"));
        }

        public async Task LoadDataAsync()
        {
            // Replace with real queries
            await Task.Delay(300);
            Revenue = 120000;
            COGS = 40000;
            OperatingExpenses = 25000;
        }

        // Events
        public event PropertyChangedEventHandler PropertyChanged;
        public Action<string> OnNavigationRequested { get; set; }
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
