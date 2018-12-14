using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalanche.Models
{
    public class HomeRequest
    {
        public MobilePage Header { get; set; }
        public MobilePage Footer { get; set; }
        public MobilePage Page { get; set; }
        public Dictionary<string,string> Attributes { get; set; }
    }
}
