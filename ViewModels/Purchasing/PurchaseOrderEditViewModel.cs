using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using CbcRoastersErp.Repositories;
using CbcRoastersErp.Models.Purchasing;
using CbcRoastersErp.Helpers;
using CbcRoastersErp.Models;
using CbcRoastersErp.Services;
using CbcRoastersErp.Repositories.Purchasing;
using QuestPDF.Fluent;
using System.IO;
using QuestPDF.Companion;
using System.Diagnostics;
using System.Text;
using UglyToad.PdfPig;
using CbcRoastersErp.Services.Parsers;

namespace CbcRoastersErp.ViewModels
{
    public class PurchaseOrderEditViewModel : INotifyPropertyChanged
    {
        private readonly PurchaseOrderRepository _repository;
        private readonly SupplierRepository _supplierRepository;
        private readonly InvoicePdfParserService _invoiceParser = new InvoicePdfParserService();

        public event PropertyChangedEventHandler PropertyChanged;
        public event Action OnCloseRequested;
        public event Action OnPrintRequested;

        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public bool IsEditMode => Order?.PurchaseOrderId > 0;

        private PurchaseOrder _order;
        public PurchaseOrder Order
        {
            get => _order;
            set
            {
                if (_order != value)
                {
                    _order = value;
                    OnPropertyChanged(nameof(Order));
                }
            }
        }

        private Suppliers _selectedSupplier;
        public Suppliers SelectedSupplier
        {
            get => _selectedSupplier;
            set
            {
                if (_selectedSupplier != value)
                {
                    _selectedSupplier = value;
                    Order.Supplier_id = value?.Supplier_id ?? 0;
                    OnPropertyChanged(nameof(SelectedSupplier));
                    OnPropertyChanged(nameof(Order));
                    RecalculateTotal(); // in case tax logic is tied to supplier
                }
            }
        }

        public ObservableCollection<string> StatusOptions { get; } = new()
        {
            "Pending", "Approved", "Ordered", "Partially Received", "Received", "Cancelled", "Closed"
        };

        public ObservableCollection<Suppliers> SupplierOptions { get; set; } = new();
        public ObservableCollection<PurchaseOrderItem> OrderItems => Order.Items;

        private PurchaseOrderItem _selectedItem;
        public PurchaseOrderItem SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (_selectedItem != value)
                {
                    _selectedItem = value;
                    OnPropertyChanged(nameof(SelectedItem));
                }
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand AddItemCommand { get; }
        public ICommand RemoveItemCommand { get; }
        public ICommand PrintCommand { get; }
        public ICommand UploadInvoiceCommand { get; }
        public ICommand ViewInvoiceCommand { get; }
        public ICommand ImportFromPdfCommand { get; }
        public ICommand ExportToPdfCommand { get; }

        public PurchaseOrderEditViewModel(PurchaseOrder order = null)
        {
            _repository = new PurchaseOrderRepository();
            _supplierRepository = new SupplierRepository();

            Order = order != null
                ? new PurchaseOrder
                {
                    PurchaseOrderId = order.PurchaseOrderId,
                    po_number = order.po_number,
                    Supplier_id = order.Supplier_id,
                    OrderDate = order.OrderDate,
                    Status = order.Status,
                    TotalAmount = order.TotalAmount,
                    Subtotal = order.Subtotal,
                    TaxAmount = order.TaxAmount,
                    ShippingCost = order.ShippingCost,
                    InvoicePdfPath  = order.InvoicePdfPath,
                    Items = new ObservableCollection<PurchaseOrderItem>(order.Items)
                }
                : new PurchaseOrder
                {
                    OrderDate = DateTime.Today,
                    Status = "Pending",
                    ShippingCost = 0.0m,
                    Items = new ObservableCollection<PurchaseOrderItem>()
                };

            SaveCommand = new RelayCommand(async () => await SaveAsync());
            CancelCommand = new RelayCommand(_ => OnCloseRequested?.Invoke());
            AddItemCommand = new RelayCommand(_ => AddNewItem());
            RemoveItemCommand = new RelayCommand(_ => RemoveSelectedItem(), _ => SelectedItem != null);
            PrintCommand = new RelayCommand(_ => OnPrintRequested?.Invoke());
            UploadInvoiceCommand = new RelayCommand(_ => UploadInvoicePdf());
            ViewInvoiceCommand = new RelayCommand(_ => ViewInvoicePdf(), _ => !string.IsNullOrWhiteSpace(Order.InvoicePdfPath) && File.Exists(Order.InvoicePdfPath));
            ImportFromPdfCommand = new RelayCommand(_ => ImportFromPdf());

            ExportToPdfCommand = new RelayCommand(async () =>
            {
                await ExportToPdfAsync();
            });

            foreach (var item in Order.Items)
                item.PropertyChanged += Item_PropertyChanged;

            // Listen for shipping cost changes
            Order.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(Order.ShippingCost))
                    RecalculateTotal();
            };

