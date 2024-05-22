using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

class CountryBlock
{
    public IEnumerable<ProvinceBlock> Edges { get; set; }
    public IEnumerable<ProvinceBlock> Provinces { get; set; }

    internal void Add(ProvinceBlock provinceBlock)
    {
        throw new NotImplementedException();
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


        var colors = new HashSet<Color>();
        while (colors.Count < countryBlocks.Count)
        {
            colors.Add(new Color(random.Next(0, 10) / 10.0f, random.Next(0, 10) / 10.0f, random.Next(0, 10) / 10.0f));
        }

        tilemap.Clear();
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
        var maxSize = random.Next(5, 10);

        var countryBlock = new CountryBlock();

        countryBlock.Add(provinceBlocks[0]);
        provinceBlocks.Remove(provinceBlocks[0]);

        while (true)
        {
            var outters = countryBlock.Edges.SelectMany(x => x.GetNeighbors(provinceBlocks));
            if (outters.Count() == 0)
            {
                break;
            }

            foreach (var outter in outters)
            {
                if (random.Next(0, 100) < 50)
                {
                    countryBlock.Add(provinceBlocks[0]);
                    provinceBlocks.Remove(provinceBlocks[0]);
                }

                if (countryBlock.Provinces.Count() > maxSize)
                {
                    break;
                }

                if (provinceBlocks.Count() == 0)
                {
                    break;
                }
            }
        }

        return countryBlock;

    }
}
