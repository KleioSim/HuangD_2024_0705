using Chrona.Engine.Core.Events;
using Chrona.Engine.Core.Interfaces;
using Chrona.Engine.Core.Modders;
using HuangD.Sessions;
using HuangD.Sessions.Interfaces;
using System.Diagnostics.Metrics;

namespace HuangD.Modders.Native;

[DefTo(typeof(Country))]
public class CountryInteractionDef_Start : InteractionDef
{
    public override Func<IEntity, ISession, IEnumerable<IMessage>> Invoke { get; } = (owner, session) =>
    {
        return new IMessage[] { new Message_Start() { Target = owner, Value = session.Player } };
    };

    public override string GetDesc(IEntity owner)
    {
        var country = owner as ICountry;
        return $"Start_{country.Name}";
    }

    public override IEnumerable<(bool flag, string desc)> GetVaildGroups(IEntity owner, ISession session)
    {
        var country = owner as ICountry;
        return new[] {
            (!country.Wars.Any(x => x.From == session.Player || x.To == session.Player), "not in war"),
            (country.Neighbors.Contains(session.Player), "must player country neighbor")
        };
    }
}

[DefTo(typeof(Country))]
public class CountryInteractionDef_Peace : InteractionDef
{
    public override Func<IEntity, ISession, IEnumerable<IMessage>> Invoke { get; } = (owner, session) =>
    {
        return new IMessage[] { new Message_Peace() { Target = owner, Value = session.Player } };
    };

    public override string GetDesc(IEntity owner)
    {
        var country = owner as ICountry;
        return $"Peace_{country.Name}";
    }

    public override IEnumerable<(bool flag, string desc)> GetVaildGroups(IEntity owner, ISession session)
    {
        var country = owner as ICountry;
        return new[]
        {
            (country.Wars.Any(x => x.From == session.Player || x.To == session.Player), "alreay in war"),
            (country.WarWeary > 3, $"peace attitude {country.WarWeary} must > 3"),
        };
    }
}