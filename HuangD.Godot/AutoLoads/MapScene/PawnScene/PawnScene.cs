using Chrona.Engine.Godot;
using Godot;
using HuangD.Sessions.Interfaces;
using System.Linq;

public partial class PawnScene : ViewControl
{
    public ItemContainer<ProvincePawnItem> provinceContainer;

    protected override void Initialize()
    {
        provinceContainer = new ItemContainer<ProvincePawnItem>(() =>
        {
            return GetNode<InstancePlaceholder>("ProvincePawnItem");
        });
    }

    protected override void Update()
    {
        if (Session == null)
        {
            return;
        }

        var session = Session as HuangD.Sessions.Session;
        provinceContainer.Refresh(session.Provinces.Select(x => x as object).ToHashSet());
    }

    public void OnCameraZoomed(Vector2 value)
    {
        foreach (var item in provinceContainer.GetCurrentItems())
        {
            item.Scale = Vector2.One / value;
        }
    }
}
