using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CbcRoastersErp.Services;

namespace CbcRoastersErp.ViewModels.Administration.MasterData
{
    public class MasterDataDashDbViewModel
    {
        public bool IsAdmin => CurrentUserSession.HasPermission("AdminAll");

        // Commands for navigation
        public ICommand NavigateToManageFinishedGoodsCommand => new RelayCommand(_ => OnNavigationRequested?.Invoke("FinishedGoods"));
        public ICommand NavigateToManageRoastingProfilesCommand => new RelayCommand(_ => OnNavigationRequested?.Invoke("RoastingProfiles"));
        public ICommand NavigateBackCommand { get; }

        // Actions and Events
        public event Action<string> OnNavigationRequested;

        public MasterDataDashDbViewModel()
        {
            // Initialize commands or properties if needed
            NavigateBackCommand = new RelayCommand(_ => OnNavigationRequested?.Invoke("Dashboard"));
        }
    }
}
