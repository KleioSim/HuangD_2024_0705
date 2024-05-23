using Godot;
using System;
using System.Linq;
using System.Reflection;

public partial class TilemapTest : Control
{
    public Button button => GetNode<Button>("CanvasLayer/Button");
    public TileMapRoot MapRoot => GetNode<TileMapRoot>("CanvasLayer2/TileMapRoot");

    public override void _Ready()
    {
        button.Pressed += () =>
        {
            var random = new Random();
            MapRoot.BuildMap(random.Next().ToString());
        };
    }
}