            LoadSuppliers();
            RecalculateTotal();
        }

        private async void LoadSuppliers()
        {
            try
            {
                SupplierOptions.Clear();
                var suppliers = await _supplierRepository.GetAllAsync();
                foreach (var s in suppliers)
                    SupplierOptions.Add(s);

                SelectedSupplier = SupplierOptions.FirstOrDefault(s => s.Supplier_id == Order.Supplier_id);
                OnPropertyChanged(nameof(SupplierOptions));
            }
            catch (Exception ex)
            {
                ApplicationLogger.Log(ex, nameof(LoadSuppliers), nameof(PurchaseOrderEditViewModel), System.Environment.UserName);
            }
        }

        private void AddNewItem()
        {
            var item = new PurchaseOrderItem
            {
                ProductName = "New Item",
                Quantity = 1,
                UnitPrice = 0.0m
            };
            item.PropertyChanged += Item_PropertyChanged;
            Order.Items.Add(item);
            OnPropertyChanged(nameof(OrderItems));
            RecalculateTotal();
        }

        private void RemoveSelectedItem()
        {
            if (SelectedItem != null)
            {
                SelectedItem.PropertyChanged -= Item_PropertyChanged;
                Order.Items.Remove(SelectedItem);
                OnPropertyChanged(nameof(OrderItems));
                RecalculateTotal();
            }
        }

        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(PurchaseOrderItem.Quantity) ||
                e.PropertyName == nameof(PurchaseOrderItem.UnitPrice))
            {
                RecalculateTotal();
            }
        }

        private void RecalculateTotal()
        {
            Order.Subtotal = Order.Items.Sum(i => i.Quantity * i.UnitPrice);

            bool isWholesale = SelectedSupplier?.Supplier_Name?.ToLower().Contains("wholesale") == true;
            decimal taxRate = isWholesale ? 0.0m : 0.08m;

            Order.TaxAmount = System.Math.Round(Order.Subtotal * taxRate, 2);
            Order.TotalAmount = Order.Subtotal + Order.TaxAmount + Order.ShippingCost;

            OnPropertyChanged(nameof(Order));
        }

        private async Task SaveAsync()
        {
            try
            {
                var resultId = await _repository.AddOrUpdateAsync(Order);
                if (resultId > 0)
                    OnCloseRequested?.Invoke();
            }
            catch (Exception ex)
            {
                ApplicationLogger.Log(ex, nameof(SaveAsync), nameof(PurchaseOrderEditViewModel), System.Environment.UserName);
            }
        }

        public async Task ExportToPdfAsync(string filePath = null)
        {
            try
            {
                QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

                var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                const string resourceName = "CbcRoastersErp.Assets.Logos.CBC_Logo_500x500.png";

                using var stream = assembly.GetManifestResourceStream(resourceName);
                if (stream == null)
                    throw new FileNotFoundException($"Could not find embedded resource: {resourceName}");

                using var ms = new MemoryStream();
                await stream.CopyToAsync(ms);
                var logoBytes = ms.ToArray();

                var document = new PurchaseOrderDocument(Order, SelectedSupplier, logoBytes);

                // Generate a temporary preview PDF
                var tempPath = Path.Combine(Path.GetTempPath(), $"PO_{Order.PurchaseOrderId}_Preview.pdf");
                document.GeneratePdf(tempPath);

                // Open the preview PDF in default viewer
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = tempPath,
                    UseShellExecute = true
                });

                // Ask user to export final copy
                var dialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "PDF files (*.pdf)|*.pdf",
                    FileName = $"PO_{Order.PurchaseOrderId}.pdf"
                };

                if (dialog.ShowDialog() == true)
                {
                    document.GeneratePdf(dialog.FileName);
                }
            }
            catch (Exception ex)
            {
                ApplicationLogger.Log(ex, nameof(ExportToPdfAsync), nameof(PurchaseOrderEditViewModel), Environment.UserName);
            }
        }

        private void UploadInvoicePdf()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "PDF files (*.pdf)|*.pdf",
                Title = "Select Invoice PDF"
            };

            if (dialog.ShowDialog() == true)
            {
                string destDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Invoices");
                Directory.CreateDirectory(destDir);
                string destFile = Path.Combine(destDir, $"PO_{Order.PurchaseOrderId}_{Path.GetFileName(dialog.FileName)}");
                File.Copy(dialog.FileName, destFile, true);
                Order.InvoicePdfPath = destFile;
                OnPropertyChanged(nameof(Order));
            }
        }

        private void ViewInvoicePdf()
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(Order.InvoicePdfPath) && File.Exists(Order.InvoicePdfPath))
                {
                    Process.Start(new ProcessStartInfo(Order.InvoicePdfPath) { UseShellExecute = true });
                }
            }
            catch (Exception ex)
            {
                ApplicationLogger.Log(ex, nameof(ViewInvoicePdf), nameof(PurchaseOrderEditViewModel), Environment.UserName);
            }
        }

        // Create PO from PDF
        private async void ImportFromPdf()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "PDF files (*.pdf)|*.pdf",
                Title = "Select Supplier Invoice PDF"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    var supplierRepo = new SupplierRepository();
                    var allSuppliers = await supplierRepo.GetAllAsync();

                    var supplier = SelectedSupplier; // Assuming user selects supplier first
                    if (supplier == null)
                    {
                        // Prompt user to select supplier before importing
                        return;
                    }

                    var parserManager = new InvoiceParserManager();
                    var result = parserManager.ParseInvoice(dialog.FileName, supplier.Supplier_Name);

                    Order.Items.Clear();
                    foreach (var item in result.Items)
                        Order.Items.Add(item);

                    Order.Subtotal = result.Subtotal;
                    Order.TaxAmount = result.TaxAmount;
                    Order.ShippingCost = result.ShippingCost;
                    Order.TotalAmount = result.TotalAmount;

                    OnPropertyChanged(nameof(OrderItems));
                    RecalculateTotal();
                }
                catch (Exception ex)
                {
                    ApplicationLogger.Log(ex, nameof(ImportFromPdf), nameof(PurchaseOrderEditViewModel), Environment.UserName);
                }
            }
        }



    }
}
