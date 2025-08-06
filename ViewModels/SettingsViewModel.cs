// ViewModels/SettingsViewModel.cs
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;
using CbcRoastersErp.Services;
using Microsoft.Win32;

public class SettingsViewModel : INotifyPropertyChanged
{
    private string _artisanPath;
    private string _operator;
    private string _defaultOrigin;
    private string _templatePath;
    private string _defaultWeight;

    public string ArtisanPath { get => _artisanPath; set { _artisanPath = value; OnPropertyChanged(); } }
    public string Operator { get => _operator; set { _operator = value; OnPropertyChanged(); } }
    public string DefaultOrigin { get => _defaultOrigin; set { _defaultOrigin = value; OnPropertyChanged(); } }
    public string TemplatePath { get => _templatePath; set { _templatePath = value; OnPropertyChanged(); } }
    public string DefaultWeight { get => _defaultWeight; set { _defaultWeight = value; OnPropertyChanged(); } }

    public ICommand BrowseCommand { get; }
    public ICommand BrowseTemplateCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand NavigateBackCommand => new RelayCommand(_ => OnNavigationRequested?.Invoke("Dashboard"));

    public SettingsViewModel()
    {
        BrowseCommand = new RelayCommand(_ => BrowseForArtisan());
        BrowseTemplateCommand = new RelayCommand(_ => BrowseForTemplate());
        SaveCommand = new RelayCommand(_ => SaveSettings());
        LoadSettings();
    }

    private void BrowseForArtisan()
    {
        var dialog = new OpenFileDialog
        {
            Filter = "Executable files (*.exe)|*.exe",
            Title = "Select Artisan Executable"
        };
        var result = dialog.ShowDialog();
        if (result == true)
        {
            ArtisanPath = dialog.FileName;
        }
    }

    private void BrowseForTemplate()
    {
        var dialog = new OpenFileDialog
        {
            Filter = "Roast Template (*.alog;*.json)|*.alog;*.json",
            Title = "Select Artisan Roast Template"
        };
        var result = dialog.ShowDialog();
        if (result == true)
        {
            TemplatePath = dialog.FileName;
        }
    }

    private void LoadSettings()
    {
        try
        {
            var json = File.ReadAllText("appsettings.json");
            using var doc = JsonDocument.Parse(json);
            if (doc.RootElement.TryGetProperty("Artisan", out var artisan))
            {
                if (artisan.TryGetProperty("ExecutablePath", out var path)) ArtisanPath = path.GetString();
                if (artisan.TryGetProperty("Operator", out var op)) Operator = op.GetString();
                if (artisan.TryGetProperty("DefaultOrigin", out var origin)) DefaultOrigin = origin.GetString();
                if (artisan.TryGetProperty("DefaultWeight", out var weight)) DefaultWeight = weight.GetString();
                if (artisan.TryGetProperty("TemplatePath", out var tpl)) TemplatePath = tpl.GetString();
            }
        }
        catch { }
    }

    private void SaveSettings()
    {
        try
        {
            string json = File.ReadAllText("appsettings.json");
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement.Clone();
            using var stream = File.CreateText("appsettings.json");

            var updated = new
            {
                Artisan = new
                {
                    ExecutablePath = ArtisanPath,
                    Operator = Operator,
                    DefaultOrigin = DefaultOrigin,
                    DefaultWeight = DefaultWeight,
                    TemplatePath = TemplatePath
                },
                ConnectionStrings = new
                {
                    DefaultConnection = root.GetProperty("ConnectionStrings").GetProperty("DefaultConnection").GetString()
                }
            };

            string output = JsonSerializer.Serialize(updated, new JsonSerializerOptions { WriteIndented = true });
            stream.Write(output);
            MessageBox.Show("Settings saved.", "Success");
        }
        catch (Exception ex)
        {
            MessageBox.Show("Failed to save settings: " + ex.Message, "Error");
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    public event Action<string> OnNavigationRequested;
    protected void OnPropertyChanged([CallerMemberName] string name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
