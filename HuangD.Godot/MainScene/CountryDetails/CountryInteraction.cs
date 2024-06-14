using Chrona.Engine.Core.Events;
using Chrona.Engine.Core.Interfaces;
using Chrona.Engine.Godot;
using Godot;
using HuangD.Sessions.Interfaces;

public partial class CountryInteraction : ViewControl, IItemView
{
    public object Id { get; set; }

    private Button Button => GetNode<Button>("Button");
    private TooltipTrigger TooltipTrigger => GetNode<TooltipTrigger>("Button/ToolTipTrigger");

    protected override void Initialize()
    {
        Button.Connect(Button.SignalName.ButtonDown, new Callable(this, MethodName.OnInvoke));
        TooltipTrigger.funcGetToolTipString = () =>
        {
            return "Test";
        };
    }

    protected override void Update()
    {
        var interaction = Id as IInteraction;

        Button.Text = interaction.Desc;
        Button.Disabled = !interaction.IsVaild(Session);
    }

    private void OnInvoke()
    {
        var interaction = Id as IInteraction;
        interaction.Invoke(Session);

        SendCommand(new Message_UIRefresh());
    }
}