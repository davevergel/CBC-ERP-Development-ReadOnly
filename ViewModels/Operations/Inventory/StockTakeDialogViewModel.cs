using System;
using System.ComponentModel;
using System.Windows.Input;
using CbcRoastersErp.Helpers;
using CbcRoastersErp.Services;

namespace CbcRoastersErp.ViewModels.Inventory
{
    public class StockTakeViewModel : INotifyPropertyChanged
    {
        public int ItemId { get; set; }
        public string ItemType { get; set; }
        public string ItemName { get; set; }
        public int RecordedQuantity { get; set; }
        private int _CountedQuantity;
        public int CountedQuantity 
        {
            get => _CountedQuantity;
            set
            {
                _CountedQuantity = value;
                OnPropertyChanged(nameof(CountedQuantity));
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public event Action OnCloseRequested;
        public event Action<string, int, int> OnSaveRequested;
        public event PropertyChangedEventHandler? PropertyChanged;

        public StockTakeViewModel()
        {
            SaveCommand = new RelayCommand(_ => Save());
            CancelCommand = new RelayCommand(_ => OnCloseRequested?.Invoke());
        }

        private void Save()
        {
            OnSaveRequested?.Invoke(ItemType, ItemId, CountedQuantity);
            OnCloseRequested?.Invoke();
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}


