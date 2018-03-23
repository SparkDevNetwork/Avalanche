using System;
using System.Collections.Generic;
using System.Text;

namespace Avalanche.Interfaces
{
    interface IHasMedia
    {
        bool IsFullScreen { get; }

        event EventHandler<bool> FullScreenChanged;

        void BackButtonPressed();

    }
}
