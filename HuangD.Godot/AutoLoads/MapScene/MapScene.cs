using Chrona.Engine.Godot;
using Godot;
using Chrona.Engine.Godot.Utilties;
using HuangD.Sessions;
using System;
using System.Collections.Generic;
using System.Linq;
using HuangD.Sessions.Interfaces;
using System.Diagnostics.Metrics;

public partial class MapScene : Control
{
    [Signal]
    public delegate void MapClickEventHandler(Vector2I index, string provinceName, string country);

    public TileMapRoot MapRoot => GetNode<TileMapRoot>("CanvasLayer/TileMapRoot");
    public PawnScene PawnScene => GetNode<PawnScene>("CanvasLayer/PawnScene");

    public Global Global => GetNode<Global>("/root/Chrona_Global");

    public override void _Ready()
    {
        MapRoot.ClickTile += (index) =>
        {
            var provinceBlock = MapRoot.ProvinceBlocks.Values.SingleOrDefault(x => x.Cells.Contains(index));
            var countryBlock = provinceBlock == null ? null : MapRoot.CountryBlocks.Values.SingleOrDefault(x => x.Provinces.Contains(provinceBlock));

            var session = Global.Chroncle.Session as Session;

            EmitSignal(SignalName.MapClick, index,
                provinceBlock == null ? null : session.Provinces.Single(x => x.id == provinceBlock.id).Name,
                countryBlock == null ? null : session.Countries.Single(x => x.id == countryBlock.id).Name);
        };

        Session.LOG = (log) =>
        {
            LOG.INFO(log);
        };

        Session.ChangeProvinceOwner = (province, country) =>
        {
            var provBlock = MapRoot.ProvinceBlocks[province.id];
            var newCountryBlock = MapRoot.CountryBlocks.Values.SingleOrDefault(c => c.Provinces.Contains(provBlock));

            MapRoot.ChangeProvinceOwner(provBlock, newCountryBlock);
        };

        Country.FindCapital = (country) =>
        {
            var coutryBlock = MapRoot.CountryBlocks[country.id];

            var session = Global.Chroncle.Session as Session;
            return session.Provinces.Single(x => x.id == coutryBlock.Capital.id);
        };

        Country.FindProvinces = (country) =>
        {
            var coutryBlock = MapRoot.CountryBlocks[country.id];

            var session = Global.Chroncle.Session as Session;
            return coutryBlock.Provinces.Select(x => session.Provinces.Single(prov => prov.id == x.id));
        };

        Country.FindNeighbors = (country) =>
        {
            var coutryBlock = MapRoot.CountryBlocks[country.id];

            var session = Global.Chroncle.Session as Session;
            return MapRoot.CountryBlocks.Values.Where(x => x != coutryBlock)
                        .Where(x => coutryBlock.Edges.SelectMany(x => x.Neighbors).Intersect(x.Edges).Any())
                        .Select(x => session.Countries.Single(c => c.id == x.id));
        };

        Country.FindEdges = (country) =>
        {
            var coutryBlock = MapRoot.CountryBlocks[country.id];

            var session = Global.Chroncle.Session as Session;
            return coutryBlock.Edges.Select(x => session.Provinces.Single(p => p.id == x.id));
        };

        Province.FindPopCount = (prov) =>
        {
            var provBlock = MapRoot.ProvinceBlocks[prov.id];
            return provBlock.Cells.Sum(x => MapRoot.Pops[x]);
        };

        Province.FindOwner = (prov) =>
        {
            var provBlock = MapRoot.ProvinceBlocks[prov.id];

            var countryBlock = MapRoot.CountryBlocks.Values.Single(x => x.Provinces.Contains(provBlock));

            var session = Global.Chroncle.Session as Session;
            return session.Countries.Single(x => x.id == countryBlock.id);
        };

        Province.FindNeighbors = (prov) =>
        {
            var provBlock = MapRoot.ProvinceBlocks[prov.id];

            var session = Global.Chroncle.Session as Session;
            return provBlock.Neighbors.Select(x => session.Provinces.Single(p => p.id == x.id));
        };

        Province.CheckIsConnectToCapital = (prov) =>
        {
            var provBlock = MapRoot.ProvinceBlocks[prov.id];

            var countryBlock = MapRoot.CountryBlocks.Values.Single(x => x.Provinces.Contains(provBlock));
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

        ProvincePawnItem.GetPawnPosition = (prov) =>
        {
            var provBlock = MapRoot.ProvinceBlocks[prov.id];
            return MapRoot.ProvinceMap.MapToLocal(provBlock.CenterCell);
        };

        CountryPawnItem.GetPawnPosition = (country) =>
        {
            var coutryBlock = MapRoot.CountryBlocks[country.id];

            var countryCells = coutryBlock.Provinces.SelectMany(x => x.Cells).ToHashSet();
            var maxX = countryCells.Select(c => c.X).Max();
            var maxY = countryCells.Select(c => c.Y).Max();
            var minX = countryCells.Select(c => c.X).Min();
            var minY = countryCells.Select(c => c.Y).Min();

            var center = new Vector2I((maxX - minX) / 2 + minX, (maxY - minY) / 2 + minY);

            var edgeCells = coutryBlock.Provinces.SelectMany(x => x.Edges).Where(x => MapRoot.ProvinceMap.GetNeighborCells_8(x).Any(p => !countryCells.Contains(p.Value)));


            Vector2I labelCell = countryCells.First();
            var maxFactor =int.MinValue;

            foreach (var cell in countryCells)
            {
                var centerDist = (cell - center).LengthSquared();
                //var centerDist = 0;
                var egdeDist = edgeCells.Select(x => (cell - x).LengthSquared()).Min();
                //var egdeDist =0;
                var factor = egdeDist * 100 + 100/(centerDist+1);
                if (maxFactor < factor)
                {
                    maxFactor = factor;
                    labelCell = cell;
                }
            }


            return MapRoot.ProvinceMap.MapToLocal(labelCell);
        };
    }
}
