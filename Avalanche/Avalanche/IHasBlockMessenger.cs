using System;
using System.Collections.Generic;
using System.Text;
using Avalanche.Models;
using Avalanche.Utilities;

namespace Avalanche
{
    public interface IHasBlockMessenger
    {
        BlockMessenger MessageHandler { get; set; }
    }
}
