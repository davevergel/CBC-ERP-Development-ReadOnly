using CbcRoastersErp.Helpers;
using CbcRoastersErp.Models;
using CbcRoastersErp.Repositories;
using CbcRoastersErp.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace CbcRoastersErp.ViewModels
{
    public class ApplicationLogViewModel : INotifyPropertyChanged
    {
        private readonly ApplicationLogRepository _repository = new();
        public ObservableCollection<ApplicationErrorLogs> Logs { get; } = new();
        public ICommand SearchCommand => new RelayCommand(_ => LoadLogs());

        public ICommand PageChangedCommand => new RelayCommand(param =>
        {
            if (param?.ToString() == "Next" && CurrentPage < TotalPages)
                CurrentPage++;
            else if (param?.ToString() == "Previous" && CurrentPage > 1)
                CurrentPage--;

            LoadLogs();
        });
        public ICommand NavigateBackCommand { get; }

        private int _currentPage = 1;
        private int _totalPages;
        private string _searchText = "";

        public string SearchText
        {
            get => _searchText;
            set { _searchText = value; OnPropertyChanged(); }
        }

        public int CurrentPage
        {
            get => _currentPage;
            set { _currentPage = value; OnPropertyChanged(); }
        }

        public int TotalPages
        {
            get => _totalPages;
            set { _totalPages = value; OnPropertyChanged(); }
        }

        private const int PageSize = 20;

        public ApplicationLogViewModel()
        {
            LoadLogs();
            NavigateBackCommand = new RelayCommand(_ => OnNavigationRequested?.Invoke("Dashboard"));
        }

        private void LoadLogs()
        {
            try
            {
                int totalCount;
                var logs = _repository.GetLogs(SearchText, CurrentPage, PageSize, out totalCount);

                Logs.Clear();
                foreach (var log in logs)
                    Logs.Add(log);

                TotalPages = (totalCount + PageSize - 1) / PageSize;
            }
            catch (Exception ex)
            {
                ApplicationLogger.Log(ex, "System", "Error");
                // Handle error (e.g., show message to user)
            }
            finally
            {
                // Ensure UI is updated even if an error occurs
                OnPropertyChanged(nameof(Logs));
                OnPropertyChanged(nameof(CurrentPage));
                OnPropertyChanged(nameof(TotalPages));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
       
        public event Action<string> OnNavigationRequested;
    }
}

