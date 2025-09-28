using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CbcRoastersErp.Models.Finance;
using CbcRoastersErp.Repositories.Finance;
using CbcRoastersErp.Services;

namespace CbcRoastersErp.ViewModels.Finance
{
    public class AccountViewModel : INotifyPropertyChanged
    {
        private readonly IAccountRepository _repo;
        public ObservableCollection<Account> Accounts { get; set; }
        public Account SelectedAccount { get; set; }

        public ICommand LoadCommand { get; }
        public ICommand OpenAddEditAccountCommand { get; set; }
        public ICommand DeleteCommand { get; set;  }
        public ICommand NavigateBackCommand { get; set; }

        public AccountViewModel()
        {
            _repo = new AccountRepository();
            LoadCommand = new RelayCommand(async _ => await LoadAccounts());
            OpenAddEditAccountCommand = new RelayCommand(AddEditAccount);
            NavigateBackCommand = new RelayCommand(_ => OnNavigationRequested?.Invoke("Dashboard"));
            DeleteCommand = new RelayCommand(async _ => await _repo.DeleteAsync(SelectedAccount.AccountID));
        }

        private async Task AddEditAccount()
        {
            Account accountToEdit;
            if (SelectedAccount != null)
            {
                accountToEdit = await _repo.GetByIdAsync(SelectedAccount.AccountID);
            }
            else
            {
                accountToEdit = new Account();
            }

            var addEditViewModel = new AddEditAccountViewModel(accountToEdit);
            addEditViewModel.OnCloseRequested += async () =>
            {
                await LoadAccounts();
                OnOpenAddEditView?.Invoke(null);
            };

            OnOpenAddEditView?.Invoke(addEditViewModel); // ✅ FIXED
        }

        public async Task LoadAccounts()
        {
            var list = await _repo.GetAllAsync();
            Accounts = new ObservableCollection<Account>(list);
            OnPropertyChanged(nameof(Accounts));
        }

        // Events
        public Action<string> OnNavigationRequested { get; internal set; }
        public Action<object> OnOpenAddEditView { get; set; }
        public event Action<string> OnDeleteRequested;
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
