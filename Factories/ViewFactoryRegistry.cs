using System.Collections;
using System.Windows.Controls;
using CbcRoastersErp.Helpers;
using CbcRoastersErp.ViewModels;
using CbcRoastersErp.ViewModels.Administration.MasterData;
using CbcRoastersErp.ViewModels.Administration.MasterData.RoastingProfilesViewModels;
using CbcRoastersErp.ViewModels.Finance;
using CbcRoastersErp.ViewModels.HR;
using CbcRoastersErp.ViewModels.Operations.Planning;
using CbcRoastersErp.ViewModels.Purchasing;
using CbcRoastersErp.ViewModels.Reporting;
using CbcRoastersErp.ViewModels.OrderManagement;
using CbcRoastersErp.Views;
using CbcRoastersErp.Views.Administration.MasterData;
using CbcRoastersErp.Views.Administration.MasterData.FinishedGoods;
using CbcRoastersErp.Views.Administration.MasterData.RoastingProfilesViews;
using CbcRoastersErp.Views.Finance;
using CbcRoastersErp.Views.HR;
using CbcRoastersErp.Views.Operations;
using CbcRoastersErp.Views.Operations.Planning;
using CbcRoastersErp.Views.OrderManagement;
using CbcRoastersErp.Views.Purchasing;
using CbcRoastersErp.Views.Reporting;

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
                // Operations
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
                    vm.OnOpenAddEditView += main.HandleOpenAddEditView;
                    return new EmployeeView { DataContext = vm };
                }},
                { "PurchaseOrder", () => {
                    var vm = new PurchaseOrderViewModel();
                    vm.OnNavigationRequested += main.HandleNavigation;
                    vm.OnOpenAddEditView += main.HandleOpenAddEditView;
                    return new PurchaseOrderView { DataContext = vm };
                }},
                { "FarmersMarketProductionScheduleItems", () => {
                    var vm = new FarmersMarketProductionScheduleItemsViewModel(CurrentScheduleContext.Instance.SelectedScheduleId);
                    vm.OnNavigationRequested += main.HandleNavigation;
                    vm.OnOpenAddEditView += main.HandleOpenAddEditView;
                    return new FarmersMarketProductionScheduleItemsView { DataContext = vm };
                }},
                { "FarmersMarketProductionSchedule", () => {
                    var vm = new FarmersMarketProductionScheduleViewModel();
                    vm.OnNavigationRequested += main.HandleNavigation;
                    vm.OnOpenAddEditView += main.HandleOpenAddEditView;
                    return new FarmersMarketProductionScheduleView { DataContext = vm };
                }},
                { "Inventory", () => {
                    var vm = new InventoryViewModel();
                    vm.OnNavigationRequested += main.HandleNavigation;
                    vm.OnOpenAddEditView += main.HandleOpenAddEditView;
                    return new InventoryView { DataContext = vm };
                }},
                { "BatchSchedule", () => {
                    var vm = new BatchScheduleViewModel();
                    vm.OnNavigationRequested += main.HandleNavigation;
                    vm.OnOpenAddEditView += main.HandleOpenAddEditView;
                    return new BatchScheduleView { DataContext = vm };
                }},
                { "InventoryReport", () => {
                    try
                    {
                        var vm = new InventoryReportViewModel();
                        vm.OnNavigationRequested += main.HandleNavigation;
                        return new InventoryReportView { DataContext = vm };
                    }
                    catch (Exception ex)
                    {
                        ApplicationLogger.Log(ex,nameof(ViewFactoryRegistry), nameof(DictionaryBase));
                        return new UserControl(); // return empty to avoid crash
                    }
                }},

                // HR
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
                    vm.OnOpenAddEditView += main.HandleOpenAddEditView;
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
                    try
                    {
                    var vm = new BigCommerceSyncViewModel();
                    vm.OnNavigationRequested += main.HandleNavigation;
                    return new BigCommerceSyncView { DataContext = vm };
                    }
                    catch(Exception ex)
                    {
                        ApplicationLogger.Log(ex,nameof(ViewFactoryRegistry), nameof(DictionaryBase));
                        return new UserControl(); // return empty to avoid crash
                    }
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
                { "SalesDashboard", () => {
                    var vm = new SalesDashboardViewModel();
                    vm.OnNavigationRequested += main.HandleNavigation;
                    return new SalesDashboardView { DataContext = vm };
                }},
                // Administration
                { "MasterDataDashboard", () => {
                    var vm = new  MasterDataDashDbViewModel();
                    vm.OnNavigationRequested += main.HandleNavigation;
                    return new  MasterDataDashDbView { DataContext = vm };
                }},
                { "MdFinishedGoods", () => {
                    var vm = new FinishedGoodsViewModel();
                    vm.OnNavigationRequested += main.HandleNavigation;
                    vm.OnOpenAddEditView += main.HandleOpenAddEditView;
                    return new FinishedGoodsView { DataContext = vm };
                }},
                { "MdRoastingProfiles", () => {
                    var vm = new ManageRoastingProfilesViewModel();
                    vm.OnNavigationRequested += main.HandleNavigation;
                    vm.OnOpenAddEditView += main.HandleOpenAddEditView;
                    return new ManageRoastingProfilesView { DataContext = vm };
                }},
                {"ManageArtisan", () =>
                {
                    var vm = new SettingsViewModel();
                    vm.OnNavigationRequested += main.HandleNavigation;
                    return new SettingsView { DataContext = vm };
                } },
                { "CustomerView", () => {
                    var vm = new CustomerViewModel();
                    vm.OnNavigationRequested += main.HandleNavigation;
                    return new CustomerView { DataContext = vm };
                }},
                { "SupplierView", () => {
                    var vm = new SupplierViewModel();
                    vm.OnNavigationRequested += main.HandleNavigation;
                    vm.OnOpenAddEditView += main.HandleOpenAddEditView;
                    return new SupplierView { DataContext = vm };
                }},

                { "Finance_Dashboard", () => {
                    var vm = new FinanceDashboardViewModel();
                    vm.OnNavigationRequested += main.HandleNavigation;
                    return new FinanceDashboardView { DataContext = vm };
                }}
            };
        }

        private static void Vm_OnNavigationRequested(string obj)
        {
            throw new NotImplementedException();
        }
    }
}
