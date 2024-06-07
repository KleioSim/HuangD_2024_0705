using Chrona.Engine.Core.Interfaces;
using System.Collections.Generic;

namespace HuangD.Sessions.Interfaces;

public interface ICountry : IEntity
{
    public IEnumerable<IProvince> Provinces { get; }
    public IEnumerable<ICountry> Neighbors { get; }
}
