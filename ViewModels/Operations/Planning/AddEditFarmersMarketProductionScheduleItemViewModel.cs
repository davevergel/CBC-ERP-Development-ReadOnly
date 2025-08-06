using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CbcRoastersErp.Helpers;
using CbcRoastersErp.Models;
using CbcRoastersErp.Models.Planning;
using CbcRoastersErp.Repositories;
using CbcRoastersErp.Repositories.Operations.Planning;
using CbcRoastersErp.Services;

namespace CbcRoastersErp.ViewModels.Operations.Planning
{
    public class AddEditFarmersMarketProductionScheduleItemViewModel : INotifyPropertyChanged
    {
        private readonly FarmersMarketProductionScheduleItemRepository _itemRepository;
        private readonly ProductionRepository _productionRepository;

        public event PropertyChangedEventHandler PropertyChanged;
        public event Action OnCloseRequested;

        public FarmersMarketProductionScheduleItem Item { get; set; }
        public List<FinishedGoods> ProductList { get; set; }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public AddEditFarmersMarketProductionScheduleItemViewModel(FarmersMarketProductionScheduleItem item)
        {
            _itemRepository = new FarmersMarketProductionScheduleItemRepository();
            _productionRepository = new ProductionRepository();

            Item = item ?? new FarmersMarketProductionScheduleItem
            {
                RoastDate = DateTime.Now
            };

            ProductList = LoadProductList();

            SaveCommand = new RelayCommand(async (_) => await SaveItemAsync());
            CancelCommand = new RelayCommand(_ => OnCloseRequested?.Invoke());
        }

        private List<FinishedGoods> LoadProductList()
        {
            try
            {
                return new List<FinishedGoods>(_productionRepository.GetAllFinishedGoods());
            }
            catch (Exception ex)
            {
                ApplicationLogger.Log(ex, nameof(AddEditFarmersMarketProductionScheduleItemViewModel), nameof(LoadProductList), Environment.UserName);
                return new List<FinishedGoods>();
            }
        }

        private async Task SaveItemAsync()
        {
            try
            {
                if (Item.Id == 0)
                {
                    int newId = await _itemRepository.AddItemAsync(Item);
                    if (newId > 0)
                        Item.Id = newId;
                }
                else
                {
                    await _itemRepository.UpdateItemAsync(Item);
                }

                OnCloseRequested?.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving item: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
