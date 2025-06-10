using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using CbcRoastersErp.Models;
using CbcRoastersErp.Repositories;
using CbcRoastersErp.Helpers;
using CbcRoastersErp.Services;
using System.Runtime.CompilerServices;
using System.Windows;

namespace CbcRoastersErp.ViewModels
{
    public class ProductionViewModel : INotifyPropertyChanged
    {
        private readonly ProductionRepository _productionRepository;
        private BatchRoasting _selectedBatch;
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

        public ObservableCollection<BatchRoasting> RoastBatches { get; set; }
        public event Action<string> OnNavigationRequested;

        public BatchRoasting SelectedBatch
        {
            get => _selectedBatch;
            set
            {
                _selectedBatch = value;
                OnPropertyChanged(nameof(SelectedBatch));
            }
        }

        public ICommand DeleteBatchCommand { get; }
        public ICommand NavigateBackCommand { get; }
        public ICommand OpenAddEditBatchCommand { get; }

        // Pagination
        public ICommand PageChangedCommand => new RelayCommand(param =>
        {
            if (param?.ToString() == "Next" && CurrentPage < TotalPages)
                CurrentPage++;
            else if (param?.ToString() == "Previous" && CurrentPage > 1)
                CurrentPage--;

            LoadRoastBatches();
        });

        public event Action<object> OnOpenAddEditView;

        public event PropertyChangedEventHandler PropertyChanged;

        public ProductionViewModel()
        {
            _productionRepository = new ProductionRepository();
            LoadRoastBatches();

            OpenAddEditBatchCommand = new RelayCommand(OpenAddEditBatch);
            DeleteBatchCommand = new RelayCommand(DeleteBatch, CanExecuteBatchCommand);
            NavigateBackCommand = new RelayCommand(_ => OnNavigationRequested?.Invoke("Dashboard"));
        }

        private void LoadRoastBatches()
        {
            try
            {
            int totalItems = _productionRepository.GetRoastBatchCount();
            TotalPages = (int)Math.Ceiling((double)totalItems / PageSize);

            var pagedBatches = _productionRepository.GetRoastBatchesPaged(CurrentPage, PageSize);
            RoastBatches = new ObservableCollection<BatchRoasting>(pagedBatches);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading batches: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                OnPropertyChanged(nameof(TotalPages));
                OnPropertyChanged(nameof(CurrentPage));
                OnPropertyChanged(nameof(RoastBatches));
            }
        }
        private void OpenAddEditBatch(object parameter)
        {
            BatchRoasting batchToEdit;

            if (SelectedBatch != null)
            {
                // Fetch full details for the selected batch
                var fullBatch = _productionRepository.GetRoastBatchById(SelectedBatch.BatchID);
                var profile = _productionRepository.GetRoastingProfiles()
                                                   .FirstOrDefault(p => p.ProfileID == fullBatch.ProfileID);
                var finishedGood = _productionRepository.GetFinishedGoods()
                                                        .FirstOrDefault(fg => fg.FinishedGoodID == fullBatch.FinishedGoodID);

                fullBatch.ProfileName = profile?.ProfileName ?? "N/A";
                fullBatch.FinishedGoodName = finishedGood?.ProductName ?? "N/A";
                batchToEdit = fullBatch;
            }
            else
            {
                batchToEdit = new BatchRoasting(); // New entry
            }

            var addEditViewModel = new AddEditBatchViewModel(batchToEdit);
            addEditViewModel.OnCloseRequested += () =>
            {
                LoadRoastBatches(); // Refresh list after add/edit
                OnNavigationRequested?.Invoke("Production");
            };

            OnOpenAddEditView?.Invoke(addEditViewModel);
        }


        private void DeleteBatch(object parameter)
        {
            _productionRepository.DeleteRoastBatch(SelectedBatch.BatchID);
            LoadRoastBatches();
        }

        private bool CanExecuteBatchCommand(object parameter)
        {
            return SelectedBatch != null;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

    }
}

