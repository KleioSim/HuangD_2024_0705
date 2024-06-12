using Chrona.Engine.Godot;
using Godot;
using HuangD.Sessions.Interfaces;
using System;

public partial class CountryDetail : ViewControl
{
    public Label Name => GetNode<Label>("VBoxContainer/Label");
    public ICountry Country { get; set; }

    protected override void Initialize()
    {

    }

    protected override void Update()
    {
        Name.Text = Country.Name;
    }
}