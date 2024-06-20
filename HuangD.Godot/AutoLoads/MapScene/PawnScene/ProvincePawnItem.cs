using Chrona.Engine.Godot;
using Godot;
using HuangD.Sessions;
using HuangD.Sessions.Interfaces;
using System;

public partial class ProvincePawnItem : ViewControl, IItemView
{
    public static Func<Province, Vector2> GetPawnPosition;

    public object Id { get; set; }

    private Label Label => GetNode<Label>("Label");

    protected override void Initialize()
    {
        var province = Id as Province;
        Label.Text = province.Name;

        this.Position = GetPawnPosition(province);
    }

    protected override void Update()
    {
        var province = Id as Province;
        Label.Text = province.Name;

    }
}