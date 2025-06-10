using System;
using System.Collections.Generic;
using System.Windows.Controls;
using CbcRoastersErp.ViewModels;
using CbcRoastersErp.Views;
using CbcRoastersErp.ViewModels.HR;
using CbcRoastersErp.Views.HR;
using CbcRoastersErp.ViewModels.Finance;
using CbcRoastersErp.Views.Finance;
using CbcRoastersErp.ViewModels.Administration.MasterData.FinishedGoodsViewModels;
using CbcRoastersErp.ViewModels.Administration.MasterData.RoastingProfilesViewModels;
using CbcRoastersErp.Views.Administration.MasterData.FinishedGoodsViews;
using CbcRoastersErp.Views.Administration.MasterData.RoastingProfilesViews;

namespace CbcRoastersErp.Factories
{
    public static class AddEditViewFactoryRegistry
    {
        public static Dictionary<Type, Func<object, UserControl>> GetFactories()
        {
            return new Dictionary<Type, Func<object, UserControl>>
            {
                { typeof(AddEditGreenCoffeeViewModel), vm => new AddEditGreenCoffeeView { DataContext = vm } },
                { typeof(AddEditTeaViewModel), vm => new AddEditTeaView { DataContext = vm } },
                { typeof(AddEditPackingViewModel), vm => new AddEditPackingView { DataContext = vm } },
                { typeof(AddEditMdFinishedGoodsViewModel), vm => new AddEditMdFinishedGoodsView { DataContext = vm } },
                { typeof(AddEditBatchViewModel), vm => new AddEditBatchView { DataContext = vm } },
                { typeof(AddEditEmployeeViewModel), vm => new AddEditEmployeeView { DataContext = vm } },
                { typeof(AddEditScheduleViewModel), vm => new AddEditScheduleView { DataContext = vm } },
                { typeof(AddEditUserViewModel), vm => new AddEditUserView { DataContext = vm } },
                { typeof(AddEditUserRolesViewModel), vm => new AddEditUserRolesView { DataContext = vm } },
                { typeof(AddEditBatchScheduleViewModel), vm => new AddEditBatchScheduleView { DataContext = vm } },
                { typeof(AddEditRoastingProfileViewModel), vm => new AddEditRoastingProfileView { DataContext = vm } },

                // HR
                { typeof(AddEditHrEmployeeViewModel), vm => new AddEditHrEmployeeView { DataContext = vm } },
                { typeof(AddEditCandidateViewModel), vm => new AddEditCandidateView { DataContext = vm } },
                { typeof(AddEditInterviewViewModel), vm => new AddEditInterviewView { DataContext = vm } },
                { typeof(AddEditJobPostingViewModel), vm => new AddEditJobPostingView { DataContext = vm } },
                { typeof(AddEditPerformanceReviewViewModel), vm => new AddEditPerformanceReviewView { DataContext = vm } },

                // Finance
                { typeof(AddEditAccountViewModel), vm => new AddEditAccountView { DataContext = vm } },
                { typeof(AddEditJournalEntryViewModel), vm => new AddEditJournalEntryView { DataContext = vm } }
            };
        }
    }
}
