using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Avalanche.Models
{
    public class MobileListView : INotifyPropertyChanged
    {
        public string Id { get; set; }
        private string _title;
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
                OnPropertyChanged( "Title" );
            }
        }

        private string _subtitle;
        public string Subtitle
        {
            get
            {
                return _subtitle;
            }
            set
            {
                _subtitle = value;
                OnPropertyChanged( "Subtitle" );
            }
        }

        private string _image;
        public string Image
        {
            get
            {
                return _image;
            }
            set
            {
                _image = value;
                OnPropertyChanged( "Image" );
            }
        }
        private string _icon;
        public string Icon
        {
            get
            {
                return _icon;
            }
            set
            {
                _icon = value;
                OnPropertyChanged( "Icon" );
            }
        }
        private double _fontSize = 20;
        public double FontSize
        {
            get
            {
                return _fontSize;
            }
            set
            {
                _fontSize = value;
                IconFontSize = value * 2;
                SubtitleFontSize = value * .75;
                OnPropertyChanged( "FontSize" );
            }
        }

        public double IconFontSize
        {
            get
            {
                return _fontSize * 2;
            }
            private set
            {
                OnPropertyChanged( "IconFontSize" );
            }
        }

        public double SubtitleFontSize
        {
            get
            {
                return _fontSize * .75;
            }
            private set
            {
                OnPropertyChanged( "SubtitleFontSize" );
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged( string propertyName )
        {
            PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
        }

    }
}
