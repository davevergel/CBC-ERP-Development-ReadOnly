using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CbcRoastersErp.Models;
using CbcRoastersErp.Repositories;
using CbcRoastersErp.Services;
using System.Windows.Input;
using System.Windows;
using System.ComponentModel;

namespace CbcRoastersErp.ViewModels
{
    public class ManagePermissionsViewModel : BaseViewModel
    {
        private readonly RolesRepository _rolesRepo = new();
        private readonly PermissionsRepository _permissionsRepo = new();

        public event Action<string> OnNavigationRequested;
        public event Action OnCloseRequested;
        public event Action<object> OnOpenAddEditView;
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<Role> Roles { get; set; } = new();
        public ObservableCollection<PermissionItem> PermissionItems { get; set; } = new();

        private Role? _selectedRole;
        public Role? SelectedRole
        {
            get => _selectedRole;
            set
            {
                _selectedRole = value;
                OnPropertyChanged();
                LoadPermissionsForRole();
            }
        }

        public ICommand SaveCommand => new RelayCommand(_ => SavePermissions());
        public ICommand CancelCommand => new RelayCommand(_ => OnCloseRequested?.Invoke());
        public ICommand NavigateBackCommand => new RelayCommand(_ => OnNavigationRequested?.Invoke("Dashboard"));
        public ManagePermissionsViewModel()
        {
            LoadRoles();
            LoadAllPermissions();            
        }

        private void LoadRoles()
        {
            Roles.Clear();
            foreach (var role in _rolesRepo.GetAllRoles())
                Roles.Add(role);
        }

        private void LoadAllPermissions()
        {
            var allPerms = _permissionsRepo.GetAllPermissions();
            PermissionItems.Clear();
            foreach (var perm in allPerms)
            {
                PermissionItems.Add(new PermissionItem { Permission = perm, IsAssigned = false });
            }
        }

        private void LoadPermissionsForRole()
        {
            if (SelectedRole == null) return;

            var assignedPerms = _permissionsRepo.GetPermissionsForRole(SelectedRole.RoleID);
            foreach (var item in PermissionItems)
            {
                item.IsAssigned = assignedPerms.Any(p => p.PermissionId == item.Permission.PermissionId);
            }
        }

        private void SavePermissions()
        {
            if (SelectedRole == null) return;

            var assignedIds = PermissionItems.Where(p => p.IsAssigned).Select(p => p.Permission.PermissionId).ToList();
            _permissionsRepo.UpdateRolePermissions(SelectedRole.RoleID, assignedIds);
            MessageBox.Show("Permissions updated successfully.");
        }
    }

    public class PermissionItem : BaseViewModel
    {
        public Permission Permission { get; set; }
        public bool IsAssigned { get; set; }
    }

}
