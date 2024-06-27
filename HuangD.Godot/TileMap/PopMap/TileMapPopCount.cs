using Godot;
using System;
using System.Linq;

public partial class TileMapPopCount : TileMap
{
    public override void _Ready()
    {
        var terrainDict = HuangD.Sessions.Map.TerrainBuilder.Build(64, "Test");
        GD.Print(string.Join(",", terrainDict.Values.GroupBy(x => x).Select(g => $"{g.Key}:{g.Count()}")));

        var popDict = HuangD.Sessions.Map.PopCountBuilder.Build(terrainDict, "test");
        GD.Print($"total pop count {popDict.Values.Sum()}, max {popDict.Values.Max()}, min {popDict.Values.Min()}");

        for (int i = 0; i < 10; i++)
        {
            this.AddLayer(i);
            this.SetLayerModulate(i, new Color(1f, (10 - i) * 0.1f, (10 - i) * 0.1f));
        }

        var max = popDict.Max(x => x.Value);
        var min = popDict.Min(x => x.Value);
        foreach (var pair in popDict)
        {
            this.SetCellEx(Math.Min(pair.Value / 1000, 10), new Vector2I(pair.Key.X, pair.Key.Y), 0);
        }
    }
}
