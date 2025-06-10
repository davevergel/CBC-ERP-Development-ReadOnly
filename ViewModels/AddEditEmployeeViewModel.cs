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

namespace CbcRoastersErp.ViewModels
{
    public class AddEditEmployeeViewModel : INotifyPropertyChanged
    {
        private readonly UserRepository _repo;
        public EmployeeProfiles Employee { get; set; }
        private readonly bool _isEditMode;

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public event Action OnCloseRequested;
        public event PropertyChangedEventHandler PropertyChanged;

        public AddEditEmployeeViewModel(EmployeeProfiles employee = null)
        {
            _repo = new UserRepository();
            _isEditMode = employee != null && employee.EmployeeID > 0;
            Employee = employee ?? new EmployeeProfiles();

            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(_ => OnCloseRequested?.Invoke());
        }

        private void Save(object parameter)
        {
            if (_isEditMode)
                _repo.UpdateEmployee(Employee);
            else
                _repo.AddEmployee(Employee);

            OnCloseRequested?.Invoke();
        }

        protected virtual void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
