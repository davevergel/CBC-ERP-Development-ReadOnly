using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CbcRoastersErp.Models;
using CbcRoastersErp.Repositories;
using CbcRoastersErp.Services;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Windows;

namespace CbcRoastersErp.ViewModels
{
    public class AddEditScheduleViewModel : INotifyPropertyChanged
    {
        private readonly UserRepository _repo;
        public WorkSchedules Schedule { get; set; }
        private readonly bool _isEditMode;

        public ObservableCollection<EmployeeProfiles> Employees { get; set; }
        public EmployeeProfiles SelectedEmployee { get; set; }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public event Action OnCloseRequested;
        public event PropertyChangedEventHandler PropertyChanged;

        public AddEditScheduleViewModel(WorkSchedules schedule = null)
        {
            _repo = new UserRepository();
            _isEditMode = schedule != null && schedule.ScheduleID > 0;
            Schedule = schedule ?? new WorkSchedules();

            // Load available employees for selection
            Employees = new ObservableCollection<EmployeeProfiles>(_repo.GetAllEmployees());

            // Set selected employee if editing
            if (_isEditMode)
            {
                SelectedEmployee = Employees.FirstOrDefault(e => e.EmployeeID == Schedule.EmployeeID);
            }

            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(_ => OnCloseRequested?.Invoke());
        }

        private void Save(object parameter)
        {
            //check if user is selected
            if (SelectedEmployee != null)
            {
                Schedule.EmployeeID = SelectedEmployee.EmployeeID;
            }
            else
            {
                MessageBox.Show("Please select an employee");
                return;
            }
            if (_isEditMode)
                _repo.UpdateSchedule(Schedule);
            else
                _repo.AddSchedule(Schedule);

            OnCloseRequested?.Invoke();
        }

        protected virtual void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

}
