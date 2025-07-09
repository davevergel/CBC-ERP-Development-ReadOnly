using System.Collections.ObjectModel;
using System.ComponentModel;

namespace CbcRoastersErp.Models.Purchasing
{
    public class PurchaseOrder : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

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

        private string _poNumber;
        public string po_number
        {
            get => _poNumber;
            set
            {
                if (_poNumber != value)
                {
                    _poNumber = value;
                    OnPropertyChanged(nameof(po_number));
                }
            }
        }


        private int _supplierId;
        public int Supplier_id
        {
            get => _supplierId;
            set
            {
                if (_supplierId != value)
                {
                    _supplierId = value;
                    OnPropertyChanged(nameof(Supplier_id));
                }
            }
        }

        private DateTime _orderDate;
        public DateTime OrderDate
        {
            get => _orderDate;
            set
            {
                if (_orderDate != value)
                {
                    _orderDate = value;
                    OnPropertyChanged(nameof(OrderDate));
                }
            }
        }

        private string _status;
        public string Status
        {
            get => _status;
            set
            {
                if (_status != value)
                {
                    _status = value;
                    OnPropertyChanged(nameof(Status));
                }
            }
        }

        private decimal _totalAmount;
        public decimal TotalAmount
        {
            get => _totalAmount;
            set
            {
                if (_totalAmount != value)
                {
                    _totalAmount = value;
                    OnPropertyChanged(nameof(TotalAmount));
                }
            }
        }

        private DateTime _createdAt;
        public DateTime CreatedAt
        {
            get => _createdAt;
            set
            {
                if (_createdAt != value)
                {
                    _createdAt = value;
                    OnPropertyChanged(nameof(CreatedAt));
                }
            }
        }

        private string _supplierName;
        public string SupplierName
        {
            get => _supplierName;
            set
            {
                if (_supplierName != value)
                {
                    _supplierName = value;
                    OnPropertyChanged(nameof(SupplierName));
                }
            }
        }

        private decimal _subtotal;
        public decimal Subtotal
        {
            get => _subtotal;
            set
            {
                if (_subtotal != value)
                {
                    _subtotal = value;
                    OnPropertyChanged(nameof(Subtotal));
                }
            }
        }

        private decimal _taxAmount;
        public decimal TaxAmount
        {
            get => _taxAmount;
            set
            {
                if (_taxAmount != value)
                {
                    _taxAmount = value;
                    OnPropertyChanged(nameof(TaxAmount));
                }
            }
        }

        private decimal _shippingCost;
        public decimal ShippingCost
        {
            get => _shippingCost;
            set
            {
                if (_shippingCost != value)
                {
                    _shippingCost = value;
                    OnPropertyChanged(nameof(ShippingCost));
                }
            }
        }
        private string _invoicePdfPath;
        public string InvoicePdfPath
        {
            get => _invoicePdfPath;
            set
            {
                if (_invoicePdfPath != value)
                {
                    _invoicePdfPath = value;
                    OnPropertyChanged(nameof(InvoicePdfPath));
                }
            }
        }

        public ObservableCollection<PurchaseOrderItem> Items { get; set; } = new();
    }
}

