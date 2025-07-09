using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CbcRoastersErp.ViewModels;

namespace CbcRoastersErp.Views.Purchasing
{
    /// <summary>
    /// Interaction logic for PurchaseOrderEditView.xaml
    /// </summary>
    public partial class PurchaseOrderEditView : UserControl
    {
        public PurchaseOrderEditView()
        {
            InitializeComponent();
            Loaded += PurchaseOrderEditView_Loaded;
        }

        private void PurchaseOrderEditView_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is PurchaseOrderEditViewModel vm)
            {
                vm.OnPrintRequested += () =>
                {
                    PrintPurchaseOrder();
                };
            }
        }

        private void PrintPurchaseOrder()
        {
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                // Example: Print entire user control
                printDialog.PrintVisual(this, "Purchase Order");
            }
        }
    }
}
