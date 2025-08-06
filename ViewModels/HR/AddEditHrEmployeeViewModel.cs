using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CbcRoastersErp.Models.HR;
using CbcRoastersErp.Helpers;
using CbcRoastersErp.Services;
using System.Windows;
using CBCRoastersERP.Repositories.HR;

namespace CbcRoastersErp.ViewModels.HR
{
    public class AddEditHrEmployeeViewModel : INotifyPropertyChanged
    {
        private readonly EmployeeRepository _employeeRepository;

        public event PropertyChangedEventHandler PropertyChanged;
        public event Action OnCloseRequested;

        private Employee _item;
        public Employee Item
        {
            get => _item;
            set { _item = value; OnPropertyChanged(); }
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public AddEditHrEmployeeViewModel(Employee item)
        {
            _employeeRepository = new EmployeeRepository();
            Item = item;
            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(_ => OnCloseRequested?.Invoke());
        }

        private async void Save(object _)
        {
            try
            {
                if (Item != null && Item.EmployeeID > 0)
                {
                    // Update existing employee
                    await _employeeRepository.UpdateAsync(Item);
                }
                else
                {
                    // Add new employee
                    await _employeeRepository.AddAsync(Item);
                }


                OnCloseRequested?.Invoke();
                System.Windows.MessageBox.Show("Employee saved successfully!", "Success",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Failed to save candidate: {ex.Message}", "Save Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}