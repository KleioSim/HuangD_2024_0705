using Chrona.Engine.Core;
using Chrona.Engine.Core.Events;
using Chrona.Engine.Core.Interfaces;
using Chrona.Engine.Core.Sessions;
using HuangD.Sessions.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
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

    public IEnumerable<Map.Index> Expend(Map.Index index, int Length)
    {
        for (int i = -Length; i <= Length; i++)
        {
            for (int j = -Length; j <= Length; j++)
            {
                if (i == 0 && j == 0) continue;
                yield return new Map.Index(index.X - i, index.Y - j);
            }
        }
    }

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
        Dictionary<Direction, Index> GetNeighborCells(Index index);
        bool IsConnectNode(Index index, HashSet<Index> indexs);
        IEnumerable<Index> Expend(Index currentIndex, int v);
    }

    public static IIndexMethods IndexMethods { get; set; } = new SquareIndexMethods();

    public class Index
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
            //var mountionIndexs = BuildMountion(startPoint, hillIndexs, seed);

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
            //foreach (var index in mountionIndexs)
            //{
            //    rslt[index] = TerrainType.Mount;
            //}

            return rslt;
        }

        private static HashSet<Index> BuildMountion(Index startPoint, HashSet<Index> hillIndexs, string seed)
        {
            //var rslt = FlushLandEdge(hillIndexs, startPoint, seed, 0.7, false);
            //return rslt;

            var random = new Random();

            var cellQueue = new Queue<Index>(hillIndexs.OrderBy(_ => random.Next()));

            var mountionIndexs = new HashSet<Index>();
            while (cellQueue.Count != 0 && mountionIndexs.Count < hillIndexs.Count * 0.35)
            {
                var currentIndex = cellQueue.Dequeue();
                var expends = Map.IndexMethods.Expend(currentIndex, 1);

                var factor = 2 * expends.Count(x => mountionIndexs.Contains(x)) - expends.Count(x => !hillIndexs.Contains(x));

                if (random.Next(0, 100) <= factor)
                {
                    mountionIndexs.Add(currentIndex);
                }
                else
                {
                    cellQueue.Enqueue(currentIndex);
                }
            }

            return mountionIndexs;
        }

        private static HashSet<Index> BuildHill(Index startPoint, HashSet<Index> landIndexs, string seed)
        {

            var baseHills = AddBaseHill(landIndexs, startPoint, 1);

            var rslt = FlushLandEdge(baseHills, startPoint, seed, 0.35, true);
            rslt = FlushLandEdge(rslt, startPoint, seed, 0.15, false);

            //var isolatePlains = AddIsolatePlains(rslt, startPoint, seed, 0.25);
            //var isolateHills =  AddIsolateHill(landIndexs.Except(rslt).ToHashSet(), startPoint, seed, 0.25);

            //rslt.UnionWith(isolateHills);
            //rslt.ExceptWith(isolatePlains);
            return rslt;
        }

        private static HashSet<Index> AddBaseHill(HashSet<Index> landIndexs, Index startPoint, double percent)
        {
            var maxX = landIndexs.Select(index => index.X).Max();
            var maxY = landIndexs.Select(index => index.Y).Max();

            var baseLength = Math.Max(maxX, maxY);

            var rslt = new HashSet<Index>();
            for (int i = 0; i < baseLength; i++)
            {
                for (int j = 0; j < baseLength; j++)
                {
                    var index = new Index(Math.Abs(startPoint.X - i), Math.Abs(startPoint.Y - j));
                    if (landIndexs.Contains(index))
                    {
                        rslt.Add(index);
                        if (rslt.Count() > landIndexs.Count() * percent)
                        {
                            return rslt;
                        }
                    }

                    index = new Index(Math.Abs(startPoint.X - j), Math.Abs(startPoint.Y - i));
                    if (landIndexs.Contains(index))
                    {

                        rslt.Add(index);
                        if (rslt.Count() > landIndexs.Count() * percent)
                        {
                            return rslt;
                        }
                    }

                }
            }

            return rslt;
        }

        private static HashSet<Index> AddIsolateHill(HashSet<Index> indexs, Index startPoint, string seed, double percent)
        {
            var random = new Random();

            var cellQueue = new Queue<Index>(indexs.OrderBy(_ => random.Next()));

            var eraserIndexs = new HashSet<Index>();
            while (eraserIndexs.Count < indexs.Count * 0.35)
            {
                var currentIndex = cellQueue.Dequeue();
                var expends = Map.IndexMethods.Expend(currentIndex, 3);

                if (random.Next(0, 100) <= expends.Count(e => eraserIndexs.Contains(e)) * 100.0 / expends.Count())
                {
                    eraserIndexs.Add(currentIndex);
                }
                else
                {
                    cellQueue.Enqueue(currentIndex);
                }
            }

            return eraserIndexs;
        }

        private static HashSet<Index> AddIsolatePlains(HashSet<Index> indexs, Index startPoint, string seed, double percent)
        {

            var random = new Random();

            var cellQueue = new Queue<Index>(indexs.OrderBy(_ => random.Next()));

            var eraserIndexs = new HashSet<Index>();
            while (eraserIndexs.Count < indexs.Count * 0.25)
            {
                var currentIndex = cellQueue.Dequeue();
                var expends = Map.IndexMethods.Expend(currentIndex, 3);

                if (random.Next(0, 100) <= expends.Count(e => eraserIndexs.Contains(e)) * 100.0 / expends.Count())
                {
                    eraserIndexs.Add(currentIndex);
                }
                else
                {
                    cellQueue.Enqueue(currentIndex);
                }
            }

            return eraserIndexs;
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

            return FlushLandEdge(rslt, startPoint, seed, 0.25, false);
        }

        private static HashSet<Index> FlushLandEdge(HashSet<Index> indexs, Index startPoint, string seed, double percent, bool allowIsland)
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

            var rslt = new HashSet<Index>(indexs);

            int eraseCount = 0;
            var gCount = rslt.Count();
            while (true)
            {
                var eraserIndexs = new List<Index>();

                foreach (var index in edgeFactors.Keys.OrderBy(x => x.ToString()).ToArray())
                {
                    var factor = edgeFactors[index];

                    if (!allowIsland)
                    {
                        if (Map.IndexMethods.IsConnectNode(index, rslt))
                        {
                            edgeFactors.Remove(index);
                            continue;
                        }
                    }

                    var factor2 = Map.IndexMethods.GetNeighborCells(index).Values.Where(x => rslt.Contains(x)).Count();
                    if ((!allowIsland && factor2 <= 3) || random.Next(0, 10000) <= 1800 / factor)
                    {
                        edgeFactors.Remove(index);
                        rslt.Remove(index);
                        eraseCount++;
                        eraserIndexs.Add(index);

                        if (eraseCount * 1.0 / gCount > percent)
                        {
                            return rslt;
                        }
                    }
                    else
                    {
                        edgeFactors[index] += 3;
                    }

                }

                foreach (var index in eraserIndexs)
                {
                    var neighbors = Map.IndexMethods.GetNeighborCells(index).Values.Where(x => rslt.Contains(x));
                    foreach (var neighbor in neighbors)
                    {
                        edgeFactors.TryAdd(neighbor, 1);
                    }
                }

            }
        }
    }

    public static class PopCountBuilder
    {
        public static Dictionary<Index, int> Build(Dictionary<Index, TerrainType> terrainDict, string seed)
        {
            var random = new Random();
            var baseValueDict = terrainDict.Where(x => x.Value != TerrainType.Water).ToDictionary(k => k.Key, v =>
            {
                int popCount = 0;

                var index = v.Key;
                var terrain = v.Value;

                switch (terrain)
                {
                    case TerrainType.Land:
                        popCount = random.Next(500, 1000);
                        break;
                    case TerrainType.Hill:
                        popCount = random.Next(100, 200);
                        break;
                    case TerrainType.Mount:
                        popCount = random.Next(10, 50);
                        break;
                }


                popCount += Map.IndexMethods.GetNeighborCells(index).Values.Select(neighbor =>
                {
                    if (terrainDict.TryGetValue(neighbor, out TerrainType neighborTerrain))
                    {
                        switch (neighborTerrain)
                        {
                            case TerrainType.Land:
                                return random.Next(100, 500);
                            case TerrainType.Hill:
                                return random.Next(0, 100);
                            case TerrainType.Mount:
                                return 0;
                        }
                    }

                    return 0;
                }).Sum();

                return popCount;
            });

            return baseValueDict.ToDictionary(k => k.Key, v =>
            {
                var currIndex = v.Key;
                var currValue = v.Value;

                var neighorValues = Map.IndexMethods.GetNeighborCells(currIndex).Values
                    .Where(neighbor => baseValueDict.ContainsKey(neighbor))
                    .Select(neighbor => baseValueDict[neighbor]);

                return currValue * Math.Min(1, (int)(neighorValues.Average()) / 1000);
            });
        }
    }

    public static class ProvinceMapBuilder
    {
        public static Dictionary<Index, string> Build(Dictionary<Index, int> popDict, string seed, int maxPopCount, int maxIndexCount)
        {
            var indexs = popDict.Keys.ToHashSet();

            var rslt = new Dictionary<Index, string>();

            while (indexs.Count != 0)
            {
                var maxPopIndex = indexs.MaxBy(k => popDict[k]);
                indexs.Remove(maxPopIndex);

                string currentId = Guid.NewGuid().ToString();
                var currentGroup = new HashSet<Index>() { maxPopIndex };
                var neighbors = Map.IndexMethods.GetNeighborCells(maxPopIndex).Values.Intersect(indexs).ToHashSet();

                while (neighbors.Count != 0)
                {
                    var next = neighbors.Where(k => Map.IndexMethods.GetNeighborCells(k).Values.Intersect(indexs).Count() == 0).FirstOrDefault();
                    if (next == null)
                    {
                        next = neighbors.MaxBy(k => popDict[k]);
                    }

                    currentGroup.Add(next);
                    if (currentGroup.Count >= maxIndexCount)
                    {
                        break;
                    }
                    if (currentGroup.Sum(k => popDict[k]) >= maxPopCount)
                    {
                        break;
                    }

                    neighbors.Remove(next);
                    indexs.Remove(next);

                    neighbors.UnionWith(Map.IndexMethods.GetNeighborCells(next).Values.Intersect(indexs));
                }

                foreach (var index in currentGroup)
                {
                    rslt.Add(index, currentId);
                }
            }

            return rslt;
        }
    }
}