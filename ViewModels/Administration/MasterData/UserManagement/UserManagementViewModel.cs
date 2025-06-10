using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CbcRoastersErp.Models;
using CbcRoastersErp.Repositories;
using CbcRoastersErp.Services;
using System.Windows.Input;
using System.Windows;

namespace CbcRoastersErp.ViewModels
{
    public class UserManagementViewModel : INotifyPropertyChanged
    {
        private readonly UserRepository _userRepo;
        private readonly RolesRepository _rolesRepo;

        public ObservableCollection<UserAccount> Users { get; set; }
        public UserAccount SelectedUser { get; set; }

        public ObservableCollection<Role> Roles { get; set; }
        public Role SelectedRole { get; set; }

        public ICommand OpenAddEditUserCommand { get; }
        public ICommand DeleteUserCommand { get; }

        public ICommand OpenAddEditRoleCommand { get; }
        public ICommand DeleteRoleCommand { get; }

        public ICommand NavigateBackCommand { get; }

        public event Action<object> OnOpenAddEditView;
        public event Action<string> OnNavigationRequested;
        public event PropertyChangedEventHandler PropertyChanged;

        public UserManagementViewModel()
        {
            _userRepo = new UserRepository();
            _rolesRepo = new RolesRepository();

            LoadUsers();
            LoadRoles();

            OpenAddEditUserCommand = new RelayCommand(OpenAddEditUser);
            DeleteUserCommand = new RelayCommand(DeleteUser, _ => SelectedUser != null);

            OpenAddEditRoleCommand = new RelayCommand(OpenAddEditRole);
            DeleteRoleCommand = new RelayCommand(DeleteRole, _ => SelectedRole != null);

            NavigateBackCommand = new RelayCommand(_ => OnNavigationRequested?.Invoke("Dashboard"));
        }

        private void LoadUsers()
        {
            Users = new ObservableCollection<UserAccount>(_userRepo.GetAllUsers());
            OnPropertyChanged(nameof(Users));
        }

        private void LoadRoles()
        {
            Roles = new ObservableCollection<Role>(_rolesRepo.GetAllRoles());
            OnPropertyChanged(nameof(Roles));
        }

        private void OpenAddEditUser(object _)
        {
            var viewModel = new AddEditUserViewModel(SelectedUser);
            viewModel.OnCloseRequested += () =>
            {
                LoadUsers();
                OnNavigationRequested?.Invoke("UserManagement");
            };

            OnOpenAddEditView?.Invoke(viewModel);
        }

        private void DeleteUser(object _)
        {
            if (SelectedUser == null) return;

            var result = MessageBox.Show("Are you sure you want to delete this user?", "Delete User", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                _userRepo.DeleteUser(SelectedUser.UserId);
                LoadUsers();
            }
        }

        private void OpenAddEditRole(object _)
        {
            var viewModel = new AddEditUserRolesViewModel(SelectedRole);
            viewModel.CloseAction += () =>
            {
                LoadRoles();
                OnNavigationRequested?.Invoke("UserManagement");
            };

            OnOpenAddEditView?.Invoke(viewModel);
        }

        private void DeleteRole(object _)
        {
            if (SelectedRole == null) return;

            var result = MessageBox.Show("Are you sure you want to delete this role?", "Delete Role", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                _rolesRepo.DeleteRole(SelectedRole.RoleID);
                LoadRoles();
            }
        }

        private void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
