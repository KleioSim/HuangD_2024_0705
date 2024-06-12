using Chrona.Engine.Core.Interfaces;
using System.Collections.Generic;

namespace HuangD.Sessions.Interfaces;

public interface ICountry : IEntity
{
    string Name { get; }
    IEnumerable<IProvince> Provinces { get; }
    IEnumerable<ICountry> Neighbors { get; }

    IEnumerable<IWar> Wars { get; }
    IEnumerable<IInteraction> Interactions { get; }
}

public interface IWar
{
    ICountry From { get; }
    ICountry To { get; }
}

public interface IInteraction
{
    string Desc { get; }
}