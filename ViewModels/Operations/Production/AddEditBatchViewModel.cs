using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using CbcRoastersErp.Models;
using CbcRoastersErp.Repositories;
using CbcRoastersErp.Helpers;
using CbcRoastersErp.Services;
using System.Windows;

namespace CbcRoastersErp.ViewModels
{
    public class AddEditBatchViewModel : INotifyPropertyChanged
    {
        private readonly ProductionRepository _productionRepository;
        private bool _isEditMode;

        public event PropertyChangedEventHandler PropertyChanged;
        public event Action OnCloseRequested;

        public BatchRoasting Batch { get; set; }

        public ObservableCollection<FinishedGoods> FinishedGoodsList { get; set; }
        public FinishedGoods SelectedFinishedGood { get; set; }

        public ObservableCollection<RoastingProfiles> RoastingProfilesList { get; set; }
        public RoastingProfiles SelectedRoastingProfile { get; set; }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public AddEditBatchViewModel(BatchRoasting batch = null)
        {
            _productionRepository = new ProductionRepository();
            _isEditMode = batch != null && batch.BatchID > 0;
            Batch = batch ?? new BatchRoasting();

            // Load Finished Goods for dropdown
            FinishedGoodsList = new ObservableCollection<FinishedGoods>(_productionRepository.GetFinishedGoods());

            // Set selected Finished Good if editing
            if (_isEditMode)
            {
                SelectedFinishedGood = FinishedGoodsList.FirstOrDefault(fg => fg.FinishedGoodID == Batch.FinishedGoodID);
            }

            // Load Roasting Profiles for selection
            RoastingProfilesList = new ObservableCollection<RoastingProfiles>(_productionRepository.GetRoastingProfiles());

            // Set selected roasting profile if editing
            if (_isEditMode)
            {
                SelectedRoastingProfile = RoastingProfilesList.FirstOrDefault(rp => rp.ProfileID == Batch.ProfileID);
            }

            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(_ => OnCloseRequested?.Invoke());
        }

        private void Save(object parameter)
        {
            // Check if a Finished Good is selected
            if (SelectedFinishedGood != null)
            {
                Batch.FinishedGoodID = SelectedFinishedGood.FinishedGoodID; // Ensure FinishedGoodID is set
            }
            else
            {
                MessageBox.Show("Please select a Finished Good before saving.", "Missing Data", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Check if a roasting profile is selected
            if (SelectedRoastingProfile != null)
            {
                Batch.ProfileID = SelectedRoastingProfile.ProfileID; // Assigns ProfileID
            }
            else
            {
                MessageBox.Show("Please select a Roasting Profile before saving.", "Missing Data", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!_isEditMode)
            {
                Batch.BatchNumber = GenerateBatchNumber();
                Batch.ProductionDate = DateTime.Now;

                _productionRepository.AddRoastBatch(Batch);
                _productionRepository.AddFinishedGoodInventory(Batch); // Insert inventory after batch creation
            }
            else
            {
                if (Batch.ProductionDate == DateTime.MinValue)
                    Batch.ProductionDate = null;

                _productionRepository.UpdateRoastBatch(Batch);
            }

            OnCloseRequested?.Invoke();
        }

        private string GenerateBatchNumber()
        {
            return $"B-{DateTime.Now:yyyyMMddHHmmss}"; // Example: B-20240306123045
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

