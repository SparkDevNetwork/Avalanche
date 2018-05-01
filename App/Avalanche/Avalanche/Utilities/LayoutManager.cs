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
using Avalanche.Models;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace Avalanche.Utilities
{
    public class LayoutManager
    {
        private Dictionary<string, Layout<View>> layouts = new Dictionary<string, Layout<View>>();
        public ContentView Content = new ContentView();

        public LayoutManager( string json )
        {
            StackLayout mainStack = new StackLayout
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                Spacing = 0
            };
            Content.Content = mainStack;

            if ( string.IsNullOrWhiteSpace( json ) )
            {
                return;
            }
            try
            {
                var layoutTemplates = JsonConvert.DeserializeObject<List<LayoutTemplate>>( json );


                //NavBar
                var navBar = new StackLayout()
                {
                    IsVisible = false,
                };
                mainStack.Children.Add( navBar );
                layouts.Add( "navbar", navBar );


                foreach ( var layoutTemplate in layoutTemplates )
                {
                    AddLayout( layoutTemplate, mainStack );
                }
            }
            catch
            {
            }
        }

        private void AddLayout( LayoutTemplate layoutTemplate, Layout<View> parentLayout )
        {
            Layout<View> newLayout = null;
            ScrollView scrollView = null;

            if ( layoutTemplate.ScrollX || layoutTemplate.ScrollY )
            {
                scrollView = new ScrollView();
                AttributeHelper.ApplyTranslation( scrollView, layoutTemplate.Attributes );
                if ( layoutTemplate.ScrollX && layoutTemplate.ScrollY )
                {
                    scrollView.Orientation = ScrollOrientation.Both;
                }
                else if ( layoutTemplate.ScrollY )
                {
                    scrollView.Orientation = ScrollOrientation.Vertical;
                }
                else if ( layoutTemplate.ScrollX )
                {
                    scrollView.Orientation = ScrollOrientation.Horizontal;
                }
                if ( parentLayout is Grid )
                {
                    try
                    {
                        ( ( Grid ) parentLayout ).Children.Add(
                            newLayout,
                            layoutTemplate.Column,
                            layoutTemplate.Row
                            );
                        Grid.SetColumnSpan( newLayout, layoutTemplate.ColumnSpan );
                        Grid.SetRowSpan( newLayout, layoutTemplate.RowSpan );
                    }
                    catch { }
                }
                else
                {
                    parentLayout.Children.Add( scrollView );
                }
            }


            if ( layoutTemplate.Type == "StackLayout" )
            {
                var sl = new StackLayout();
                sl.Orientation = layoutTemplate.Orientation;
                if ( layoutTemplate.Spacing >= 0 )
                {
                    sl.Spacing = layoutTemplate.Spacing;
                }
                newLayout = sl;
            }
            else if ( layoutTemplate.Type == "Grid" )
            {
                var g = new Grid();
                if ( layoutTemplate.Spacing >= 0 )
                {
                    g.RowSpacing = layoutTemplate.Spacing;
                    g.ColumnSpacing = layoutTemplate.Spacing;
                }
                var gridConverter = new GridLengthTypeConverter();
                var rowDefinitions = layoutTemplate.RowDefinitions.Split( new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries );
                foreach ( var def in rowDefinitions )
                {
                    var gridLength = ( GridLength ) gridConverter.ConvertFromInvariantString( def );
                    g.RowDefinitions.Add( new RowDefinition { Height = gridLength } );

                }
                var columnDefinitions = layoutTemplate.ColumnDefinitions.Split( new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries );
                foreach ( var def in columnDefinitions )
                {
                    var gridLength = ( GridLength ) gridConverter.ConvertFromInvariantString( def );
                    g.ColumnDefinitions.Add( new ColumnDefinition { Width = gridLength } );

                }
                newLayout = g;
            }

            if ( newLayout == null )
            {
                return;
            }

            if ( scrollView != null )
            {
                scrollView.Content = newLayout;
            }
            else
            {
                if ( parentLayout is Grid )
                {
                    try
                    {
                        ( ( Grid ) parentLayout ).Children.Add(
                            newLayout,
                            layoutTemplate.Column,
                            layoutTemplate.Row
                            );
                        Grid.SetColumnSpan( newLayout, layoutTemplate.ColumnSpan );
                        Grid.SetRowSpan( newLayout, layoutTemplate.RowSpan );
                    }
                    catch { }

                }
                else
                {
                    parentLayout.Children.Add( newLayout );
                }
            }

            layouts.Add( layoutTemplate.Name.ToLower(), newLayout );

            AttributeHelper.ApplyTranslation( newLayout, layoutTemplate.Attributes );

            foreach ( var child in layoutTemplate.Children )
            {
                AddLayout( child, newLayout );
            }
        }

        public Layout<View> GetElement( string name )
        {
            name = name.ToLower();
            if ( layouts.ContainsKey( name ) )
            {
                return layouts[name];
            }
            return null;
        }
    }
}
