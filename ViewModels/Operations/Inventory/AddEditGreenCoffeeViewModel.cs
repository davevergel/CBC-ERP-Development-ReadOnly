using System;
using System.ComponentModel;
using System.Windows.Input;
using CbcRoastersErp.Models;
using CbcRoastersErp.Repositories;
using CbcRoastersErp.Helpers;
using CbcRoastersErp.Services;
using System.Collections.ObjectModel;

namespace CbcRoastersErp.ViewModels
{
    public class AddEditGreenCoffeeViewModel : INotifyPropertyChanged
    {
        private readonly InventoryRepository _inventoryRepository;
        private GreenCoffeeInventory _greenCoffee;
        private bool _isEditMode;

        public event PropertyChangedEventHandler PropertyChanged;
        public event Action OnCloseRequested; // Event to close the view after saving/canceling
        public ObservableCollection<Suppliers> SuppliersList { get; set; } // List of suppliers
        public Suppliers SelectedSupplier { get; set; } // Selected supplier from the list

        public GreenCoffeeInventory GreenCoffee
        {
            get => _greenCoffee;
            set
            {
                _greenCoffee = value;
                OnPropertyChanged(nameof(GreenCoffee));
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public AddEditGreenCoffeeViewModel(GreenCoffeeInventory greenCoffee = null)
        {
            _inventoryRepository = new InventoryRepository();
            _isEditMode = greenCoffee != null && greenCoffee.GreenCoffeeID > 0; // Ensure it's an existing item
            GreenCoffee = greenCoffee ?? new GreenCoffeeInventory();

            SuppliersList = new ObservableCollection<Suppliers>((IEnumerable<Suppliers>)_inventoryRepository.GetSuppliers());



            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(_ => OnCloseRequested?.Invoke());
        }

        private void Save(object parameter)
        {
            if (SelectedSupplier != null)
            {
                GreenCoffee.SupplierId = SelectedSupplier.Supplier_id; // Store the selected supplier ID
                GreenCoffee.SupplierName = SelectedSupplier.Supplier_Name; // Store the supplier name for display
            }

            if (_isEditMode)
            {
                _inventoryRepository.UpdateGreenCoffee(GreenCoffee);
            }
            else
                _inventoryRepository.AddGreenCoffee(GreenCoffee);

            OnCloseRequested?.Invoke();
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
