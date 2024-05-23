using Godot;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

public partial class TileMapRoot : Node2D
{
    public TileMap TerrainMap => GetNode<TileMap>("TerrainMap");
    public TileMap PopCountMap => GetNode<TileMap>("PopCountMap");
    public TileMap ProvinceMap => GetNode<TileMap>("ProvinceMap");
    public TileMap CountryMap => GetNode<TileMap>("CountryMap");

    internal void BuildMap(string seed)
    {
        var algo = SHA1.Create();
        var hash = BitConverter.ToInt32(algo.ComputeHash(Encoding.UTF8.GetBytes(seed)), 0);

        var random = new Random(hash);

        var terrains = TerrainBuilder.Build(TerrainMap, random);
        GD.Print(String.Join(",", terrains.GroupBy(x => x.Value).Select(group => $"{group.Key}:{group.Count()}")));

        var pops = PopCountBuilder.Build(PopCountMap, terrains, random);
        GD.Print($"total popCount:{pops.Values.Sum()}, max popCount {pops.Values.Max()}, min popCount {pops.Values.Min()}");

        var provinceBlocks = ProvinceBlock.Builder.Build(ProvinceMap, pops, random);
        GD.Print($"total provCount:{provinceBlocks.Count()}, max provSize {provinceBlocks.Max(x => x.Cells.Count())}, min provSize {provinceBlocks.Min(x => x.Cells.Count())}");

        var countryBlocks = CountryBuilder.Build(CountryMap, provinceBlocks, random);
        GD.Print($"total countryCount:{countryBlocks.Count()}, max countrySize {countryBlocks.Max(x => x.Provinces.Count())}, min countrySize {countryBlocks.Min(x => x.Provinces.Count())}");
    }
}
