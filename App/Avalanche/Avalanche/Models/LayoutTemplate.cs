using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Avalanche.Models
{
    public class LayoutTemplate
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public int Row { get; set; } = 0;
        public int Column { get; set; } = 0;
        public int RowSpan { get; set; } = 1;
        public int ColumnSpan { get; set; } = 1;
        public string RowDefinitions { get; set; } = "*";
        public string ColumnDefinitions { get; set; } = "*";
        public bool ScrollY { get; set; }
        public bool ScrollX { get; set; }
        public StackOrientation Orientation { get; set; } = StackOrientation.Vertical;
        public double Spacing { get; set; } = -1;
        public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();
        public List<LayoutTemplate> Children { get; set; } = new List<LayoutTemplate>();
    }
}
