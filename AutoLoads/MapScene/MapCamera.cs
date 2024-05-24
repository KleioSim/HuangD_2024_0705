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
                var changed = Vector2.One * ZoomStep;
                if (eventKey.ButtonIndex == MouseButton.WheelDown)
                {

                }
                else if (eventKey.ButtonIndex == MouseButton.WheelUp)
                {
                    changed *= -1;
                }

                if (Zoom + changed <= ZoomMax * Vector2.One
                    && Zoom + changed >= ZoomMin * Vector2.One)
                {
                    Zoom += changed;
                }
            }

            return;
        }
    }
}
