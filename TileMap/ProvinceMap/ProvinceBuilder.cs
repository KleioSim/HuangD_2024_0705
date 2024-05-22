using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

internal class ProvinceBlock
{
    public IEnumerable<ProvinceBlock> Neighbors => _neighbors;
    public IEnumerable<Vector2I> Edges => _edges;
    public IEnumerable<Vector2I> Cells => _cells;

    private HashSet<Vector2I> _edges = new HashSet<Vector2I>();
    private HashSet<Vector2I> _cells = new HashSet<Vector2I>();
    private HashSet<ProvinceBlock> _neighbors = new HashSet<ProvinceBlock>();

    internal void Add(TileMap tilemap, Vector2I index)
    {
        _cells.Add(index);

        _edges.Add(index);

        var needRemove = _edges.Where(x => tilemap.GetNeighborCells_4(x).Values.All(neighor => _cells.Contains(neighor))).ToArray();
        _edges.ExceptWith(needRemove);
    }

    internal void AddRange(TileMap tilemap, IEnumerable<Vector2I> cells)
    {
        foreach (var cell in cells)
        {
            Add(tilemap, cell);
        }
    }


    internal class Builder
    {
        internal static List<ProvinceBlock> Build(TileMap tilemap, Dictionary<Vector2I, int> pops, Random random)
        {
            var provinceBlocks = new List<ProvinceBlock>();

            var cells = new List<Vector2I>(pops.Keys.OrderBy(_ => random.Next()));
            while (cells.Count > 0)
            {
                var provinceBlock = BuildProvinceBlock(tilemap, cells, random);
                provinceBlocks.Add(provinceBlock);
            }

            var needUnions = provinceBlocks.Where(x => x.Cells.Count() < 8).ToArray();
            provinceBlocks = provinceBlocks.Except(needUnions).ToList();
            foreach (var item in needUnions)
            {
                var block = provinceBlocks.First(block => block.Edges.Intersect(item.Edges.SelectMany(x => tilemap.GetNeighborCells_4(x).Values)).Any());

                block.AddRange(tilemap, item.Cells);
            }

            foreach(var block in provinceBlocks)
            {
                var outter = block.Edges.SelectMany(edge => tilemap.GetNeighborCells_4(edge).Values.Except(block.Cells)).ToArray();
                block._neighbors = provinceBlocks.Where(other => other != block && other.Edges.Intersect(outter).Any()).ToHashSet();
            }

            tilemap.Clear();
            for (int i = 0; i < tilemap.GetLayersCount(); i++)
            {
                tilemap.RemoveLayer(i);
            }

            var colors = new HashSet<Color>();
            while (colors.Count < provinceBlocks.Count)
            {
                colors.Add(new Color(random.Next(0, 10) / 10.0f, random.Next(0, 10) / 10.0f, random.Next(0, 10) / 10.0f));
            }

            tilemap.Clear();
            for (int i = 0; i < provinceBlocks.Count; i++)
            {
                tilemap.AddLayer(i);
                tilemap.SetLayerModulate(i, colors.ElementAt(i));

                foreach (var cell in provinceBlocks[i].Cells)
                {
                    tilemap.SetCellEx(i, cell, 0);
                }
            }

            return provinceBlocks;
        }


        static ProvinceBlock BuildProvinceBlock(TileMap tilemap, List<Vector2I> cells, Random random)
        {
            var maxSize = random.Next(8, 40);

            var block = new ProvinceBlock();

            block.Add(tilemap, cells[0]);

            cells.Remove(cells[0]);

            while (true)
            {
                var outters = block.Edges.SelectMany(index => tilemap.GetNeighborCells_4(index).Values.Where(neighbor => cells.Contains(neighbor)))
                    .OrderBy(x => random.Next()).ToArray();

                if (outters.Length == 0)
                {
                    return block;
                }

                foreach (var index in outters)
                {
                    if (random.Next(0, 100) < 50)
                    {
                        block.Add(tilemap, index);
                        cells.Remove(index);

                        if (block.Cells.Count() > maxSize)
                        {
                            return block;
                        }

                        if (cells.Count() == 0)
                        {
                            return block;
                        }
                    }
                }
            }
        }
    }
}

