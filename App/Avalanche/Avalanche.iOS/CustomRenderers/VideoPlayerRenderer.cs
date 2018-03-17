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
using System.ComponentModel;
using System.Text;
using Avalanche.CustomControls;
using Avalanche.Services;
using AVFoundation;
using AVKit;
using Foundation;
using UIKit;
using Xamarin.Forms.Platform.iOS;

[assembly: Xamarin.Forms.ExportRenderer( typeof( Avalanche.CustomControls.VideoPlayer ), typeof( Avalanche.iOS.CustomRenderers.VideoPlayerRenderer ) )]
namespace Avalanche.iOS.CustomRenderers
{
    public class VideoPlayerRenderer : ViewRenderer<VideoPlayer, UIView>, INativePlayer
    {
        AVPlayer _player;
        AVPlayerViewController _playerController;

        private bool _prepared;
        public bool Prepared
        {
            get
            {
                return _prepared;
            }
            set
            {
                _prepared = value;
                DidVideoPrepared();
            }
        }

        public event EventHandler<bool> FullScreenStatusChanged;
        public event EventHandler<bool> PreparedStatusChanged;

        public static new void Init() { }

        protected override void OnElementChanged( ElementChangedEventArgs<VideoPlayer> e )
        {
            base.OnElementChanged( e );
            if ( e.OldElement != null )
                return;

            // Set Native Control
            _playerController = new AVPlayerViewController();
            _playerController.View.Frame = this.Frame;
            _playerController.ShowsPlaybackControls = true;
            AVAudioSession.SharedInstance().SetCategory( AVAudioSessionCategory.Playback );
            SetNativeControl( _playerController.View );
            Element.SetNativeContext( this );
            SetSource();
        }

        protected override void OnElementPropertyChanged( object sender, PropertyChangedEventArgs e )
        {
            base.OnElementPropertyChanged( sender, e );
            if ( VideoPlayer.SourceProperty.PropertyName.Equals( e.PropertyName ) )
            {
                SetSource();
            }
        }


        #region Private Methods

        private void SetSource()
        {
            try
            {
                if ( string.IsNullOrWhiteSpace( Element.Source ) )
                    return;
                Prepared = false;
                if ( _player != null )
                {
                    _player.Dispose();
                    _player = null;
                }

                AVPlayerItem playerItem = null;
                if ( Element.Source.StartsWith( "http://" ) || Element.Source.StartsWith( "https://" ) )
                    playerItem = new AVPlayerItem( AVAsset.FromUrl( NSUrl.FromString( Element.Source ) ) );
                else
                    playerItem = new AVPlayerItem( NSUrl.FromFilename( Element.Source ) );

                NSNotificationCenter.DefaultCenter.AddObserver( AVPlayerItem.DidPlayToEndTimeNotification, DidVideoFinishPlaying, playerItem );
                NSNotificationCenter.DefaultCenter.AddObserver( AVPlayerItem.ItemFailedToPlayToEndTimeNotification, DidVideoErrorOcurred, playerItem );
                NSNotificationCenter.DefaultCenter.AddObserver( AVPlayerItem.NewErrorLogEntryNotification, DidVideoErrorOcurred, playerItem );
                //NSNotificationCenter.DefaultCenter.AddObserver(AVPlayerItem.no, DidVideoPrepared, playerItem);

                _player = new AVPlayer( playerItem );
                _player.ActionAtItemEnd = AVPlayerActionAtItemEnd.None;
                _playerController.Player = _player;
                Prepared = true;

                if ( Element.AutoPlay )
                    _player.Play();

                if ( _player.Error != null )
                {
                    Element.OnError( _playerController?.Player?.Error?.LocalizedDescription );
                }
            }
            catch ( Exception e )
            {
                Element.OnError( e.Message );
            }
        }

        #endregion

        #region INativePlayer

        public int Duration
        {
            get
            {
                return Prepared ? ( int ) _player.CurrentItem.Duration.Seconds : 0;
            }
        }

        public int CurrentPosition
        {
            get
            {
                return Prepared ? ( int ) _player.CurrentItem.CurrentTime.Seconds : 0;
            }
        }

        public void DisplaySeekbar( bool value )
        {
            _playerController.ShowsPlaybackControls = value;
        }

        public bool IsSeekbarVisible
        {
            get
            {
                if ( _playerController == null )
                    return false;
                return _playerController.ShowsPlaybackControls;
            }
        }

        public void Play()
        {
            if ( !Prepared )
                return;
            _player.Play();
        }

        public void Pause()
        {
            if ( !Prepared )
                return;
            _player.Pause();
        }

        public void Stop()
        {
            if ( !Prepared )
                return;
            _player.Pause();
        }

        public void Seek( int seconds )
        {
            if ( !Prepared )
                return;
            _player.Seek( CoreMedia.CMTime.FromSeconds( seconds, 1 ) );
        }

        public void SetScreen( bool isPortrait )
        {

            //AVPlayerViewController provide by default this feature
        }

        //public void FullScreen()
        //{
        //    if (!Prepared) return;
        //    //_player.Frame = NativeView.Frame;
        //    //NativeView.Layer.AddSublayer(_player);
        //}

        //public void ExitFullScreen()
        //{
        //    if (!Prepared) return;
        //    //_player.Frame = NativeView.Frame;
        //    //NativeView.Layer.AddSublayer(_player);

        //}

        #endregion

        #region Events

        private void DidVideoPrepared()
        {
            Element.OnPrepare();
        }

        private void DidVideoFinishPlaying( NSNotification obj )
        {
            Element.OnCompletion();
        }

        private void DidVideoErrorOcurred( NSNotification obj )
        {
            Element.OnError( _player.Error?.Description ?? "Unable to play video." );
        }


        #endregion
    }
}
