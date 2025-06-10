using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CbcRoastersErp.Helpers;
using CbcRoastersErp.Models;
using CbcRoastersErp.Repositories;
using CbcRoastersErp.Services;
using CbcRoastersErp.ViewModels.Administration.MasterData.FinishedGoodsViewModels;
using CbcRoastersErp.Views;
using MaterialDesignThemes.Wpf;

namespace CbcRoastersErp.ViewModels
{
    public class FinishedGoodsViewModel : INotifyPropertyChanged
    {
        private readonly ProductionRepository _finishedGoodsRepository;

        //   private List<FinishedGoods> _finishedGoodsList;
        private FinishedGoods _finishedGoods;
        public ObservableCollection<FinishedGoods> FinishedGoods { get; private set; }

        public FinishedGoods SelectedFinishedGoods
        {
            get => _finishedGoods;
            set
            {
                _finishedGoods = value;
                OnPropertyChanged(nameof(SelectedFinishedGoods));
                // Update command availability based on selection
                ((RelayCommand)DeleteFinishedGoodCommand).RaiseCanExecuteChanged();
            }
        }

        private int _currentPage = 1;
        private int _totalPages;

        public int CurrentPage
        {
            get => _currentPage;
            set { _currentPage = value; OnPropertyChanged(); }
        }

        public int TotalPages
        {
            get => _totalPages;
            set { _totalPages = value; OnPropertyChanged(); }
        }

        private const int PageSize = 25;

        // Pagination
        public ICommand PageChangedCommand => new RelayCommand(param =>
        {
            if (param?.ToString() == "Next" && CurrentPage < TotalPages)
                CurrentPage++;
            else if (param?.ToString() == "Previous" && CurrentPage > 1)
                CurrentPage--;

            LoadFinishedGoods();
        });

        public event Action<string> OnNavigationRequested;
        public event Action<object> OnOpenAddEditView;
        public ICommand NavigateBackCommand => new RelayCommand(_ => OnNavigationRequested?.Invoke("MasterDataDashDb"));
        public ICommand OpenAddEditFinishedGoodsCommand { get; }
        public ICommand DeleteFinishedGoodCommand { get; }



        protected virtual void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public FinishedGoodsViewModel()
        {
            _finishedGoodsRepository = new ProductionRepository();
            LoadFinishedGoods();

            OpenAddEditFinishedGoodsCommand = new RelayCommand(OpenAddEditFinishedGoods);
            DeleteFinishedGoodCommand = new RelayCommand(DeleteFinishedGoodsAsync, CanExecuteFinishedGoodCommand);
        }

        private async Task DeleteFinishedGoodsAsync(object parameter)
        {
            if (SelectedFinishedGoods == null)
                return;

            var dialog = new ConfirmDeleteDialog($"Delete '{SelectedFinishedGoods.ProductName}'?");

            var result = await DialogHost.Show(dialog, "RootDialog");

            if (result is bool accepted && accepted)
            {
                _finishedGoodsRepository.DeleteFinishedGoods(SelectedFinishedGoods.FinishedGoodID);
                LoadFinishedGoods();
            }

            LoadFinishedGoods();


        }

        private void OpenAddEditFinishedGoods(object parameter)
        {
            FinishedGoods finishedGoodsToEdit;
            if (SelectedFinishedGoods != null && SelectedFinishedGoods.FinishedGoodID > 0)
            {
                finishedGoodsToEdit = _finishedGoodsRepository.GetFinishedGoodById(SelectedFinishedGoods.FinishedGoodID);
                var profile = _finishedGoodsRepository.GetRoastingProfiles()
                                   .FirstOrDefault(p => p.ProfileID == finishedGoodsToEdit.ProfileID);
            }
            else
            {
                finishedGoodsToEdit = new FinishedGoods();
            }
            var addEditViewModel = new AddEditMdFinishedGoodsViewModel(finishedGoodsToEdit);
            addEditViewModel.OnCloseRequested += () =>
            {
                LoadFinishedGoods(); // Refresh list after add/edit
                OnNavigationRequested?.Invoke("AdminFinishedGoodsView"); // Ensure UI updates correctly
            };

            OnOpenAddEditView?.Invoke(addEditViewModel);
        }

        private void LoadFinishedGoods()
        {
           try
            {
                var finishedGoodsList = _finishedGoodsRepository.GetFinishedGoodsPaged(CurrentPage, PageSize);
                FinishedGoods = new ObservableCollection<FinishedGoods>(finishedGoodsList);
                TotalPages = (int)Math.Ceiling((double)_finishedGoodsRepository.GetTotalFinishedGoodsCount() / PageSize);
                OnPropertyChanged(nameof(FinishedGoods));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading finished goods: {ex.Message}", "Error", MessageBoxButton.OK);
                ApplicationLogger.Log(ex, "Error loading finished goods in FinishedGoodsViewModel");
            }
            finally
            {
                OnPropertyChanged(nameof(TotalPages));
                OnPropertyChanged(nameof(CurrentPage));
                OnPropertyChanged(nameof(FinishedGoods));    
                
            }
        }

        private bool CanExecuteFinishedGoodCommand(object parameter)
        {
            return SelectedFinishedGoods != null;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
