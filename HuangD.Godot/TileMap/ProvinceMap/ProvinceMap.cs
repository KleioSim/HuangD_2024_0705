using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class ProvinceMap : TileMap
{
    public override void _Ready()
    {
        var terrainDict = HuangD.Sessions.Map.TerrainBuilder.Build(64, "Test");
        GD.Print(string.Join(",", terrainDict.Values.GroupBy(x => x).Select(g => $"{g.Key}:{g.Count()}")));

        var popDict = HuangD.Sessions.Map.PopCountBuilder.Build(terrainDict, "test");
        GD.Print($"total pop count {popDict.Values.Sum()}, max {popDict.Values.Max()}, min {popDict.Values.Min()}");

        var provinceDict = HuangD.Sessions.Map.ProvinceMapBuilder.Build(popDict, "test", popDict.Values.Max() * 8, popDict.Count() / 50);
        var provinceGroups = provinceDict.GroupBy(x => x.Value).ToArray();
        GD.Print($"total province count {provinceGroups.Count()}, max cell count {provinceGroups.Max(g => g.Count())}, min cell count {provinceGroups.Min(g => g.Count())}, max pop count {provinceGroups.Max(g => g.Sum(i => popDict[i.Key]))}, min pop count {provinceGroups.Min(g => g.Sum(i => popDict[i.Key]))}");

        var random = new Random();

        var colors = new HashSet<Color>();
        while (colors.Count < provinceGroups.Length)
        {
            colors.Add(new Color(random.Next(0, 10) / 10.0f, random.Next(0, 10) / 10.0f, random.Next(0, 10) / 10.0f));
        }

        for (int i = 0; i < provinceGroups.Length; i++)
        {
            this.AddLayer(i);
            this.SetLayerModulate(i, colors.ElementAt(i));

            foreach (var index in provinceGroups[i])
            {
                this.SetCellEx(i, new Vector2I(index.Key.X, index.Key.Y), 0);
            }
        }
    }
}
