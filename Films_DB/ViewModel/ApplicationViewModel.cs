using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using Films_DB.Commands;
using Films_DB.Model;

namespace Films_DB.ViewModel;

public class ApplicationViewModel : INotifyPropertyChanged
{
    private NpgSelfManager manager;
    private ObservableCollection<GenericObject> toShowTable;
    public NpgSelfManager Manager
    {
        get => manager;
        set
        {
            manager = value;
            OnPropertyChanged(nameof(manager));
        }
    }

    public ObservableCollection<GenericObject> ToShowTable
    {
        get => toShowTable;
        set => SetField(ref toShowTable, value);
    }

    private RelayCommand search;
    private RelayCommand clearSearch;
    public RelayCommand Search
    {
        get
        {
            return search ?? 
                   (search = new RelayCommand((obj) =>
                   {
                       var objInStr = obj.ToString();
                       if (objInStr is "" or " ") return;
                ToShowTable = manager.Search(objInStr);
            }));
        }
    }
    
    public RelayCommand ClearSearch
    {
        get
        {
            return clearSearch ??= new RelayCommand(_ =>
            {
                ToShowTable = manager.Table;
            });
        }
    }

    public ApplicationViewModel()
    {
        manager = new NpgSelfManager();
        ToShowTable = manager.Table;
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}