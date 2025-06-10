using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CbcRoastersErp.Factories;
using CbcRoastersErp.Helpers;
using CbcRoastersErp.Models.Finance;
using CbcRoastersErp.Repositories.Finance;
using CbcRoastersErp.Services;

namespace CbcRoastersErp.ViewModels.Finance
{
    public class AddEditAccountViewModel : IAddEditViewModel, INotifyPropertyChanged
    {
        private readonly IAccountRepository _repo;

        public Account Item { get; set; }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public AddEditAccountViewModel(Account item)
        {
            _repo = new AccountRepository();
            Item = item ?? new Account { IsActive = true };

            SaveCommand = new RelayCommand(async _ => await Save());
            CancelCommand = new RelayCommand(_ => OnCloseRequested?.Invoke());
        }

        private async Task Save()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Item.AccountName))
                {
                    System.Windows.MessageBox.Show("Account name is required.", "Validation Error");
                    return;
                }

                if (Item.AccountID > 0)
                    await _repo.UpdateAsync(Item);
                else
                    await _repo.AddAsync(Item);

                OnCloseRequested?.Invoke();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Failed to save account: {ex.Message}", "Error");
            }
        }

        // Events
        public event Action OnCloseRequested;
        public event PropertyChangedEventHandler PropertyChanged;


        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
