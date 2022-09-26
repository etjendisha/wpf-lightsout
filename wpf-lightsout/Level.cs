using System;
using System.Collections.Generic;
using System.Text;

namespace wpf_lightsout
{
    public class Level
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Columns { get; set; }
        public int Rows { get; set; }
        public List<int> On { get; set; }
    }
}
