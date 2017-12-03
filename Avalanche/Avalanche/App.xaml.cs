using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Avalanche.Utilities;
using Xamarin.Forms;

namespace Avalanche
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            RockClient.CreateDatabase();
            RockClient.ClearDatabase();
            LoadConverters();
            MainPage = new Avalanche.MainPage( "page/509" );
        }

        private void LoadConverters()
        {
            var types = AppDomain.CurrentDomain.GetAssemblies()
                       .SelectMany( assembly => assembly.GetTypes() )
                       .Where( type => type.IsSubclassOf( typeof( TypeConverter ) ) ).ToList();
            foreach (var type in types )
            {
                var attributes = type.GetCustomAttributes( false );
            }
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
