using Godot;
using System;

public abstract partial class PresentControl<TView, TModel> : PresentBase
    where TView : ViewControl
    where TModel : class
{
    public new TModel Model
    {
        get => PresentBase.Model as TModel;
        set
        {
            PresentBase.Model = value;
        }
    }

    public TView View => GetParent<TView>();
    private MockControl<TView, TModel> MockControl => GetNodeOrNull<MockControl<TView, TModel>>("Mock");

    public bool IsInitialized { get; private set; } = false;

    public PresentControl()
    {
        this.Ready += () =>
        {
            GD.Print($"[P] {this.GetType().Name} Ready");

            IsDirty = true;

            if (IsMock)
            {
                mockManager.OnPresentReady(this);
            }
        };
    }

    public override void _Process(double delta)
    {
        int f0 = Engine.GetFramesDrawn();

        if (!IsVisibleInTree())
        {
            IsDirty = true;

            return;
        }

        if (!IsDirty)
        {
            return;
        }

        IsDirty = false;

        if (Model == null)
        {
            if (!IsMock)
            {
                throw new Exception();
            }

            Model = MockControl.Mock;

            GD.Print($"[P]Use {this.GetType().Name} mock model");

        }

        if (!IsInitialized)
        {
            IsInitialized = true;

            Initialize(View, Model);
        }

        Update(View, Model);
    }

    //internal void SendCommand(object command)
    //{
    //    if (command is not Cmd_UIRefresh)
    //    {
    //        GD.Print($"OnUICommand:{command.GetType()}");

    //        Model.OnCommand(command);
    //    }

    //    foreach (PresentBase item in list)
    //    {
    //        item.IsDirty = true;
    //    }
    //}

    protected abstract void Initialize(TView view, TModel model);
    protected abstract void Update(TView view, TModel model);
}
