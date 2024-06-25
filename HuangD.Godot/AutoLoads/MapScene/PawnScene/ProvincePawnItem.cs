using Chrona.Engine.Godot;
using Godot;
using HuangD.Sessions;
using HuangD.Sessions.Interfaces;
using System;
using System.Linq;

public partial class ProvincePawnItem : ViewControl, IItemView
{
    public static Func<Province, Vector2> GetPawnPosition;

    public object Id
    {
        get => id;
        set
        {
            if (id != value)
            {
                id = value;
                this.Position = GetPawnPosition(Province);
            }
        }
    }

    private object id;

    private Province Province => Id as Province;

    private Label Label => GetNode<Label>("VBoxContainer/Label");
    private Label ArmyCount => GetNode<Label>("VBoxContainer/ArmyCount");

    protected override void Initialize()
    {

    }

    protected override void Update()
    {
        Label.Text = Province.Name;
        ArmyCount.Text = Province.LocalArmies.Count().ToString();
        this.Name = Province.Name;
    }
}
