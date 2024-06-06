using Chrona.Engine.Core.Events;
using Chrona.Engine.Core.Interfaces;
using Chrona.Engine.Core.Modders;
using Chrona.Engine.Core.Sessions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HuangD.Sessions;

//public interface IMessage
//{
//    string Desc { get; }
//}

//public class Message_NextTurn : IMessage
//{
//    public string Desc => "NexTurn";
//}

//public class Message_ChangeProvinceOwner : IMessage
//{
//    public readonly Province province;
//    public readonly Country owner;

//    public Message_ChangeProvinceOwner(Province prov, Country country)
//    {
//        province = prov;
//        owner = country;
//    }

//    public string Desc => $"{province.Name} owner is changed from {province.Owner.Name} to {owner.Name}";

//}

//public class Message_CountryDestroyed : IMessage
//{
//    public readonly Country country;

//    public Message_CountryDestroyed(Country country)
//    {
//        this.country = country;
//    }

//    public string Desc => $"{country.Name} is destroyed";
//}

//public interface IEvent
//{
//    Country From { get; }
//    Country To { get; }

//    void Process(Session session);
//}


//public class Event : IEvent
//{
//    public Country From { get; set; }

//    public Country To { get; set; }

//    public void Process(Session session)
//    {
//        session.AddWar(From, To);
//    }
//}

[DefTo(typeof(Country))]
public class EventDef : IEventDef
{
    public ICondition Condition { get; } = new TrueCondtion();

    public ITargetFinder TargetFinder { get; } = new TargetFinder()
    {
        Targets = new NeighorCountires(),
        ConditionFactors = new ICondtionFactor[]
        {
            new CondtionFactor()
            {
                Condition = new TrueCondtion(),
                Factor = 0.3
            }
        }
    };

    public IOption Option { get; } = new Option()
    {
        MessageBinds = new IMessageBind[]
        {
            new MessageBind()
            {
                MessageType = typeof(Message_Start),
                TargetVisitor = new EventFromVisitor(),
                ValueVisitor = new EventTargetVisitor()
            }
        }
    };
}

internal class EventTargetVisitor : DataVisitor
{
    public object Get(IEvent @event)
    {
        return @event.To;
    }
}

internal class EventFromVisitor : DataVisitor
{
    public object Get(IEvent @event)
    {
        return @event.From;
    }
}

internal class CondtionFactor : ICondtionFactor
{
    public ICondition Condition { get; set; }

    public double Factor { get; set; }
}

internal class NeighorCountires : IEventTarget
{
    public IEnumerable<IEntity> Get(IEntity entity, ISession session)
    {
        var country = (Country)entity;
        return country.Neighbors;
    }
}

public class TargetFinder : ITargetFinder
{
    public IEnumerable<ICondtionFactor> ConditionFactors { get; set; }

    public IEventTarget Targets { get; set; }

    public IEntity Find(IEntity entity, ABSSession session)
    {
        throw new NotImplementedException();
    }
}

public class Message_Start : IMessage
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
    public Country currCountry { get; set; }
    public Date Date { get; set; }

    public override IEnumerable<IEntity> Entities => countries;

    public override IEntity Player { get => player; set => player = (Country)value; }

    private List<Country> countries = new List<Country>();
    private List<Province> provinces = new List<Province>();
    private List<War> wars = new List<War>();


    public Session(int provinceCount, int countryCount)
    {
        Country.FindWars = (country) => wars.Where(x => x.From == country || x.To == country);

        player = null;
        Date = new Date();
        countries.AddRange(Enumerable.Range(0, countryCount).Select(_ => new Country()));
        provinces.AddRange(Enumerable.Range(0, provinceCount).Select(_ => new Province()));

        dictMessageProcess.Add(typeof(Message_Start), (msg) => TestStart(msg as Message_Start));
    }

    public void TestStart(Message_Start msg)
    {
        LOG(msg.GetType().Name);
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

public class War
{
    public Country From { get; init; }
    public Country To { get; init; }
}

public class Country : IEntity
{
    static int count;

    public static Func<Country, IEnumerable<Province>> FindProvinces;
    public static Func<Country, IEnumerable<Country>> FindNeighbors;
    public static Func<Country, IEnumerable<Province>> FindEdges;
    public static Func<Country, Province> FindCapital;
    public static Func<Country, IEnumerable<War>> FindWars;

    public string Name { get; private set; }
    public IEnumerable<Province> Provinces => FindProvinces(this);
    public IEnumerable<Province> Edges => FindEdges(this);
    public IEnumerable<Country> Neighbors => FindNeighbors(this);
    public Province Capital => FindCapital(this);
    public IEnumerable<War> Wars => FindWars(this);

    public Country()
    {
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