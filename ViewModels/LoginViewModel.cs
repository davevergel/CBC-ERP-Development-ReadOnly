using System;
using System.ComponentModel;
using System.Windows.Input;
using CbcRoastersErp.Models;
using CbcRoastersErp.Repositories;
using CbcRoastersErp.Helpers;
using CbcRoastersErp.Services;
using CbcRoastersErp.Views;
using System.Windows;

namespace CbcRoastersErp.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private string _username;
        private string _password;
        private string _errorMessage;
        private readonly UserRepository _userRepository;

        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }

        public ICommand LoginCommand { get; }

        public event Action OnLoginSuccess;
        public event PropertyChangedEventHandler PropertyChanged;

        public LoginViewModel()
        {
            _userRepository = new UserRepository();
            LoginCommand = new RelayCommand(ExecuteLogin, CanExecuteLogin);
            ErrorMessage = string.Empty;
        }

        private void ExecuteLogin(object parameter)
        {
            if (_userRepository.ValidateUser(Username, Password))
            {
                _userRepository.UpdateLastLoginDate(Username);
                var user = _userRepository.GetUserWithPermissions(Username);
                CurrentUserSession.User = user;

                OnLoginSuccess?.Invoke();
            }
            else
            {
                ErrorMessage = "Invalid credentials. Try again.";
                
            }
        }

        private bool CanExecuteLogin(object parameter)
        {
            return !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password);
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
