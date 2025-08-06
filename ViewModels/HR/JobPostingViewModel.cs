using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using CbcoastersErp.Repositories.HR;
using CbcRoastersErp.Models.HR;
using CbcRoastersErp.Repositories.HR;
using CbcRoastersErp.Services;

namespace CbcRoastersErp.ViewModels.HR
{
    public class JobPostingViewModel : INotifyPropertyChanged
    {
        private readonly JobPostingRepository _repo;

        public ObservableCollection<JobPosting> JobPostings { get; set; } = new();
        public JobPosting SelectedPosting { get; set; }

        public ICommand LoadCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand UpdateCommand { get; }
        public ICommand DeleteCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;
        public Action<string> OnNavigationRequested { get; internal set; }
        public event Action<object> OnOpenAddEditView;

        public JobPostingViewModel()
        {
            _repo = new JobPostingRepository();
            LoadCommand = new RelayCommand(async _ => await LoadPostings());
            AddCommand = new RelayCommand(async _ => await _repo.AddAsync(SelectedPosting));
            UpdateCommand = new RelayCommand(async _ => await _repo.UpdateAsync(SelectedPosting));
            DeleteCommand = new RelayCommand(async _ => await _repo.DeleteAsync(SelectedPosting.JobID));
        }

        private async Task LoadPostings()
        {
            JobPostings.Clear();
            var items = await _repo.GetAllAsync();
            foreach (var item in items)
                JobPostings.Add(item);
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}