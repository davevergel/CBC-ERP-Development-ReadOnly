using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Win32;
using CbcRoastersErp.Models;
using CbcRoastersErp.Models.Production;
using CbcRoastersErp.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Linq;
using System.Threading.Tasks;

public class RoastProfilesViewModel : INotifyPropertyChanged
{
    public ObservableCollection<RoastProfile> Profiles { get; set; } = new();
    public ObservableCollection<RoastDataPoint> DataPoints { get; set; } = new();

    public ICommand ImportCommand { get; }
    public ICommand NavigateBackCommand { get; }
    public ICommand DeleteCommand { get; }

    private RoastProfile _selectedProfile;
    public RoastProfile SelectedProfile
    {
        get => _selectedProfile;
        set
        {
            _selectedProfile = value;
            OnPropertyChanged();
            LoadChartData();
        }
    }

    public SeriesCollection ChartSeries { get; set; } = new SeriesCollection();

    private readonly RoastProfileRepository _repository;
    private readonly RoastProfileImporter _importer;

    public event Action<string> OnNavigationRequested;
    public event PropertyChangedEventHandler PropertyChanged;

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            _isLoading = value;
            OnPropertyChanged();
        }
    }

    public RoastProfilesViewModel()
    {
        _repository = new RoastProfileRepository();
        _importer = new RoastProfileImporter(_repository);

        ImportCommand = new RelayCommand(ImportProfile);
        DeleteCommand = new RelayCommand(DeleteSelectedProfile, _ => SelectedProfile != null);
        NavigateBackCommand = new RelayCommand(_ => OnNavigationRequested?.Invoke("Dashboard"));
        LoadProfiles();
    }

    private void LoadProfiles()
    {
        Profiles.Clear();
        foreach (var p in _repository.GetAllProfiles())
            Profiles.Add(p);
        OnPropertyChanged(nameof(Profiles));
    }

    private async void ImportProfile(object _)
    {
        var dialog = new OpenFileDialog
        {
            Filter = "Artisan JSON Files (*.alog;*.json)|*.alog;*.json",
            Title = "Select Roast Profile JSON"
        };
        if (dialog.ShowDialog() == true)
        {
            IsLoading = true;
            await Task.Run(() => _importer.ImportFromJsonFile(dialog.FileName));
            LoadProfiles();
            IsLoading = false;
        }
    }

    private void LoadChartData()
    {
        ChartSeries.Clear();
        DataPoints.Clear();

        if (SelectedProfile == null) return;

        var points = _repository.GetDataPointsByProfileId(SelectedProfile.Id);
        foreach (var pt in points)
            DataPoints.Add(pt);

        ChartSeries.Add(new LineSeries
        {
            Title = "Bean Temp",
            Values = new ChartValues<double>(points.Select(p => p.BeanTemp)),
            LineSmoothness = 0.8
        });
        ChartSeries.Add(new LineSeries
        {
            Title = "Env Temp",
            Values = new ChartValues<double>(points.Select(p => p.EnvironmentTemp)),
            LineSmoothness = 0.8
        });
        ChartSeries.Add(new LineSeries
        {
            Title = "ROR",
            Values = new ChartValues<double>(points.Select(p => p.ROR)),
            LineSmoothness = 0.8
        });

        OnPropertyChanged(nameof(ChartSeries));
    }

    private void DeleteSelectedProfile(object _)
    {
        if (SelectedProfile == null) return;

        var result = System.Windows.MessageBox.Show(
            $"Are you sure you want to delete the roast profile for '{SelectedProfile.BeanType}' on {SelectedProfile.RoastDate:d}?",
            "Confirm Deletion",
            System.Windows.MessageBoxButton.YesNo,
            System.Windows.MessageBoxImage.Warning);

        if (result == System.Windows.MessageBoxResult.Yes)
        {
            _repository.DeleteRoastProfile(SelectedProfile.Id);
            LoadProfiles();
            SelectedProfile = null;
            ChartSeries.Clear();
            DataPoints.Clear();
        }
    }

    protected void OnPropertyChanged([CallerMemberName] string name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
