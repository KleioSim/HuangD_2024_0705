using Chrona.Engine.Core.Interfaces;
using System;
using System.Collections.Generic;

namespace HuangD.Sessions.Interfaces;

public interface ICountry : IEntity
{
    string Name { get; }
    IEnumerable<IProvince> Provinces { get; }
    IEnumerable<ICountry> Neighbors { get; }
    IEnumerable<IArmy> Armies { get; }

    IEnumerable<IWar> Wars { get; }
    float WarWeary { get; }
}

public interface IWar
{
    ICountry From { get; }
    ICountry To { get; }
}

public interface IArmy
{
    public IProvince Origin { get; }
    public IProvince Local { get; }
    public IProvince Target { get; }
    public ICountry Owner { get; }
}