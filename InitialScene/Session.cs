using System;
using System.Collections.Generic;
using System.Linq;

public class Session
{
    public IEnumerable<Province> Provinces { get; set; }
    public IEnumerable<Country> Countries { get; set; }
    public Country Player { get; internal set; }

    public Session(int provinceCount, int countryCount)
    {
        Provinces = Enumerable.Range(0, provinceCount).Select(_ => new Province()).ToArray();
        Countries = Enumerable.Range(0, countryCount).Select(_ => new Country()).ToArray();
    }
}


public class Country
{
    static int count;

    public static Func<Country, IEnumerable<Province>> FindProvinces;

    public string Name { get; private set; }
    public IEnumerable<Province> Provinces => FindProvinces(this);

    public Country()
    {
        Name = $"C{count}";
        count++;
    }
}

public class Province
{
    static int count;

    public static Func<Province, int> FindPopCount;

    public int PopCount => FindPopCount(this);

    public string Name { get; private set; }

    public Province()
    {
        Name = $"P{count}";
        count++;
    }
}