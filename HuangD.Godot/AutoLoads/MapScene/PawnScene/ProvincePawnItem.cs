using Chrona.Engine.Godot;
using Godot;
using HuangD.Sessions;
using HuangD.Sessions.Interfaces;
using System;

public partial class ProvincePawnItem : ViewControl, IItemView
{
    public static Func<Province, Vector2> GetPawnPosition;

    public object Id { get; set; }

    private Province Province => Id as Province;
    private Label Label => GetNode<Label>("Label");

    protected override void Initialize()
    {
        Label.Text = Province.Name;
        this.Position = GetPawnPosition(Province);
    }

    protected override void Update()
    {
        Label.Text = Province.Name;
        this.Position = GetPawnPosition(Province);
    }
}
