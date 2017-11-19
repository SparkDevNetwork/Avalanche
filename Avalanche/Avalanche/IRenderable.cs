using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace org.secc.Avalanche
{
    public interface IRenderable
    {
        Dictionary<string, string> Attributes { get; set; }
        View Render();
    }
}
