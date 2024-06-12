using Chrona.Engine.Godot;
using Godot;
using HuangD.Sessions.Interfaces;

public partial class CountryInteraction : ViewControl, IItemView
{
    public object Id { get; set; }

    private Label Desc => GetNode<Label>("Button/Label");

    protected override void Initialize()
    {

    }

    protected override void Update()
    {
        var interaction = Id as IInteraction;

        Desc.Text = interaction.Desc;
    }
}