using System;
using System.Collections.Generic;
using System.Text;

namespace Avalanche.Models
{
    public class HomeRequest
    {
        public MobilePage Header { get; set; }
        public MobilePage Footer { get; set; }
        public MobilePage Page { get; set; }
        public IDictionary<string, string> Attributes { get; set; }
    }
}