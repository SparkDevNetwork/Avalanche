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

namespace Avalanche.Models
{
    public class MobileListViewItem : INotifyPropertyChanged
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

        private string _description;
        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
                OnPropertyChanged( "Description" );
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

        private string _resource;
        public string Resource
        {
            get
            {
                return _resource;
            }
            set
            {
                _resource = value;
                OnPropertyChanged( "Resource" );
            }
        }

        private string _actionType;
        public string ActionType
        {
            get
            {
                return _actionType;
            }
            set
            {
                _actionType = value;
                OnPropertyChanged( "ActionType" );
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
