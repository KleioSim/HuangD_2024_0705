using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

class CountryBlock
{
    public readonly string id;
    public IEnumerable<ProvinceBlock> Edges => _edges;
    public IEnumerable<ProvinceBlock> Provinces => _province;

    public ProvinceBlock Capital { get; set; }

    private HashSet<ProvinceBlock> _province = new HashSet<ProvinceBlock>();
    private HashSet<ProvinceBlock> _edges = new HashSet<ProvinceBlock>();

    internal void Add(ProvinceBlock provinceBlock)
    {
        if (_province.Count == 0)
        {
            Capital = provinceBlock;
        }

        _province.Add(provinceBlock);
        _edges.Add(provinceBlock);

        var needRemove = _edges.Where(x => x.Neighbors.Except(_province).Count() == 0).ToArray();
        _edges.ExceptWith(needRemove);

    }

    internal void Remove(ProvinceBlock provinceBlock)
    {
        _province.Remove(provinceBlock);
        _edges.Remove(provinceBlock);

        _edges.UnionWith(_province.Intersect(provinceBlock.Neighbors));
    }

    static int count = 0;

    public CountryBlock(string id)
    {
        this.id = id;
    }

    public static class Builder
    {

        internal static Dictionary<string, CountryBlock> Build(TileMap tilemap, IEnumerable<ProvinceBlock> provinceBlocks, Random random)
        {

            CountryBlock.count = 0;

            var blockList = provinceBlocks.ToList();
            var countryBlocks = new List<CountryBlock>();
            while (blockList.Count > 0)
            {
                var country = BuildCountry(blockList, random);
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

            return countryBlocks.ToDictionary(k => k.id, v => v);
        }

        private static CountryBlock BuildCountry(List<ProvinceBlock> provinceBlocks, Random random)
        {
            var maxSize = random.Next(3, 10);

            var countryBlock = new CountryBlock($"COUNTRY_{count++}");
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
}
