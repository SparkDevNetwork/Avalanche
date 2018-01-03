using System.Collections.Generic;
using Avalanche.Models;

namespace Avalanche
{
    public interface IMobileResource
    {
        MobileBlock GetMobile(string parameter);
        MobileBlockResponse HandleRequest( string request, Dictionary<string, string> Body );
    }
}
