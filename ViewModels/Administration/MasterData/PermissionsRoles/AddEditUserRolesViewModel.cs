using CbcRoastersErp.Models;
using CbcRoastersErp.Repositories;
using CbcRoastersErp.Services;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace CbcRoastersErp.ViewModels
{
    public class AddEditUserRolesViewModel : INotifyPropertyChanged
    {
        private readonly RolesRepository _repository = new();
        private Role _role;

        public Role Role
        {
            get => _role;
            set { _role = value; OnPropertyChanged(); }
        }

        public ICommand SaveCommand => new RelayCommand(Save);
        public ICommand CancelCommand => new RelayCommand(_ => CloseAction?.Invoke());

        public Action CloseAction { get; set; }

        // Constructor for adding a new role
        public AddEditUserRolesViewModel() : this(null) { }

        // Constructor for editing an existing role
        public AddEditUserRolesViewModel(Role role)
        {
            Role = role != null ? new Role
            {
                RoleID = role.RoleID,
                RoleName = role.RoleName,
                Description = role.Description
            } : new Role();
        }

        private void Save(object _)
        {
            if (Role.RoleID == 0)
                _repository.AddRole(Role);
            else
                _repository.UpdateRole(Role);

            CloseAction?.Invoke();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
