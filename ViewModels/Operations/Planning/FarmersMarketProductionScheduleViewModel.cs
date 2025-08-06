using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CbcRoastersErp.Models.Planning;
using CbcRoastersErp.Repositories.Operations.Planning;
using CbcRoastersErp.Helpers;
using CbcRoastersErp.Services;
using System.Reflection.Metadata;

namespace CbcRoastersErp.ViewModels.Operations.Planning
{
    public class FarmersMarketProductionScheduleViewModel : INotifyPropertyChanged
    {
        private readonly FarmersMarketProductionScheduleRepository _scheduleRepository;
        private FarmersMarketProductionSchedule _selectedSchedule;

        public ObservableCollection<FarmersMarketProductionSchedule> Schedules { get; set; } = new();

        private int _currentPage = 1;
        private int _totalPages;
        private const int PageSize = 25;

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

        public event Action<string> OnNavigationRequested;
        public event Action<object> OnOpenAddEditView;
        public event PropertyChangedEventHandler PropertyChanged;

        public FarmersMarketProductionSchedule SelectedSchedule
        {
            get => _selectedSchedule;
            set
            {
                _selectedSchedule = value;
                OnPropertyChanged(nameof(SelectedSchedule));
            }
        }

        public ICommand AddScheduleCommand { get; }
        public ICommand OpenScheduleItemsCommand { get; }

        public ICommand DeleteScheduleCommand { get; }
        public ICommand OpenAddEditScheduleCommand { get; }
        public ICommand NavigateBackCommand { get; }
        public ICommand PageChangedCommand { get; }

        public FarmersMarketProductionScheduleViewModel()
        {
            _scheduleRepository = new FarmersMarketProductionScheduleRepository();
            AddScheduleCommand = new RelayCommand(async (_) => await AddScheduleAsync());
            DeleteScheduleCommand = new RelayCommand(async (_) => await DeleteScheduleAsync(), CanExecuteScheduleCommand);
            OpenScheduleItemsCommand = new RelayCommand(OpenScheduleItems, CanExecuteScheduleCommand);

            OpenAddEditScheduleCommand = new RelayCommand(OpenAddEditScheduleAsync);
           // OpenAddEditScheduleCommand = new RelayCommand(OpenAddEditScheduleAsync);
            NavigateBackCommand = new RelayCommand(_ => OnNavigationRequested?.Invoke("Dashboard"));
            PageChangedCommand = new RelayCommand(async param => await ChangePageAsync(param));

            _ = LoadSchedulesAsync();
        }

        private async Task LoadSchedulesAsync()
        {
            try
            {
                var totalItems = await _scheduleRepository.GetSchedulesCountAsync();
                TotalPages = (int)Math.Ceiling((double)totalItems / PageSize);

                var pagedSchedules = await _scheduleRepository.GetSchedulesPagedAsync(CurrentPage, PageSize);
                Schedules = new ObservableCollection<FarmersMarketProductionSchedule>(pagedSchedules);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading schedules: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                OnPropertyChanged(nameof(Schedules));
                OnPropertyChanged(nameof(CurrentPage));
                OnPropertyChanged(nameof(TotalPages));
            }
        }

        private async Task ChangePageAsync(object param)
        {
            if (param?.ToString() == "Next" && CurrentPage < TotalPages)
                CurrentPage++;
            else if (param?.ToString() == "Previous" && CurrentPage > 1)
                CurrentPage--;

            await LoadSchedulesAsync();
        }

        private async Task AddScheduleAsync()
        {
            var newSchedule = new FarmersMarketProductionSchedule
            {
                MarketDate = DateTime.Now,
                CreatedBy = Environment.UserName,
                CreatedAt = DateTime.Now
            };

            var addEditViewModel = new AddEditFarmersMarketScheduleViewModel(newSchedule);
            addEditViewModel.OnCloseRequested += async () =>
            {
                await LoadSchedulesAsync(); // Refresh list after add/edit
                OnNavigationRequested?.Invoke("FarmersMarketProductionSchedule");
            };

            OnOpenAddEditView?.Invoke(addEditViewModel);
        }

        private async Task OpenAddEditScheduleAsync(object parameter)
        {
            var scheduleToEdit = (parameter as FarmersMarketProductionSchedule ?? SelectedSchedule) ?? new FarmersMarketProductionSchedule
                {
                    MarketDate = DateTime.Now,
                    CreatedBy = Environment.UserName,
                    CreatedAt = DateTime.Now
                };

            // var fullSchedule = await _scheduleRepository.GetScheduleByIdAsync(SelectedSchedule.Id);
            var editViewModel = new AddEditFarmersMarketScheduleViewModel(scheduleToEdit);
            editViewModel.OnCloseRequested += async () =>
            {
                await LoadSchedulesAsync(); // Refresh list after edit
                OnNavigationRequested?.Invoke("FarmersMarketProductionSchedule");
            };

            OnOpenAddEditView?.Invoke(editViewModel);
        }

        private void OpenScheduleItems(object parameter)
        {
            if (SelectedSchedule == null) return;

            CurrentScheduleContext.Instance.SelectedScheduleId = SelectedSchedule.Id;
            OnNavigationRequested?.Invoke("FarmersMarketProductionScheduleItems");
        }

        private async Task DeleteScheduleAsync()
        {
            if (SelectedSchedule == null)
                return;

            var result = MessageBox.Show("Are you sure you want to delete this schedule?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                await _scheduleRepository.DeleteScheduleAsync(SelectedSchedule.Id);
                await LoadSchedulesAsync();
            }
        }

        private bool CanExecuteScheduleCommand(object parameter)
        {
            return SelectedSchedule != null;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
