using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using CbcRoastersErp.Models;
using CbcRoastersErp.Repositories;
using CbcRoastersErp.Services;
using System.Windows;
using System.Windows.Data;

namespace CbcRoastersErp.ViewModels
{
    public class EmployeeViewModel : INotifyPropertyChanged
    {
        private readonly UserRepository _userRepository;
        private EmployeeProfiles _selectedEmployee;
        private WorkSchedules _selectedSchedule;
        private DateTime _currentDate;


        public ICollectionView GroupedSchedules { get; set; }
        public ObservableCollection<WorkSchedules> AllSchedules { get; set; }
        public ObservableCollection<CalendarDayViewModel> CalendarDays { get; set; } = [];
        public ObservableCollection<EmployeeProfiles> Employees { get; set; }
        public ObservableCollection<WorkSchedules> Schedules { get; set; }

        public EmployeeProfiles SelectedEmployee
        {
            get => _selectedEmployee;
            set
            {
                _selectedEmployee = value;
                OnPropertyChanged(nameof(SelectedEmployee));
            }
        }

        public WorkSchedules SelectedSchedule
        {
            get => _selectedSchedule;
            set
            {
                _selectedSchedule = value;
                OnPropertyChanged(nameof(SelectedSchedule));
            }
        }
        public DateTime CurrentDate
        {
            get => _currentDate;
            set
            {
                _currentDate = value;
                OnPropertyChanged(nameof(CurrentDate));
                LoadMonthlyCalendar(_currentDate);
            }
        }

        public ICommand NavigateBackCommand { get; }
        public ICommand OpenAddEditEmployeeCommand { get; }
        public ICommand OpenAddEditScheduleCommand { get; }
        public ICommand DeleteEmployeeCommand { get; }
        public ICommand DeleteScheduleCommand { get; }
        public ICommand PreviousMonthCommand { get; }
        public ICommand NextMonthCommand { get; }

        public event Action<string> OnNavigationRequested;
        public event Action<object> OnOpenAddEditView;
        public event PropertyChangedEventHandler PropertyChanged;

        public EmployeeViewModel()
        {
            _userRepository = new UserRepository();

            NavigateBackCommand = new RelayCommand(_ => OnNavigationRequested?.Invoke("Dashboard"));
            OpenAddEditEmployeeCommand = new RelayCommand(OpenAddEditEmployee);
            OpenAddEditScheduleCommand = new RelayCommand(OpenAddEditSchedule);
            DeleteEmployeeCommand = new RelayCommand(DeleteEmployee, _ => SelectedEmployee != null);
            DeleteScheduleCommand = new RelayCommand(DeleteSchedule, _ => SelectedSchedule != null);
            PreviousMonthCommand = new RelayCommand(_ => CurrentDate = CurrentDate.AddMonths(-1));
            NextMonthCommand = new RelayCommand(_ => CurrentDate = CurrentDate.AddMonths(1));

            _currentDate = DateTime.Now;

            LoadEmployees();
            LoadSchedules();
            LoadGroupedSchedules();
            LoadMonthlyCalendar(CurrentDate);
        }

        private void LoadEmployees()
        {
            Employees = new ObservableCollection<EmployeeProfiles>(_userRepository.GetAllEmployees());
            OnPropertyChanged(nameof(Employees));
        }

        private void LoadSchedules()
        {
            var employees = _userRepository.GetAllEmployees();
            var schedules = _userRepository.GetAllSchedules();

           Schedules = new ObservableCollection<WorkSchedules>(schedules);
            OnPropertyChanged(nameof(Schedules));
        }

        private void OpenAddEditEmployee(object parameter)
        {
            var toEdit = SelectedEmployee ?? parameter as EmployeeProfiles ?? new EmployeeProfiles();

            var viewModel = new AddEditEmployeeViewModel(toEdit);
            viewModel.OnCloseRequested += () =>
            {
                LoadEmployees();
                OnNavigationRequested?.Invoke("Employee");
            };

            OnOpenAddEditView?.Invoke(viewModel);
        }

        private void OpenAddEditSchedule(object parameter)
        {
            var toEdit = SelectedSchedule ?? parameter as WorkSchedules ?? new WorkSchedules();

            var viewModel = new AddEditScheduleViewModel(toEdit);
            viewModel.OnCloseRequested += () =>
            {
                LoadSchedules();
                OnNavigationRequested?.Invoke("Employee");
            };

            OnOpenAddEditView?.Invoke(viewModel);
        }

        private void DeleteEmployee(object parameter)
        {
            if (SelectedEmployee == null) return;

            var result = MessageBox.Show(
                $"Are you sure you want to delete employee: {SelectedEmployee.FullName}?",
                "Confirm Deletion",
                MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                _userRepository.DeleteEmployee(SelectedEmployee.EmployeeID);
                LoadEmployees();
            }

        }

        private void DeleteSchedule(object parameter)
        {
            if (SelectedSchedule == null) return;

            var result = MessageBox.Show(
                $"Are you sure you want to delete schedule for: {SelectedSchedule.EmployeeFullName}?",
                "Confirm Deletion",
                MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                _userRepository.DeleteSchedule(SelectedSchedule.ScheduleID);
                LoadSchedules();
            }
        }

        // Grouped Schedules by Employee
        private void LoadGroupedSchedules()
        {
            var schedules = _userRepository.GetAllSchedulesWithEmployeeNames();

            // Make sure each StartDate has no null
            foreach (var schedule in schedules)
            {
                if (schedule.StartDate == default)
                    schedule.StartDate = DateTime.Today;
            }

            var grouped = CollectionViewSource.GetDefaultView(schedules);
            grouped.GroupDescriptions.Add(new PropertyGroupDescription("StartDate"));

            GroupedSchedules = grouped;
            OnPropertyChanged(nameof(GroupedSchedules));
        }

        public void LoadMonthlyCalendar(DateTime forMonth)
        {
            var allSchedules = _userRepository.GetAllSchedulesWithEmployeeNames();
            var daysInMonth = DateTime.DaysInMonth(forMonth.Year, forMonth.Month);
            var calendarDays = new ObservableCollection<CalendarDayViewModel>();

            for (int day = 1; day <= daysInMonth; day++)
            {
                var date = new DateTime(forMonth.Year, forMonth.Month, day);
                var schedulesForDay = allSchedules
                    .Where(s => s.StartDate?.Date == date.Date)
                    .ToList();

                calendarDays.Add(new CalendarDayViewModel
                {
                    Date = date,
                    Schedules = schedulesForDay
                });
            }

            CalendarDays = calendarDays;
            OnPropertyChanged(nameof(CalendarDays));
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
