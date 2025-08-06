using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using CbcRoastersErp.Models.HR;
using CbcRoastersErp.Repositories.HR;
using CbcRoastersErp.Services;

namespace CbcRoastersErp.ViewModels.HR
{
    public class InterviewViewModel : INotifyPropertyChanged
    {
        private readonly InterviewRepository _repo;

        public ObservableCollection<Interview> Interviews { get; set; } = new();
        public Interview SelectedInterview { get; set; }

        public ICommand LoadCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand UpdateCommand { get; }
        public ICommand DeleteCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;
        public Action<string> OnNavigationRequested { get; internal set; }
        public event Action<object> OnOpenAddEditView;

        public InterviewViewModel()
        {
            _repo = new InterviewRepository();
            LoadCommand = new RelayCommand(async _ => await LoadInterviews(SelectedInterview?.CandidateID ?? 0));
            AddCommand = new RelayCommand(async _ => await _repo.AddAsync(SelectedInterview));
            UpdateCommand = new RelayCommand(async _ => await _repo.UpdateAsync(SelectedInterview));
            DeleteCommand = new RelayCommand(async _ => await _repo.DeleteAsync(SelectedInterview.InterviewID));
        }

        public async Task LoadInterviews(int candidateId)
        {
            Interviews.Clear();
            var items = await _repo.GetByCandidateIdAsync(candidateId);
            foreach (var item in items)
                Interviews.Add(item);
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}