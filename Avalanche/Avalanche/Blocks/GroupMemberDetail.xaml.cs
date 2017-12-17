using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Avalanche.Blocks
{
    [XamlCompilation( XamlCompilationOptions.Compile )]
    public partial class GroupMemberDetail : ContentView, IRenderable
    {
        public string Image
        {
            get
            {
                return ffImage.Source.ToString();
            }
            set
            {
                ffImage.Source = value;
            }
        }

        public string Name
        {
            get
            {
                return lName.Text;
            }
            set
            {
                lName.Text = value;
            }
        }

        private Color _prevBarColor;
        public Color AccentColor
        {
            get
            {
                return slAccent.BackgroundColor;
            }
            set
            {
                if ( App.Current.MainPage is NavigationPage )
                {
                    ( ( NavigationPage ) App.Current.MainPage ).BarBackgroundColor = value;
                }
                slAccent.BackgroundColor = value;
            }
        }


        public string Markdown
        {
            get
            {
                return mdContent.Markdown;
            }
            set
            {
                mdContent.Markdown = value;
            }
        }

        public GroupMemberDetail()
        {
            InitializeComponent();
            Task.Run( async () =>
            {
                await Task.Delay(250);
                App.Current.MainPage.Navigation.NavigationStack[App.Current.MainPage.Navigation.NavigationStack.Count - 1].Disappearing += GroupMemberDetail_Disappearing;
            } );
        }

        private void GroupMemberDetail_Disappearing( object sender, EventArgs e )
        {
            if ( _prevBarColor != null )
            {
                ( ( NavigationPage ) App.Current.MainPage ).BarBackgroundColor = _prevBarColor;
            }
        }

        public Dictionary<string, string> Attributes { get; set; }

        public View Render()
        {
            return this;
        }
    }
}