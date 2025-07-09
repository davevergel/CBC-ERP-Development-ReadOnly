using System.Windows;
using MaterialDesignThemes.Wpf;


namespace CbcRoastersErp.Helpers
{
    public static class ThemeHelper
    {
        private static readonly PaletteHelper _paletteHelper = new();
        private static bool _isDarkMode = false;

        public static bool IsDarkTheme()
        {
            var theme = _paletteHelper.GetTheme();
            return theme.GetBaseTheme() == BaseTheme.Dark;
        }

        public static void ToggleTheme()
        {
            var app = Application.Current;
            if (app == null) return;

            var dictionaries = app.Resources.MergedDictionaries;

            string newTheme = _isDarkMode ? "Themes/LightTheme.xaml" : "Themes/DarkTheme.xaml";
            dictionaries.Add(new ResourceDictionary() { Source = new Uri(newTheme, UriKind.Relative) });

            _isDarkMode = !_isDarkMode;

            // Optionally also update MaterialDesign base theme for controls that use PaletteHelper
            var theme = _paletteHelper.GetTheme();
            IBaseTheme newBaseTheme = _isDarkMode
                ? new MaterialDesignDarkTheme()
                : new MaterialDesignLightTheme();

            theme.SetBaseTheme(newBaseTheme);
            _paletteHelper.SetTheme(theme);
        }

        public static void SetDarkTheme()
        {
            var app = Application.Current;
            if (app == null) return;

            var dictionaries = app.Resources.MergedDictionaries;

            // Remove LightTheme if loaded
            var lightDict = dictionaries
                .FirstOrDefault(d => d.Source != null && d.Source.OriginalString.Contains("LightTheme.xaml"));
            if (lightDict != null)
                dictionaries.Remove(lightDict);

            // Add DarkTheme
            dictionaries.Add(new ResourceDictionary() { Source = new Uri("Themes/DarkTheme.xaml", UriKind.Relative) });

            _isDarkMode = true;

            var theme = _paletteHelper.GetTheme();
            theme.SetBaseTheme(new MaterialDesignDarkTheme());
            _paletteHelper.SetTheme(theme);
        }

        public static void SetLightTheme()
        {
            var app = Application.Current;
            if (app == null) return;

            var dictionaries = app.Resources.MergedDictionaries;

            // Remove DarkTheme if loaded
            var darkDict = dictionaries
                .FirstOrDefault(d => d.Source != null && d.Source.OriginalString.Contains("DarkTheme.xaml"));
            if (darkDict != null)
                dictionaries.Remove(darkDict);

            // Add LightTheme
            dictionaries.Add(new ResourceDictionary() { Source = new Uri("Themes/LightTheme.xaml", UriKind.Relative) });

            _isDarkMode = false;

            var theme = _paletteHelper.GetTheme();
            theme.SetBaseTheme(new MaterialDesignLightTheme());
            _paletteHelper.SetTheme(theme);
        }
    }
}

