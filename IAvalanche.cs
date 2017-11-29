using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalanche.Models;
using Rock.Web.Cache;
using Rock.Web.UI;

namespace Avalanche
{
    public interface IAvalanche
    {
        MobileBlock GetMobile();
        Dictionary<string, string> HandlePostback( Dictionary<string,string> Body);
    }
}
