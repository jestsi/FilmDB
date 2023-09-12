using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;

namespace Films_DB.Model;

public class GenericObject
{
    private readonly ObservableCollection<GenericProperty> properties = new ObservableCollection<GenericProperty>();

    public GenericObject(params GenericProperty[] properties)
    {
        foreach (var property in properties)
            Properties.Add(property);
    }

    public ObservableCollection<GenericProperty> Properties
    {
        get { return properties; }
    }

    public object? Find(string nameProp)
    {
        return (from prop in properties where prop.Name == nameProp select prop.Value).FirstOrDefault();
    }
    
    public static ObservableCollection<GenericObject> Convert(DataTable toConvert)
    {
        var _result = new ObservableCollection<GenericObject>();

        foreach (DataRow _row in toConvert.Rows)
        {
            var _genericObject = new GenericObject();
            foreach (DataColumn _column in toConvert.Columns)
            {
                _genericObject.Properties.Add(new GenericProperty(_column.ColumnName,_row[_column]));
            }
            _result.Add(_genericObject);
        }

        return _result;
    }
}

public class GenericProperty : INotifyPropertyChanged
{
    public GenericProperty(string name, object value)
    {
        Name = name;
        Value = value;
    }

    public string Name { get; private set; }
    public object Value { get; set; }


    public event PropertyChangedEventHandler PropertyChanged;
}


