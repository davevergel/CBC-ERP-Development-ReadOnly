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
    public class AddEditUserViewModel : INotifyPropertyChanged
    {
        private readonly UserRepository _repo = new();
        public UserAccount User { get; set; }
        public string PlainPassword { get; set; }
        public List<Role> Roles { get; set; }
        private readonly bool _isEditMode;
        public ObservableCollection<EmployeeProfiles> Employees { get; set; }
        public EmployeeProfiles SelectedEmployee { get; set; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public event Action OnCloseRequested;
        public event PropertyChangedEventHandler PropertyChanged;

        public AddEditUserViewModel(UserAccount user = null)
        {
            _isEditMode = user != null && user.UserId > 0;
            User = user ?? new UserAccount();
            Roles = (List<Role>?)_repo.GetAllRoles();

            // Load available employees for selection
            Employees = new ObservableCollection<EmployeeProfiles>(_repo.GetAllEmployees());

            // Set selected employee if editing
            if (_isEditMode)
            {
                SelectedEmployee = Employees.FirstOrDefault(e => e.EmployeeID == User.EmployeeId);
            }

            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(_ => OnCloseRequested?.Invoke());
        }

        private void Save(object _)
        {
            //check if user is selected
            if (SelectedEmployee != null)
            {
                User.EmployeeId = SelectedEmployee.EmployeeID;
            }
            else 
            {
                MessageBox.Show("Please select an employee");
                return;
            }

            if (!_isEditMode && string.IsNullOrWhiteSpace(PlainPassword))
            {
                MessageBox.Show("Password is required for new users.");
                return;
            }

            if (_isEditMode)
                _repo.UpdateUser(User);
            else
                _repo.AddUser(User, PlainPassword);

            OnCloseRequested?.Invoke();
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
