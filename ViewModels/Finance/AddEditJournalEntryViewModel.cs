using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CbcRoastersErp.Models.Finance;
using CbcRoastersErp.Repositories.Finance;
using CbcRoastersErp.Helpers;
using CbcRoastersErp.Services;
using CbcRoastersErp.Factories;

namespace CbcRoastersErp.ViewModels.Finance
{
    public class AddEditJournalEntryViewModel : IAddEditViewModel, INotifyPropertyChanged
    {
        private readonly IJournalEntryRepository _repo;

        public JournalEntry Entry { get; set; }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public event Action OnCloseRequested;

        public AddEditJournalEntryViewModel(JournalEntry entry = null)
        {
            _repo = new JournalEntryRepository();
            Entry = entry ?? new JournalEntry
            {
                EntryDate = DateTime.Today,
                Lines = new ObservableCollection<JournalEntryLine>()
            };

            SaveCommand = new RelayCommand(async _ => await Save());
            CancelCommand = new RelayCommand(_ => OnCloseRequested?.Invoke());
        }

        private async Task Save()
        {
            try
            {
                if (Entry.Lines == null || Entry.Lines.Count == 0)
                {
                    System.Windows.MessageBox.Show("At least one line is required.", "Validation Error");
                    return;
                }

                await _repo.AddAsync(Entry, Entry.Lines);
                OnCloseRequested?.Invoke();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Failed to save entry: {ex.Message}", "Save Error");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
