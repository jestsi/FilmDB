using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using Films_DB.Commands;
using Films_DB.Model;

namespace Films_DB.ViewModel;

public class ApplicationViewModel : INotifyPropertyChanged
{
    private NpgSelfManager manager;
    private ObservableCollection<GenericObject> toShowTable;
    private string searchText;

    public string SearchText
    {
        get => searchText;
        set => SetField(ref searchText, value);
    }

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
            return search ??= new RelayCommand((obj) =>
            {
                if (searchText is "" or " ") return;
                var result = manager.Search(searchText, (int)obj);
                if (result.Count == 0)
                {
                    MessageBox.Show("Error finding", "No results");
                    ToShowTable = new ObservableCollection<GenericObject>();
                } else
                {
                    ToShowTable = result;
                }
            });
        }
    }
    
    public RelayCommand ClearSearch
    {
        get
        {
            return clearSearch ??= new RelayCommand((obj) =>
            {
                SearchText = "";
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