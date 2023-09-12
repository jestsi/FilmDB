using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using Films_DB.Commands;
using Films_DB.Model;

namespace Films_DB.ViewModel;

public class ApplicationViewModel : INotifyPropertyChanged
{
    private NpgSelfManager manager;
    public NpgSelfManager Manager
    {
        get => manager;
        set
        {
            manager = value;
            OnPropertyChanged(nameof(manager));
        }
    }

    public ApplicationViewModel()
    {
        manager = new NpgSelfManager();
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