using Godot;
using System;
using System.Linq;
using System.Reflection;

public partial class TilemapTest : Control
{
    public Button button => GetNode<Button>("CanvasLayer/Button");
    public TileMap TerrainMap => GetNode<TileMap>("CanvasLayer2/TileMapTerrain");
    public TileMap popCountMap => GetNode<TileMap>("CanvasLayer2/TileMapPopCount");
    public TileMap provinceMap => GetNode<TileMap>("CanvasLayer2/TileMapProvince");
    public TileMap countryMap => GetNode<TileMap>("CanvasLayer2/TileMapCountry");

    public override void _Ready()
    {
        button.Pressed += () =>
        {
            TerrainMap.Clear();

            var random = new Random();

            var terrains = TerrainBuilder.Build(TerrainMap, random);
            GD.Print(String.Join(",", terrains.GroupBy(x => x.Value).Select(group => $"{group.Key}:{group.Count()}")));

            var pops = PopCountBuilder.Build(popCountMap, terrains, random);
            GD.Print($"total popCount:{pops.Values.Sum()}, max popCount {pops.Values.Max()}, min popCount {pops.Values.Min()}");

            var provinceBlocks = ProvinceBlock.Builder.Build(provinceMap, pops, random);
            GD.Print($"total provCount:{provinceBlocks.Count()}, max provSize {provinceBlocks.Max(x => x.Cells.Count())}, min provSize {provinceBlocks.Min(x => x.Cells.Count())}");

            var countryBlocks = CountryBuilder.Build(countryMap, provinceBlocks, random);
            GD.Print($"total countryCount:{countryBlocks.Count()}, max countrySize {countryBlocks.Max(x => x.Provinces.Count())}, min countrySize {countryBlocks.Min(x => x.Provinces.Count())}");
        };
    }
}
