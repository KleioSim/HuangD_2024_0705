using Chrona.Engine.Core.Events;
using Chrona.Engine.Core.Interfaces;
using Chrona.Engine.Core.Modders;
using HuangD.Sessions;
using HuangD.Sessions.Interfaces;

namespace HuangD.Modders.Native;

[DefTo(typeof(Country))]
public class EventDef2 : IEventDef
{
    public IOption Option { get; } = new Option()
    {
        Desc = "Test",

        ProductMessage = (entity, to, session) =>
        {
            return new[] { new Message_Start() { Target = entity, Value = to } };
        }
    };

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