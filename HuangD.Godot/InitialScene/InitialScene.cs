using Chrona.Engine.Godot;
using Godot;
using HuangD.Sessions;
using System;
using System.Diagnostics.Metrics;
using System.Linq;

public partial class InitialScene : Control
{
    public GameBuilder Builder => GetNode<GameBuilder>("/root/GameBuilder");
    public Global Global => GetNode<Global>("/root/Chrona_Global");

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
        Builder.Connect(GameBuilder.SignalName.MapClick, new Callable(this, MethodName.SelectCountry));
    }

    private void BuildGame()
    {
        Builder.BuildGame(Seed.Text);
    }

    private void ConfirmPlayCountry()
    {
        Global.GetSession<Session>().Player = Global.GetSession<Session>().Countries.Single(x => x.Name == CountryName.Text);

        GetTree().ChangeSceneToFile("res://MainScene/MainScene.tscn");
    }

    private void SelectCountry(Vector2I index, string provinceName, string countryName)
    {
        ConfirmCountry.Disabled = countryName == "";
        CountryName.Text = countryName != "" ? countryName : "--";

        var selectCountry = countryName == "" ? null : Global.GetSession<Session>().Countries.Single(x => x.Name == CountryName.Text);
        ProvinceCount.Text = selectCountry != null ? selectCountry.Provinces.Count().ToString() : "--";
        PopCount.Text = selectCountry != null ? selectCountry.Provinces.Sum(x => x.PopCount).ToString() : "--";

    }
}
