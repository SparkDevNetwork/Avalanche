using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using FFImageLoading.Forms.Droid;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;

namespace Avalanche.Droid
{
        [Activity( Label = "Avalanche", Icon = "@drawable/icon", Theme = "@style/Theme.Splash", //Indicates the theme to use for this activity
             MainLauncher = true, //Set it as boot activity
             NoHistory = true )] //Doesn't place it in back stack
    class SplashActivity : Activity
    {
        protected override void OnCreate( Bundle bundle )
        {
            base.OnCreate( bundle );
            this.StartActivity( typeof( MainActivity ) );
        }
        public override void OnRequestPermissionsResult( int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults )
        {
            PermissionsImplementation.Current.OnRequestPermissionsResult( requestCode, permissions, grantResults );
        }
    }
}