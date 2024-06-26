using Chrona.Engine.Core;
using Chrona.Engine.Core.Events;
using Chrona.Engine.Core.Interfaces;
using Chrona.Engine.Core.Sessions;
using HuangD.Sessions.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static HuangD.Sessions.Map;

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

public class Message_ChangePlayCountry : IMessage
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
    public IEnumerable<Army> Armies => armies;

    public Country player { get; set; }
    public Date Date { get; set; }

    public override IEnumerable<IEntity> Entities => countries;

    public override IEntity Player { get => player; set => player = (Country)value; }

    private List<Country> countries = new List<Country>();
    private List<Province> provinces = new List<Province>();
    private List<War> wars = new List<War>();
    private List<Army> armies = new List<Army>();

    public Session(IEnumerable<string> provinceIds, IEnumerable<string> countryIds, IReadOnlyDictionary<Type, IEnumerable<IInteractionDef>> interactionDefs)
    {
        Country.count = 0;
        Province.count = 0;

        Country.FindWars = (country) => wars.Where(x => x.From == country || x.To == country);
        Country.FindArmys = (country) => armies.Where(x => x.Owner == country);
        Province.FindLocationArmies = (province) => armies.Where(x => x.Local == province);
        player = null;
        Date = new Date();

        countries.AddRange(countryIds.Select(id => new Country(id, interactionDefs[typeof(Country)])));
        provinces.AddRange(provinceIds.Select(id => new Province(id)));

        foreach (var province in provinces)
        {
            var army = new Army();
            army.Local = province;
            armies.Add(army);
        }
    }

    //public class Builder
    //{
    //    public Session Build(string seed)
    //    {
    //        var session = new Session(seed);
    //        session.Map = Map.Builder.Build(seed);
    //        session.Provinces = Province.Builder.Build(session.Map.Cell2PopCount);
    //        session.Countries = Country.Builder.Build(session.Provinces);
    //    }
    //}

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

    [MessageProcess]
    public void Message_ChangePlayCountryProcess(Message_ChangePlayCountry msg)
    {
        Player = msg.Target as Country;
    }
}

public class War : IWar
{
    public ICountry From { get; init; }
    public ICountry To { get; init; }
}

public class Army : IArmy
{
    public IProvince Origin { get; internal set; }

    public IProvince Local { get; internal set; }

    public IProvince Target { get; private set; }

    public ICountry Owner { get; internal set; }
}

public class Country : ICountry
{
    internal static int count;

    public readonly string id;

    public static Func<Country, IEnumerable<Province>> FindProvinces;
    public static Func<Country, IEnumerable<Country>> FindNeighbors;
    public static Func<Country, IEnumerable<Province>> FindEdges;
    public static Func<Country, Province> FindCapital;
    public static Func<Country, IEnumerable<War>> FindWars;
    public static Func<Country, IEnumerable<Army>> FindArmys;

    public string Name { get; private set; }
    public IEnumerable<IProvince> Provinces => FindProvinces(this);
    public IEnumerable<Province> Edges => FindEdges(this);
    public IEnumerable<ICountry> Neighbors => FindNeighbors(this);
    public Province Capital => FindCapital(this);
    public IEnumerable<IWar> Wars => FindWars(this);

    public bool IsInteractionDateOut { get; set; }
    public IEnumerable<IInteraction> Interactions { get; }

    public float WarWeary { get; private set; }

    public IEnumerable<IArmy> Armies => throw new NotImplementedException();

    public Country(string id, IEnumerable<IInteractionDef> interactionDefs)
    {
        this.id = id;
        Name = $"C{count}";
        count++;

        Interactions = interactionDefs.Select(def => new Interaction(def, this));
    }

