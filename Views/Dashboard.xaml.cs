using System.Windows;
using System.Windows.Controls;
using CbcRoastersErp.Helpers;

namespace CbcRoastersErp.Views
{
    /// <summary>
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Dashboard : UserControl
    {
        public Dashboard()
        {
            InitializeComponent();
            Loaded += Dashboard_Loaded;
        }

        private void ThemeToggle_Click(object sender, RoutedEventArgs e)
        {
            ThemeHelper.ToggleTheme();
        }

        private void Dashboard_Loaded(object sender, RoutedEventArgs e)
        {
            // Set initial theme based on user preference or system settings
            if (ThemeHelper.IsDarkTheme())
            {
                ThemeHelper.SetDarkTheme();
            }
            else
            {
                ThemeHelper.SetLightTheme();
            }

        }
    }
}
