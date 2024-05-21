using Godot;
using System;
using System.Collections.Generic;

static class PopCountBuilder
{
    internal static void Build(TileMap popCountMap, Dictionary<Vector2I, TerrainBuilder.TileType> dict)
    {
        popCountMap.Clear();
        for (int i = 0; i < dict.Count; i++)
        {
            popCountMap.AddLayer(i);
        }

        popCountMap.AddLayer(1);

        foreach (var pair in dict)
        {
            if(pair.Value != TerrainBuilder.TileType.Water)
            {


                popCountMap.SetCellEx(99, pair.Key, 0);
                popCountMap.SetLayerModulate(99, new Color(1f, 0.4f, 0.4f));
            }
        }
    }
}