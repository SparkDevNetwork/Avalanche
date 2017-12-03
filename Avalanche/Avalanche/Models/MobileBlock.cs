using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalanche.Models
{
    public class MobileBlock
    {
        public int BlockId { get; set; }
        public string Zone { get; set; }
        public string BlockType { get; set; }
        public Dictionary<string, string> Body { get; set; }
    }
}
