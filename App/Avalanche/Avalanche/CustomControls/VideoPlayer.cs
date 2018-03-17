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
using Avalanche.Services;
using Xamarians.MediaPlayer;
using Xamarin.Forms;

namespace Avalanche.CustomControls
{
    public class VideoPlayer : View
    {
        INativePlayer nativePlayer;

        #region Properties

        public static readonly BindableProperty SourceProperty = BindableProperty.Create( "Source", typeof( string ), typeof( VideoPlayer ), null );
        public string Source
        {
            get { return ( string ) GetValue( SourceProperty ); }
            set { SetValue( SourceProperty, value ); }
        }

        public static readonly BindableProperty AutoPlayProperty = BindableProperty.Create( "AutoPlay", typeof( bool ), typeof( VideoPlayer ), false );
        public bool AutoPlay
        {
            get { return ( bool ) GetValue( AutoPlayProperty ); }
            set { SetValue( AutoPlayProperty, value ); }
        }

        #endregion

        #region Events
        public event EventHandler<bool> FullScreenStatusChanged;
        public event EventHandler Completed;
        public event EventHandler<PlayerErrorEventArgs> Error;
        public event EventHandler Prepared;
        #endregion

        #region Public Methods
        public VideoPlayer()
        {

        }

        #endregion

        #region Interal Methods

        internal void SetNativeContext( INativePlayer player )
        {
            nativePlayer = player;
            nativePlayer.FullScreenStatusChanged += ( s, e ) => FullScreenStatusChanged?.Invoke( this, e );
        }

        internal void OnError( string error )
        {
            Error?.Invoke( this, new PlayerErrorEventArgs( error ) );
        }

        internal void OnCompletion()
        {
            Completed?.Invoke( this, EventArgs.Empty );
        }

        internal void OnPrepare()
        {
            Prepared?.Invoke( this, EventArgs.Empty );
        }



        #endregion

        #region INativePlayer

        //public bool IsFullScreen
        //{
        //    get
        //    {
        //        return nativePlayer?.IsFullScreen ?? false;
        //    }
        //}

        public int Duration
        {
            get
            {
                return nativePlayer?.Duration ?? 0;
            }
        }

        public int CurrentPosition
        {
            get
            {
                return nativePlayer?.CurrentPosition ?? 0;
            }
        }

        public bool CanPlay
        {
            get
            {
                return nativePlayer != null;
            }
        }


        public void Play()
        {
            nativePlayer?.Play();
        }

        public void Pause()
        {
            nativePlayer?.Pause();
        }

        public void Stop()
        {
            nativePlayer?.Stop();
        }

        public void Seek( int seconds )
        {
            nativePlayer?.Seek( seconds );
        }


        /// <summary>
        /// Change screen orientation to Landscape and set video player in full screen mode.
        /// </summary>
        /// <param name="resizeLayout">set it True if you are using video player inside a scroo view</param>
        //public void FullScreen(bool resizeLayout = false)
        //{
        //    if (nativePlayer == null)
        //        return;
        //    nativePlayer.FullScreen(resizeLayout);
        //}

        //public void ExitFullScreen()
        //{
        //    if (nativePlayer == null)
        //        return;
        //    nativePlayer.ExitFullScreen();
        //}

        //public void HideSeekbar()
        //{
        //    nativePlayer?.HideSeekbar();
        //}
        #endregion

    }
}