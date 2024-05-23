using Godot;
using System;

public partial class InitialScene : Control
{
    public Global Global => GetNode<Global>("/root/Global");

    public TextEdit Seed => GetNode<TextEdit>("CanvasLayer/VBoxContainer/BuildMapPanel/VBoxContainer/SeedEdit");
    public Button BuildMap => GetNode<Button>("CanvasLayer/VBoxContainer/BuildMapPanel/VBoxContainer/Button");

    public Label CountryName => GetNode<Label>("CanvasLayer/VBoxContainer/SelectCountryPanel/CountryName");

    private Random random = new Random();

    private Session session;

    public override void _Ready()
    {
        Seed.Text = random.Next().ToString();

        BuildMap.Pressed += () =>
        {
            Global.BuildGame(Seed.Text);
        };

        Global.MapClick += (cell, province, country) =>
        {
            if (country != null)
            {
                CountryName.Text = country.Name;
            }
        };
    }
}
