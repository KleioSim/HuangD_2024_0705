using System;
using System.Collections.Generic;
using System.Linq;

namespace HuangD.Sessions;

public interface IMessage
{

}

public class Message_NextTurn : IMessage
{

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
}

public class Session
{
    public static Action<string> LOG;
    public static Action<Province, Country> ChangeProvinceOwner;

    public IEnumerable<Province> Provinces { get; set; }
    public IEnumerable<Country> Countries { get; set; }
    public Country Player { get; set; }
    public Date Date { get; set; }

    public Session(int provinceCount, int countryCount)
    {
        Date = new Date();

        Provinces = Enumerable.Range(0, provinceCount).Select(_ => new Province()).ToArray();
        Countries = Enumerable.Range(0, countryCount).Select(_ => new Country()).ToArray();
    }

    public void OnMessage(IMessage message)
    {
        switch (message)
        {
            case Message_NextTurn:
                Date.MonthsInc();
                foreach (var country in Countries)
                {
                    foreach (var newMessage in country.NexTurn())
                    {
                        OnMessage(newMessage);
                    }
                }
                break;
            case Message_ChangeProvinceOwner changeProvinceOwner:
                {
                    LOG($"{changeProvinceOwner.province.Name} owner is changed from {changeProvinceOwner.province.Owner.Name} to {changeProvinceOwner.owner.Name}");

                    ChangeProvinceOwner(changeProvinceOwner.province, changeProvinceOwner.owner);
                }
                break;
        }
    }
}


public class Country
{
    static int count;

    public static Func<Country, IEnumerable<Province>> FindProvinces;
    public static Func<Country, IEnumerable<Country>> FindNeighbors;

    public string Name { get; private set; }
    public IEnumerable<Province> Provinces => FindProvinces(this);
    public IEnumerable<Country> Neighbors => FindNeighbors(this);

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
            var neighor = Neighbors.OrderBy(_ => random.Next()).First();

            var province = neighor.Provinces.First(x => x.Neighbors.Intersect(Provinces).Any());

            yield return new Message_ChangeProvinceOwner(province, this);
        }
    }
}

public class Province
{
    static int count;

    public static Func<Province, int> FindPopCount;
    public static Func<Province, IEnumerable<Province>> FindNeighbors;
    public static Func<Province, Country> FindOwner;

    public int PopCount => FindPopCount(this);
    public IEnumerable<Province> Neighbors => FindNeighbors(this);
    public Country Owner => FindOwner(this);

    public string Name { get; private set; }

    public Province()
    {
        Name = $"P{count}";
        count++;
    }
}