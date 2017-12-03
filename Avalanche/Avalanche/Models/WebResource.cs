using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace Avalanche.Models
{
    public class WebResource
    {
        [PrimaryKey]
        public string Url { get; set; }
        public string Resource { get; set; }
        public DateTime EOL { get; set; }
    }
}
