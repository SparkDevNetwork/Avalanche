// <copyright>
// Copyright Southeast Christian Church
//
// Licensed under the  Southeast Christian Church License (the "License");
// you may not use this file except in compliance with the License.
// A copy of the License shoud be included with this file.
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//
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
using Avalanche.Attribute;

namespace RockWeb.Plugins.Avalanche
{
    [DisplayName( "Video Player Block" )]
    [Category( "Avalanche" )]
    [Description( "Mobile video player." )]

    [TextField( "Source", "Video to be displayed. Data is parsed through Lava with the request {{parameter}}.", false )]
    [LavaCommandsField( "Enabled Lava Commands", "The Lava commands that should be enabled for this block.", false )]
    [BooleanField( "AutoPlay", "Start playing video on load" )]
    [TextField( "Aspect Ratio", "The aspect ratio to display the video. Common ratios: 0.5625 = HD Video, 1.7778 = Vertical Video, 0.75 = SD Video", true, "0.5625" )]
    public partial class VideoPlayerBlock : AvalancheBlock
    {

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summarysni>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad( EventArgs e )
        {
            lLava.Text = GetAttributeValue( "Source" );
        }

        public override MobileBlock GetMobile( string parameter )
        {
            CustomAttributes.Add( "Source", AvalancheUtilities.ProcessLava( GetAttributeValue( "Source" ),
                                                                            CurrentPerson,
                                                                            parameter,
                                                                            GetAttributeValue( "EnabledLavaCommands" )
                                                                            ) );
            CustomAttributes.Add( "AutoPlay", GetAttributeValue( "AutoPlay" ) );
            CustomAttributes.Add( "AspectRatio", GetAttributeValue( "AspectRatio" ) );
            return new MobileBlock()
            {
                BlockType = "Avalanche.Blocks.VideoPlayerBlock",
                Attributes = CustomAttributes
            };
        }
    }
}