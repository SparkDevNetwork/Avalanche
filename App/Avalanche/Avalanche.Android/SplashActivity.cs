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
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using FFImageLoading.Forms.Droid;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;

namespace Avalanche.Droid
{
        [Activity( Label = "Southeast Christian", Icon = "@drawable/icon", Theme = "@style/Theme.Splash", //Indicates the theme to use for this activity
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