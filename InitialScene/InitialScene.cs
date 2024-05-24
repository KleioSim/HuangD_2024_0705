using Godot;
using System;
using System.Linq;

public partial class InitialScene : Control
{
    public Global Global => GetNode<Global>("/root/Global");

    public TextEdit Seed => GetNode<TextEdit>("CanvasLayer/VBoxContainer/BuildMapPanel/VBoxContainer/SeedEdit");
    public Button BuildMap => GetNode<Button>("CanvasLayer/VBoxContainer/BuildMapPanel/VBoxContainer/Button");

    public Label CountryName => GetNode<Label>("CanvasLayer/VBoxContainer/SelectCountryPanel/MarginContainer/VBoxContainer/CountryName");
    public Button ConfirmCountry => GetNode<Button>("CanvasLayer/VBoxContainer/SelectCountryPanel/MarginContainer/VBoxContainer/Button");

    private Random random = new Random();

    public override void _Ready()
    {
        Seed.Text = random.Next().ToString();

        BuildMap.Pressed += () =>
        {
            Global.BuildGame(Seed.Text);
        };

        ConfirmCountry.Pressed += () =>
        {
            Global.Session.Player = Global.Session.Countries.Single(x => x.Name == CountryName.Text);
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
