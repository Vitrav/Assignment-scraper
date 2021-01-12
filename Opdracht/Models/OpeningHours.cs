using System;
using System.Collections.Generic;
using System.Text;

namespace Opdracht.Models
{
    public class OpeningHours
    {
        public DateTime Date { get; set; }
        public string Day { get; set; }
        public DateTime? OpeningHour { get; set; }
        public DateTime? ClosingHour { get; set; }
        public bool Opened { get; set; }
    }
}
