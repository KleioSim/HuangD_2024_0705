#if TOOLS
using Godot;
using System;

namespace CameraMoveControl
{
    [Tool]
    public partial class Plugin : EditorPlugin
    {
        public override void _EnterTree()
        {
            var script = GD.Load<Script>("res://addons/CameraMoveControl/CameraMoveControl.cs");
            var texture = GD.Load<Texture2D>("res://addons/CameraMoveControl/Icon.png");
            AddCustomType("CameraMoveControl", "Control", script, texture);
        }

        public override void _ExitTree()
        {
            RemoveCustomType("CameraMoveControl");
        }
    }
}

#endif
