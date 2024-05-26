using Godot;
using System;
using System.Collections.Generic;

public abstract partial class PresentBase : Control
{
    public static bool IsMock { get; set; } = true;

    public static object Model;
    public static Action<object> SendCommand;

    protected static MockManager mockManager = new MockManager();

    protected static HashSet<PresentBase> list = new HashSet<PresentBase>();

    internal bool IsDirty { get; set; }

    static PresentBase()
    {
        SendCommand += (command) =>
        {
            foreach (PresentBase item in list)
            {
                item.IsDirty = true;
            }
        };
    }

    public PresentBase()
    {
        this.TreeEntered += () =>
        {
            list.Add(this);
        };

        this.TreeExiting += () =>
        {
            list.Remove(this);
        };
    }
}
