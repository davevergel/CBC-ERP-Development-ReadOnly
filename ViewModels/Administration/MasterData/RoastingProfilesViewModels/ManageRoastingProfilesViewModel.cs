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
using CbcRoastersErp.Views;
using MaterialDesignThemes.Wpf;

namespace CbcRoastersErp.ViewModels.Administration.MasterData.RoastingProfilesViewModels
{
    public class ManageRoastingProfilesViewModel : INotifyPropertyChanged
    {
        // Events
        public event PropertyChangedEventHandler? PropertyChanged;
        public event Action<string>? OnNavigationRequested;
        public event Action<object> OnOpenAddEditView;

        // production repository
        private readonly ProductionRepository _roastingProfilesRepository;
        // private List<RoastingProfile> _roastingProfilesList;
        private RoastingProfiles _roastingProfile;
        public ObservableCollection<RoastingProfiles> RoastingProfiles { get; private set; }
        public RoastingProfiles SelectedRoastingProfile
        {
            get => _roastingProfile;
            set
            {
                _roastingProfile = value;
                OnPropertyChanged(nameof(SelectedRoastingProfile));
                // Update command availability based on selection
                ((RelayCommand)DeleteRoastingProfileCommand).RaiseCanExecuteChanged();
            }
        }

        public ObservableCollection<GreenCoffeeInventory> GreenCoffeeList { get; set; }
        public GreenCoffeeInventory SelectedGreenCoffee { get; set; }

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

        // ICommands
        public ICommand AddEditRoastingProfileCommand { get; }
        public ICommand DeleteRoastingProfileCommand { get; }
        public ICommand NavigateBackCommand => new RelayCommand(_ => OnNavigationRequested?.Invoke("MasterDataDashDb"));
        public ICommand PageChangedCommand => new RelayCommand(param =>
        {
            if (param is int page)
            {
                CurrentPage = page;
                LoadRoastingProfiles();
            }
        });
        // Constructor
        public ManageRoastingProfilesViewModel()
        {
            _roastingProfilesRepository = new ProductionRepository();
            RoastingProfiles = new ObservableCollection<RoastingProfiles>();
            LoadRoastingProfiles();

            // Initialize commands
            AddEditRoastingProfileCommand = new RelayCommand(OpenAddEditRoastingProfiles);
            DeleteRoastingProfileCommand = new RelayCommand(DeleteRoastingProfilesAsync, CanExecuteRoastingProfileCommand);
        }
        private void LoadRoastingProfiles()
        {
            try
            {
                var roastingProfilesList = _roastingProfilesRepository.GetRoastingProfilesPaged(CurrentPage, PageSize);
                RoastingProfiles = new ObservableCollection<RoastingProfiles>(roastingProfilesList);
                TotalPages = (int)Math.Ceiling((double)_roastingProfilesRepository.GetTotalRoastingProfilesCount() / PageSize);

                // Load Green Coffee Inventory for selection
                GreenCoffeeList = new ObservableCollection<GreenCoffeeInventory>(_roastingProfilesRepository.GetGreenCoffees());

                OnPropertyChanged(nameof(RoastingProfiles));
                OnPropertyChanged(nameof(GreenCoffeeList));
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., log them)
                MessageBox.Show($"Error loading roasting profiles: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                ApplicationLogger.Log(ex, "Error loading roasting profiles");
            }
            finally
            {
                OnPropertyChanged(nameof(TotalPages));
                OnPropertyChanged(nameof(CurrentPage));
                OnPropertyChanged(nameof(RoastingProfiles));
                OnPropertyChanged(nameof(GreenCoffeeList));
            }
        }

        private void OpenAddEditRoastingProfiles(object paramenter)
        {
            RoastingProfiles roastingProfileToEdit;

            if (SelectedRoastingProfile != null && SelectedRoastingProfile.ProfileID > 0)
            {
                roastingProfileToEdit = _roastingProfilesRepository.GetRoastingProfileById(SelectedRoastingProfile.ProfileID);
                SelectedGreenCoffee = GreenCoffeeList.FirstOrDefault(gc => gc.GreenCoffeeID == roastingProfileToEdit.GreenCoffeeID);
            }
            else
            {
                roastingProfileToEdit = new RoastingProfiles();
            }

            var addEditViewModel = new AddEditRoastingProfileViewModel(roastingProfileToEdit);
            addEditViewModel.OnCloseRequested += () =>
            {
                LoadRoastingProfiles();
                OnNavigationRequested?.Invoke("RoastingProfiles");
            };
            OnOpenAddEditView?.Invoke(addEditViewModel);
        }

        private async Task DeleteRoastingProfilesAsync(object parameter)
        {
            if (SelectedRoastingProfile == null || SelectedRoastingProfile.ProfileID <= 0)
            {
                MessageBox.Show("Please select a valid roasting profile to delete.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var dialog = new ConfirmDeleteDialog($"Are you sure you want to delete the roasting profile '{SelectedRoastingProfile.ProfileName}'?");
            var result = await DialogHost.Show(dialog, "RootDialog");
            if (result is bool accepted && accepted)
            {
                try
                {
                    await _roastingProfilesRepository.DeleteRoastingProfileAsync(SelectedRoastingProfile.ProfileID);
                    LoadRoastingProfiles();
                    MessageBox.Show("Roasting profile deleted successfully.", "Success", MessageBoxButton.OK);
                    LoadRoastingProfiles();
                }
                catch (Exception ex)
                {
                    // Handle exceptions (e.g., log them)
                    MessageBox.Show($"Error deleting roasting profile: {ex.Message}", "Error", MessageBoxButton.OK);
                    ApplicationLogger.Log(ex, "Error deleting roasting profile");
                }

                // Refresh the list after deletion
                LoadRoastingProfiles();
            }
        }
        private bool CanExecuteRoastingProfileCommand(object parameter)
        {
            // Enable command only if a roasting profile is selected
            return SelectedRoastingProfile != null && SelectedRoastingProfile.ProfileID > 0;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
