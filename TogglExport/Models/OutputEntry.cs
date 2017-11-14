using System;
using System.Collections.Generic;
using System.Text;

namespace TogglExport.Models {
    public class OutputEntry {
        public DateTime Date { get; set; }
        public string Code { get; set; }
        public double Duration { get; set; }
        public string Description { get; set; }
        public string Identifier { get; set; }
    }
}
