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
    public class AddEditTeaViewModel : INotifyPropertyChanged
    {
        private readonly InventoryRepository _inventoryRepository;
        private TeaInventory _tea;
        private bool _isEditMode;

        public event PropertyChangedEventHandler PropertyChanged;
        public event Action OnCloseRequested;

        public ObservableCollection<Suppliers> SuppliersList { get; set; }
        public Suppliers SelectedSupplier { get; set; }

        public TeaInventory Tea
        {
            get => _tea;
            set
            {
                _tea = value;
                OnPropertyChanged(nameof(Tea));
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public AddEditTeaViewModel(TeaInventory tea = null)
        {
            _inventoryRepository = new InventoryRepository();
            _isEditMode = tea != null && tea.TeaID > 0;  // Ensure it's an exisiting item
            Tea = tea ?? new TeaInventory();

            SuppliersList = new ObservableCollection<Suppliers>((IEnumerable<Suppliers>)_inventoryRepository.GetSuppliers());

            // Set selected supplier if editing
            SelectedSupplier = SuppliersList.FirstOrDefault(s => s.Supplier_id == Tea.SupplierId);

            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(_ => OnCloseRequested?.Invoke());
        }

        private void Save(object parameter)
        {
            if (SelectedSupplier != null)
            {
                Tea.SupplierId = SelectedSupplier.Supplier_id; // ✅ Store the selected supplier ID
            }

            if (_isEditMode)
                _inventoryRepository.UpdateTea(Tea);
            else
                _inventoryRepository.AddTea(Tea);

            OnCloseRequested?.Invoke();
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

