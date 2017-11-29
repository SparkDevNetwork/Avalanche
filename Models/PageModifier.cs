using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalanche.Models
{
    public class PageModifier
    {
        public string BlockType { get; set; }
        public Dictionary<string, string> Body { get; set; }
    }
}
