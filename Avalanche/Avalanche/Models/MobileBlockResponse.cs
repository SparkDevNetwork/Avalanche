using System;
using System.Collections.Generic;
using System.Text;

namespace Avalanche.Models
{
    public class MobileBlockResponse
    {
        public string Request { get; set; }
        public string Response { get; set; }
        public int TTL { get; set; }
    }
}
