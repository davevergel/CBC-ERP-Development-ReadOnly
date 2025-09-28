using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using CbcRoastersErp.ViewModels.Reporting;

namespace CbcRoastersErp.Views.Reporting
{
    public partial class InventoryReportView : UserControl
    {
        public InventoryReportView()
        {
            InitializeComponent();

            Loaded += (s, e) =>
            {
                if (DataContext is InventoryReportViewModel vm)
                {
                    vm.PropertyChanged += Vm_PropertyChanged;

                    // Ensure it loads any pre-generated PDF at startup
                    if (!string.IsNullOrWhiteSpace(vm.ReportPath) && File.Exists(vm.ReportPath))
                    {
                        PdfViewer.Load(vm.ReportPath);
                    }
                }
            };

            this.DataContextChanged += InventoryReportView_DataContextChanged;
        }

        private void InventoryReportView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is InventoryReportViewModel vm)
            {
                vm.PropertyChanged += Vm_PropertyChanged;
            }
        }

        private void Vm_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ReportPath")
            {
                if (sender is InventoryReportViewModel vm &&
                    !string.IsNullOrWhiteSpace(vm.ReportPath) &&
                    File.Exists(vm.ReportPath))
                {
                    Dispatcher.Invoke(() => PdfViewer.Load(vm.ReportPath));
                }
            }
        }
    }
}