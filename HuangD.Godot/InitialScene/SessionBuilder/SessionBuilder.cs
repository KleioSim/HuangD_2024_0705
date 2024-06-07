using Chrona.Engine.Godot;
using Godot;
using HuangD.Sessions;
using System;
using System.Linq;

public partial class SessionBuilder : Node2D
{
    public MapScene MapScene => GetNode<MapScene>("/root/MapScene");

    internal void BuildGame(string seed)
    {
        MapScene.MapRoot.BuildMap(seed);

        var session = new Session(MapScene.MapRoot.ProvinceBlocks.Keys, MapScene.MapRoot.CountryBlocks.Keys);

        GetNode<Global>("/root/Chrona_Global").Chroncle.Session = session;
    }
}
