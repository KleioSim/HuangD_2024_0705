using System.Collections.Generic;
using System.Linq;

public class Session
{
    public IEnumerable<Province> Provinces { get; set; }
    public IEnumerable<Country> Countries { get; set; }

    public Session(int provinceCount, int countryCount)
    {
        Provinces = Enumerable.Range(0, provinceCount).Select(_ => new Province()).ToArray();
        Countries = Enumerable.Range(0, countryCount).Select(_ => new Country()).ToArray();
    }
}


public class Country
{
    static int count;
    public string Name { get; private set; }

    public Country()
    {
        Name = count++.ToString();
    }
}

public class Province
{
    static int count;
    public string Name { get; private set; }

    public Province()
    {
        Name = count++.ToString();
    }
}