using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using CbcRoastersErp.Models.HR;
using CbcRoastersErp.Services;
using CbcRoastersErp.Views;
using CBCRoastersERP.Repositories.HR;

namespace CbcRoastersErp.ViewModels.HR
{
    public class HrEmployeeViewModel : INotifyPropertyChanged
    {
        private readonly EmployeeRepository _employeeRepository;
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

        public ObservableCollection<Employee> Employees { get; set; }
        private Employee _selectedEmployee;
        public Employee SelectedEmployee
        {
            get => _selectedEmployee;
            set { _selectedEmployee = value; OnPropertyChanged(nameof(SelectedEmployee)); }
        }

        // Pagination
        public ICommand PageChangedCommand => new RelayCommand(param =>
        {
            if (param?.ToString() == "Next" && CurrentPage < TotalPages)
                CurrentPage++;
            else if (param?.ToString() == "Previous" && CurrentPage > 1)
                CurrentPage--;

            _ = LoadEmployees();
        });

        public ICommand NavigateBackCommand { get; }
        public ICommand DeleteEmployeeCommand { get; }
        public ICommand OpenAddEditEmployeeCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;
        public Action<string> OnNavigationRequested { get; internal set; }
        public event Action<object> OnOpenAddEditView;

        public HrEmployeeViewModel()
        {
            _employeeRepository = new EmployeeRepository(); 

            OpenAddEditEmployeeCommand = new RelayCommand(OpenAddEditEmployee);
            NavigateBackCommand = new RelayCommand(_ => OnNavigationRequested?.Invoke("Dashboard"));
            DeleteEmployeeCommand = new RelayCommand(DeleteEmployee, CanExecuteEmployeeCommand);

            _ = LoadEmployees();

        }

        private bool CanExecuteEmployeeCommand(object arg)
        {
            return SelectedEmployee != null;
        }

        private async Task OpenAddEditEmployee()
        {
            Employee employeeToEdit;
            if (SelectedEmployee != null)
            {
                employeeToEdit = SelectedEmployee;
            }
            else
            {
                employeeToEdit = new Employee();
            }
            var addEditEmployeeView = new AddEditHrEmployeeViewModel(employeeToEdit);
            addEditEmployeeView.OnCloseRequested += () =>
            {
                _ = LoadEmployees();
                OnNavigationRequested?.Invoke("HR_Employee");
            };

            OnOpenAddEditView?.Invoke(addEditEmployeeView);

        }

        private async Task LoadEmployees()
        {
            try
            {
                int totalEmployees = await _employeeRepository.GetTotalCountAsync();
                TotalPages = (int)Math.Ceiling((double)totalEmployees / 25);

                var employeesPaged = await _employeeRepository.GetByPageAsync(CurrentPage, 25);
                Employees = new ObservableCollection<Employee>(employeesPaged);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading employees: {ex.Message}");
            }
            finally
            {
                OnPropertyChanged(nameof(Employees));
                OnPropertyChanged(nameof(TotalPages));
                OnPropertyChanged(nameof(CurrentPage));
            }
        }

        private async Task DeleteEmployee()
        {
            await _employeeRepository.DeleteAsync(SelectedEmployee.EmployeeID);
            await LoadEmployees();
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}