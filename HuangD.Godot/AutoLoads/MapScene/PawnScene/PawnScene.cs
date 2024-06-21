using Chrona.Engine.Godot;
using Godot;
using HuangD.Sessions.Interfaces;
using System;
using System.Linq;

public partial class PawnScene : ViewControl
{
    public ItemContainer<ProvincePawnItem> provinceContainer;
    public ItemContainer<CountryPawnItem> countryContainer;

    private Vector2 zoom = Vector2.One;

    protected override void Initialize()
    {
        provinceContainer = new ItemContainer<ProvincePawnItem>(() =>
        {
            return GetNode<InstancePlaceholder>("ProvincePawns/Item");
        });

        countryContainer = new ItemContainer<CountryPawnItem>(() =>
        {
            return GetNode<InstancePlaceholder>("CountryPawns/Item");
        });

        provinceContainer.OnAddedItem = OnAddedItem;
        countryContainer.OnAddedItem = OnAddedItem;
    }

    private void OnAddedItem(ViewControl item)
    {
        item.Scale = Vector2.One / zoom;
    }

    protected override void Update()
    {
        if (Session == null)
        {
            return;
        }

        var session = Session as HuangD.Sessions.Session;
        provinceContainer.Refresh(session.Provinces.Select(x => x as object).ToHashSet());
        countryContainer.Refresh(session.Countries.Select(x => x as object).ToHashSet());
    }

    public void OnCameraZoomed(Vector2 value)
    {
        zoom = value;

        if (provinceContainer != null)
        {
            foreach (var item in provinceContainer.GetCurrentItems())
            {
                item.Scale = Vector2.One / zoom;
            }
        }
    }
}
