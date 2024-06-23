using Chrona.Engine.Godot;
using Godot;
using HuangD.Sessions;
using System;
using System.Linq;

public partial class InitialScene : ViewControl
{
    public MapScene MapScene => GetNode<MapScene>("/root/MapScene");

    public SessionBuilder SessionBuilder => GetNode<SessionBuilder>("SessionBuilder");

    public TextEdit Seed => GetNode<TextEdit>("CanvasLayer/VBoxContainer/BuildMapPanel/VBoxContainer/SeedEdit");
    public Button BuildMap => GetNode<Button>("CanvasLayer/VBoxContainer/BuildMapPanel/VBoxContainer/Button");

    public Label CountryName => GetNode<Label>("CanvasLayer/VBoxContainer/SelectCountryPanel/MarginContainer/VBoxContainer/CountryName");
    public Label ProvinceCount => GetNode<Label>("CanvasLayer/VBoxContainer/SelectCountryPanel/MarginContainer/VBoxContainer/ProvinceCount");
    public Label PopCount => GetNode<Label>("CanvasLayer/VBoxContainer/SelectCountryPanel/MarginContainer/VBoxContainer/PopCount");
    public Button ConfirmCountry => GetNode<Button>("CanvasLayer/VBoxContainer/SelectCountryPanel/MarginContainer/VBoxContainer/Button");


    private Random random = new Random();

    protected override void Initialize()
    {
        BuildMap.Connect(Button.SignalName.Pressed, new Callable(this, MethodName.BuildGame));
        ConfirmCountry.Connect(Button.SignalName.Pressed, new Callable(this, MethodName.ConfirmPlayCountry));
        MapScene.Connect(MapScene.SignalName.MapClick, new Callable(this, MethodName.SelectCountry));

        Seed.Text = random.Next().ToString();
    }

    protected override void Update()
    {

        if (Session == null || Session.Player == null)
        {
            ConfirmCountry.Disabled = true;
            CountryName.Text = "--";
            ProvinceCount.Text = "--";
            PopCount.Text = "--";
            return;
        }

        ConfirmCountry.Disabled = false;

        var country = Session.Player as Country;
        CountryName.Text = country.Name;
        ProvinceCount.Text = country.Provinces.Count().ToString();
        PopCount.Text = country.Provinces.Sum(x => x.PopCount).ToString();
    }

    private void BuildGame()
    {
        SessionBuilder.BuildGame(Seed.Text);

        SendCommand(new Message_UIRefresh());
    }

    private void ConfirmPlayCountry()
    {
        GetTree().ChangeSceneToFile("res://MainScene/MainScene.tscn");
    }

    private void SelectCountry(Vector2I index, string provinceName, string countryName)
    {
        var session = Session as Session;
        var player = session.Countries.Single(x => x.Name == countryName);

        SendCommand(new Message_ChangePlayCountry() { Target = player });
    }

}
