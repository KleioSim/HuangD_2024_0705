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
    const int mountionWidth = 25;
    const int mountionLong = 50;

    const int steppeWidth = 20;
    const int steppeLong0 = 25;
    const int steppeLong1 = 35;

    public enum TileType
    {
        Water,
        Land,
        Mount,
        Steppe,
        Hill
    }

    static Dictionary<TileType, int> tile2sourceId = new()
    {
        { TileType.Land, 0 },
        { TileType.Steppe, 1 },
        { TileType.Mount, 2 },
        { TileType.Water, 3 },
        { TileType.Hill, 4 }
    };

    static Dictionary<TileType, int> tile2LayerId = new()
    {
        { TileType.Land, 1 },
        { TileType.Steppe, 3 },
        { TileType.Mount, 2 },
        { TileType.Water, 0 }
    };

    static Vector2I[] startPoints = new[]
    {
        new Vector2I(0,0) * (maxSize-1),
        new Vector2I(0,1) * (maxSize-1),
        new Vector2I(1,0) * (maxSize-1),
        new Vector2I(1,1) * (maxSize-1)
    };

    public static Dictionary<Vector2I, TileType> Build(TileMap tilemap, Random random)
    {
        var startPoint = startPoints[0];

        BuildSea(tilemap);

        BuildLand(tilemap, startPoint, random);
        BuildMountion(tilemap, startPoint, random);
        BuildHill(tilemap, startPoint, random);
        BuildSteppe(tilemap, startPoint, random);

        return tilemap.GetUsedCells(tile2LayerId[TileType.Water]).ToDictionary(k => k, v =>
        {
            if (tilemap.IsCellUsed(tile2LayerId[TileType.Mount], v))
            {
                if(tilemap.GetCellSourceId(tile2LayerId[TileType.Mount], v) == tile2sourceId[TileType.Mount])
                {
                    return TileType.Mount;
                }
                if (tilemap.GetCellSourceId(tile2LayerId[TileType.Mount], v) == tile2sourceId[TileType.Hill])
                {
                    return TileType.Hill;
                }

            }
            if (tilemap.IsCellUsed(tile2LayerId[TileType.Land], v))
            {
                return TileType.Land;
            }

            return TileType.Water;
        });
    }

    private static void BuildHill(TileMap tilemap, Vector2I startPoint, Random random)
    {
        int layerId = tile2LayerId[TileType.Mount];
        int hillSourceId = tile2sourceId[TileType.Hill];
        int mountSourceId = tile2sourceId[TileType.Mount];

        var mountions = tilemap.GetUsedCells(layerId);
        var cellQueue = new Queue<Vector2I>(mountions.OrderBy(_ => random.Next()));
        int addCount = 0;
        while (true)
        {
            var currentIndex = cellQueue.Dequeue();

            if (tilemap.GetCellSourceId(layerId, currentIndex) == hillSourceId)
            {
                continue;
            }

            var percent = 5;
            if (currentIndex.X == startPoint.X || currentIndex.Y == startPoint.Y)
            {
                
            }
            else if (tilemap.GetNeighborCells_4(currentIndex).Values.All(index => !tilemap.IsCellUsed(layerId, index)))
            {
                percent = 100;
            }
            else if(tilemap.GetNeighborCells_4(currentIndex).Values.Any(index => !tilemap.IsCellUsed(layerId, index)))
            {
                percent = 70;
            }
            else if (tilemap.GetNeighbor4CellsById(currentIndex, hillSourceId).Any())
            {
                percent = 20;
            }

            if (random.Next(0, 100) < percent)
            {
                tilemap.SetCellEx(layerId, currentIndex, hillSourceId);
                addCount++;

                if (addCount > mountions.Count() * 0.75)
                {
                    return;
                }
            }
            else
            {
                cellQueue.Enqueue(currentIndex);
            }

        }
    }

    private static void BuildSteppe(TileMap tilemap, Vector2I startPoint, Random random)
    {
        int layerId = tile2LayerId[TileType.Steppe];
        int sourceId = tile2sourceId[TileType.Steppe];

        var long0 = steppeLong0;
        var long1 = steppeLong1;

        if (random.Next(0, 2) == 0)
        {
            long0 = steppeLong1;
            long1 = steppeLong0;
        }

        for (int j = 0; j < steppeWidth; j++)
        {
            for (int i = 0; i < long0; i++)
            {
                tilemap.SetCellEx(layerId, new Vector2I(Math.Abs(startPoint.X - j), Math.Abs(startPoint.Y - i)), sourceId);
            }
        }

        for (int i = 0; i < steppeWidth; i++)
        {
            for (int j = 0; j < long1; j++)
            {
                tilemap.SetCellEx(layerId, new Vector2I(Math.Abs(startPoint.X - j), Math.Abs(startPoint.Y - i)), sourceId);
            }
        }


        FlushLandEdge(tilemap, layerId, startPoint, random);
    }

    private static void BuildMountion(TileMap tilemap, Vector2I startPoint, Random random)
    {
        var landCells = tilemap.GetUsedCells(tile2LayerId[TileType.Land]);

        int layerId = tile2LayerId[TileType.Mount];
        int sourceId = tile2sourceId[TileType.Mount];

        for (int i = 0; i < mountionLong; i++)
        {
            for (int j = 0; j < mountionWidth; j++)
            {
                tilemap.SetCellEx(layerId, new Vector2I(Math.Abs(startPoint.X - i), Math.Abs(startPoint.Y - j)), sourceId);
                tilemap.SetCellEx(layerId, new Vector2I(Math.Abs(startPoint.X - j), Math.Abs(startPoint.Y - i)), sourceId);
            }
        }

        FlushLandEdge(tilemap, layerId, startPoint, random);

        var mountions = tilemap.GetUsedCells(layerId);

        AddIsolatePlains(tilemap, mountions.Where(index => index.X != startPoint.X && index.Y != startPoint.Y), random);
        AddIsolateMountions(tilemap, landCells.Except(mountions), random);
    }

    private static void AddIsolatePlains(TileMap tilemap, IEnumerable<Vector2I> mountions, Random random)
    {
        int layerId = tile2LayerId[TileType.Mount];

        var cellQueue = new Queue<Vector2I>(mountions.OrderBy(_ => random.Next()));
        int addCount = 0;
        while (true)
        {
            var currentIndex = cellQueue.Dequeue();
            if (!tilemap.IsCellUsed(layerId, currentIndex))
            {
                continue;
            }

            foreach (var index in tilemap.Expend(currentIndex, 3))
            {
                if (!tilemap.IsCellUsed(layerId, currentIndex))
                {
                    continue;
                }
                if (!mountions.Contains(index))
                {
                    continue;
                }
                if (random.Next(0, 100) < 50)
                {
                    continue;
                }

                tilemap.EraseCell(layerId, index);
                addCount++;

                if (addCount > mountions.Count() * 0.4)
                {
                    return;
                }
            }
        }
    }

    private static void AddIsolateMountions(TileMap tilemap, IEnumerable<Vector2I> plainCells, Random random)
    {
        int layerId = tile2LayerId[TileType.Mount];
        int sourceId = tile2sourceId[TileType.Mount];

        var cellQueue = new Queue<Vector2I>(plainCells.OrderBy(_ => random.Next()));
        int addCount = 0;
        while (true)
        {
            var currentIndex = cellQueue.Dequeue();
            if (tilemap.GetCellSourceId(layerId, currentIndex) == sourceId)
            {
                continue;
            }

            foreach (var index in tilemap.Expend(currentIndex, 2))
            {
                if (tilemap.GetCellSourceId(layerId, currentIndex) == sourceId)
                {
                    continue;
                }
                if (!plainCells.Contains(index))
                {
                    continue;
                }
                if (random.Next(0, 100) < 50)
                {
                    continue;
                }

                tilemap.SetCellEx(layerId, index, sourceId);
                addCount++;

                if (addCount > plainCells.Count() * 0.25)
                {
                    return;
                }
            }
        }
    }

    private static IEnumerable<Vector2I> BuildLand(TileMap tilemap, Vector2I startPoint, Random random)
    {
        int layerId = tile2LayerId[TileType.Land];
        int sourceId = tile2sourceId[TileType.Land];

        for (int i = 0; i < landSize; i++)
        {
            for (int j = 0; j < landSize; j++)
            {
                tilemap.SetCellEx(layerId, new Vector2I(Math.Abs(startPoint.X - i), Math.Abs(startPoint.Y - j)), sourceId);
            }
        }

        FlushLandEdge(tilemap, layerId, startPoint, random);

        return tilemap.GetUsedCells(layerId);
    }

    private static void BuildSea(TileMap tilemap)
    {
        int layerId = tile2LayerId[TileType.Water];
        int sourceId = tile2sourceId[TileType.Water];

        for (int i = 0; i < maxSize; i++)
        {
            for (int j = 0; j < maxSize; j++)
            {
                tilemap.SetCellEx(layerId, new Vector2I(i, j), sourceId);
            }
        }
    }

    private static void FlushLandEdge(TileMap tilemap, int layerId, Vector2I startPoint, Random random)
    {
        var usedSize = tilemap.GetUsedRect().Size;

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
                if (factor2 <= 3 || random.Next(0, 10000) <= 3000 / factor)
                {
                    edgeIndexs.Remove(index);
                    tilemap.EraseCell(layerId, index);
                    eraseCount++;
                    eraserIndexs.Add(index);
                }
                else
                {
                    edgeIndexs[index]++;
                }

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