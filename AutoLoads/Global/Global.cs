using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Global : Node2D
{
    public TileMapRoot MapRoot => GetNode<TileMapRoot>("/root/MapScene/CanvasLayer/TileMapRoot");

    public Session Session { get; private set; }

    public Action<Vector2I, Province, Country> MapClick { get; set; }

    private Dictionary<ProvinceBlock, Province> block2Province = new Dictionary<ProvinceBlock, Province>();
    private Dictionary<CountryBlock, Country> block2Country = new Dictionary<CountryBlock, Country>();

    public override void _Ready()
    {
        MapRoot.ClickTile += (index) =>
        {
            var provinceBlock = MapRoot.ProvinceBlocks.SingleOrDefault(x => x.Cells.Contains(index));
            var countryBlock = provinceBlock == null ? null : MapRoot.CountryBlocks.SingleOrDefault(x => x.Provinces.Contains(provinceBlock));

            MapClick?.Invoke(index,
                provinceBlock == null ? null : block2Province[provinceBlock],
                countryBlock == null ? null : block2Country[countryBlock]);
        };

        Country.FindProvinces = (country) =>
        {
            var coutryBlock = block2Country.Single(x => x.Value == country).Key;
            return coutryBlock.Provinces.Select(x => block2Province[x]);
        };
    }


    internal void BuildGame(string seed)
    {
        MapRoot.BuildMap(seed);

        Session = new Session(MapRoot.ProvinceBlocks.Count, MapRoot.CountryBlocks.Count);

        block2Province.Clear();
        block2Country.Clear();

        for (int i = 0; i < MapRoot.ProvinceBlocks.Count; i++)
        {
            block2Province.Add(MapRoot.ProvinceBlocks[i], Session.Provinces.ElementAt(i));
        }

        for (int i = 0; i < MapRoot.CountryBlocks.Count; i++)
        {
            block2Country.Add(MapRoot.CountryBlocks[i], Session.Countries.ElementAt(i));
        }
    }
}
