using System.Collections.Generic;
using Avalanche.Models;

namespace Avalanche
{
    public interface IMobileResource
    {
        MobileBlock GetMobile();
        Dictionary<string, string> HandlePostback( Dictionary<string, string> Body );
    }
}
