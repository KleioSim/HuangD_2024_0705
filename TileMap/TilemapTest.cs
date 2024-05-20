using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public partial class TilemapTest : Control
{
    public Button button => GetNode<Button>("CanvasLayer/Button");
    public TileMapBase BaseMap => GetNode<TileMapBase>("CanvasLayer2/TileMap2048");
    //public TileMap1024 Terrain1024 => GetNode<TileMap1024>("CanvasLayer2/TileMap1024");
    public TileMap512 Terrain512 => GetNode<TileMap512>("CanvasLayer2/TileMap512");
    public TileMapTerrain TerrainMap => GetNode<TileMapTerrain>("CanvasLayer2/TileMapTerrain");
    //public TileMapAuto AutoMap => GetNode<TileMapAuto>("CanvasLayer2/TilemapAuto");
    //public TileMapFractal TileMap2048 => GetNode<TileMapFractal>("CanvasLayer2/TileMap2048");
    public Slider slider => GetNode<Slider>("CanvasLayer/Slider");

    public override void _Ready()
    {
        button.Pressed += () =>
        {
            TerrainMap.Clear();

            var random = new Random();

            BlockBuilder.Build(TerrainMap, random);


            //var bpoint = BaseMap.GenerateMap();
            //Terrain512.GenerateMap(BaseMap, bpoint);
            //TerrainMap.GenerateMap(Terrain512, bpoint);

            //var landcount = TerrainMap.GetUsedCells(0).Where(x => TerrainMap.GetCellSourceId(0, x) != 3).Count();

            //GD.Print($"{landcount}, {landcount / 20}, {landcount / 100}");


        };
    }
}
