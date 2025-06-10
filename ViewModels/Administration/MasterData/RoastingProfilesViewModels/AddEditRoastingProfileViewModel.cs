using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using CbcRoastersErp.Models;
using CbcRoastersErp.Services;

namespace CbcRoastersErp.ViewModels.Administration.MasterData.RoastingProfilesViewModels
{
    public class AddEditRoastingProfileViewModel : INotifyPropertyChanged
    {
        // Events
        public event PropertyChangedEventHandler? PropertyChanged;
        public event Action OnCloseRequested;

        private readonly Repositories.ProductionRepository _productionRepository;
        private RoastingProfiles _roastingProfile;
        private bool _isEditMode;

        public ObservableCollection<Models.RoastingProfiles> RoastingProfilesList { get; set; }
        public RoastingProfiles SelectedRoastingProfile { get; set; }

        public ObservableCollection<GreenCoffeeInventory> GreenCoffeeList { get; set; }
        public GreenCoffeeInventory SelectedGreenCoffee { get; set; }
        public RoastingProfiles RoastingProfile
        {
            get => _roastingProfile;
            set
            {
                _roastingProfile = value;
                OnPropertyChanged(nameof(RoastingProfile));
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        // Constructor
        public AddEditRoastingProfileViewModel(RoastingProfiles roastingProfile = null)
        {
            _productionRepository = new Repositories.ProductionRepository();
            _isEditMode = roastingProfile != null && roastingProfile.ProfileID > 0;
            RoastingProfile = roastingProfile ?? new RoastingProfiles();

            // Load green coffee inventory for selection
            GreenCoffeeList = new ObservableCollection<GreenCoffeeInventory>(_productionRepository.GetGreenCoffees());

            // Set selected green coffee if editing
            if (_isEditMode)
            {
                SelectedGreenCoffee = GreenCoffeeList.FirstOrDefault(gc => gc.GreenCoffeeID == RoastingProfile.GreenCoffeeID);
            }

            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(_ => OnCloseRequested?.Invoke());
        }

        private void Save(object parameter)
        {
            if (_isEditMode)
            {
                // Update existing profile
                _productionRepository.UpdateRoastingProfile(RoastingProfile);
            }
            else
            {
                // Add new profile
                _productionRepository.AddRoastingProfile(RoastingProfile);
            }
            OnCloseRequested?.Invoke();

            MessageBox.Show("Roasting Profile saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }


        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }

}
