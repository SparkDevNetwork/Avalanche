using System;
using System.Collections.Generic;
using System.Text;

namespace Avalanche.Components
{
    public interface IIconFont
    {
        Dictionary<string, string> LookupTable { get; }
        string iOSFont { get; }
        string AndroidFont { get; }
    }
}
