using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Avalanche.Models
{
    public class ObservableResource<T> : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged( string propertyName )
        {
            PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
        }

        private T _resource;

        public T Resource
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
    }
}
