using Chrona.Engine.Godot;
using Godot;
using HuangD.Sessions;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class GameBuilder : Node2D
{
    [Signal]
    public delegate void MapClickEventHandler(Vector2I index, string provinceName, string country);

    public TileMapRoot MapRoot => GetNode<TileMapRoot>("/root/MapScene/CanvasLayer/TileMapRoot");
    public Camera2D Camera => GetNode<Camera2D>("CanvasLayer/Camera2D");

    private Dictionary<ProvinceBlock, Province> block2Province = new Dictionary<ProvinceBlock, Province>();
    private Dictionary<CountryBlock, Country> block2Country = new Dictionary<CountryBlock, Country>();

    public override void _Ready()
    {
        MapRoot.ClickTile += (index) =>
        {
            var provinceBlock = MapRoot.ProvinceBlocks.SingleOrDefault(x => x.Cells.Contains(index));
            var countryBlock = provinceBlock == null ? null : MapRoot.CountryBlocks.SingleOrDefault(x => x.Provinces.Contains(provinceBlock));

            EmitSignal(SignalName.MapClick, index,
                provinceBlock == null ? null : block2Province[provinceBlock].Name,
                countryBlock == null ? null : block2Country[countryBlock].Name);
        };

        Session.LOG = (log) =>
        {
            GD.Print(log);
        };

        Session.ChangeProvinceOwner = (province, country) =>
        {
            var provBlock = block2Province.SingleOrDefault(x => x.Value == province).Key;
            var newCountryBlock = block2Country.SingleOrDefault(x => x.Value == country).Key;

            MapRoot.ChangeProvinceOwner(provBlock, newCountryBlock);
        };

        Country.FindCapital = (country) =>
        {
            var coutryBlock = block2Country.Single(x => x.Value == country).Key;
            return block2Province[coutryBlock.Capital];
        };

        Country.FindProvinces = (country) =>
        {
            var coutryBlock = block2Country.Single(x => x.Value == country).Key;
            return coutryBlock.Provinces.Select(x => block2Province[x]);
        };

        Country.FindNeighbors = (country) =>
        {
            var coutryBlock = block2Country.Single(x => x.Value == country).Key;

            return MapRoot.CountryBlocks.Where(x => x != coutryBlock)
                        .Where(x => coutryBlock.Edges.SelectMany(x => x.Neighbors).Intersect(x.Edges).Any())
                        .Select(x => block2Country[x]);
        };

        Country.FindEdges = (country) =>
        {
            var coutryBlock = block2Country.Single(x => x.Value == country).Key;
            return coutryBlock.Edges.Select(x => block2Province[x]);
        };

        Province.FindPopCount = (prov) =>
        {
            var provBlock = block2Province.Single(x => x.Value == prov).Key;
            return provBlock.Cells.Sum(x => MapRoot.Pops[x]);
        };

        Province.FindOwner = (prov) =>
        {
            var provBlock = block2Province.Single(x => x.Value == prov).Key;

            var countryBlock = MapRoot.CountryBlocks.Single(x => x.Provinces.Contains(provBlock));
            return block2Country[countryBlock];
        };

        Province.FindNeighbors = (prov) =>
        {
            var provBlock = block2Province.Single(x => x.Value == prov).Key;
            return provBlock.Neighbors.Select(x => block2Province[x]);
        };

        Province.CheckIsConnectToCapital = (prov) =>
        {
            var provBlock = block2Province.Single(x => x.Value == prov).Key;

            var countryBlock = MapRoot.CountryBlocks.Single(x => x.Provinces.Contains(provBlock));
            if (provBlock == countryBlock.Capital)
            {
                return true;
            }

            var searched = new HashSet<ProvinceBlock>();

            var queue = new Queue<ProvinceBlock>();
            queue.Enqueue(countryBlock.Capital);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                var neighbors = current.Neighbors.Intersect(countryBlock.Provinces)
                    .Except(searched).ToArray();
                if (neighbors.Contains(provBlock))
                {
                    return true;
                }
                foreach (var neighbor in neighbors)
                {
                    queue.Enqueue(neighbor);
                    searched.Add(neighbor);
                }
            }

            return false;

        };
    }


    internal void BuildGame(string seed)
    {
        MapRoot.BuildMap(seed);

        var session = new Session(MapRoot.ProvinceBlocks.Count, MapRoot.CountryBlocks.Count);

        block2Province.Clear();
        block2Country.Clear();

        for (int i = 0; i < MapRoot.ProvinceBlocks.Count; i++)
        {
            block2Province.Add(MapRoot.ProvinceBlocks[i], session.Provinces.ElementAt(i));
        }

        for (int i = 0; i < MapRoot.CountryBlocks.Count; i++)
        {
            block2Country.Add(MapRoot.CountryBlocks[i], session.Countries.ElementAt(i));
        }

        GetNode<Global>("/root/Chrona_Global").Chroncle.Session = session;
    }
}
