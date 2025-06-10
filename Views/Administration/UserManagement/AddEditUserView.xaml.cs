using System.Windows;
using System.Windows.Controls;
using CbcRoastersErp.ViewModels;

namespace CbcRoastersErp.Views
{
    /// <summary>
    /// Interaction logic for AddEditUserView.xaml
    /// </summary>
    public partial class AddEditUserView : UserControl
    {
        public AddEditUserView()
        {
            InitializeComponent();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is AddEditUserViewModel vm)
            {
                vm.PlainPassword = ((PasswordBox)sender).Password;
            }
        }
    }
}
