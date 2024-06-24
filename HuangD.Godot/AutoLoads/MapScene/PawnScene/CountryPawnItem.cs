using Chrona.Engine.Godot;
using Godot;
using HuangD.Sessions;
using System;

public partial class CountryPawnItem : ViewControl, IItemView
{
    public static Func<Country, Vector2> GetPawnPosition;

    public object Id
    {
        get => id;
        set
        {
            if (id != value)
            {
                id = value;
                this.Position = GetPawnPosition(Country);
            }
        }
    }

    private object id;

    private Country Country => Id as Country;
    private Label Label => GetNode<Label>("Label");

    protected override void Initialize()
    {

    }

    protected override void Update()
    {
        Label.Text = Country.Name;
        this.Name = Country.Name;
    }
}