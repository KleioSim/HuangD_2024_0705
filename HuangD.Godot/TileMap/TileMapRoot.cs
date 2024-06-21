using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

public partial class TileMapRoot : Node2D
{
    [Export]
    Camera2D Camera { get; set; }

    internal TileMap TerrainMap => GetNode<TileMap>("TerrainMap");
    internal TileMap PopCountMap => GetNode<TileMap>("PopCountMap");
    internal TileMap ProvinceMap => GetNode<TileMap>("ProvinceMap");
    internal TileMap CountryMap => GetNode<TileMap>("CountryMap");

    internal Dictionary<Vector2I, TerrainType> Terrains { get; private set; }
    internal Dictionary<Vector2I, int> Pops { get; private set; }

    internal Dictionary<string, ProvinceBlock> ProvinceBlocks { get; private set; }
    internal Dictionary<string, CountryBlock> CountryBlocks { get; private set; }

    //internal List<ProvinceBlock> ProvinceBlocks { get; private set; }
    //internal List<CountryBlock> CountryBlocks { get; private set; }

    private Dictionary<CountryBlock, int> block2LayerId = new Dictionary<CountryBlock, int>();

    [Signal]
    public delegate void ClickTileEventHandler(Vector2I index);

    internal void BuildMap(string seed)
    {
        var algo = SHA1.Create();
        var hash = BitConverter.ToInt32(algo.ComputeHash(Encoding.UTF8.GetBytes(seed)), 0);

        var random = new Random(hash);

        Terrains = TerrainBuilder.Build(TerrainMap, random);
        GD.Print(String.Join(",", Terrains.GroupBy(x => x.Value).Select(group => $"{group.Key}:{group.Count()}")));

        Pops = PopCountBuilder.Build(PopCountMap, Terrains, random);
        GD.Print($"total popCount:{Pops.Values.Sum()}, max popCount {Pops.Values.Max()}, min popCount {Pops.Values.Min()}");

        ProvinceBlocks = ProvinceBlock.Builder.Build(ProvinceMap, Pops, random);
        GD.Print($"total provCount:{ProvinceBlocks.Count()}, max provSize {ProvinceBlocks.Values.Max(x => x.Cells.Count())}, min provSize {ProvinceBlocks.Values.Min(x => x.Cells.Count())}");

        CountryBlocks = CountryBlock.Builder.Build(CountryMap, ProvinceBlocks.Values, random);
        GD.Print($"total countryCount:{CountryBlocks.Count()}, max countrySize {CountryBlocks.Values.Max(x => x.Provinces.Count())}, min countrySize {CountryBlocks.Values.Min(x => x.Provinces.Count())}");


        block2LayerId.Clear();

        var colors = new HashSet<Color>();
        while (colors.Count < CountryBlocks.Count)
        {
            colors.Add(new Color(random.Next(0, 10) / 10.0f, random.Next(0, 10) / 10.0f, random.Next(0, 10) / 10.0f));
        }

        CountryMap.Clear();
        for (int i = 0; i < CountryMap.GetLayersCount(); i++)
        {
            CountryMap.RemoveLayer(i);
        }

        for (int i = 0; i < CountryBlocks.Count; i++)
        {
            block2LayerId.Add(CountryBlocks.Values.ElementAt(i), i);

            CountryMap.AddLayer(i);
            CountryMap.SetLayerModulate(i, colors.ElementAt(i));

            foreach (var cell in CountryBlocks.Values.ElementAt(i).Provinces.SelectMany(x => x.Cells))
            {
                CountryMap.SetCellEx(i, cell, 0);
            }
        }

        if (Camera != null)
        {
            Camera.Position = TerrainMap.MapToLocal(TerrainMap.GetUsedRect().GetCenter());
        }

    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton eventKey)
        {
            if (eventKey.Pressed)
            {
                if (eventKey.ButtonIndex == MouseButton.Left)
                {
                    var tileIndex = TerrainMap.LocalToMap(GetGlobalMousePosition());
                    GD.Print(tileIndex);

                    if (TerrainMap.GetCellSourceId(0, tileIndex) != -1)
                    {
                        EmitSignal(SignalName.ClickTile, tileIndex);
                    }
                }
            }
            return;
        }
    }


    internal void ChangeProvinceOwner(ProvinceBlock provBlock, CountryBlock newCountryBlock)
    {
        var oldCountryBlock = CountryBlocks.Values.SingleOrDefault(x => x.Provinces.Contains(provBlock));
        oldCountryBlock.Remove(provBlock);

        newCountryBlock.Add(provBlock);

        foreach (var cell in provBlock.Cells)
        {
            CountryMap.EraseCell(block2LayerId[oldCountryBlock], cell);
        }

        foreach (var cell in provBlock.Cells)
        {
            CountryMap.SetCellEx(block2LayerId[newCountryBlock], cell, 0);
        }
    }
}
