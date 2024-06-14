using Chrona.Engine.Godot;
using Godot;
using HuangD.Sessions;
using HuangD.Sessions.Interfaces;
using System;
using System.Linq;

public partial class CountryDetail : ViewControl
{
    public Label Label => GetNode<Label>("VBoxContainer/Label");

    public Control PlayerMask => GetNode<Control>("VBoxContainer/HBoxContainer/Interactions/PlayerMask");
    public Control DateMask => GetNode<Control>("VBoxContainer/HBoxContainer/Interactions/DateMask");

    public ICountry Country
    {
        get => country;
        set
        {
            if (country == value)
            {
                return;
            }

            country = value;
            SendCommand(new Message_UIRefresh());
        }
    }

    public ItemContainer<CountryInteraction> CountryIteractions;

    private ICountry country;

    protected override void Initialize()
    {
        CountryIteractions = new ItemContainer<CountryInteraction>(() =>
        {
            return GetNode<InstancePlaceholder>("VBoxContainer/HBoxContainer/Interactions/VBoxContainer/CountryInteraction");
        });
    }

    protected override void Update()
    {
        Label.Text = Country.Name;

        PlayerMask.Visible = Country == Session.Player;
        DateMask.Visible = Country.IsInteractionDateOut;

        if (!PlayerMask.Visible && !DateMask.Visible)
        {
            CountryIteractions.Refresh(Country.Interactions.Select(x => (object)x).ToHashSet());
        }
    }
}