using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Intrinsics.X86;

static class PopCountBuilder
{
    internal static Dictionary<Vector2I, int> Build(TileMap popCountMap, Dictionary<Vector2I, TerrainType> terrainDict, Random random)
    {
        popCountMap.Clear();
        for (int i = 0; i < 10; i++)
        {
            popCountMap.AddLayer(i);
            popCountMap.SetLayerModulate(i, new Color(1f, (10 - i) * 0.1f, (10 - i) * 0.1f));
        }

        var popCountDict = BuildPopCountDict(popCountMap, terrainDict, random);
        var max = popCountDict.Max(x => x.Value);
        var min = popCountDict.Min(x => x.Value);
        foreach (var pair in popCountDict)
        {
            popCountMap.SetCellEx(Math.Min(pair.Value / 1000, 10), pair.Key, 0);
        }

        return popCountDict;
    }

    private static Dictionary<Vector2I, int> BuildPopCountDict(TileMap tileMap, Dictionary<Vector2I, TerrainType> terrainDict, Random random)
    {
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
                    popCount = random.Next(0, 50);
                    break;
            }


            popCount += tileMap.GetNeighborCells_8(index).Values.Select(neighbor =>
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

            var neighorValues = tileMap.GetNeighborCells_8(currIndex).Values
                .Where(neighbor => baseValueDict.ContainsKey(neighbor))
                .Select(neighbor => baseValueDict[neighbor]);

            return currValue * Math.Min(1, (int)(neighorValues.Average()) / 1000);
        });
    }
}