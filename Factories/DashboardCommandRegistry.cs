using System.Windows.Input;
using CbcRoastersErp.Helpers;
using CbcRoastersErp.Services;
using CbcRoastersErp.ViewModels;

namespace CbcRoastersErp.Factories
{
    public static class DashboardCommandRegistry
    {
        public static void Register(DashboardViewModel vm)
        {
            // Operations
            vm.NavigateToPurchaseOrder = new RelayCommand(_ => vm.OnNavigationRequested?.Invoke("PurchaseOrder"));
            vm.NavigateToInventoryCommand = new RelayCommand(_ => vm.OnNavigationRequested?.Invoke("Inventory"));
            vm.NavigateToProductionCommand = new RelayCommand(_ => vm.OnNavigationRequested?.Invoke("Production"));
            vm.NavigateToBatchScheduleCommand = new RelayCommand(_ => vm.OnNavigationRequested?.Invoke("BatchSchedule"));
            vm.NavigateToEmployeesCommand = new RelayCommand(_ => vm.OnNavigationRequested?.Invoke("Employee"));
            vm.NavigateToArtisanCommand = new RelayCommand(_ => vm.OnNavigationRequested?.Invoke("ArtisanRoastProfiles"));
            vm.NavigateToFarmersMarketCommand = new RelayCommand(_ => vm.OnNavigationRequested?.Invoke("FarmersMarketProductionSchedule"));

            // Reporting
            vm.NavigateToInventoryReportCommand = new RelayCommand(_ => vm.OnNavigationRequested?.Invoke("InventoryReport"));

            // User Management
            vm.NavigateToUserManagementCommand = new RelayCommand(_ => vm.OnNavigationRequested?.Invoke("UserManagement"));
            vm.NavigateToManagePermissionsCommand = new RelayCommand(_ => vm.OnNavigationRequested?.Invoke("ManagePermissions"));

            // Order Management
            vm.NavigateToBigCommerceCommand = new RelayCommand(_ => vm.OnNavigationRequested?.Invoke("BigCommerceSync"));
            vm.NavigateToBigCommerceOrdersCommand = new RelayCommand(_ => vm.OnNavigationRequested?.Invoke("BigCommerceOrders"));
            vm.NavigateToCustomerCommand = new RelayCommand(_ => vm.OnNavigationRequested?.Invoke("CustomerView"));
            vm.NavigateToSalesDashboardCommand = new RelayCommand(_ => vm.OnNavigationRequested?.Invoke("SalesDashboard"));


            // HR Management
            vm.NavigateToHRPerformanceCommand = new RelayCommand(_ => vm.OnNavigationRequested?.Invoke("HR_PerformanceReview"));
            vm.NavigateToHRJobPostingCommand = new RelayCommand(_ => vm.OnNavigationRequested?.Invoke("HR_JobPosting"));
            vm.NavigateToHRCandidateCommand = new RelayCommand(_ => vm.OnNavigationRequested?.Invoke("HR_Candidate"));
            vm.NavigateToHRInterviewCommand = new RelayCommand(_ => vm.OnNavigationRequested?.Invoke("HR_Interview"));
            vm.NavigateToHrEmployeeCommand = new RelayCommand(_ => vm.OnNavigationRequested?.Invoke("HR_Employee"));

            // Finance Management
            vm.NavigateToFinanceAccountsCommand = new RelayCommand(_ => vm.OnNavigationRequested?.Invoke("Finance_Accounts"));
            vm.NavigateToFinanceJournalCommand = new RelayCommand(_ => vm.OnNavigationRequested?.Invoke("Finance_JournalEntry"));
            vm.NavigateToProfitLossCommand = new RelayCommand(_ => vm.OnNavigationRequested?.Invoke("Finance_ProfitLoss"));
            vm.NavigateToFinanceDashboardCommand = new RelayCommand(_ => vm.OnNavigationRequested?.Invoke("Finance_Dashboard"));
            vm.NavigateToImportSquareCsvCommand = new RelayCommand(_ => vm.OnNavigationRequested?.Invoke("ImportSquareCsv"));
            vm.NavigateToImportDriposSalesCommand = new RelayCommand(_ => vm.OnNavigationRequested?.Invoke("ImportDriposSales"));
            vm.NavigateToDriposSalesDashboardCommand = new RelayCommand(_ => vm.OnNavigationRequested?.Invoke("DriposSalesDashboard"));

            // Administration
            vm.NavigateToApplicationLogsCommand = new RelayCommand(_ => vm.OnNavigationRequested?.Invoke("ApplicationLogs"));
            vm.NavigateToMasterDataCommand = new RelayCommand(_ => vm.OnNavigationRequested?.Invoke("MasterDataDashboard"));
            vm.NavigateToSuppliersCommand = new RelayCommand(_ => vm.OnNavigationRequested?.Invoke("SupplierView"));
            vm.NavigateToFinishedGoodsCommand = new RelayCommand(_ => vm.OnNavigationRequested.Invoke("MdFinishedGoods"));
            vm.NavigateToManageRoastingProfilesCommand = new RelayCommand(_ => vm.OnNavigationRequested.Invoke("MdRoastingProfiles"));
            vm.NavigateToMangaeArtisanCommand = new RelayCommand(_ => vm.OnNavigationRequested.Invoke("ManageArtisan"));

            // Navigate Back to Dashboard
            vm.NavigateBackCommand = new RelayCommand(_ => vm.OnNavigationRequested?.Invoke("Dashboard"));

            vm.ExitCommand = new RelayCommand(_ => System.Windows.Application.Current.Shutdown());
        }
    }
}
