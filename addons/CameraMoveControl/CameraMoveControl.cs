using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace CameraMoveControl
{
    public partial class CameraMoveControl : Control
    {
        [Export]
        Camera2D Camera { get; set; }

        bool isMoveOn;
        float moveAngle;

        Control[] controlPanel => new[]
        {
            GetNode<Control>("Content/Top"),
            GetNode<Control>("Content/Bottom"),
            GetNode<Control>("Content/Right"),
            GetNode<Control>("Content/Left"),
        };

        public override void _EnterTree()
        {
            var controlPanel = GD.Load<PackedScene>("res://addons/CameraMoveControl/CameraMoveControl.tscn").Instantiate();
            this.AddChild(controlPanel, true);

            var paths = new[] { "Top", "Bottom", "Right", "Left" };
            foreach (var path in paths)
            {
                var control = controlPanel.GetNode<Control>(path);
                control.GuiInput += OnControlGUIInput;
                control.MouseEntered += OnMouseEnterControl;
                control.MouseExited += OnMouseExitControl;
            }

            this.MouseFilter = MouseFilterEnum.Ignore;
        }


        public void OnMouseEnterControl()
        {
            isMoveOn = true;
        }

        public void OnMouseExitControl()
        {
            isMoveOn = false;
        }

        public void OnControlGUIInput(InputEvent @event)
        {
            if (!isMoveOn)
            {
                return;
            }

            if (@event is InputEventMouseMotion moveEvent)
            {

                moveAngle = (GlobalPosition + (Size / 2)).AngleToPoint(moveEvent.GlobalPosition);

                GD.Print($"{moveAngle})");

            }
        }

        public override void _Process(double delta)
        {
            if (!isMoveOn)
            {
                return;
            }


            Camera.Position = Camera.Position + (Vector2.Right * 10).Rotated(moveAngle);
        }
    }
}
