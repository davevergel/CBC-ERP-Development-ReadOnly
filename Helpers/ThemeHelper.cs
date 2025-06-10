using MaterialDesignThemes.Wpf;


namespace CbcRoastersErp.Helpers
{
    public static class ThemeHelper
    {
        private static readonly PaletteHelper _paletteHelper = new();

        public static bool IsDarkTheme()
        {
            var theme = _paletteHelper.GetTheme();
            return theme.GetBaseTheme() == BaseTheme.Dark;
        }

        public static void ToggleTheme()
        {
            var theme = _paletteHelper.GetTheme();

            // Switch to appropriate IBaseTheme
            IBaseTheme newBaseTheme = theme.GetBaseTheme() == BaseTheme.Dark
                ? new MaterialDesignLightTheme()
                : new MaterialDesignDarkTheme();

            theme.SetBaseTheme(newBaseTheme);
            _paletteHelper.SetTheme(theme);
        }

        public static void SetDarkTheme()
        {
            var theme = _paletteHelper.GetTheme();
            theme.SetBaseTheme(new MaterialDesignDarkTheme());
            _paletteHelper.SetTheme(theme);
        }

        public static void SetLightTheme()
        {
            var theme = _paletteHelper.GetTheme();
            theme.SetBaseTheme(new MaterialDesignLightTheme());
            _paletteHelper.SetTheme(theme);
        }
    }
}

