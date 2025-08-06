using System.Windows;
using Microsoft.Extensions.Configuration;
using CbcRoastersErp.Services;
using CbcRoastersErp.Helpers;
using CbcRoastersErp.Helpers.converters;
using CbcRoastersErp.Helpers.Converters;
using CbcRoastersErp.Converters;

namespace CbcRoastersErp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Load Configuration
            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            ConfigHelper.Initialize(config);
            DatabaseService.Initialize(config);

            // Register global resources
            Resources.Add("BoolToVisibilityConverter", new BoolToVisibilityConverter { CollapseInsteadOfHidden = true });
            Resources.Add("IsGreaterThanOneConverter", new IsGreaterThanOneConverter());
            Resources.Add("IsLessThanTotalPagesConverter", new IsLessThanTotalPagesConverter());
            Resources.Add("BooleanToVisibilityConverter", new BooleanToVisibilityConverter() );
            Resources.Add("InverseBooleanToVisibilityConverter", new InverseBooleanToVisibilityConverter());
            Resources.Add("NullToBoolConverter", new NullToBoolConverter());
            Resources.Add("BooleanToModeConverter", new BooleanToModeConverter());
            Resources.Add("NullOrZeroToAddEditTitleConverter", new NullOrZeroToAddEditTitleConverter());
        }
    }

}
