using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CbcRoastersErp.Factories;
using CbcRoastersErp.Models;
using CbcRoastersErp.Repositories;
using CbcRoastersErp.Services;

public class SupplierEditViewModel : IAddEditViewModel, INotifyPropertyChanged
{
    public Suppliers Supplier { get; set; }
    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }

    private readonly SupplierRepositoryAdmin _repository = new();
    private readonly bool _isNew;

    public SupplierEditViewModel(Suppliers supplier = null)
    {
        Supplier = supplier != null ? new Suppliers
        {
            Supplier_id = supplier.Supplier_id,
            Supplier_Name = supplier.Supplier_Name,
            Contact_email = supplier.Contact_email,
            Contact_phone = supplier.Contact_phone,
            Address = supplier.Address
        } : new Suppliers();

        _isNew = supplier == null;

        SaveCommand = new RelayCommand(_ => Save());
        CancelCommand = new RelayCommand(_ => OnCloseRequested?.Invoke());
    }

    private void Save()
    {
        if (_isNew)
            _repository.Add(Supplier);
        else
            _repository.Update(Supplier);

        OnSaved?.Invoke();
    }

    // Events
    public event Action OnCloseRequested;
    public event PropertyChangedEventHandler PropertyChanged;
    public event Action OnSaved;
    public event Action OnCancel;

    protected void OnPropertyChanged([CallerMemberName] string name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

}

