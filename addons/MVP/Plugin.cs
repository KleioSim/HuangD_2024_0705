#if TOOLS
using Godot;
using System;

namespace MVP;

[Tool]
public partial class Plugin : EditorPlugin
{
    public override void _EnterTree()
    {
        var script = GD.Load<Script>("res://addons/MVP/ViewControl.cs");
        var texture = GD.Load<Texture2D>("res://addons/MVP/Icon.png");
        AddCustomType("ViewControl", "Control", script, texture);
    }

    public override void _ExitTree()
    {
        RemoveCustomType("ViewControl");
    }
}

#endif
