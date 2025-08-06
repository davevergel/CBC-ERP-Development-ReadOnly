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

namespace CbcRoastersErp.ViewModels.Operations.Planning
{
    public class FarmersMarketProductionScheduleItemsViewModel : INotifyPropertyChanged
    {
        private readonly FarmersMarketProductionScheduleItemRepository _itemRepository;

        public event PropertyChangedEventHandler PropertyChanged;
        public event Action<string> OnNavigationRequested;
        public event Action<object> OnOpenAddEditView;

        public ObservableCollection<FarmersMarketProductionScheduleItem> ScheduleItems { get; set; } = new();

        private FarmersMarketProductionScheduleItem _selectedItem;
        public FarmersMarketProductionScheduleItem SelectedItem
        {
            get => _selectedItem;
            set { _selectedItem = value; OnPropertyChanged(); }
        }

        public int ScheduleId { get; }

        public ICommand AddItemCommand { get; }
        public ICommand EditItemCommand { get; }
        public ICommand DeleteItemCommand { get; }
        public ICommand NavigateBackCommand { get; }
        public ICommand PageChangedCommand { get; }

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

        public FarmersMarketProductionScheduleItemsViewModel(int scheduleId)
        {
            _itemRepository = new FarmersMarketProductionScheduleItemRepository();
            ScheduleId = scheduleId;

            AddItemCommand = new RelayCommand(async (_) => await AddItemAsync());
            EditItemCommand = new RelayCommand(async (_) => await EditItemAsync(), CanExecuteItemCommand);
            DeleteItemCommand = new RelayCommand(async (_) => await DeleteItemAsync(), CanExecuteItemCommand);
            NavigateBackCommand = new RelayCommand(_ => OnNavigationRequested?.Invoke("FarmersMarketProductionSchedule"));
            PageChangedCommand = new RelayCommand(async param => await ChangePageAsync(param));

            _ = LoadItemsAsync();
        }

        private async Task LoadItemsAsync()
        {
            try
            {
                var count = await _itemRepository.GetItemsCountByScheduleIdAsync(ScheduleId);
                TotalPages = (int)Math.Ceiling((double)count / PageSize);

                var items = await _itemRepository.GetItemsPagedByScheduleIdAsync(ScheduleId, CurrentPage, PageSize);
                ScheduleItems = new ObservableCollection<FarmersMarketProductionScheduleItem>(items);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading items: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                OnPropertyChanged(nameof(ScheduleItems));
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

            await LoadItemsAsync();
        }

        private async Task AddItemAsync()
        {
            var newItem = new FarmersMarketProductionScheduleItem
            {
                ScheduleId = ScheduleId,
                RoastDate = DateTime.Now
            };

            var addEditViewModel = new AddEditFarmersMarketProductionScheduleItemViewModel(newItem);
            addEditViewModel.OnCloseRequested += async () =>
            {
                await LoadItemsAsync();
                OnNavigationRequested?.Invoke("FarmersMarketProductionScheduleItems");
            };

            OnOpenAddEditView?.Invoke(addEditViewModel);
        }

        private async Task EditItemAsync()
        {
            if (SelectedItem == null)
                return;

            var fullItem = await _itemRepository.GetItemByIdAsync(SelectedItem.Id);
            var editViewModel = new AddEditFarmersMarketProductionScheduleItemViewModel(fullItem);

            editViewModel.OnCloseRequested += async () =>
            {
                await LoadItemsAsync();
                OnNavigationRequested?.Invoke("FarmersMarketProductionScheduleItems");
            };

            OnOpenAddEditView?.Invoke(editViewModel);
        }

        private async Task DeleteItemAsync()
        {
            if (SelectedItem == null)
                return;

            var result = MessageBox.Show("Are you sure you want to delete this item?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                await _itemRepository.DeleteItemAsync(SelectedItem.Id);
                await LoadItemsAsync();
            }
        }

        private bool CanExecuteItemCommand(object parameter)
        {
            return SelectedItem != null;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
