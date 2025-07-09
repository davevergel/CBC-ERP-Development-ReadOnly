using System.Collections.ObjectModel;
using System.ComponentModel;
using CbcRoastersErp.Helpers;
using CbcRoastersErp.Models.Purchasing;
using CbcRoastersErp.Services;
using System.Windows.Input;
using CbcRoastersErp.Repositories;

namespace CbcRoastersErp.ViewModels.Purchasing
{
    public class PurchaseOrderViewModel : INotifyPropertyChanged
    {
        private readonly PurchaseOrderRepository _repository;

        private ObservableCollection<PurchaseOrder> _purchaseOrders;
        public ObservableCollection<PurchaseOrder> PurchaseOrders
        {
            get => _purchaseOrders;
            set
            {
                if (_purchaseOrders != value)
                {
                    _purchaseOrders = value;
                    OnPropertyChanged(nameof(PurchaseOrders));
                }
            }
        }

        private PurchaseOrder _selectedOrder;
        public PurchaseOrder SelectedOrder
        {
            get => _selectedOrder;
            set
            {
                if (_selectedOrder != value)
                {
                    _selectedOrder = value;
                    OnPropertyChanged(nameof(SelectedOrder));
                    (DeleteCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        public ICommand LoadCommand { get; }
        public ICommand NavigateBackCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand OpenAddEditPurchaseCommand { get; }

        public Action NavigateBackAction { get; set; }
        public Action<PurchaseOrder> NavigateToEditViewAction { get; set; }

        public PurchaseOrderViewModel()
        {
            _repository = new PurchaseOrderRepository();
            LoadCommand = new RelayCommand(async () => await LoadOrdersAsync());
            NavigateBackCommand = new RelayCommand(_ => OnNavigationRequested?.Invoke("Dashboard"));
            DeleteCommand = new RelayCommand(async () => await DeleteSelectedAsync(), CanExecuteDelete);
            OpenAddEditPurchaseCommand = new RelayCommand(OpenAddEditPurchaseOrder);

            //load orders
            LoadCommand.Execute(null);
        }

        private async Task OpenAddEditPurchaseOrder()
        {
            PurchaseOrder PurchaseOrderToEdit;
            if (SelectedOrder != null)
            {
                PurchaseOrderToEdit = await _repository.GetByIdAsync(SelectedOrder.PurchaseOrderId);
            }
            else
            {
                PurchaseOrderToEdit = new PurchaseOrder();
            }

            var addEditViewModel = new PurchaseOrderEditViewModel(PurchaseOrderToEdit);
            addEditViewModel.OnCloseRequested += async () =>
            {
                await LoadOrdersAsync();
                OnNavigationRequested?.Invoke("PurchaseOrder");
            };

            OnOpenAddEditView?.Invoke(addEditViewModel);
        }

        private async Task LoadOrdersAsync()
        {
            try
            {
                var orders = await _repository.GetAllAsync();
                PurchaseOrders = new ObservableCollection<PurchaseOrder>(orders);
            }
            catch (Exception ex)
            {
                ApplicationLogger.Log(ex, nameof(LoadOrdersAsync), nameof(PurchaseOrderViewModel), Environment.UserName);

            }
        }

        private bool CanExecuteDelete(object parameter) => SelectedOrder != null;

        private async Task DeleteSelectedAsync()
        {
            if (SelectedOrder == null)
                return;

            bool success = await _repository.DeleteAsync(SelectedOrder.PurchaseOrderId);
            if (success)
                await LoadOrdersAsync();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public event Action<string> OnNavigationRequested;
        public event Action<object> OnOpenAddEditView;

        protected virtual void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

