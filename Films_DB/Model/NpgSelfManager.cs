using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows;
using Npgsql;

namespace Films_DB.Model;

public sealed class NpgSelfManager : INotifyPropertyChanged
{
    private readonly string connectionString = ConfigurationManager.ConnectionStrings["FilmDB"].ConnectionString;
    private ObservableCollection<GenericObject> table;
    private NpgsqlCommand vCmd;
    private NpgsqlConnection vCon;
    
    public NpgSelfManager()
    {
       // SetMoreData(160);

        table = GenericObject.Convert(GetData("select * from \"FilmDatas\""));
    }

    public ObservableCollection<GenericObject> Table
    {
        get => table;
        set => SetField(ref table, value);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public ObservableCollection<GenericObject> Search(string finding_data, int find_data_type = -1)
    {            
        ObservableCollection<GenericObject> res = new();

        var variants = new List<string>{ "Name", "Description" };

        foreach (var obj in table)
        {
            var x = obj.Properties.FirstOrDefault();

            if (find_data_type == -1)
            {
                 x = obj.Properties.FirstOrDefault(x => x.Value.ToString()
                    .Contains(finding_data, StringComparison.InvariantCultureIgnoreCase));
            } else
            {
                x = obj.Properties.FirstOrDefault(x => x.Value.ToString()
                        .Contains(finding_data, StringComparison.InvariantCultureIgnoreCase) && x.Name == variants[find_data_type]);
            }
            if (x is not null) res.Add(obj);
        }
        return res;
    }
    
    private void connection()
    {
        try
        {
            vCon = new NpgsqlConnection(connectionString);
            if (vCon.State == ConnectionState.Closed) vCon.Open();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            MessageBox.Show($"Error {e.Message}", "Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private DataTable GetData(string sql)
    {
        var dt = new DataTable();
        connection();
        vCmd = new NpgsqlCommand();
        vCmd.Connection = vCon;
        vCmd.CommandText = sql;

        var dr = vCmd.ExecuteReader();
        dt.Load(dr);
        return dt;
    }

    private void AddData(FilmPreResult data)
    {
        connection();
        vCmd = new NpgsqlCommand(null, vCon);
        vCmd.CommandText = "INSERT INTO \"FilmDatas\" " +
                           "(\"ReleaseDate\", \"LengthMinutes\", \"Name\", \"Description\")" +
                           "VALUES (@ReleaseDate, @LengthMinutes, @Name, @Description)";
        vCmd.Parameters.AddWithValue("@ReleaseDate", data.year);
        vCmd.Parameters.AddWithValue("@LengthMinutes", data.movieLength ?? 0);
        vCmd.Parameters.AddWithValue("@Name", data.name);
        vCmd.Parameters.AddWithValue("@Description", (string)(data.description ?? " "));

        vCmd.ExecuteNonQuery();
    }

    private bool SetMoreData(int count)
    {
        foreach (var _ in Enumerable.Range(0, count))
        {
            var s = JsonSerializer.Deserialize<FilmPreResult>(
                Get("https://api.kinopoisk.dev/v1/movie/random")
            );
            if (CheckOnNull(s))
                AddData(s);
        }

        return true;
    }

    private static bool CheckOnNull(FilmPreResult result)
    {
        if (string.IsNullOrEmpty(result.description)) return false;
        return result.movieLength is not (null or 0);
    }

    private static string Get(string url)
    {
        var web = new WebClient();
        const string paramName = "X-API-KEY";
        web.Headers.Add(paramName, ConfigManager.ReadSetting(paramName));
        return web.DownloadString(url);
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}