using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using CbcRoastersErp.Models;
using CbcRoastersErp.Repositories;
using CbcRoastersErp.Services;

namespace CbcRoastersErp.ViewModels
{
    public class BatchScheduleViewModel : INotifyPropertyChanged
    {
        private readonly BatchScheduleRepository _repository = new();
        private readonly ProductionRepository _productionRepository = new();
        private ObservableCollection<BatchSchedule> _schedules;
        private BatchSchedule _selectedSchedule;

        public ICommand NavigateBackCommand { get; }
        public ICommand OpenAddEditScheduleCommand { get; }
        public ICommand AssignToBatchCommand { get; }
        public ICommand DeleteScheduleCommand { get; }

        public ObservableCollection<BatchSchedule> Schedules
        {
            get => _schedules;
            set { _schedules = value; OnPropertyChanged(); }
        }

        public BatchSchedule SelectedSchedule
        {
            get => _selectedSchedule;
            set { _selectedSchedule = value; OnPropertyChanged(); }
        }

        public BatchScheduleViewModel()
        {          

            OpenAddEditScheduleCommand = new RelayCommand(_ => OpenAddEditBatchSchedule());
            DeleteScheduleCommand = new RelayCommand(DeleteSelectedSchedule, CanExecuteDeleteSchedule);
            AssignToBatchCommand = new RelayCommand(_ => AssignSelectedToBatch(), _ => SelectedSchedule != null);
            NavigateBackCommand = new RelayCommand(_ => OnNavigationRequested?.Invoke("Dashboard"));

            LoadSchedules();
        }

        private bool CanExecuteDeleteSchedule(object arg)
        {
            return SelectedSchedule != null;
        }

        public void LoadSchedules()
        {
            var schedules = _repository.GetAllSchedules();
            Schedules = new ObservableCollection<BatchSchedule>(schedules);
        }

        private void DeleteSelectedSchedule(object parameter)
        {
            if (SelectedSchedule == null) return;

            var result = MessageBox.Show("Are you sure you want to delete this schedule?", "Confirm Delete", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                _repository.DeleteSchedule(SelectedSchedule.ScheduleID);
                Schedules.Remove(SelectedSchedule);
                LoadSchedules();
            }
        }

        private void OpenAddEditBatchSchedule()
        {
            BatchSchedule scheduleToEdit;

            if (SelectedSchedule != null)
            {
                var fullSchedule = _repository.GetScheduleById(SelectedSchedule.ScheduleID);
                scheduleToEdit = fullSchedule ?? new BatchSchedule();
            }
            else
            {
                scheduleToEdit = new BatchSchedule(); // New record
            }

            var addEditViewModel = new AddEditBatchScheduleViewModel(scheduleToEdit);
            addEditViewModel.OnCloseRequested += () =>
            {
                LoadSchedules(); // refresh list
                OnNavigationRequested?.Invoke("BatchSchedule");
            };

            OnOpenAddEditView?.Invoke(addEditViewModel); // <- this must be properly wired in MainViewModel
        }

        private void AssignSelectedToBatch()
        {
            if (SelectedSchedule == null) return;

            var batch = new BatchRoasting
            {
                FinishedGoodID = SelectedSchedule.FinishedGoodID,
                RoastDate = DateTime.Now,
                BatchSize = SelectedSchedule.Quantity
            };

            _productionRepository.AddRoastBatch(batch);

            MessageBox.Show("Batch roast created from schedule.");
            _repository.DeleteSchedule(SelectedSchedule.ScheduleID);
            Schedules.Remove(SelectedSchedule);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public event Action<string> OnNavigationRequested;
        public event Action<object> OnOpenAddEditView;
        public event Action OnCloseRequested;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}

