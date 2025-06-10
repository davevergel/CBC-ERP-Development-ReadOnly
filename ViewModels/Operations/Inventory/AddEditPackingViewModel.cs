using System;
using System.ComponentModel;
using System.Windows.Input;
using CbcRoastersErp.Models;
using CbcRoastersErp.Repositories;
using CbcRoastersErp.Helpers;
using CbcRoastersErp.Services;

namespace CbcRoastersErp.ViewModels
{
    public class AddEditPackingViewModel : INotifyPropertyChanged
    {
        private readonly InventoryRepository _inventoryRepository;
        private PackingMaterials _packingMaterial;
        private bool _isEditMode;

        public event PropertyChangedEventHandler PropertyChanged;
        public event Action OnCloseRequested;

        public PackingMaterials PackingMaterial
        {
            get => _packingMaterial;
            set
            {
                _packingMaterial = value;
                OnPropertyChanged(nameof(PackingMaterial));
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public AddEditPackingViewModel(PackingMaterials packingMaterial = null)
        {
            _inventoryRepository = new InventoryRepository();
            _isEditMode = packingMaterial != null && packingMaterial.MaterialID > 0;
            PackingMaterial = packingMaterial ?? new PackingMaterials();
            PackingMaterial = packingMaterial ?? new PackingMaterials { LastUpdated = DateTime.Now }; // Initialize LastUpdated

            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(_ => OnCloseRequested?.Invoke());
        }

        private void Save(object parameter)
        {
            if (_isEditMode)
            { 
            _inventoryRepository.UpdatePackingMaterial(PackingMaterial);
            }
            else
            { 
            _inventoryRepository.AddPackingMaterial(PackingMaterial);
            }
            OnCloseRequested?.Invoke();
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

