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
using Avalanche.Interfaces;
using Xamarin.Forms;

namespace Avalanche.PageAttributes
{
    class BackgroundImage : IPageAttribute
    {
        public void Modify( ContentPage contentPage, string value )
        {
            var mainGrid = contentPage.FindByName<Grid>( "MainGrid" );
            if ( mainGrid != null )
            {
                var image = new Image()
                {
                    Aspect = Aspect.AspectFill,
                    Source = value

                };
                mainGrid.Children.Add( image );
            }
        }
    }
}
