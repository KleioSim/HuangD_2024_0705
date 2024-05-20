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
    const int landSize = 63;

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
        var startPoint = startPoints[random.Next(startPoints.Length)];

        BuildSea(tilemap, 0);
        BuildLand(tilemap, 1, startPoint, random);
    }

    private static void BuildLand(TileMap tilemap, int layerId, Vector2I startPoint, Random random)
    {
        for (int i = 0; i < landSize; i++)
        {
            for (int j = 0; j < landSize; j++)
            {
                tilemap.SetCell(layerId, 
                    new Vector2I(Math.Abs(startPoint.X - i), Math.Abs(startPoint.Y - j)), 
                    tileSet2Id[TileSetType.Land], 
                    new Vector2I(0, 0), 0);
            }
        }

        FlushLandEdge(tilemap, layerId, startPoint, random);
    }

    private static void BuildSea(TileMap tilemap, int layerId)
    {
        for (int i = 0; i < maxSize; i++)
        {
            for (int j = 0; j < maxSize; j++)
            {
                tilemap.SetCell(0, new Vector2I(i, j), 3, new Vector2I(0, 0), 0);
            }
        }
    }

    private static void FlushLandEdge(TileMap tilemap, int layerId,  Vector2I startPoint, Random random)
    {
        var usedSize = tilemap.GetUsedRect().Size;

        int max = tilemap.GetUsedCells(layerId).Select(x => x.X).Max();
        Dictionary<Vector2I, int> edgeIndexs = tilemap.GetUsedCells(layerId).Where(index =>
        {
            if (startPoint.X == index.X && index.Y != Math.Abs(startPoint.X - (usedSize.X - 1))
            || startPoint.Y == index.Y && index.X != Math.Abs(startPoint.X - (usedSize.Y - 1)))
            {
                return false;
            }

            var neighborDict = tilemap.GetNeighborCells_4(index);
            if (neighborDict.Values.All(x => tilemap.IsCellUsed(layerId, x)))
            {
                return false;
            }

            if (tilemap.IsConnectNode(layerId, index))
            {
                return false;
            }

            return true;

        }).ToDictionary(k => k, v => 1); ;

        int eraseCount = 0;
        var gCount = tilemap.GetUsedCells(layerId).Count();
        while (eraseCount * 100 / gCount < 25)
        {
            var eraserIndexs = new List<Vector2I>();

            foreach (var index in edgeIndexs.Keys.ToArray())
            {
                var factor = edgeIndexs[index];

                if (tilemap.IsConnectNode(layerId, index))
                {
                    edgeIndexs.Remove(index);
                    continue;
                }

                var factor2 = tilemap.GetNeighborCells_8(index).Values.Where(x => tilemap.IsCellUsed(layerId, x)).Count();
                if (factor2 <= 3 || random.Next(0, 10000) <= 3000 / factor )
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
            }

            foreach (var index in eraserIndexs)
            {
                var neighbors = tilemap.GetNeighborCells_4(index).Values.Where(x => tilemap.IsCellUsed(layerId, x));
                foreach (var neighbor in neighbors)
                {
                    edgeIndexs.TryAdd(neighbor, 1);
                }
            }

        }
    }
}