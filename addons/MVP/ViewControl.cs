using Godot;
using System;

public partial class ViewControl : Control
{
    private static PresentManager presentManager = new PresentManager();

    public ViewControl()
    {
        this.Ready += () =>
        {
            GD.Print($"[V] {this.GetType().Name} Ready");
            presentManager.OnViewReady(this);
        };
    }
}
