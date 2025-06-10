using System;
using System.ComponentModel;
using System.Windows.Input;
using CbcRoastersErp.Models;
using CbcRoastersErp.Repositories;
using CbcRoastersErp.Helpers;
using CbcRoastersErp.Services;
using System.Collections.ObjectModel;
using System.Windows;

namespace CbcRoastersErp.ViewModels.Administration.MasterData.FinishedGoodsViewModels
{
    public class AddEditMdFinishedGoodsViewModel : INotifyPropertyChanged
    {
        private readonly ProductionRepository _productionRepository;
        private FinishedGoods _finishedGood;
        private bool _isEditMode;

        public ObservableCollection<BatchRoasting> FinishedGoodsList { get; set; }
        public FinishedGoods SelectedFinishedGood { get; set; }
        public ObservableCollection<RoastingProfiles> RoastingProfilesList { get; set; }
        public RoastingProfiles SelectedRoastingProfile { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;
        public event Action OnCloseRequested;

        public FinishedGoods FinishedGood
        {
            get => _finishedGood;
            set
            {
                _finishedGood = value;
                OnPropertyChanged(nameof(FinishedGood));
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public AddEditMdFinishedGoodsViewModel(FinishedGoods finishedGood = null)
        {
            _productionRepository = new ProductionRepository();
            _isEditMode = finishedGood != null && finishedGood.FinishedGoodID > 0;
            FinishedGood = finishedGood ?? new FinishedGoods();



            // Load Roasting Profiles for selection
            RoastingProfilesList = new ObservableCollection<RoastingProfiles>(_productionRepository.GetRoastingProfiles());

            // Set selected roasting profile if editing
            if (_isEditMode)
            {
                SelectedRoastingProfile = RoastingProfilesList.FirstOrDefault(rp => rp.ProfileID == FinishedGood.ProfileID);
            }

            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(_ => OnCloseRequested?.Invoke());
        }

        private void Save(object parameter)
        {
            if (_isEditMode)
            {
                // Update existing Finished Good
                _productionRepository.UpdateFinishedGood(FinishedGood);
            }
            else
            {
                // Create new Finished Good
                _productionRepository.AddFinishedGood(FinishedGood);
            }
            OnCloseRequested?.Invoke();

            MessageBox.Show("Finished Good saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

