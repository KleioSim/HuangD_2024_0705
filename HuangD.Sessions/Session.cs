using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HuangD.Sessions;

public interface IMessage
{
    string Desc { get; }

}

public class Message_NextTurn : IMessage
{
    public string Desc => "NexTurn";
}

public class Message_ChangeProvinceOwner : IMessage
{
    public readonly Province province;
    public readonly Country owner;

    public Message_ChangeProvinceOwner(Province prov, Country country)
    {
        province = prov;
        owner = country;
    }

    public string Desc => $"{province.Name} owner is changed from {province.Owner.Name} to {owner.Name}";

}

public class Message_CountryDestroyed : IMessage
{
    public readonly Country country;

    public Message_CountryDestroyed(Country country)
    {
        this.country = country;
    }

    public string Desc => $"{country.Name} is destroyed";
}

public class Session
{
    public static Action<string> LOG;
    public static Action<Province, Country> ChangeProvinceOwner;

    public IEnumerable<Province> Provinces => provinces;
    public IEnumerable<Country> Countries => countries;
    public Country Player { get; set; }
    public Country currCountry { get; set; }
    public Date Date { get; set; }

    private List<Country> countries = new List<Country>();
    private List<Province> provinces = new List<Province>();

    public Session(int provinceCount, int countryCount)
    {
        Date = new Date();

        countries.AddRange(Enumerable.Range(0, countryCount).Select(_ => new Country()));
        provinces.AddRange(Enumerable.Range(0, provinceCount).Select(_ => new Province()));
    }

    public void OnMessage(IMessage message)
    {
        LOG(message.Desc);

        switch (message)
        {
            case Message_ChangeProvinceOwner changeProvinceOwner:
                {
                    var province = changeProvinceOwner.province;
                    var oldOwner = province.Owner;
                    var newOwner = changeProvinceOwner.owner;

                    ChangeProvinceOwner(changeProvinceOwner.province, changeProvinceOwner.owner);

                    if (oldOwner.Provinces.Count() == 0)
                    {
                        OnMessage(new Message_CountryDestroyed(oldOwner));
                    }
                }
                break;
            case Message_CountryDestroyed countryDestroyed:
                {
                    countries.Remove(countryDestroyed.country);
                }
                break;
        }
    }

    public async void OnNextTurn()
    {
        Date.MonthsInc();
        foreach (var country in Countries.ToArray())
        {
            currCountry = country;

            foreach (var newMessage in country.NexTurn())
            {
                OnMessage(newMessage);
            }

            await Task.Delay(1000);
        }

        currCountry = null;
        return;
    }
}

public class Country
{
    static int count;

    public static Func<Country, IEnumerable<Province>> FindProvinces;
    public static Func<Country, IEnumerable<Country>> FindNeighbors;
    public static Func<Country, IEnumerable<Province>> FindEdges;
    public static Func<Country, Province> FindCapital;

    public string Name { get; private set; }
    public IEnumerable<Province> Provinces => FindProvinces(this);
    public IEnumerable<Province> Edges => FindEdges(this);
    public IEnumerable<Country> Neighbors => FindNeighbors(this);
    public Province Capital => FindCapital(this);


    public Country()
    {
        Name = $"C{count}";
        count++;
    }

    internal IEnumerable<IMessage> NexTurn()
    {
        var random = new Random();
        if (random.Next(0, 100) < 30)
        {
            var province = Edges.Where(x => x != Capital && x.IsConnectToCapital)
                .SelectMany(x => x.Neighbors)
                .Except(Provinces)
                .OrderBy(_ => random.Next()).FirstOrDefault();

            yield return new Message_ChangeProvinceOwner(province != null ? province : Capital, this);
        }
    }
}

public class Province
{
    static int count;

    public static Func<Province, int> FindPopCount;
    public static Func<Province, IEnumerable<Province>> FindNeighbors;
    public static Func<Province, Country> FindOwner;
    public static Func<Province, bool> CheckIsConnectToCapital;
    public int PopCount => FindPopCount(this);
    public IEnumerable<Province> Neighbors => FindNeighbors(this);
    public Country Owner => FindOwner(this);
    public bool IsConnectToCapital => CheckIsConnectToCapital(this);

    public string Name { get; private set; }

    public Province()
    {
        Name = $"P{count}";
        count++;
    }
}