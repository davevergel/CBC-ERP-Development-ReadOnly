using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CbcRoastersErp.ViewModels;
using System.Windows.Controls;

namespace CbcRoastersErp.Services
{
    public class NavigationService
    {
        private readonly MainViewModel _mainViewModel;

        public NavigationService(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
        }

        public void NavigateTo(string viewName)
        {
            _mainViewModel.HandleNavigation(viewName);
        }

        public void NavigateToUserControl(UserControl control)
        {
            _mainViewModel.CurrentView = control;
        }
    }


}
