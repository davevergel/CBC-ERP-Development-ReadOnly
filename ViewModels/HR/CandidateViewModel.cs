using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using CbcRoastersErp.Helpers;
using CbcRoastersErp.Models.HR;
using CbcRoastersErp.Repositories.HR;
using CbcRoastersErp.Services;
using CbcRoastersErp.Views;
using CBCRoastersErp.Repositories.HR;
using MaterialDesignThemes.Wpf;

namespace CbcRoastersErp.ViewModels.HR
{
    public class CandidateViewModel : INotifyPropertyChanged
    {
        private readonly CandidateRepository _repo;
        private int _currentPage = 1;
        private int _totalPages;
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

        public ObservableCollection<Candidate> Candidates { get; set; }
        public Candidate SelectedCandidate { get; set; }

        public ICommand AddEditCandidateCommand  { get; }
        public ICommand NavigateBackCommand { get; }
        public ICommand DeleteCandidateCommand { get; }

    
        // Pagination
        public ICommand PageChangedCommand => new RelayCommand(param =>
        {
            if (param?.ToString() == "Next" && CurrentPage < TotalPages)
                CurrentPage++;
            else if (param?.ToString() == "Previous" && CurrentPage > 1)
                CurrentPage--;

            LoadCandidates();
        });

        // Events
        public event PropertyChangedEventHandler PropertyChanged;
        public Action<string> OnNavigationRequested { get; internal set; }
        public ObservableCollection<Candidate> Candidate { get; private set; }

        public event Action<object> OnOpenAddEditView;

        public CandidateViewModel()
        {
            _repo = new CandidateRepository();
            LoadCandidates();

            AddEditCandidateCommand = new RelayCommand(OpenAddEditCandidate);
            NavigateBackCommand = new RelayCommand(_ => OnNavigationRequested?.Invoke("Dashboard"));
            DeleteCandidateCommand = new RelayCommand(DeleteCandidateAsync, CanExecuteCandidateCommand);
        }

        private async Task OpenAddEditCandidate()
        {
            Candidate candidateToEdit;
            if (SelectedCandidate == null || SelectedCandidate.CandidateID <= 0)
            {
                candidateToEdit = new Candidate();
            }
            else
            {
                candidateToEdit = await _repo.GetByIdAsync(SelectedCandidate.CandidateID);
            }

            var addEditCandidateView = new AddEditCandidateViewModel(candidateToEdit);
            addEditCandidateView.OnCloseRequested += () =>
            {
                LoadCandidates();
                OnOpenAddEditView?.Invoke("HR_Candidate");
            };

            OnOpenAddEditView?.Invoke(addEditCandidateView);
        }

        private bool CanExecuteCandidateCommand(object arg)
        {
            return SelectedCandidate != null;
        }

        private async Task DeleteCandidateAsync()
        {
            if (SelectedCandidate == null || SelectedCandidate.CandidateID <= 0)
            {
                MessageBox.Show("Please select a valid candidate to delete.", "Error", MessageBoxButton.OK);
                return;
            }
            var dialog = new ConfirmDeleteDialog($"Are you sure you want to delete candidate '{SelectedCandidate.CandidateID}'?");
            var result = await DialogHost.Show(dialog, "RootDialog");
            if (result is bool accepted && accepted)
            {
                try
                {
                    await _repo.DeleteAsync(SelectedCandidate.CandidateID);
                    LoadCandidates();
                    MessageBox.Show("Candidate deleted successfully.", "Success", MessageBoxButton.OK);
                    LoadCandidates();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting candidate: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    ApplicationLogger.Log(ex, "Error deleting candidate");
                }
                // refresh list after deletion
                LoadCandidates();
            }


        }

        private async void LoadCandidates()
        {
            try
            {
                int totalCount = await _repo.GetTotalCountAsync();
                TotalPages = (int)Math.Ceiling((double)totalCount / 10); // Assuming 10 items per page

                var candidatesPaged = await _repo.GetByPageAsync(CurrentPage, 25);
                Candidates = new ObservableCollection<Candidate>(candidatesPaged);
                OnPropertyChanged(nameof(Candidates));


            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening add/edit candidate view: {ex.Message}", "Error", MessageBoxButton.OK);
                ApplicationLogger.Log(ex, "Error opening add/edit candidate view");
            }
            finally
            {
                OnPropertyChanged(nameof(Candidates));
                OnPropertyChanged(nameof(TotalPages));
                OnPropertyChanged(nameof(CurrentPage));
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}