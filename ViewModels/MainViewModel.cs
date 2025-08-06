using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;
using CbcRoastersErp.Factories;
using CbcRoastersErp.Views;

namespace CbcRoastersErp.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private UserControl _currentView;
        private readonly Dictionary<string, Func<UserControl>> _viewFactoryMap;
        private readonly Dictionary<Type, Func<object, UserControl>> _addEditViewMap;
        private readonly Stack<UserControl> _viewHistory = new();

        public Services.NavigationService NavigationService { get; }
        public int CurrentScheduleId { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public UserControl CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentView)));
            }
        }

        public MainViewModel()
        {
            NavigationService = new Services.NavigationService(this);
            _viewFactoryMap = ViewFactoryRegistry.GetFactories(this);
            _addEditViewMap = AddEditViewFactoryRegistry.GetFactories();

            var loginViewModel = new LoginViewModel();
            loginViewModel.OnLoginSuccess += LoadDashboardView;
            CurrentView = new LoginView { DataContext = loginViewModel };
        }

        public void HandleNavigation(string viewName)
        {
            if (_viewFactoryMap.TryGetValue(viewName, out var viewFactory))
            {
                SaveCurrentViewToHistory();
                CurrentView = viewFactory.Invoke();
            }
            else
            {
                LoadDashboardView();
            }
        }

        public void HandleOpenAddEditView(object viewModel)
        {
            if (viewModel == null) return;

            var type = viewModel.GetType();
            if (_addEditViewMap.TryGetValue(type, out var viewFactory))
            {
                SaveCurrentViewToHistory();

                if (viewModel is IAddEditViewModel closable)
                {
                    closable.OnCloseRequested += NavigateBack;
                }

                CurrentView = viewFactory(viewModel);
            }
        }

        private void SaveCurrentViewToHistory()
        {
            if (CurrentView != null)
            {
                _viewHistory.Push(CurrentView);
            }
        }

        private void NavigateBack()
        {
            if (_viewHistory.TryPop(out var previousView))
            {
                CurrentView = previousView;
            }
            else
            {
                LoadDashboardView();
            }
        }

        private void LoadDashboardView()
        {
            var dashboardViewModel = new DashboardViewModel();
            dashboardViewModel.OnNavigationRequested = HandleNavigation;
            CurrentView = new Dashboard { DataContext = dashboardViewModel };
        }
    }
}