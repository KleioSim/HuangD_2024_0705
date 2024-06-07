﻿using Chrona.Engine.Godot;
using Godot;
using HuangD.Godot.Utilties;
using HuangD.Sessions;
using System;
using System.Diagnostics.Metrics;
using System.Linq;

public partial class InitialScene : Control
{
    public MapScene MapScene => GetNode<MapScene>("/root/MapScene");
    public Global Global => GetNode<Global>("/root/Chrona_Global");

    public SessionBuilder SessionBuilder => GetNode<SessionBuilder>("SessionBuilder");

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

        BuildMap.Connect(Button.SignalName.Pressed, new Callable(this, MethodName.BuildGame));
        ConfirmCountry.Connect(Button.SignalName.Pressed, new Callable(this, MethodName.ConfirmPlayCountry));
        MapScene.Connect(MapScene.SignalName.MapClick, new Callable(this, MethodName.SelectCountry));
    }

    private void BuildGame()
    {
        SessionBuilder.BuildGame(Seed.Text);
    }

    private void ConfirmPlayCountry()
    {
        var session = Global.Chroncle.Session as Session;
        session.Player = session.Countries.Single(x => x.Name == CountryName.Text);

        GetTree().ChangeSceneToFile("res://MainScene/MainScene.tscn");
    }

    private void SelectCountry(Vector2I index, string provinceName, string countryName)
    {
        ConfirmCountry.Disabled = countryName == "";
        CountryName.Text = countryName != "" ? countryName : "--";

        var session = Global.Chroncle.Session as Session;

        var selectCountry = countryName == "" ? null : session.Countries.Single(x => x.Name == CountryName.Text);
        ProvinceCount.Text = selectCountry != null ? selectCountry.Provinces.Count().ToString() : "--";
        PopCount.Text = selectCountry != null ? selectCountry.Provinces.Sum(x => x.PopCount).ToString() : "--";

    }
}
