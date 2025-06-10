using System;
using System.Collections.Generic;
using System.Windows.Controls;
using CbcRoastersErp.Models;
using CbcRoastersErp.Services.Finance;
using CbcRoastersErp.ViewModels;
using CbcRoastersErp.ViewModels.Administration.MasterData;
using CbcRoastersErp.ViewModels.Finance;
using CbcRoastersErp.ViewModels.HR;
using CbcRoastersErp.Views;
using CbcRoastersErp.Views.Administration.MasterData;
using CbcRoastersErp.Views.Finance;
using CbcRoastersErp.Views.HR;
using CbcRoastersErp.Views.Operations;

namespace CbcRoastersErp.Factories
{
    public static class ViewFactoryRegistry
    {
        public static Dictionary<string, Func<UserControl>> GetFactories(MainViewModel main)
        {
            return new Dictionary<string, Func<UserControl>>
            {
                { "Dashboard", () => {
                    var vm = new DashboardViewModel();
                    vm.OnNavigationRequested += main.HandleNavigation;
                    return new Dashboard { DataContext = vm };
                }},
                { "Production", () => {
                    var vm = new ProductionViewModel();
                    vm.OnNavigationRequested += main.HandleNavigation;
                    vm.OnOpenAddEditView += main.HandleOpenAddEditView;
                    return new ProductionView { DataContext = vm };
                }},
                    { "ArtisanRoastProfiles", () => {
                        var vm = new RoastProfilesViewModel();
                        vm.OnNavigationRequested += main.HandleNavigation;
                        return new RoastProfilesView { DataContext = vm };
                    }},
                    { "Employee", () => {
                        var vm = new EmployeeViewModel();
                        vm.OnNavigationRequested += main.HandleNavigation;
                        return new EmployeeView { DataContext = vm };
                    }},
                { "Inventory", () => {
                    var vm = new InventoryViewModel();
                    vm.OnNavigationRequested += main.HandleNavigation;
                    vm.OnOpenAddEditView += main.HandleOpenAddEditView;
                    return new InventoryView { DataContext = vm };
                }},
                { "HR_Employee", () => {
                    var vm = new HrEmployeeViewModel();
                    vm.OnNavigationRequested += main.HandleNavigation;
                    vm.OnOpenAddEditView += main.HandleOpenAddEditView;
                    return new HrEmployeeView { DataContext = vm };
                }},
                { "HR_Candidate", () => {
                    var vm = new CandidateViewModel();
                    vm.OnNavigationRequested += main.HandleNavigation;
                    vm.OnOpenAddEditView += main.HandleOpenAddEditView;
                    return new CandidateView { DataContext = vm };
                }},
                // Finance
                { "ImportSquareCsv", () => {
                    var vm = new ImportSquareCsvViewModel();
                    vm.OnNavigationRequested += main.HandleNavigation;
                    return new ImportSquareCsvView { DataContext = vm };
                }},
                { "UserManagement", () => {
                    var vm = new UserManagementViewModel();
                    vm.OnNavigationRequested += main.HandleNavigation;
                    return new UserManagementView { DataContext = vm };
                }},
                { "ManagePermissions", () => {
                    var vm = new ManagePermissionsViewModel();
                    vm.OnNavigationRequested += main.HandleNavigation;
                    return new ManagePermissionsView { DataContext = vm };
                }},
                { "Finance_Accounts", () => {
                    var vm = new AccountViewModel();
                    vm.OnNavigationRequested += main.HandleNavigation;
                    vm.OnOpenAddEditView += main.HandleOpenAddEditView;
                    return new AccountView { DataContext = vm };
                }},
                { "Finance_JournalEntry", () => {
                    var vm = new JournalEntryViewModel();
                    vm.OnNavigationRequested += main.HandleNavigation;
                    vm.OnOpenAddEditView += main.HandleOpenAddEditView;
                    return new JournalEntryView { DataContext = vm };
                }},
                { "ImportDriposSales", () => {
                    var vm = new ImportDriposCombinedViewModel();
                    vm.OnNavigationRequested += main.HandleNavigation;
                    return new ImportDriposCombinedView { DataContext = vm };
                }},
                { "Finance_ProfitLoss", () =>{
                   var vm = new ProfitAndLossViewModel();
                   vm.OnNavigationRequested += main.HandleNavigation;
                    return new ProfitAndLossView { DataContext = vm };
                }},
                { "DriposSalesDashboard", () => {
                    var vm = new DriposDashboardViewModel();
                    vm.OnNavigationRequested += main.HandleNavigation;
                    return new DriposDashboardView { DataContext = vm };
                }},
                { "HR_PerformanceReview", () => {
                    var vm = new PerformanceReviewViewModel();
                    vm.OnNavigationRequested += main.HandleNavigation;
                    vm.OnOpenAddEditView += main.HandleOpenAddEditView;
                    return new PerformanceReviewView { DataContext = vm };
                }},
                { "HR_JobPosting", () => {
                    var vm = new JobPostingViewModel();
                    vm.OnNavigationRequested += main.HandleNavigation;
                    vm.OnOpenAddEditView += main.HandleOpenAddEditView;
                    return new JobPostingView { DataContext = vm };
                }},
                { "HR_Interview", () => {
                    var vm = new InterviewViewModel();
                    vm.OnNavigationRequested += main.HandleNavigation;
                    vm.OnOpenAddEditView += main.HandleOpenAddEditView;
                    return new InterviewView { DataContext = vm };
                }},
                { "BigCommerceSync", () => {
                    var vm = new BigCommerceSyncViewModel();
                    vm.OnNavigationRequested += main.HandleNavigation;
                    return new BigCommerceSyncView { DataContext = vm };
                }},
                { "BigCommerceOrders", () => {
                    var vm = new BigCommerceOrdersViewModel();
                    vm.OnNavigationRequested += main.HandleNavigation;
                    return new BigCommerceOrdersView { DataContext = vm };
                }},
                { "ApplicationLogs", () => {
                    var vm = new ApplicationLogViewModel();
                    vm.OnNavigationRequested += main.HandleNavigation;
                    return new ApplicationLogView{ DataContext = vm };
                }},
                { "BatchSchedule", () => {
                    var vm = new BatchScheduleViewModel();
                    vm.OnNavigationRequested += main.HandleNavigation;
                    return new BatchScheduleView { DataContext = vm };
                }},
                { "SalesDashboard", () => {
                    var vm = new SalesDashboardViewModel();
                    vm.OnNavigationRequested += main.HandleNavigation;
                    return new SalesDashboardView { DataContext = vm };
                }},
                { "MasterDataDashboard", () => {
                    var vm = new  MasterDataDashDbViewModel();
                    vm.OnNavigationRequested += main.HandleNavigation;
                    return new  MasterDataDashDbView { DataContext = vm };
                }},
                { "CustomerView", () => {
                    var vm = new CustomerViewModel();
                    vm.OnNavigationRequested += main.HandleNavigation;
                    return new CustomerView { DataContext = vm };
                }},

                { "Finance_Dashboard", () => {
                    var vm = new FinanceDashboardViewModel();
                    vm.OnNavigationRequested += main.HandleNavigation;
                    return new FinanceDashboardView { DataContext = vm };
                }}
            };
        }
    }
}
