using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using static Godot.OpenXRInterface;
using static Godot.Time;

class TerrainBuilder
{
    const int maxSize = 64;
    const int landSize = 60;

    static Dictionary<string, int> layerName2Id = new ()
    {
        { "sea", 0 },
        { "land", 1 },
        { "mount", 2 },
        { "steppe", 3 }
    };

    enum TileSetType
    {
        Water,
        Land,
        Mount,
        Steppe
    }

    static Dictionary<TileSetType, int> tileSet2Id = new()
    {
        { TileSetType.Land, 0 },
        { TileSetType.Steppe, 1 },
        { TileSetType.Mount, 2 },
        { TileSetType.Water, 3 }
    };

    static Vector2I[] startPoints = new[]
    {
        new Vector2I(0,0) * (maxSize-1),
        new Vector2I(0,1) * (maxSize-1),
        new Vector2I(1,0) * (maxSize-1),
        new Vector2I(1,1) * (maxSize-1)
    };

    public static void Build(TileMap tilemap, Random random)
    {
        foreach(var pair in layerName2Id)
        {
            tilemap.SetLayerName(pair.Value, pair.Key);
        }

        BuildSea(tilemap);

        var startPoint = startPoints[random.Next(startPoints.Length)];
        BuildLand(tilemap, startPoint, random);
    }

    private static void BuildLand(TileMap tilemap, Vector2I startPoint, Random random)
    {
        for (int i = 0; i < landSize; i++)
        {
            for (int j = 0; j < landSize; j++)
            {
                tilemap.SetCell(layerName2Id["land"], new Vector2I(Math.Abs(startPoint.X - i), Math.Abs(startPoint.Y - j)), 0, new Vector2I(0, 0), 0);
            }
        }

        FlushLandEdge(tilemap, startPoint, random);
    }

    private static void BuildSea(TileMap tilemap)
    {
        for (int i = 0; i < maxSize; i++)
        {
            for (int j = 0; j < maxSize; j++)
            {
                tilemap.SetCell(layerName2Id["sea"], new Vector2I(i, j), 3, new Vector2I(0, 0), 0);
            }
        }
    }

    private static void FlushLandEdge(TileMap tilemap, Vector2I startPoint, Random random)
    {
        var layerId = layerName2Id["land"];

        int max = tilemap.GetUsedCells(layerId).Select(x => x.X).Max();
        Dictionary<Vector2I, int> edgeIndexs = tilemap.GetUsedCells(layerId).Where(index =>
        {
            if ((startPoint.X == index.X && index.Y != Math.Abs( startPoint.Y - (landSize - 1)))
            || (startPoint.Y == index.Y && index.X != Math.Abs(startPoint.X - (landSize - 1))))
            {
                return false;
            }

            var neighborDict = tilemap.GetNeighborCells_4(index);
            if (neighborDict.Values.All(x => tilemap.GetCellSourceId(layerId, x) is not -1 and not 3))
            {
                return false;
            }

            if (tilemap.IsConnectNode(index, (cell) => tilemap.GetCellSourceId(layerId, cell) is not -1 and not 3))
            {
                return false;
            }

            return true;

        }).ToDictionary(k => k, v => 1); ;

        int eraseCount = 0;
        var gCount = tilemap.GetUsedCells(layerId).Count();
        while (eraseCount * 100 / gCount < 15)
        {
            var eraserIndexs = new List<Vector2I>();

            foreach (var index in edgeIndexs.Keys.ToArray())
            {
                var factor = edgeIndexs[index];

                if (tilemap.IsConnectNode(index, (cell) => tilemap.GetCellSourceId(layerId, cell) is not -1 and not 3))
                {
                    edgeIndexs.Remove(index);
                    continue;
                }

                var factor2 = tilemap.GetNeighborCells_8(index).Values.Where(x => tilemap.GetCellSourceId(layerId, x) is not - 1 and not 3).Count();
                if (factor2 <= 3 || random.Next(0, 10000) <= 3000 / factor)
                {
                    edgeIndexs.Remove(index);
                    tilemap.EraseCell(layerId, index);
                    eraseCount++;
                    eraserIndexs.Add(index);
                }

            }

            foreach (var key in edgeIndexs.Keys)
            {
                edgeIndexs[key]++;
                if (edgeIndexs[key] > 3000)
                {
                    edgeIndexs[key] = 3000;
                }
            }

            foreach (var index in eraserIndexs)
            {
                var neighbors = tilemap.GetNeighborCells_4(index).Values.Where(x => tilemap.GetCellSourceId(layerId, x) != -1 && tilemap.GetCellSourceId(layerId, x) != 3);
                foreach (var neighbor in neighbors)
                {
                    edgeIndexs.TryAdd(neighbor, 1);
                }
            }

        }
    }
}