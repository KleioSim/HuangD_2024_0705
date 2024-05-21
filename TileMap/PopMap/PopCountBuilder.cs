using Godot;
using System;
using System.Collections.Generic;

static class PopCountBuilder
{
    internal static void Build(TileMap popCountMap, Dictionary<Vector2I, TerrainBuilder.TileType> dict)
    {
        foreach(var pair in dict)
        {
            if(pair.Value != TerrainBuilder.TileType.Water)
            {
                popCountMap.SetCellEx(0, pair.Key, 0);
            }
        }
    }
}