using Godot;
using System;
using System.Linq;
using System.Reflection;

public partial class TilemapTest : Control
{
    public Button button => GetNode<Button>("CanvasLayer/Button");
    public TileMapTerrain TerrainMap => GetNode<TileMapTerrain>("CanvasLayer2/TileMapTerrain");
    public TileMapPopCount popCountMap => GetNode<TileMapPopCount>("CanvasLayer2/TileMapPopCount");
    public TileMap provinceMap => GetNode<TileMap>("CanvasLayer2/TileMapProvince");

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

            var provinceBlocks = ProvinceBuilder.Build(provinceMap, pops, random);
            GD.Print($"total provCount:{provinceBlocks.Count()}, max provSize {provinceBlocks.Max(x => x.Cells.Count())}, min provSize {provinceBlocks.Min(x => x.Cells.Count())}");
        };
    }
}
