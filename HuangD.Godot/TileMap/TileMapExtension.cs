using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public static class TileMapExtension
{
    public static Dictionary<TileSet.CellNeighbor, Vector2I> GetNeighborCells_8(this TileMap tilemap, Vector2I index)
    {
        var directs = new[] {
            TileSet.CellNeighbor.TopSide,
            TileSet.CellNeighbor.TopLeftCorner,
            TileSet.CellNeighbor.LeftSide,
            TileSet.CellNeighbor.BottomLeftCorner,
            TileSet.CellNeighbor.BottomSide,
            TileSet.CellNeighbor.BottomRightCorner,
            TileSet.CellNeighbor.RightSide,
            TileSet.CellNeighbor.TopRightCorner,};
        return directs.ToDictionary(x => x, x => tilemap.GetNeighborCell(index, x));
    }

    public static Dictionary<TileSet.CellNeighbor, Vector2I> GetNeighborCells_4(this TileMap tilemap, Vector2I index)
    {
        var directs = new[] {
            TileSet.CellNeighbor.TopSide,
            TileSet.CellNeighbor.LeftSide,
            TileSet.CellNeighbor.BottomSide,
            TileSet.CellNeighbor.RightSide,
        };
        return directs.ToDictionary(x => x, x => tilemap.GetNeighborCell(index, x));
    }


    public static IEnumerable<Vector2I> GetNeighbor4CellsById(this TileMap tilemap, Vector2I index, int id)
    {
        return tilemap.GetNeighborCells_4(index).Values.Where(x => tilemap.GetCellSourceId(0, x) == id);
    }

    public static bool IsConnectNode(this TileMap tilemap, int layerId, Vector2I index, Func<Vector2I, bool> IsCellUsed = null)
    {
        if (IsCellUsed == null)
        {
            IsCellUsed = (index) => tilemap.IsCellUsed(layerId, index);
        }

        var neighbors = tilemap.GetNeighborCells_8(index);

        if (IsCellUsed(neighbors[TileSet.CellNeighbor.LeftSide]) && IsCellUsed(neighbors[TileSet.CellNeighbor.RightSide])
            && !IsCellUsed(neighbors[TileSet.CellNeighbor.BottomSide]) && !IsCellUsed(neighbors[TileSet.CellNeighbor.TopSide]))
        {
            return true;
        }
        if (!IsCellUsed(neighbors[TileSet.CellNeighbor.LeftSide]) && !IsCellUsed(neighbors[TileSet.CellNeighbor.RightSide])
            && IsCellUsed(neighbors[TileSet.CellNeighbor.BottomSide]) && IsCellUsed(neighbors[TileSet.CellNeighbor.TopSide]))
        {
            return true;
        }
        if (IsCellUsed(neighbors[TileSet.CellNeighbor.LeftSide]) && IsCellUsed(neighbors[TileSet.CellNeighbor.BottomSide])
            && !IsCellUsed(neighbors[TileSet.CellNeighbor.BottomLeftCorner]))
        {
            return true;
        }
        if (IsCellUsed(neighbors[TileSet.CellNeighbor.LeftSide]) && IsCellUsed(neighbors[TileSet.CellNeighbor.TopSide])
            && !IsCellUsed(neighbors[TileSet.CellNeighbor.TopLeftCorner]))
        {
            return true;
        }
        if (IsCellUsed(neighbors[TileSet.CellNeighbor.RightSide]) && IsCellUsed(neighbors[TileSet.CellNeighbor.BottomSide])
            && !IsCellUsed(neighbors[TileSet.CellNeighbor.BottomRightCorner]))
        {
            return true;
        }
        if (IsCellUsed(neighbors[TileSet.CellNeighbor.RightSide]) && IsCellUsed(neighbors[TileSet.CellNeighbor.TopSide])
            && !IsCellUsed(neighbors[TileSet.CellNeighbor.TopRightCorner]))
        {
            return true;
        }
        return false;
    }

    public static bool IsCellUsed(this TileMap tileMap, int layerId, Vector2I index)
    {
        return tileMap.GetCellSourceId(layerId, index) != -1;
    }

    public static void SetCellEx(this TileMap tileMap, int layerId, Vector2I index, int sourceId)
    {
        tileMap.SetCell(layerId, index, sourceId, Vector2I.Zero, 0);
    }

    public static IEnumerable<Vector2I> Expend(this TileMap tileMap, Vector2I index, int radius)
    {
        for (int i = radius * -1; i < radius + 1; i++)
        {
            for (int j = radius * -1; j < radius + 1; j++)
            {
                yield return index + new Vector2I(i, j);
            }
        }
    }
}