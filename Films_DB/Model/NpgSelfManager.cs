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

public class NpgSelfManager : INotifyPropertyChanged
{
    private string connectionString =  ConfigurationManager.ConnectionStrings["FilmDB"].ConnectionString;
    private NpgsqlConnection vCon;
    private NpgsqlCommand vCmd;
    private ObservableCollection<GenericObject> table;
    public ObservableCollection<GenericObject> Table
    {
        get => table;
        set
        {
            table = value;
            OnPropertyChanged(nameof(table));
        }
    } 

    public NpgSelfManager()
    {
        table = GenericObject.Convert(GetData("select * from \"FilmDatas\""));

        
    }
    private void connection()
    {
        try
        {
            vCon = new NpgsqlConnection(connectionString);
            if (vCon.State==ConnectionState.Closed) vCon.Open();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            MessageBox.Show($"Error {e.Message}","Error", 
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    
    public DataTable GetData(string sql)
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
    
    private bool AddData(FilmPreResult data)
    {
        connection();
        vCmd = new NpgsqlCommand(null, vCon);
        vCmd.CommandText = "INSERT INTO \"FilmDatas\" " +
                           "(\"ReleaseDate\", \"LengthMinutes\", \"Name\", \"Description\", \"Premiere\")" +
                           "VALUES (@ReleaseDate, @LengthMinutes, @Name, @Description, @Premiere)";
        vCmd.Parameters.AddWithValue("@ReleaseDate", data.year);
        vCmd.Parameters.AddWithValue("@LengthMinutes", (int)(data.movieLength ?? 0));
        vCmd.Parameters.AddWithValue("@Name", data.name);
        vCmd.Parameters.AddWithValue("@Description", (string)(data.description ?? " "));
        vCmd.Parameters.AddWithValue("@Premiere", data.premiere.World.ToString("MM/dd/yyyy"));

        try
        {
            vCmd.ExecuteNonQuery();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }

        return true;
    }
    
    private bool SetMoreData(int count)
    {
        foreach (var _ in Enumerable.Range(0, count))
        {
            var s = JsonSerializer.Deserialize<FilmPreResult>(
                Get("https://api.kinopoisk.dev/v1/movie/random")
            );
            AddData(s);
        }
        return true;
    }
    
    private static string Get(string url)
    {
        var web = new WebClient();
        const string paramName = "X-API-KEY";
        web.Headers.Add(paramName, ReadSetting(paramName));
        return web.DownloadString(url);
    }
    private static string ReadSetting(string key)  
    {  
        try  
        {  
            var appSettings = ConfigurationManager.AppSettings;
            return appSettings[key] ?? "Not Found";
        }  
        catch (ConfigurationErrorsException)  
        {  
            Console.WriteLine("Error reading app settings");
            return "Not found";
        }  
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