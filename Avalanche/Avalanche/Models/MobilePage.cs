using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalanche.Models;

namespace Avalanche.Models
{
    public class MobilePage
    {
        public int PageId { get; set; }
        public string LayoutType { get; set; }
        public List<MobileBlock> Blocks { get; set; }
        public Dictionary<string, string> Attributes { get; set; }
        public MobilePage()
        {
            Blocks = new List<MobileBlock>();
        }
    }
}
