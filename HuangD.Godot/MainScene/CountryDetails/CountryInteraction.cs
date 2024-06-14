using Chrona.Engine.Core.Events;
using Chrona.Engine.Core.Interfaces;
using Chrona.Engine.Godot;
using Chrona.Engine.Godot.TooltipTrigger;
using Chrona.Engine.Godot.UBBCodes;
using Godot;
using HuangD.Sessions.Interfaces;
using System;
using System.Linq;

public partial class CountryInteraction : ViewControl, IItemView
{
    public object Id { get; set; }

    IInteraction Interaction => Id as IInteraction;

    private Button Button => GetNode<Button>("Button");
    private TooltipTrigger TooltipTrigger => GetNode<TooltipTrigger>("Button/ToolTipTrigger");

    protected override void Initialize()
    {
        Button.Connect(Button.SignalName.ButtonDown, new Callable(this, MethodName.OnInvoke));
        TooltipTrigger.funcGetToolTipString = () =>
        {
            var vaildGroup = Interaction.GetVaildGroups(Session);
            return string.Join("\n", vaildGroup.Select(x => new UBBCore($"{x.flag} {x.desc}").Color(x.flag ? UBBColor.GREEN : UBBColor.RED)));
        };
    }

    protected override void Update()
    {

        Button.Text = Interaction.Desc;
        Button.Disabled = !Interaction.GetVaildGroups(Session).All(x => x.flag);
    }

    private void OnInvoke()
    {
        Interaction.Invoke(Session);

        SendCommand(new Message_UIRefresh());
    }
}