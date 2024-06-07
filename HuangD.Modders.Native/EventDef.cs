using Chrona.Engine.Core.Events;
using Chrona.Engine.Core.Interfaces;
using Chrona.Engine.Core.Modders;
using HuangD.Sessions;

namespace HuangD.Modders.Native;

[DefTo(typeof(Country))]
public class EventDef : IEventDef
{
    public ICondition Condition { get; } = new TrueCondtion();

    public ITargetFinder TargetFinder { get; } = new TargetFinder()
    {
        Targets = new NeighorCountires(),
        ConditionFactors = new ICondtionFactor[]
        {
            new CondtionFactor()
            {
                Condition = new TrueCondtion(),
                Factor = 0.3
            }
        }
    };

    public IOption Option { get; } = new Option()
    {
        MessageBinds = new IMessageBind[]
        {
            new MessageBind()
            {
                MessageType = typeof(Message_Start),
                TargetVisitor = new EventTargetVisitor(),
                ValueVisitor = new EventFromVisitor()
            }
        }
    };
}