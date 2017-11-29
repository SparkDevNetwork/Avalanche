using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalanche
{
    public static class AvalancheUtilities
    {
        public static string GetShortAssembly( Type type)
        {
            return string.Format(
                "{0}, {1}",
                type.FullName,
                type.Assembly.GetName().Name
                );
        }
    }
}
