using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalanche.Interfaces;
using Avalanche.Services;
using Plugin.MediaManager;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Avalanche.Blocks
{
    [XamlCompilation( XamlCompilationOptions.Compile )]
    public partial class AudioPlayerBlock : ContentView, IRenderable, INotify
    {
        private MediaFile mediaFile;
        private TimeSpan Duration = TimeSpan.Zero;
        private DateTime LastMoved = DateTime.Now;

        private bool hasShadow = false;
        public bool HasShadow
        {
            get
            {
                return hasShadow;
            }

            set
            {
                hasShadow = value;
                fFrame.HasShadow = value;
            }
        }

        private bool autoPlay = false;
        public bool AutoPlay
        {
            get
            {
                return autoPlay;
            }
            set
            {
                value = autoPlay;
                if ( value )
                {
                    Play();
                }
            }
        }

        public Color BackgroundColor
        {
            get
            {
                return fFrame.BackgroundColor;
            }
            set
            {
                fFrame.BackgroundColor = value;
            }
        }

        public Color TextColor
        {
            get
            {
                return ilPlay.TextColor;
            }
            set
            {
                ilPlay.TextColor = value;
                lCurrent.TextColor = value;
                lDuration.TextColor = value;
            }
        }

        public AudioPlayerBlock()
        {
            InitializeComponent();
            var tgr = new TapGestureRecognizer();
            tgr.Tapped += Tgr_Tapped;
            ilPlay.GestureRecognizers.Add( tgr );
            sTime.Maximum = 100;
            sTime.Minimum = 0.0;
        }

        private void Tgr_Tapped( object sender, EventArgs e )
        {
            if ( IsCurrentPlaying() && CrossMediaManager.Current.Status == Plugin.MediaManager.Abstractions.Enums.MediaPlayerStatus.Playing )
            {
                Pause();
            }
            else
            {
                Play();
            }
        }

        public Dictionary<string, string> Attributes { get; set; }

        public View Render()
        {
            try
            {
                CrossMediaManager.Current.MediaNotificationManager.StopNotifications();
            }
            catch { }
            GetMediaFile();
            if ( IsCurrentPlaying() && CrossMediaManager.Current.Status == Plugin.MediaManager.Abstractions.Enums.MediaPlayerStatus.Playing )
            {
                ilPlay.Text = "fa fa-pause";
            }
            CrossMediaManager.Current.PlayingChanged += Current_PlayingChanged;
            CrossMediaManager.Current.StatusChanged += Current_StatusChanged;

            return this;
        }

        private void Current_StatusChanged( object sender, Plugin.MediaManager.Abstractions.EventArguments.StatusChangedEventArgs e )
        {
            if ( IsCurrentPlaying() )
            {
                UpdateDisplay();
            }
        }

        private void UpdateDisplay()
        {
            Device.BeginInvokeOnMainThread( () =>
            {
                switch ( CrossMediaManager.Current.Status )
                {
                    case Plugin.MediaManager.Abstractions.Enums.MediaPlayerStatus.Stopped:
                        ilPlay.IsVisible = true;
                        ilPlay.Text = "fa fa-play";
                        aiLoading.IsRunning = false;
                        break;
                    case Plugin.MediaManager.Abstractions.Enums.MediaPlayerStatus.Paused:
                        ilPlay.IsVisible = true;
                        ilPlay.Text = "fa fa-play";
                        aiLoading.IsRunning = false;
                        break;
                    case Plugin.MediaManager.Abstractions.Enums.MediaPlayerStatus.Playing:
                        ilPlay.IsVisible = true;
                        ilPlay.Text = "fa fa-pause";
                        aiLoading.IsRunning = false;
                        break;
                    case Plugin.MediaManager.Abstractions.Enums.MediaPlayerStatus.Loading:
                        ilPlay.IsVisible = false;
                        aiLoading.IsRunning = true;
                        break;
                    case Plugin.MediaManager.Abstractions.Enums.MediaPlayerStatus.Buffering:
                        ilPlay.IsVisible = false;
                        aiLoading.IsRunning = true;
                        break;
                    case Plugin.MediaManager.Abstractions.Enums.MediaPlayerStatus.Failed:
                        ilPlay.IsVisible = false;
                        aiLoading.IsRunning = true;
                        break;
                    default:
                        break;
                }
            } );
        }


        private void Current_PlayingChanged( object sender, Plugin.MediaManager.Abstractions.EventArguments.PlayingChangedEventArgs e )
        {
            var moveTime = DateTime.Now - LastMoved;
            if ( IsCurrentPlaying() )
            {
                Device.BeginInvokeOnMainThread( () =>
                {
                    var progress = e.Progress;
                    if ( Device.RuntimePlatform == Device.iOS )
                    {
                        progress = progress * 100;
                    }
                    if ( moveTime.TotalSeconds > 2 )
                    {
                        sTime.Value = progress;
                    }
                    Duration = e.Duration;
                    lDuration.Text = Duration.ToString( @"mm\:ss" );
                    lCurrent.Text = e.Position.ToString( @"mm\:ss" );
                } );
            }
        }

        private void Play()
        {
            if ( mediaFile != null )
            {
                if ( !IsCurrentPlaying() )
                {
                    CrossMediaManager.Current.Stop();
                    CrossMediaManager.Current.MediaQueue.Clear();
                    Device.BeginInvokeOnMainThread( () =>
                    {
                        aiLoading.IsRunning = true;
                        ilPlay.IsVisible = false;
                    } );
                    CrossMediaManager.Current.Play( mediaFile );
                }
                else if ( CrossMediaManager.Current.Status != Plugin.MediaManager.Abstractions.Enums.MediaPlayerStatus.Playing )
                {
                    CrossMediaManager.Current.Play( mediaFile );
                }
            }
        }

        private void Pause()
        {
            CrossMediaManager.Current.Pause();
        }

        private bool IsCurrentPlaying()
        {
            var current = CrossMediaManager.Current.MediaQueue.Current;
            if ( current != null )
            {
                if ( current.Url == mediaFile?.Url )
                {
                    return true;
                }
            }
            return false;
        }

        private void GetMediaFile()
        {
            if ( Attributes.ContainsKey( "Source" ) && !string.IsNullOrWhiteSpace( Attributes["Source"] ) )
            {
                var artist = "Unknown";
                if ( Attributes.ContainsKey( "Artist" ) && !string.IsNullOrWhiteSpace( Attributes["Artist"] ) )
                {
                    artist = Attributes["Artist"];
                }

                var title = "Unknown";
                if ( Attributes.ContainsKey( "Title" ) && !string.IsNullOrWhiteSpace( Attributes["Title"] ) )
                {
                    title = Attributes["Title"];
                }

                var metadata = new MediaFileMetaData()
                {
                    Artist = artist,
                    Title = title,
                    MediaUri = Attributes["Source"],
                };

                MediaFile media = new MediaFile
                {
                    MetadataExtracted = true,
                    Metadata = metadata,
                    Type = Plugin.MediaManager.Abstractions.Enums.MediaFileType.Audio,
                    Availability = Plugin.MediaManager.Abstractions.Enums.ResourceAvailability.Remote,
                    Url = Attributes["Source"]
                };

                mediaFile = media;
            }
        }

        private void sTime_ValueChanged( object sender, ValueChangedEventArgs e )
        {
            if ( mediaFile != null && IsCurrentPlaying() )
            {
                if ( e.OldValue + 1 < e.NewValue || e.OldValue - 1 > e.NewValue )
                {
                    LastMoved = DateTime.Now;
                    var seconds = Duration.TotalSeconds * ( sTime.Value / 100 );
                    var ts = TimeSpan.FromSeconds( seconds );
                    CrossMediaManager.Current.Seek( ts );
                    lCurrent.Text = CrossMediaManager.Current.Position.ToString( @"mm\:ss" );
                }
            }
        }

        public void OnAppearing()
        {

        }

        public void OnDisappearing()
        {
            if ( CrossMediaManager.Current.Status != Plugin.MediaManager.Abstractions.Enums.MediaPlayerStatus.Playing )
            {
                CrossMediaManager.Current.MediaNotificationManager.StopNotifications();
            }
        }
    }
}