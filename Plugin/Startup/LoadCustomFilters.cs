using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalanche.Lava;
using DotLiquid;

namespace Avalanche.Startup
{
    public partial class LoadCustomFilters : Rock.Utility.IRockOwinStartup
    {

        public int StartupOrder
        {
            get
            {
                return 0;
            }
        }

        public void OnStartup( global::Owin.IAppBuilder app )
        {
            Template.RegisterFilter( typeof( CustomFilters ) );
        }

    }
}
