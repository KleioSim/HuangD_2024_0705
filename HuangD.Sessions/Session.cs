﻿using Chrona.Engine.Core.Interfaces;
using Chrona.Engine.Core.Sessions;
using HuangD.Sessions.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HuangD.Sessions;

public class Message_Start : IMessage
{
    public object Target { get; set; }
    public object Value { get; set; }
}

public class Message_Peace : IMessage
{
    public object Target { get; set; }
    public object Value { get; set; }
}

public class Session : ABSSession
{
    public static Action<string> LOG;
    public static Action<Province, Country> ChangeProvinceOwner;

    public IEnumerable<Province> Provinces => provinces;
    public IEnumerable<Country> Countries => countries;

    public Country player { get; set; }
    public Date Date { get; set; }

    public override IEnumerable<IEntity> Entities => countries;

    public override IEntity Player { get => player; set => player = (Country)value; }

    private List<Country> countries = new List<Country>();
    private List<Province> provinces = new List<Province>();
    private List<War> wars = new List<War>();


    public Session(IEnumerable<string> provinceIds, IEnumerable<string> countryIds)
    {
        Country.FindWars = (country) => wars.Where(x => x.From == country || x.To == country);

        player = null;
        Date = new Date();
        countries.AddRange(countryIds.Select(id => new Country(id)));
        provinces.AddRange(provinceIds.Select(id => new Province(id)));
    }

    [MessageProcess]
    public void TestStart(Message_Start msg)
    {
        var target = msg.Target as Country;
        var from = msg.Value as Country;

        LOG($"Message_Start from:{from.Name} target:{target.Name}");

        wars.Add(new War() { To = target, From = from });
    }

    [MessageProcess]
    public void Message_PeaceProcess(Message_Peace msg)
    {
        var county1 = msg.Target as Country;
        var county2 = msg.Value as Country;

        LOG($"Message_Peace from:{county1.Name} target:{county2.Name}");

        var war = wars.Single(x => (x.From == county1 && x.To == county2) || (x.From == county2 && x.To == county1));
        wars.Remove(war);
    }

    //public void OnMessage(IMessage message)
    //{
    //    LOG(message.Desc);

    //    switch (message)
    //    {
    //        case Message_ChangeProvinceOwner changeProvinceOwner:
    //            {
    //                var province = changeProvinceOwner.province;
    //                var oldOwner = province.Owner;
    //                var newOwner = changeProvinceOwner.owner;

    //                ChangeProvinceOwner(changeProvinceOwner.province, changeProvinceOwner.owner);

    //                if (oldOwner.Provinces.Count() == 0)
    //                {
    //                    OnMessage(new Message_CountryDestroyed(oldOwner));
    //                }
    //            }
    //            break;
    //        case Message_CountryDestroyed countryDestroyed:
    //            {
    //                countries.Remove(countryDestroyed.country);
    //            }
    //            break;
    //    }
    //}

    //public IEnumerable<IEvent> OnNextTurn()
    //{
    //    Date.MonthsInc();
    //    foreach (var country in Countries.ToArray())
    //    {
    //        currCountry = country;

    //        foreach (var eventObj in currCountry.NexTurn2())
    //        {
    //            if (eventObj.To == Player)
    //            {
    //                yield return eventObj;
    //            }

    //            eventObj.Process(this);
    //        }
    //    }

    //    currCountry = null;
    //}

    //internal void AddWar(Country from, Country to)
    //{
    //    LOG($"{from.Name} start war to {to.Name}");

    //    wars.Add(new War() { From = from, To = to });
    //}
}

public class War : IWar
{
    public ICountry From { get; init; }
    public ICountry To { get; init; }
}

public class Country : ICountry
{
    static int count;

    public readonly string id;

    public static Func<Country, IEnumerable<Province>> FindProvinces;
    public static Func<Country, IEnumerable<Country>> FindNeighbors;
    public static Func<Country, IEnumerable<Province>> FindEdges;
    public static Func<Country, Province> FindCapital;
    public static Func<Country, IEnumerable<War>> FindWars;

    public string Name { get; private set; }
    public IEnumerable<IProvince> Provinces => FindProvinces(this);
    public IEnumerable<Province> Edges => FindEdges(this);
    public IEnumerable<ICountry> Neighbors => FindNeighbors(this);
    public Province Capital => FindCapital(this);
    public IEnumerable<IWar> Wars => FindWars(this);

    public Country(string id)
    {
        this.id = id;
        Name = $"C{count}";
        count++;
    }

    //internal IEnumerable<IMessage> NexTurn()
    //{
    //    var random = new Random();
    //    if (random.Next(0, 100) < 30)
    //    {
    //        var province = Edges.Where(x => x != Capital && x.IsConnectToCapital)
    //            .SelectMany(x => x.Neighbors)
    //            .Except(Provinces)
    //            .OrderBy(_ => random.Next()).FirstOrDefault();

    //        yield return new Message_ChangeProvinceOwner(province != null ? province : Capital, this);
    //    }
    //}

    //internal IEnumerable<IEvent> NexTurn2()
    //{
    //    if (Wars.Any())
    //    {
    //        yield break;
    //    }

    //    var random = new Random();
    //    if (random.Next(0, 100) < 30)
    //    {
    //        yield return new Event() { From = this, To = Neighbors.OrderBy(_ => random.Next()).First() };
    //    }
    //}
}

public class Province : IProvince
{
    static int count;

    public readonly string id;

    public static Func<Province, int> FindPopCount;
    public static Func<Province, IEnumerable<Province>> FindNeighbors;
    public static Func<Province, Country> FindOwner;
    public static Func<Province, bool> CheckIsConnectToCapital;
    public int PopCount => FindPopCount(this);
    public IEnumerable<IProvince> Neighbors => FindNeighbors(this);
    public ICountry Owner => FindOwner(this);
    public bool IsConnectToCapital => CheckIsConnectToCapital(this);

    public string Name { get; private set; }

    public Province(string id)
    {
        this.id = id;
        Name = $"P{count}";
        count++;
    }
}