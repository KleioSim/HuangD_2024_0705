using Godot;
using System;

public partial class InitialScene : Control
{
    public Button button => GetNode<Button>("CanvasLayer/Button");
    public TileMapRoot MapRoot => GetNode<TileMapRoot>("/root/MapScene/CanvasLayer/TileMapRoot");

    public override void _Ready()
    {
        button.Pressed += () =>
        {
            MapRoot.BuildMap();
        };
    }
}
