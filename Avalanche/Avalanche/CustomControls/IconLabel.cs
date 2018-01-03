using System;
using System.Collections.Generic;
using System.Text;
using Avalanche.Components;
using Avalanche.Components.IconFont;
using Xamarin.Forms;

namespace Avalanche.CustomControls
{
    class IconLabel : Label
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
                if ( value.Trim().Length > 1 )
                {
                    var prefix = value.Trim().Substring( 0, 2 );
                    if ( prefix == "fa" )
                    {
                        iconFont = ( IIconFont ) Activator.CreateInstance( typeof( FontAwesome ) );
                    }
                    else if ( prefix == "se" )
                    {
                        iconFont = ( IIconFont ) Activator.CreateInstance( typeof( SoutheastIcons ) );
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
