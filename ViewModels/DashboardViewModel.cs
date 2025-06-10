using System;
using System.Windows.Input;
using CbcRoastersErp.Factories;
using CbcRoastersErp.Helpers;
using CbcRoastersErp.Services;

namespace CbcRoastersErp.ViewModels
{
    public class DashboardViewModel
    {
        public bool CanManageEmployees => CurrentUserSession.HasPermission("ManageEmployees");
        public bool IsAdmin => CurrentUserSession.HasPermission("AdminAll");

        public ICommand NavigateToInventoryCommand { get; set; }
        public ICommand NavigateToProductionCommand { get; set; }
        public ICommand NavigateToArtisanCommand { get; set; }
        public ICommand NavigateToEmployeesCommand { get; set; }
        public ICommand NavigateToUserManagementCommand { get; set; }
        public ICommand NavigateToManagePermissionsCommand { get; set; }
        public ICommand NavigateToBigCommerceCommand { get; set; }
        public ICommand NavigateToBigCommerceOrdersCommand { get; set; }
        public ICommand NavigateToCustomerCommand { get; set; }
        public ICommand NavigateToApplicationLogsCommand { get; set; }
        public ICommand NavigateToBatchScheduleCommand { get; set; }
        public ICommand NavigateToSalesDashboardCommand { get; set; } // Sales Dashboard
        public ICommand NavigateToMasterDataCommand { get; set; } // Master Data Dashboard

        // HR
        public ICommand NavigateToHRPerformanceCommand { get; set; }
        public ICommand NavigateToHRJobPostingCommand { get; set; }
        public ICommand NavigateToHRCandidateCommand { get; set; }
        public ICommand NavigateToHRInterviewCommand { get; set; }
        public ICommand NavigateToHrEmployeeCommand { get; set; } // HR Employee

        // Finance
        public ICommand NavigateToFinanceAccountsCommand { get; set; }
        public ICommand NavigateToFinanceJournalCommand { get; set; }
        public ICommand NavigateToProfitLossCommand { get; set; }
        public ICommand NavigateToFinanceDashboardCommand { get; set; } // Finance Dashboard
        public ICommand NavigateToImportSquareCsvCommand { get; set; }
        public ICommand NavigateToImportDriposSalesCommand { get; set; } // Import Dripos Sales
        public ICommand NavigateToDriposSalesDashboardCommand { get; set; } // Dripos Sales Dashboard

        // Navigate to Dashboard
        public ICommand NavigateBackCommand { get; set; } // Navigate back to Dashboard


        public ICommand ExitCommand { get; set; }

        // Events   
        public Action<string> OnNavigationRequested { get; set; }

        public DashboardViewModel()
        {
            DashboardCommandRegistry.Register(this);
        }
    }
}
