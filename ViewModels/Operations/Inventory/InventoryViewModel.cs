﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using CbcRoastersErp.Models;
using CbcRoastersErp.Repositories;
using CbcRoastersErp.Helpers;
using CbcRoastersErp.Services;
using CbcRoastersErp.Models.Operations.Inventory;

namespace CbcRoastersErp.ViewModels
{
    public class InventoryViewModel : INotifyPropertyChanged
    {
        private readonly InventoryRepository _inventoryRepository;

        public ObservableCollection<GreenCoffeeInventory> GreenCoffeeStock { get; set; }
        public ObservableCollection<TeaInventory> TeaStock { get; set; }
        public ObservableCollection<PackingMaterials> PackingStock { get; set; }
        public ObservableCollection<FinishedGoods> FinishedGoodsStock { get; set; }
        public ObservableCollection<Suppliers> SuppliersList { get; private set; }
        public GreenCoffeeInventory SelectedGreenCoffee { get; set; }
        public TeaInventory SelectedTea { get; set; }
        public PackingMaterials SelectedPackingMaterial { get; set; }
        public FinishedGoods SelectedFinishedGood { get; set; }
        public ObservableCollection<BatchRoasting> ProductionBatches { get; set; }
        public ObservableCollection<FinishedGoodInventory> FinishedGoodInventory { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public event Action<string> OnNavigationRequested;
        public event Action<object> OnOpenAddEditView;

        // KPIs
        private InventoryKpiSummary _kpiSummary;
        public InventoryKpiSummary KpiSummary
        {
            get => _kpiSummary;
            set
            {
                _kpiSummary = value;
                OnPropertyChanged(nameof(KpiSummary));
            }
        }

        public ICommand OpenAddEditGreenCoffeeCommand { get; }
        public ICommand DeleteGreenCoffeeCommand { get; }

        public ICommand OpenAddEditTeaCommand { get; }
        public ICommand DeleteTeaCommand { get; }

        public ICommand OpenAddEditPackingCommand { get; }
        public ICommand DeletePackingCommand { get; }

     //   public ICommand OpenAddEditFinishedGoodsCommand { get; }
        public ICommand DeleteFinishedGoodsCommand { get; }

        public ICommand NavigateBackCommand { get; }

        public InventoryViewModel()
        {
            _inventoryRepository = new InventoryRepository();
            LoadInventory();

            // Green Coffee Commands
            OpenAddEditGreenCoffeeCommand = new RelayCommand(OpenAddEditGreenCoffee);
            DeleteGreenCoffeeCommand = new RelayCommand(DeleteGreenCoffee, CanExecuteGreenCoffeeCommand);

            // Tea Commands
            OpenAddEditTeaCommand = new RelayCommand(OpenAddEditTea);
            DeleteTeaCommand = new RelayCommand(DeleteTea, CanExecuteTeaCommand);

            // Packing Materials Commands
            OpenAddEditPackingCommand = new RelayCommand(OpenAddEditPacking);
            DeletePackingCommand = new RelayCommand(DeletePacking, CanExecutePackingCommand);

            // Finished Goods Commands
         //   OpenAddEditFinishedGoodsCommand = new RelayCommand(OpenAddEditFinishedGoods);
         //   DeleteFinishedGoodsCommand = new RelayCommand(DeleteFinishedGoods, CanExecuteFinishedGoodsCommand);

            NavigateBackCommand = new RelayCommand(_ => OnNavigationRequested?.Invoke("Dashboard"));
        }

        private void LoadInventory()
        {
            GreenCoffeeStock = new ObservableCollection<GreenCoffeeInventory>(_inventoryRepository.GetGreenCoffeeInventory());
            TeaStock = new ObservableCollection<TeaInventory>(_inventoryRepository.GetTeaInventory());
            PackingStock = new ObservableCollection<PackingMaterials>(_inventoryRepository.GetPackingMaterials());
            FinishedGoodsStock = new ObservableCollection<FinishedGoods>(_inventoryRepository.GetFinishedGoods());
            // FinishedGoodInventory = new ObservableCollection<FinishedGoodInventory>(_inventoryRepository.GetFinishedGoodInventory()); // Load finished good inventory
            LoadFinishedGoodInventory();

            SuppliersList = new ObservableCollection<Suppliers>((IEnumerable<Suppliers>)_inventoryRepository.GetSuppliers()); //Load suppliers
            ProductionBatches = new ObservableCollection<BatchRoasting>(_inventoryRepository.GetBatchRoasts()); // Load production batches

            // Load KPIs
            KpiSummary = _inventoryRepository.GetInventorySummary();
            OnPropertyChanged(nameof(KpiSummary));


            OnPropertyChanged(nameof(GreenCoffeeStock));
            OnPropertyChanged(nameof(TeaStock));
            OnPropertyChanged(nameof(PackingStock));
            OnPropertyChanged(nameof(FinishedGoodsStock));
        }

        // Green Coffee CRUD Methods
        private void OpenAddEditGreenCoffee(object parameter)
        {
            var coffeeToEdit = parameter as GreenCoffeeInventory ?? SelectedGreenCoffee;

            if (coffeeToEdit == null) // Ensure we have a valid selection
            {
                coffeeToEdit = new GreenCoffeeInventory();
            }

            var addEditViewModel = new AddEditGreenCoffeeViewModel(coffeeToEdit);
            addEditViewModel.OnCloseRequested += () =>
            {
                LoadInventory(); // Reload inventory after closing the add/edit window
                OnNavigationRequested?.Invoke("Inventory"); // Ensure correct navigation update
            };

            OnOpenAddEditView?.Invoke(addEditViewModel);
        }

        private void DeleteGreenCoffee(object parameter)
        {
            if (SelectedGreenCoffee != null)
            {
                _inventoryRepository.DeleteGreenCoffee(SelectedGreenCoffee.GreenCoffeeID);
                LoadInventory();
            }
        }

        private bool CanExecuteGreenCoffeeCommand(object parameter) => SelectedGreenCoffee != null;

        // Tea CRUD Methods
        private void OpenAddEditTea(object parameter)
        {
            var teaToEdit = parameter as TeaInventory ?? SelectedTea;
            if (teaToEdit == null) // Ensure we have a valid selection
            {
                teaToEdit = new TeaInventory();
            }
            var addEditViewModel = new AddEditTeaViewModel(parameter as TeaInventory);
            addEditViewModel.OnCloseRequested += () =>
            {
                LoadInventory(); // Reload inventory after closing the add/edit window
                OnNavigationRequested?.Invoke("Inventory"); // Ensure correct navigation update
            };

            OnOpenAddEditView?.Invoke(addEditViewModel);
        }

        private void DeleteTea(object parameter)
        {
            if (SelectedTea != null)
            {
                _inventoryRepository.DeleteTea(SelectedTea.TeaID);
                LoadInventory();
            }
        }

        private bool CanExecuteTeaCommand(object parameter) => SelectedTea != null;

        // Packing Materials CRUD Methods
        private void OpenAddEditPacking(object parameter)
        {
            var packingToEdit = SelectedPackingMaterial ?? parameter as PackingMaterials;

            if (packingToEdit == null) // If nothing is selected, open a new entry
            {
                packingToEdit = new PackingMaterials();
            }

            var addEditViewModel = new AddEditPackingViewModel(packingToEdit);
            addEditViewModel.OnCloseRequested += () =>
            {
                LoadInventory(); // Reload inventory after closing
                OnNavigationRequested?.Invoke("Inventory"); // Ensure UI updates
            };

            OnOpenAddEditView?.Invoke(addEditViewModel);
        }

        private void DeletePacking(object parameter)
        {
            if (SelectedPackingMaterial != null)
            {
                _inventoryRepository.DeletePackingMaterial(SelectedPackingMaterial.MaterialID);
                LoadInventory();
            }
        }

        private bool CanExecutePackingCommand(object parameter) => SelectedPackingMaterial != null;

        // Finished Goods CRUD Methods
        private void LoadFinishedGoodInventory()
        {
            var repo = new InventoryRepository();
            FinishedGoodInventory = new ObservableCollection<FinishedGoodInventory>(repo.GetFinishedGoodInventory());
        }

        /*
        private void OpenAddEditFinishedGoods(object parameter)
        {
            var finishedGoodToEdit = parameter as FinishedGoods ?? SelectedFinishedGood;

            if (finishedGoodToEdit == null) // Ensure we have a valid selection
            {
                finishedGoodToEdit = new FinishedGoods();
            }

            var addEditViewModel = new AddEditFinishedGoodsViewModel(finishedGoodToEdit);
            addEditViewModel.OnCloseRequested += () =>
            {
                LoadInventory(); // Reload inventory after closing the add/edit window
                OnNavigationRequested?.Invoke("Inventory"); // Ensure UI updates correctly
            };

            OnOpenAddEditView?.Invoke(addEditViewModel);
        } */


        /*
        private void DeleteFinishedGoods(object parameter)
        {
            if (SelectedFinishedGood != null)
            {
                _inventoryRepository.DeleteFinishedGood(SelectedFinishedGood.FinishedGoodID);
                LoadInventory();
            }
        }

        private bool CanExecuteFinishedGoodsCommand(object parameter) => SelectedFinishedGood != null;
        */

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
