using Godot;
using System;
using System.Linq;

public partial class InitialScene : Control
{
    public Global Global => GetNode<Global>("/root/Global");

    public TextEdit Seed => GetNode<TextEdit>("CanvasLayer/VBoxContainer/BuildMapPanel/VBoxContainer/SeedEdit");
    public Button BuildMap => GetNode<Button>("CanvasLayer/VBoxContainer/BuildMapPanel/VBoxContainer/Button");

    public Label CountryName => GetNode<Label>("CanvasLayer/VBoxContainer/SelectCountryPanel/MarginContainer/VBoxContainer/CountryName");
    public Label ProvinceCount => GetNode<Label>("CanvasLayer/VBoxContainer/SelectCountryPanel/MarginContainer/VBoxContainer/ProvinceCount");
    public Label PopCount => GetNode<Label>("CanvasLayer/VBoxContainer/SelectCountryPanel/MarginContainer/VBoxContainer/PopCount");
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

            GetTree().ChangeSceneToFile("res://MainScene/MainScene.tscn");
        };

        Global.MapClick += (cell, province, country) =>
        {
            CountryName.Text = country != null ? country.Name : "--";
            ProvinceCount.Text = country != null ? country.Provinces.Count().ToString() : "--";
            PopCount.Text = country != null ? country.Provinces.Sum(x => x.PopCount).ToString() : "--";

            ConfirmCountry.Disabled = country == null;
        };
    }
}
