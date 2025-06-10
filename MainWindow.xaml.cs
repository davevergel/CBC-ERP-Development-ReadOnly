using System.Windows;
using CbcRoastersErp.Views;
using CbcRoastersErp.ViewModels;

namespace CbcRoastersErp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DashboardViewModel _dashboardViewModel;
        private InventoryView _inventoryView;
        private ProductionView _productionView;
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
    }
}