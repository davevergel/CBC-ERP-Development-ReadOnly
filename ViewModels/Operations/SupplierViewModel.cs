using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CbcRoastersErp.Models;
using CbcRoastersErp.Repositories;
using CbcRoastersErp.Services;

public class SupplierViewModel : INotifyPropertyChanged
{
    private readonly SupplierRepositoryAdmin _repository = new();

    public ObservableCollection<Suppliers> Suppliers { get; set; } = new();
    public ICommand AddCommand { get; }
    public ICommand EditCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand BackCommand { get; }

    private Suppliers _selectedSupplier;
    public Suppliers SelectedSupplier
    {
        get => _selectedSupplier;
        set { _selectedSupplier = value; OnPropertyChanged(); }
    }

    public SupplierViewModel()
    {
        LoadSuppliers();
        AddCommand = new RelayCommand(_ => OpenEdit(null));
        EditCommand = new RelayCommand(_ => OpenEdit(SelectedSupplier), _ => SelectedSupplier != null);
        DeleteCommand = new RelayCommand(_ => Delete(), _ => SelectedSupplier != null);
        BackCommand = new RelayCommand(_ => OnNavigationRequested?.Invoke("Dashboard"));
    }

    private void LoadSuppliers()
    {
        Suppliers.Clear();
        foreach (var s in _repository.GetAll())
            Suppliers.Add(s);
    }

    private void Delete()
    {
        _repository.Delete(SelectedSupplier.Supplier_id);
        LoadSuppliers();
    }

    private void OpenEdit(Suppliers supplier)
    {
        Suppliers supplierToEdit;
        if (SelectedSupplier != null)
        {
            supplierToEdit = _repository.GetById(SelectedSupplier.Supplier_id);
        }
        else
        {
            supplierToEdit = new Suppliers();
        }
        var editViewModel = new SupplierEditViewModel(supplierToEdit);
        editViewModel.OnCloseRequested += () =>
        {
            LoadSuppliers();
            OnOpenAddEditView?.Invoke(null);
        };
        OnOpenAddEditView?.Invoke(editViewModel); // Notify the view to open the edit view

    }

    // Events
    public Action<string> OnNavigationRequested { get; internal set; }
    public Action<object> OnOpenAddEditView { get; set; }
    public event Action<string> OnDeleteRequested;
    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}