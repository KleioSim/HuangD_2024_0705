using Godot;
using System;

public partial class MapCamera : Camera2D
{
    [Export]
    public float ZoomMin { get; set; }

    [Export]
    public float ZoomMax { get; set; }

    [Export]
    public float ZoomStep { get; set; }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton eventKey)
        {
            if (eventKey.Pressed)
            {
                if (eventKey.ButtonIndex == MouseButton.WheelDown
                    || eventKey.ButtonIndex == MouseButton.WheelUp)
                {
                    ZoomMap((ZoomType)eventKey.ButtonIndex);
                }
            }

            return;
        }
    }

    enum ZoomType
    {
        In = (int)MouseButton.WheelDown,
        Out = (int)MouseButton.WheelUp
    }

    private void ZoomMap(ZoomType zoomType)
    {
        var changed = Vector2.One * ZoomStep * (zoomType == ZoomType.Out ? -1 : 1);

        if (Zoom + changed <= ZoomMax * Vector2.One
            && Zoom + changed >= ZoomMin * Vector2.One)
        {
            Zoom += changed;
        }
    }
}
