using Chrona.Engine.Core.Events;
using Chrona.Engine.Core.Interfaces;
using Chrona.Engine.Core.Modders;
using Chrona.Engine.Core.Sessions;
using HuangD.Sessions;
using HuangD.Sessions.Interfaces;

namespace HuangD.Modders.Native;

[DefTo(typeof(Country))]
public class EventDef2 : EventDef
{
    private Random random = new Random();

    public override IOptionDef OptionDef { get; }
    public override Func<IEntity, ISession, bool> IsSatisfied { get; }
    public override Func<IEntity, ISession, IEntity> FindTarget { get; }

    public EventDef2()
    {
        OptionDef = new OptionDef()
        {
            GetDesc = (context) => "Test",

            ProductMessage = (context) =>
            {
                return new[] { new Message_Start() { Target = context.To, Value = context.From } };
            }
        };

        FindTarget = (from, session) =>
        {
            var country = from as ICountry;

            foreach (var neighbor in country.Neighbors)
            {
                if (random.Next(0, 100) < 30)
                {
                    return neighbor;
                }
            }

            return null;
        };

        IsSatisfied = (from, session) =>
        {
            return true;
        };
    }
}
