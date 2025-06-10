using CbcRoastersErp.Models;
using CbcRoastersErp.Repositories;
using CbcRoastersErp.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace CbcRoastersErp.ViewModels
{
    public class CustomerViewModel : INotifyPropertyChanged
    {
        private readonly BigCommerceRepository _repository = new();
        private ObservableCollection<Customers> _customers;
        private string _searchText;

        public ObservableCollection<Customers> Customers
        {
            get => _customers;
            set { _customers = value; OnPropertyChanged(); }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                FilterCustomers();
            }
        }

        public ICommand NavigateBackCommand { get; }
        public ICommand SearchCommand { get; }

        public CustomerViewModel()
        {
            LoadCustomers();
            SearchCommand = new RelayCommand(_ => FilterCustomers());
            NavigateBackCommand = new RelayCommand(_ => OnNavigationRequested?.Invoke("Dashboard"));
        }

        private void LoadCustomers()
        {
            var all = _repository.GetAllCustomers();
            Customers = new ObservableCollection<Customers>(all);
        }

        private void FilterCustomers()
        {
            var all = _repository.GetAllCustomers();
            if (string.IsNullOrWhiteSpace(SearchText))
                Customers = new ObservableCollection<Customers>(all);
            else
                Customers = new ObservableCollection<Customers>(
                    all.Where(c =>
                        (!string.IsNullOrWhiteSpace(c.FullName) && c.FullName.Contains(SearchText)) ||
                        (!string.IsNullOrWhiteSpace(c.Email) && c.Email.Contains(SearchText))
                    ));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public event Action<string> OnNavigationRequested;
    }
}
