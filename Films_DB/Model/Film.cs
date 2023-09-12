using System;
using System.Collections.Generic;
using System.Windows.Documents;

namespace Films_DB.Model;

public partial class FilmPreResult
{
    public int id { get; set; }
    public string name { get; set; }
    public string? description { get; set; }
    public int year { get; set; }
    public int? movieLength { get; set; }
    public Premiere premiere { get; set; }
}

public class Premiere
{
    public string Country { get; set; }
    public DateTimeOffset World { get; set; }
    public DateTimeOffset Russia { get; set; }
    public string Digital { get; set; }
    public DateTimeOffset Cinema { get; set; }
    public string Bluray { get; set; }
    public string Dvd { get; set; }

    public static string IsNotInRussia(DateTimeOffset Russia, DateTimeOffset Eng)
    {
        var rus = Russia.ToString("MM/dd/yyyy");
        return rus == "01.01.0001" ? Eng.ToString("MM/dd/yyyy") : rus;
    }
}

