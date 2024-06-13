using Chrona.Engine.Core.Events;
using Chrona.Engine.Core.Interfaces;
using Chrona.Engine.Core.Modders;
using HuangD.Sessions;
using HuangD.Sessions.Interfaces;

namespace HuangD.Modders.Native;

[DefTo(typeof(Country))]
public class CountryInteractionDef_Start : InteractionDef
{
    public override Func<IEntity, ISession, IEnumerable<IMessage>> Invoke { get; } = (owner, session) =>
    {
        return new IMessage[] { new Message_Start() { Target = owner, Value = session.Player } };
    };

    public override Func<IEntity, ISession, bool> IsVaild { get; } = (owner, session) =>
    {
        if (owner == session.Player)
        {
            return false;
        }

        var country = owner as ICountry;
        if (country.Wars.Any(x => x.From == session.Player || x.To == session.Player))
        {
            return false;
        }

        return true;
    };

    public override string GetDesc(IEntity owner)
    {
        var country = owner as ICountry;
        return $"Start_{country.Name}";
    }
}

[DefTo(typeof(Country))]
public class CountryInteractionDef_Peace : InteractionDef
{
    public override Func<IEntity, ISession, IEnumerable<IMessage>> Invoke { get; } = (owner, session) =>
    {
        return new IMessage[] { new Message_Peace() { Target = owner, Value = session.Player } };
    };

    public override Func<IEntity, ISession, bool> IsVaild { get; } = (owner, session) =>
    {
        if (owner == session.Player)
        {
            return false;
        }

        var country = owner as ICountry;
        if (country.Wars.Any(x => x.From == session.Player || x.To == session.Player))
        {
            return true;
        }

        return false;
    };

    public override string GetDesc(IEntity owner)
    {
        var country = owner as ICountry;
        return $"Peace_{country.Name}";
    }
}