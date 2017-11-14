using System;

namespace TogglExport.Models {
    public class TimeEntry {
        public string Description { get; set; }
        public int Wid { get; set; }
        public int Pid { get; set; }
        public DateTime Start { get; set; }
        public DateTime Stop { get; set; }
        public int Duration { get; set; }
    }
}
