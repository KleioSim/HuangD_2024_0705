using Chrona.Engine.Core.Events;
using Chrona.Engine.Core.Interfaces;
using Chrona.Engine.Core.Modders;
using HuangD.Sessions;
using HuangD.Sessions.Interfaces;

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

[DefTo(typeof(Country))]
public class EventDef2 : IEventDef2
{
    public IOption2 Option { get; } = new Option2()
    {
        Desc = "Test",
    }

    private Random random = new Random();

    public IEntity FindTarget(IEntity entity, ISession session)
    {
        var country = entity as ICountry;

        foreach (var neighbor in country.Neighbors)
        {
            if (random.Next(0, 100) < 30)
            {
                return neighbor;
            }
        }

        return null;
    }

    public bool IsSatisfied(IEntity entity, ISession session)
    {
        return true;
    }
}

internal class Option2 : IOption2
{
    public string Desc { get; init; }

    public IEnumerable<IMessage> Do(IEntity entity, ISession session)
    {
        throw new NotImplementedException();
    }
}