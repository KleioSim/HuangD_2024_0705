using System.Collections.Generic;

namespace HuangD.Sessions.Interfaces;

public interface IProvince
{
    int PopCount { get; }
    IEnumerable<IProvince> Neighbors { get; }
    ICountry Owner { get; }
}