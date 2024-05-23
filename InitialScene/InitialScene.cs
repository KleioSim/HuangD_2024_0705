using Godot;
using System;

public partial class InitialScene : Control
{
    public TileMapRoot MapRoot => GetNode<TileMapRoot>("/root/MapScene/CanvasLayer/TileMapRoot");

    public TextEdit Seed => GetNode<TextEdit>("CanvasLayer/VBoxContainer/BuildMapPanel/VBoxContainer/SeedEdit");
    public Button BuildMap => GetNode<Button>("CanvasLayer/VBoxContainer/BuildMapPanel/VBoxContainer/Button");

    private Random random = new Random();

    public override void _Ready()
    {
        Seed.Text = random.Next().ToString();

        BuildMap.Pressed += () =>
        {
            MapRoot.BuildMap(Seed.Text);
        };
    }
}
