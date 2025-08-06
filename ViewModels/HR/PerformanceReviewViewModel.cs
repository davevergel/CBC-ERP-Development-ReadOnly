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
    public class PerformanceReviewViewModel : INotifyPropertyChanged
    {
        private readonly PerformanceReviewRepository _reviewRepository;

        public ObservableCollection<PerformanceReview> Reviews { get; set; } = new();
        public PerformanceReview SelectedReview { get; set; }

        public ICommand LoadCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand UpdateCommand { get; }
        public ICommand DeleteCommand { get; }
        public Action<string> OnNavigationRequested { get; internal set; }
        public event Action<object> OnOpenAddEditView;
        public event PropertyChangedEventHandler PropertyChanged;

        public PerformanceReviewViewModel()
        {
            _reviewRepository = new PerformanceReviewRepository();
            LoadCommand = new RelayCommand(async _ => await LoadReviews(SelectedReview?.EmployeeID ?? 0));
            AddCommand = new RelayCommand(async _ => await _reviewRepository.AddAsync(SelectedReview));
            UpdateCommand = new RelayCommand(async _ => await _reviewRepository.UpdateAsync(SelectedReview));
            DeleteCommand = new RelayCommand(async _ => await _reviewRepository.DeleteAsync(SelectedReview.ReviewID));
        }

        public async Task LoadReviews(int employeeId)
        {
            Reviews.Clear();
            var reviews = await _reviewRepository.GetByEmployeeIdAsync(employeeId);
            foreach (var review in reviews)
                Reviews.Add(review);
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}