    public void OnNextTurn()
    {
        WarWeary += 1;
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
    public static Func<Province, IEnumerable<Army>> FindLocationArmies { get; set; }

    internal static int count;

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

    public IEnumerable<Army> LocalArmies => FindLocationArmies(this);

    public Province(string id)
    {
        this.id = id;
        Name = $"P{count}";
        count++;
    }
}

public class SquareIndexMethods : IIndexMethods
{
    Dictionary<Map.Direction, (int x, int y)> Direction2Index = new Dictionary<Direction, (int x, int y)>()
    {
        { Map.Direction.TopSide, (0,-1) },
        { Map.Direction.TopLeftCorner, (-1, -1) },
        { Map.Direction.LeftSide,(-1, 0) },
        { Map.Direction.BottomLeftCorner,(-1, 1)},
        { Map.Direction.BottomSide,(0, 1)},
        { Map.Direction.BottomRightCorner,(1, 1)},
        { Map.Direction.RightSide,(1, 0)},
        { Map.Direction.TopRightCorner,(1, -1)}
    };
    (int x, int y)[] Neighbors = new (int x, int y)[]
    {
        (1,0),
        (0,1),
        (-1,0),
        (0,-1),
        (1,-1),
        (-1,1),
        (-1,-1),
        (1,1)
    };

    public Dictionary<Map.Direction, Map.Index> GetNeighborCells(Map.Index index)
    {
        return Direction2Index.ToDictionary(n => n.Key, m => new Map.Index(index.X + m.Value.x, index.Y + m.Value.y));
    }

    public bool IsConnectNode(Map.Index index, HashSet<Map.Index> indexs)
    {
        var neighbors = GetNeighborCells(index);

        if (indexs.Contains(neighbors[Map.Direction.LeftSide]) && indexs.Contains(neighbors[Map.Direction.RightSide])
            && !indexs.Contains(neighbors[Map.Direction.BottomSide]) && !indexs.Contains(neighbors[Map.Direction.TopSide]))
        {
            return true;
        }
        if (!indexs.Contains(neighbors[Map.Direction.LeftSide]) && !indexs.Contains(neighbors[Map.Direction.RightSide])
            && indexs.Contains(neighbors[Map.Direction.BottomSide]) && indexs.Contains(neighbors[Map.Direction.TopSide]))
        {
            return true;
        }
        if (indexs.Contains(neighbors[Map.Direction.LeftSide]) && indexs.Contains(neighbors[Map.Direction.BottomSide])
            && !indexs.Contains(neighbors[Map.Direction.BottomLeftCorner]))
        {
            return true;
        }
        if (indexs.Contains(neighbors[Map.Direction.LeftSide]) && indexs.Contains(neighbors[Map.Direction.TopSide])
            && !indexs.Contains(neighbors[Map.Direction.TopLeftCorner]))
        {
            return true;
        }
        if (indexs.Contains(neighbors[Map.Direction.RightSide]) && indexs.Contains(neighbors[Map.Direction.BottomSide])
            && !indexs.Contains(neighbors[Map.Direction.BottomRightCorner]))
        {
            return true;
        }
        if (indexs.Contains(neighbors[Map.Direction.RightSide]) && indexs.Contains(neighbors[Map.Direction.TopSide])
            && !indexs.Contains(neighbors[Map.Direction.TopRightCorner]))
        {
            return true;
        }

        return false;
    }
}

public class Map
{
    public enum Direction
    {
        TopSide,
        TopLeftCorner,
        LeftSide,
        BottomLeftCorner,
        BottomSide,
        BottomRightCorner,
        RightSide,
        TopRightCorner,
    }
    public interface IIndexMethods
    {
        Dictionary<Map.Direction, Map.Index> GetNeighborCells(Map.Index index);
        bool IsConnectNode(Index index, HashSet<Index> indexs);
    }

    public static IIndexMethods IndexMethods { get; set; } = new SquareIndexMethods();

    public struct Index
    {
        public Index(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; init; }
        public int Y { get; init; }
    }

    public enum TerrainType
    {
        Water,
        Land,
        Mount,
        Steppe,
        Hill
    }

    public Dictionary<Index, TerrainType> Cell2Terrain { get; init; }
    public Dictionary<Index, int> Cell2PopCount { get; init; }
    public Dictionary<Index, string> Cell2ProvinceId { get; init; }

    public static class Builder
    {
        public static Map Build(int maxSize, int landSize, string seed)
        {
            var map = new Map()
            {
                Cell2Terrain = TerrainBuilder.Build(maxSize, seed)
            };
            return map;
        }

    }

