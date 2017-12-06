using System.Collections.Generic;
using Avalanche.Models;

namespace Avalanche
{
    public interface IMobileResource
    {
        MobileBlock GetMobile(string arg);
        MobileBlockResponse HandleRequest( string resource, Dictionary<string, string> Body );
    }
}
