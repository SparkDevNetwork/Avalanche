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
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Avalanche.Services;
using Xamarin.Forms;

namespace Avalanche.Droid.Services
{
      public class OrientationHandler : IOrientationHandler
    {
        public void ForceLandscape()
        {
            ( ( Activity ) Forms.Context ).RequestedOrientation = ScreenOrientation.Landscape;
        }

        public void ForcePortrait()
        {
            ( ( Activity ) Forms.Context ).RequestedOrientation = ScreenOrientation.Portrait;
            ( ( Activity ) Forms.Context ).Window.ClearFlags( WindowManagerFlags.Fullscreen );
        }
    }
}