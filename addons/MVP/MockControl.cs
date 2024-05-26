using Godot;

public abstract partial class MockControl<TView, TModel> : Control
    where TView : ViewControl
    where TModel : class
{
    protected PresentControl<TView, TModel> Present => GetParent() as PresentControl<TView, TModel>;

    protected TView View => GetParent().GetParent() as TView;

    public abstract TModel Mock { get; }

    public MockControl()
    {
        this.Ready += () =>
        {
            GD.Print($"[M] {this.GetType().Name} Ready");
        };
    }
}
