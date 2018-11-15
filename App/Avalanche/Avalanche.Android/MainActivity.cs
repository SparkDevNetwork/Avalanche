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

using Android.App;
using Android.Content.PM;
using Android.OS;
using FFImageLoading.Forms.Droid;
using Android.Gms.Common;
using Firebase.Iid;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;

namespace Avalanche.Droid
{
    [Activity( Label = "Southeast Christian", Icon = "@drawable/icon", Theme = "@style/MainTheme", ScreenOrientation = ScreenOrientation.Sensor, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation )]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate( Bundle bundle )
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate( bundle );

            Plugin.CurrentActivity.CrossCurrentActivity.Current.Activity = this;
            CachedImageRenderer.Init();

            var t = IsPlayServicesAvailable();
            RegisterChannel();

            global::Xamarin.Forms.Forms.Init( this, bundle );
            LoadApplication( new Avalanche.App() );

            //This is to fix softkeyboard covering forms
            Xamarin.Forms.Application.Current.On<Xamarin.Forms.PlatformConfiguration.Android>().UseWindowSoftInputModeAdjust( WindowSoftInputModeAdjust.Resize );
        }

        private void RegisterChannel()
        {
            string chanName = "General";
            var importance = NotificationImportance.Default;
            if ( Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O )
            {
                NotificationChannel chan = new NotificationChannel( "org.southeastchristian.seccapp.notifications", chanName, importance );
                chan.Description = "General updates and important messages.";
                chan.EnableVibration( true );
                chan.LockscreenVisibility = NotificationVisibility.Public;
                NotificationManager notificationManager = ( NotificationManager ) GetSystemService( NotificationService );
                notificationManager.CreateNotificationChannel( chan );
            }
        }

        string debug;

        public bool IsPlayServicesAvailable()
        {
            //This code is left in to help you debug your firebase for android
            //You will need to rebuild your app each time you wish to make a change
            //and use Firebase. This is a long standing Xamarin Android issue.
            int resultCode = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable( this );
            if ( resultCode != ConnectionResult.Success )
            {
                if ( GoogleApiAvailability.Instance.IsUserResolvableError( resultCode ) )
                    debug = GoogleApiAvailability.Instance.GetErrorString( resultCode );
                else
                {
                    debug = "This device is not supported";
                    Finish();
                }
                return false;
            }
            else
            {
                try
                {
                    debug = FirebaseInstanceId.Instance.Token;
                }
                catch ( Exception e )
                {
                }
                return true;
            }
        }
    }
}

