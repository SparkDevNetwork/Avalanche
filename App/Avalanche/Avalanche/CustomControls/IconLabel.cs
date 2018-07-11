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
using System.Collections.Generic;
using System.Text;
using Avalanche.Components;
using Avalanche.Components.IconFont;
using Xamarin.Forms;

namespace Avalanche.CustomControls
{
    public class IconLabel : Label
    {
        public IconLabel()
        {

        }

        public static readonly new BindableProperty TextProperty = BindableProperty.Create(
            "Text",
            typeof( string ),
            typeof( IconLabel ),
            propertyChanged: TextPropertyChange,
            defaultValue: ""
            );

        private static void TextPropertyChange( BindableObject bindable, object oldValue, object newValue )
        {
            var icon = ( IconLabel ) bindable;
            icon.Text = ( string ) newValue;
        }

        public new string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                IIconFont iconFont = null;
                if ( value!=null && value.Trim().Length > 1 )
                {
                    var prefix = value.Trim().Substring( 0, 2 );
                    if ( prefix == "fa" )
                    {
                        iconFont = ( IIconFont ) Activator.CreateInstance( typeof( FontAwesome ) );
                    }
                    if ( prefix == "se" )
                    {
                        iconFont = ( IIconFont ) Activator.CreateInstance( typeof( CustomIcons ) );
                    }

                    if ( iconFont == null )
                    {
                        return;
                    }

                    if ( Device.RuntimePlatform == Device.Android )
                    {
                        FontFamily = iconFont.AndroidFont;
                    }
                    else if ( Device.RuntimePlatform == Device.iOS )
                    {
                        FontFamily = iconFont.iOSFont;
                    }

                    if ( !string.IsNullOrWhiteSpace( value ) && iconFont.LookupTable.ContainsKey( value ) )
                    {
                        base.Text = iconFont.LookupTable[value];
                    }
                }
            }
        }
    }
}
