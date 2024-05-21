using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public partial class TilemapTest : Control
{
    public Button button => GetNode<Button>("CanvasLayer/Button");
    public TileMapTerrain TerrainMap => GetNode<TileMapTerrain>("CanvasLayer2/TileMapTerrain");
    public TileMapPopCount popCountMap => GetNode<TileMapPopCount>("CanvasLayer2/TileMapTerrain");
    public override void _Ready()
    {
        button.Pressed += () =>
        {
            TerrainMap.Clear();

            var random = new Random();

            var dict = TerrainBuilder.Build(TerrainMap, random);

            var groups = dict.GroupBy(x => x.Value);

            GD.Print(String.Join(",", groups.Select(group => $"{group.Key}:{group.Count()}")));
        };
    }
}
