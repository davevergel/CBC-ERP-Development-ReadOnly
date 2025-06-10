using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CbcRoastersErp.Models.Finance;
using CbcRoastersErp.Repositories.Finance;
using CbcRoastersErp.Helpers;
using CbcRoastersErp.Services;

namespace CbcRoastersErp.ViewModels.Finance
{
    public class JournalEntryViewModel : INotifyPropertyChanged
    {
        private readonly IJournalEntryRepository _repo;
        public ObservableCollection<JournalEntry> JournalEntries { get; set; } = new();
        public JournalEntry SelectedEntry { get; set; }

        public ICommand LoadCommand { get; }
        public ICommand AddEditJournalCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand NavigateBackCommand { get; set; }

        public JournalEntryViewModel()
        {
            _repo = new JournalEntryRepository();
            LoadCommand = new RelayCommand(async _ => await LoadEntries());
            AddEditJournalCommand = new RelayCommand(OpenAddEditJournal);
            DeleteCommand = new RelayCommand(async _ => await _repo.DeleteAsync(SelectedEntry.JournalEntryID));
            NavigateBackCommand = new RelayCommand(_ => OnNavigationRequested?.Invoke("Dashboard"));
        }

        private async Task OpenAddEditJournal()
        {
            JournalEntry journalEntryToEdit;
            if (SelectedEntry != null)
            {
                journalEntryToEdit = await _repo.GetByIdAsync(SelectedEntry.JournalEntryID);
            }
            else
            {
                journalEntryToEdit = new JournalEntry();
            }
            var addEditViewModel = new AddEditJournalEntryViewModel(journalEntryToEdit);
            addEditViewModel.OnCloseRequested += async () =>
            {
                await LoadEntries();
                OnNavigationRequested?.Invoke("Finance_Accounts");
            };
            OnOpenAddEditView?.Invoke(addEditViewModel);
        }

        public async Task LoadEntries()
        {
            var list = await _repo.GetAllAsync();
            JournalEntries = new ObservableCollection<JournalEntry>(list);
            OnPropertyChanged(nameof(JournalEntries));
        }

        // Events
        public Action<string> OnNavigationRequested { get; internal set; }
        public Action<object> OnOpenAddEditView;
        public event Action<string> OnDeleteRequested;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
