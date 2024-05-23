using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

class CountryBlock
{
    public IEnumerable<ProvinceBlock> Edges => _edges;
    public IEnumerable<ProvinceBlock> Provinces => _province;

    private HashSet<ProvinceBlock> _province = new HashSet<ProvinceBlock>();
    private HashSet<ProvinceBlock> _edges = new HashSet<ProvinceBlock>();

    internal void Add(ProvinceBlock provinceBlock)
    {
        _province.Add(provinceBlock);
        _edges.Add(provinceBlock);

        var needRemove = _edges.Where(x => x.Neighbors.Except(_province).Count() == 0).ToArray();
        _edges.ExceptWith(needRemove);
    }
}

public static class CountryBuilder
{

    internal static List<CountryBlock> Build(TileMap tilemap, List<ProvinceBlock> provinceBlocks, Random random)
    {

        var countryBlocks = new List<CountryBlock>();
        while (provinceBlocks.Count > 0)
        {
            var country = BuildCountry(tilemap, provinceBlocks, random);
            countryBlocks.Add(country);
        }

        var needUnions = countryBlocks.Where(x => x.Provinces.Count() < 3).ToArray();
        countryBlocks = countryBlocks.Except(needUnions).ToList();

        foreach (var item in needUnions)
        {
            var countryBlock = countryBlocks.First(x => x.Provinces.Intersect(item.Provinces.SelectMany(x => x.Neighbors)).Any());
            foreach (var province in item.Provinces)
            {
                countryBlock.Add(province);
            }
        }

        var colors = new HashSet<Color>();
        while (colors.Count < countryBlocks.Count)
        {
            colors.Add(new Color(random.Next(0, 10) / 10.0f, random.Next(0, 10) / 10.0f, random.Next(0, 10) / 10.0f));
        }


        tilemap.Clear();
        for (int i = 0; i < tilemap.GetLayersCount(); i++)
        {
            tilemap.RemoveLayer(i);
        }

        for (int i = 0; i < countryBlocks.Count; i++)
        {
            tilemap.AddLayer(i);
            tilemap.SetLayerModulate(i, colors.ElementAt(i));

            foreach (var cell in countryBlocks[i].Provinces.SelectMany(x => x.Cells))
            {
                tilemap.SetCellEx(i, cell, 0);
            }
        }

        return countryBlocks;
    }

    private static CountryBlock BuildCountry(TileMap tilemap, List<ProvinceBlock> provinceBlocks, Random random)
    {
        var maxSize = random.Next(3, 10);

        var countryBlock = new CountryBlock();
        countryBlock.Add(provinceBlocks[0]);
        provinceBlocks.Remove(provinceBlocks[0]);

        while (true)
        {
            var outters = countryBlock.Edges.SelectMany(x => x.Neighbors.Intersect(provinceBlocks)).ToArray();
            if (outters.Count() == 0)
            {
                return countryBlock;
            }

            foreach (var outter in outters)
            {
                if (random.Next(0, 100) < 50)
                {
                    countryBlock.Add(outter);
                    provinceBlocks.Remove(outter);
                }

                if (countryBlock.Provinces.Count() > maxSize)
                {
                    return countryBlock;
                }

                if (provinceBlocks.Count() == 0)
                {
                    return countryBlock;
                }
            }
        }
    }
}
