using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Global : Node2D
{
    [Signal]
    public delegate void MapClickEventHandler(Vector2I index, string provinceName, string country);

    public TileMapRoot MapRoot => GetNode<TileMapRoot>("/root/MapScene/CanvasLayer/TileMapRoot");
    public Camera2D Camera => GetNode<Camera2D>("CanvasLayer/Camera2D");

    public Session Session { get; private set; }

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

        Country.FindProvinces = (country) =>
        {
            var coutryBlock = block2Country.Single(x => x.Value == country).Key;
            return coutryBlock.Provinces.Select(x => block2Province[x]);
        };

        Province.FindPopCount = (prov) =>
        {
            var provBlock = block2Province.Single(x => x.Value == prov).Key;
            return provBlock.Cells.Sum(x => MapRoot.Pops[x]);
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
