using System.ComponentModel;

namespace CbcRoastersErp.Models.Purchasing
{
    public class PurchaseOrderItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private int _purchaseOrderItemId;
        public int PurchaseOrderItemId
        {
            get => _purchaseOrderItemId;
            set
            {
                if (_purchaseOrderItemId != value)
                {
                    _purchaseOrderItemId = value;
                    OnPropertyChanged(nameof(PurchaseOrderItemId));
                }
            }
        }

        private int _purchaseOrderId;
        public int PurchaseOrderId
        {
            get => _purchaseOrderId;
            set
            {
                if (_purchaseOrderId != value)
                {
                    _purchaseOrderId = value;
                    OnPropertyChanged(nameof(PurchaseOrderId));
                }
            }
        }

        private string _productName;
        public string ProductName
        {
            get => _productName;
            set
            {
                if (_productName != value)
                {
                    _productName = value;
                    OnPropertyChanged(nameof(ProductName));
                }
            }
        }

        private int _quantity;
        public int Quantity
        {
            get => _quantity;
            set
            {
                if (_quantity != value)
                {
                    _quantity = value;
                    OnPropertyChanged(nameof(Quantity));
                }
            }
        }

        private decimal _unitPrice;
        public decimal UnitPrice
        {
            get => _unitPrice;
            set
            {
                if (_unitPrice != value)
                {
                    _unitPrice = value;
                    OnPropertyChanged(nameof(UnitPrice));
                }
            }
        }
    }
}

