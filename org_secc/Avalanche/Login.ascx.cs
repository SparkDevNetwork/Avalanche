using System;
using System.ComponentModel;
using Rock.Model;
using Rock.Security;
using System.Web.UI;
using Rock.Web.Cache;
using Rock.Web.UI;
using System.Web;
using Rock.Data;
using System.Linq;
using System.Collections.Generic;
using Rock;
using Avalanche;
using Avalanche.Models;
using Rock.Attribute;

namespace RockWeb.Plugins.Avalanche
{
    [DisplayName( "Login App" )]
    [Category( "SECC > Avalanche" )]
    [Description( "Login Screen" )]
    public partial class Login : RockBlock, IMobileResource
    {

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summarysni>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad( EventArgs e )
        {
        }

        public MobileBlock GetMobile()
        {
            return new MobileBlock()
            {
                BlockType = "Avalanche.Blocks.Login",
                Body = new Dictionary<string, string>()
            };
        }

        public Dictionary<string, string> HandlePostback( Dictionary<string, string> Body )
        {
            return Body;
        }
    }
}