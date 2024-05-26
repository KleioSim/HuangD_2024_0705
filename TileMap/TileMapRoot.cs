﻿using Godot;
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
    internal List<ProvinceBlock> ProvinceBlocks { get; private set; }
    internal List<CountryBlock> CountryBlocks { get; private set; }

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
        GD.Print($"total provCount:{ProvinceBlocks.Count()}, max provSize {ProvinceBlocks.Max(x => x.Cells.Count())}, min provSize {ProvinceBlocks.Min(x => x.Cells.Count())}");

        CountryBlocks = CountryBuilder.Build(CountryMap, ProvinceBlocks, random);
        GD.Print($"total countryCount:{CountryBlocks.Count()}, max countrySize {CountryBlocks.Max(x => x.Provinces.Count())}, min countrySize {CountryBlocks.Min(x => x.Provinces.Count())}");

        if(Camera != null)
        {
            Camera.Position = TerrainMap.MapToLocal(TerrainMap.GetUsedRect().GetCenter());
            Camera.Zoom = new Vector2(0.5f, 0.5f);
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
}