    public static class TerrainBuilder
    {
        public static Dictionary<Index, TerrainType> Build(int maxSize, string seed)
        {
            var startPoint = new Index(0, 0);

            var seaIndexs = BuildSea(maxSize);
            var landIndexs = BuildLand(startPoint, maxSize - 1, seed);
            var hillIndexs = BuildHill(startPoint, landIndexs, seed);

            var rslt = new Dictionary<Index, TerrainType>();
            foreach (var index in seaIndexs)
            {
                rslt[index] = TerrainType.Water;
            }

            foreach (var index in landIndexs)
            {
                rslt[index] = TerrainType.Land;
            }
            foreach (var index in hillIndexs)
            {
                rslt[index] = TerrainType.Hill;
            }

            return rslt;
        }

        private static HashSet<Index> BuildHill(Index startPoint, HashSet<Index> landIndexs, string seed)
        {
            var maxX = landIndexs.Select(index => index.X).Max();
            var maxY = landIndexs.Select(index => index.Y).Max();

            var baseLength = Math.Min(maxX, maxY);

            var rslt = new HashSet<Index>();
            for (int i = 0; i < baseLength; i++)
            {
                for (int j = 0; j < baseLength * 0.3; j++)
                {
                    var indexs = new Index[]
                    {
                        new Index(Math.Abs(startPoint.X - i), Math.Abs(startPoint.Y - j)),
                        new Index(Math.Abs(startPoint.X - j), Math.Abs(startPoint.Y - i))
                    };

                    rslt.UnionWith(indexs.Where(index => landIndexs.Contains(index)));
                }
            }

            return rslt;
        }

        private static HashSet<Index> BuildSea(int maxSize)
        {
            return Enumerable.Range(0, maxSize).SelectMany(x => Enumerable.Range(0, maxSize).Select(y => new Index(x, y))).ToHashSet();
        }

        private static HashSet<Index> BuildLand(Index startPoint, int landSize, string seed)
        {

            var rslt = new HashSet<Index>();
            for (int i = 0; i < landSize; i++)
            {
                for (int j = 0; j < landSize; j++)
                {
                    var index = new Index(Math.Abs(startPoint.X - i), Math.Abs(startPoint.Y - j));
                    rslt.Add(index);
                }
            }

            return FlushLandEdge(rslt, startPoint, seed);
        }

        private static HashSet<Index> FlushLandEdge(HashSet<Index> indexs, Index startPoint, string seed)
        {
            var random = new Random();
            Dictionary<Index, int> edgeFactors = indexs.Where(index =>
            {
                if (startPoint.X == index.X || startPoint.Y == index.Y)
                {
                    return false;
                }

                var neighbors = Map.IndexMethods.GetNeighborCells(index).Values;
                if (neighbors.All(x => indexs.Contains(x)))
                {
                    return false;
                }

                return true;

            }).ToDictionary(k => k, v => 1); ;

            int eraseCount = 0;
            var gCount = indexs.Count();
            while (true)
            {
                var eraserIndexs = new List<Index>();

                foreach (var index in edgeFactors.Keys.OrderBy(x => x.ToString()).ToArray())
                {
                    var factor = edgeFactors[index];

                    if (Map.IndexMethods.IsConnectNode(index, indexs))
                    {
                        edgeFactors.Remove(index);
                        continue;
                    }

                    var factor2 = Map.IndexMethods.GetNeighborCells(index).Values.Where(x => indexs.Contains(x)).Count();
                    if (factor2 <= 3 || random.Next(0, 10000) <= 1800 / factor)
                    {
                        edgeFactors.Remove(index);
                        indexs.Remove(index);
                        eraseCount++;
                        eraserIndexs.Add(index);

                        if (eraseCount * 100 / gCount > 25)
                        {
                            return indexs;
                        }
                    }
                    else
                    {
                        edgeFactors[index]++;
                    }

                }

                foreach (var index in eraserIndexs)
                {
                    var neighbors = Map.IndexMethods.GetNeighborCells(index).Values.Where(x => indexs.Contains(x));
                    foreach (var neighbor in neighbors)
                    {
                        edgeFactors.TryAdd(neighbor, 1);
                    }
                }

            }
        }
    }
}